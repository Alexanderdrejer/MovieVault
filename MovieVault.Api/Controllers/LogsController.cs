using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieVault.Api.Domain;
using MovieVault.Api.Domain.DTOs;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class LogsController : ControllerBase
{
    private readonly IMongoCollection<BsonDocument> _logs;

    public LogsController(IMongoCollection<BsonDocument> logs)
    {
        _logs = logs;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RequestLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<RequestLogDto>>> GetLogs(
        [FromQuery] string? username,
        [FromQuery] DateOnly? date,
        [FromQuery] string? requestType,
        [FromQuery] string? route)
    {
        var filter = Builders<BsonDocument>.Filter.Exists("Properties.LogInfo.RequestType");

        if (!string.IsNullOrWhiteSpace(username))
            filter &= Builders<BsonDocument>.Filter.Eq("Properties.LogInfo.Username", username);

        if (date != null)
        {
            var from = date.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            filter &= Builders<BsonDocument>.Filter.Gte("Properties.LogInfo.Timestamp", from)
                & Builders<BsonDocument>.Filter.Lt("Properties.LogInfo.Timestamp", from.AddDays(1));
        }

        if (!string.IsNullOrWhiteSpace(requestType))
            filter &= Builders<BsonDocument>.Filter.Eq("Properties.LogInfo.RequestType", requestType.ToUpperInvariant());

        if (!string.IsNullOrWhiteSpace(route))
            filter &= Builders<BsonDocument>.Filter.Regex("Properties.LogInfo.Route",
                new BsonRegularExpression("^" + Regex.Escape(route), "i"));

        var documents = await _logs.Find(filter)
            .Sort(Builders<BsonDocument>.Sort.Descending("Properties.LogInfo.Timestamp"))
            .ToListAsync();

        return documents.Select(d =>
        {
            var log = d["Properties"]["LogInfo"].AsBsonDocument;
            return new RequestLogDto
            {
                Username = log["Username"].AsString,
                Timestamp = log["Timestamp"].ToUniversalTime(),
                RequestType = log["RequestType"].AsString,
                Route = log["Route"].AsString,
                StatusCode = log["StatusCode"].ToInt32()
            };
        }).ToList();
    }
}
