using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieVault.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewsAndMediaOutlets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaOutlets",
                columns: table => new
                {
                    MediaOutletId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OutletType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaOutlets", x => x.MediaOutletId);
                });

            migrationBuilder.CreateTable(
                name: "Reviewers",
                columns: table => new
                {
                    ReviewerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MediaOutletId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewers", x => x.ReviewerId);
                    table.ForeignKey(
                        name: "FK_Reviewers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reviewers_MediaOutlets_MediaOutletId",
                        column: x => x.MediaOutletId,
                        principalTable: "MediaOutlets",
                        principalColumn: "MediaOutletId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Headline = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    MediaOutletId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_MediaOutlets_MediaOutletId",
                        column: x => x.MediaOutletId,
                        principalTable: "MediaOutlets",
                        principalColumn: "MediaOutletId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "ReviewerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MediaOutlets",
                columns: new[] { "MediaOutletId", "Name", "OutletType", "Website" },
                values: new object[,]
                {
                    { 1, "Aarhus Dagblad", "newspaper", "https://aarhusdagblad.example" },
                    { 2, "Kanal Nord Kultur", "tv", "https://kanalnord.example" }
                });

            migrationBuilder.InsertData(
                table: "Reviewers",
                columns: new[] { "ReviewerId", "MediaOutletId", "Name", "UserId" },
                values: new object[,]
                {
                    { 1, 1, "Rita Reel", null },
                    { 2, 2, "Jonas Frame", null }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "ReviewId", "Headline", "MediaOutletId", "MovieId", "PublishedAt", "Rating", "ReviewerId", "Text" },
                values: new object[,]
                {
                    { 1, "A cold, beautiful crossing", 1, 1, new DateTime(2023, 11, 18, 8, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Halvorsen delivers a slow-burning drama with a stunning lead performance." },
                    { 2, "Gorgeous but overlong", 2, 1, new DateTime(2023, 11, 19, 20, 30, 0, 0, DateTimeKind.Unspecified), 3, 2, "The landscapes impress, but the third act drags." },
                    { 3, "Quiet and sharp", 1, 3, new DateTime(2022, 9, 24, 9, 15, 0, 0, DateTimeKind.Unspecified), 4, 1, "A small family story told with real wit." },
                    { 4, "Lanterns that glow", 2, 4, new DateTime(2024, 2, 10, 19, 0, 0, 0, DateTimeKind.Unspecified), 4, 2, "Tanaka balances spectacle and intimacy." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_MediaOutletId",
                table: "Reviewers",
                column: "MediaOutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_UserId",
                table: "Reviewers",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MediaOutletId",
                table: "Reviews",
                column: "MediaOutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MovieId",
                table: "Reviews",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Reviewers");

            migrationBuilder.DropTable(
                name: "MediaOutlets");
        }
    }
}
