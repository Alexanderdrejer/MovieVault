using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieVault.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.ActorId);
                });

            migrationBuilder.CreateTable(
                name: "Cinemas",
                columns: table => new
                {
                    CinemaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    StreetAddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    OpeningYear = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cinemas", x => x.CinemaId);
                });

            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    DirectorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.DirectorId);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    LocationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "ProductionCompanies",
                columns: table => new
                {
                    ProductionCompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FoundingYear = table.Column<int>(type: "int", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCompanies", x => x.ProductionCompanyId);
                });

            migrationBuilder.CreateTable(
                name: "Screens",
                columns: table => new
                {
                    ScreenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CinemaId = table.Column<int>(type: "int", nullable: false),
                    ScreenNumberOrName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SeatingCapacity = table.Column<int>(type: "int", nullable: false),
                    ScreenType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Screens", x => x.ScreenId);
                    table.ForeignKey(
                        name: "FK_Screens_Cinemas_CinemaId",
                        column: x => x.CinemaId,
                        principalTable: "Cinemas",
                        principalColumn: "CinemaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DirectorId = table.Column<int>(type: "int", nullable: true),
                    ProductionCompanyId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "date", nullable: true),
                    RuntimeMinutes = table.Column<int>(type: "int", nullable: true),
                    AgeRating = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.MovieId);
                    table.ForeignKey(
                        name: "FK_Movies_Directors_DirectorId",
                        column: x => x.DirectorId,
                        principalTable: "Directors",
                        principalColumn: "DirectorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Movies_ProductionCompanies_ProductionCompanyId",
                        column: x => x.ProductionCompanyId,
                        principalTable: "ProductionCompanies",
                        principalColumn: "ProductionCompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Studios",
                columns: table => new
                {
                    StudioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionCompanyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    SoundStages = table.Column<int>(type: "int", nullable: true),
                    MaxStageCapacity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studios", x => x.StudioId);
                    table.ForeignKey(
                        name: "FK_Studios_ProductionCompanies_ProductionCompanyId",
                        column: x => x.ProductionCompanyId,
                        principalTable: "ProductionCompanies",
                        principalColumn: "ProductionCompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieActors",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    ActorId = table.Column<int>(type: "int", nullable: false),
                    CharacterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RoleType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieActors", x => new { x.MovieId, x.ActorId });
                    table.ForeignKey(
                        name: "FK_MovieActors_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "ActorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieActors_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieLocations",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    ShootingDays = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieLocations", x => new { x.MovieId, x.LocationId });
                    table.ForeignKey(
                        name: "FK_MovieLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieLocations_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionRecords",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    ProductionBudget = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    ShootingStartDate = table.Column<DateTime>(type: "date", nullable: true),
                    ShootingEndDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionRecords", x => x.MovieId);
                    table.ForeignKey(
                        name: "FK_ProductionRecords_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Screenings",
                columns: table => new
                {
                    ScreeningId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    ScreenId = table.Column<int>(type: "int", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TicketPrice = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Screenings", x => x.ScreeningId);
                    table.ForeignKey(
                        name: "FK_Screenings_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Screenings_Screens_ScreenId",
                        column: x => x.ScreenId,
                        principalTable: "Screens",
                        principalColumn: "ScreenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketSales",
                columns: table => new
                {
                    TicketSaleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScreeningId = table.Column<int>(type: "int", nullable: false),
                    SaleDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TicketCount = table.Column<int>(type: "int", nullable: false),
                    PricePerTicket = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSales", x => x.TicketSaleId);
                    table.ForeignKey(
                        name: "FK_TicketSales_Screenings_ScreeningId",
                        column: x => x.ScreeningId,
                        principalTable: "Screenings",
                        principalColumn: "ScreeningId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Actors",
                columns: new[] { "ActorId", "DateOfBirth", "Gender", "Name", "Nationality" },
                values: new object[,]
                {
                    { 1, new DateTime(1988, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Female", "Elena Ruiz", "Spanish" },
                    { 2, new DateTime(1979, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", "Tom Brannigan", "Irish" },
                    { 3, new DateTime(1992, 12, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Female", "Priya Nair", "Indian" },
                    { 4, new DateTime(1968, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", "Karl Ostermann", "German" },
                    { 5, new DateTime(1995, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Female", "Mei Chen", "Chinese" },
                    { 6, new DateTime(1983, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", "Dario Fontana", "Italian" },
                    { 7, new DateTime(1990, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Female", "Sofia Lindqvist", "Swedish" },
                    { 8, new DateTime(1972, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", "Owen Blackwood", "Canadian" }
                });

            migrationBuilder.InsertData(
                table: "Cinemas",
                columns: new[] { "CinemaId", "City", "Name", "OpeningYear", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "Aarhus", "Palace Cinema", 1954, "Skolegade 12" },
                    { 2, "Copenhagen", "Metro Screens", 1998, "Vesterbrogade 88" },
                    { 3, "Aalborg", "Kino Nord", 2011, "Boulevarden 4" }
                });

            migrationBuilder.InsertData(
                table: "Directors",
                columns: new[] { "DirectorId", "DateOfBirth", "Name", "Nationality" },
                values: new object[,]
                {
                    { 1, new DateTime(1975, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Greta Halvorsen", "Norwegian" },
                    { 2, new DateTime(1962, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marcus Webb", "British" },
                    { 3, new DateTime(1980, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aiko Tanaka", "Japanese" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "LocationId", "City", "Country", "LocationType", "Name" },
                values: new object[,]
                {
                    { 1, "Bergen", "Norway", "street", "Old Harbour Pier" },
                    { 2, "London", "United Kingdom", "building", "Grand Central Library" },
                    { 3, "Kyoto", "Japan", "landscape", "Kiyomizu Hillside" },
                    { 4, "Los Angeles", "USA", "set", "Backlot Village Set" },
                    { 5, "Florence", "Italy", "street", "Ponte Vecchio Approach" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieId", "AgeRating", "DirectorId", "ProductionCompanyId", "ReleaseDate", "RuntimeMinutes", "Status", "Title" },
                values: new object[] { 6, null, null, null, null, null, "planned", "The Glass Orchard" });

            migrationBuilder.InsertData(
                table: "ProductionCompanies",
                columns: new[] { "ProductionCompanyId", "Country", "FoundingYear", "Name" },
                values: new object[,]
                {
                    { 1, "USA", 1994, "Northlight Pictures" },
                    { 2, "United Kingdom", 2003, "Harbour Row Films" },
                    { 3, "Japan", 1988, "Kitsune Studios" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieId", "AgeRating", "DirectorId", "ProductionCompanyId", "ReleaseDate", "RuntimeMinutes", "Status", "Title" },
                values: new object[,]
                {
                    { 1, "12", 1, 1, new DateTime(2023, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 138, "released", "The Winter Crossing" },
                    { 2, "15", 1, 1, new DateTime(2021, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 121, "released", "Salt and Iron" },
                    { 3, "7", 2, 2, new DateTime(2022, 9, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 109, "released", "A Quiet Inheritance" },
                    { 4, "12", 3, 3, new DateTime(2024, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 126, "released", "Lanterns Over Kyoto" },
                    { 5, "15", 2, 3, null, 115, "in production", "The Longest Night" }
                });

            migrationBuilder.InsertData(
                table: "Screens",
                columns: new[] { "ScreenId", "CinemaId", "ScreenNumberOrName", "ScreenType", "SeatingCapacity" },
                values: new object[,]
                {
                    { 1, 1, "Screen 1", "IMAX", 220 },
                    { 2, 1, "Screen 2", "Standard", 140 },
                    { 3, 1, "Screen 3", "Standard", 90 },
                    { 4, 2, "Hall A", "IMAX", 300 },
                    { 5, 2, "Hall B", "3D", 160 },
                    { 6, 3, "Screen 1", "Standard", 120 }
                });

            migrationBuilder.InsertData(
                table: "Studios",
                columns: new[] { "StudioId", "City", "Country", "MaxStageCapacity", "Name", "ProductionCompanyId", "SoundStages" },
                values: new object[,]
                {
                    { 1, "Los Angeles", "USA", 400, "Northlight Stage A", 1, 6 },
                    { 2, "Atlanta", "USA", 250, "Northlight Stage B", 1, 4 },
                    { 3, "London", "United Kingdom", 180, "Harbour Row Docks", 2, 3 },
                    { 4, "Kyoto", "Japan", 320, "Kitsune Main Lot", 3, 5 }
                });

            migrationBuilder.InsertData(
                table: "MovieActors",
                columns: new[] { "ActorId", "MovieId", "CharacterName", "RoleType" },
                values: new object[,]
                {
                    { 1, 1, "Ingrid Solberg", "lead" },
                    { 2, 1, "Captain Hale", "lead" },
                    { 4, 1, "Dr. Neumann", "supporting" },
                    { 6, 1, "Ferry Passenger", "cameo" },
                    { 7, 1, "Young Ingrid", "supporting" },
                    { 1, 2, "Marisol", "lead" },
                    { 8, 2, "Sheriff Doyle", "supporting" },
                    { 3, 3, "Anjali Rao", "lead" },
                    { 4, 3, "Mr. Pike", "supporting" },
                    { 3, 4, "Vaidehi", "supporting" },
                    { 5, 4, "Hana", "lead" },
                    { 2, 5, "Unnamed Soldier", "lead" },
                    { 6, 5, "Luca", "supporting" }
                });

            migrationBuilder.InsertData(
                table: "MovieLocations",
                columns: new[] { "LocationId", "MovieId", "ShootingDays" },
                values: new object[,]
                {
                    { 1, 1, 24 },
                    { 2, 1, 9 },
                    { 4, 1, 31 },
                    { 4, 2, 18 },
                    { 2, 3, 27 },
                    { 3, 4, 35 },
                    { 1, 5, 7 },
                    { 5, 5, 12 }
                });

            migrationBuilder.InsertData(
                table: "ProductionRecords",
                columns: new[] { "MovieId", "ProductionBudget", "ShootingEndDate", "ShootingStartDate" },
                values: new object[,]
                {
                    { 1, 48000000.00m, new DateTime(2022, 8, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 22500000.00m, new DateTime(2020, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 9800000.00m, new DateTime(2021, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 8, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 31000000.00m, new DateTime(2023, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 27400000.00m, null, new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Screenings",
                columns: new[] { "ScreeningId", "IsCancelled", "MovieId", "ScreenId", "StartsAt", "TicketPrice" },
                values: new object[,]
                {
                    { 1, false, 1, 1, new DateTime(2026, 9, 18, 19, 0, 0, 0, DateTimeKind.Unspecified), 125.00m },
                    { 2, false, 1, 2, new DateTime(2026, 9, 18, 21, 30, 0, 0, DateTimeKind.Unspecified), 110.00m },
                    { 3, false, 1, 4, new DateTime(2026, 9, 19, 18, 0, 0, 0, DateTimeKind.Unspecified), 140.00m },
                    { 4, false, 2, 1, new DateTime(2026, 9, 19, 16, 0, 0, 0, DateTimeKind.Unspecified), 95.00m },
                    { 5, false, 2, 6, new DateTime(2026, 9, 20, 20, 0, 0, 0, DateTimeKind.Unspecified), 90.00m },
                    { 6, false, 3, 3, new DateTime(2026, 9, 20, 17, 15, 0, 0, DateTimeKind.Unspecified), 85.00m },
                    { 7, false, 3, 5, new DateTime(2026, 9, 21, 19, 45, 0, 0, DateTimeKind.Unspecified), 105.00m },
                    { 8, false, 4, 4, new DateTime(2026, 9, 21, 20, 30, 0, 0, DateTimeKind.Unspecified), 135.00m },
                    { 9, false, 4, 1, new DateTime(2026, 9, 22, 18, 30, 0, 0, DateTimeKind.Unspecified), 130.00m },
                    { 10, false, 4, 2, new DateTime(2026, 10, 2, 19, 0, 0, 0, DateTimeKind.Unspecified), 115.00m },
                    { 11, false, 1, 6, new DateTime(2026, 11, 6, 19, 0, 0, 0, DateTimeKind.Unspecified), 120.00m },
                    { 12, false, 4, 5, new DateTime(2026, 11, 14, 20, 0, 0, 0, DateTimeKind.Unspecified), 125.00m },
                    { 13, false, 2, 3, new DateTime(2026, 12, 4, 18, 30, 0, 0, DateTimeKind.Unspecified), 95.00m },
                    { 14, false, 3, 4, new DateTime(2027, 1, 15, 19, 30, 0, 0, DateTimeKind.Unspecified), 110.00m }
                });

            migrationBuilder.InsertData(
                table: "TicketSales",
                columns: new[] { "TicketSaleId", "PricePerTicket", "SaleDateTime", "ScreeningId", "TicketCount" },
                values: new object[,]
                {
                    { 1, 125.00m, new DateTime(2026, 9, 10, 12, 4, 0, 0, DateTimeKind.Unspecified), 1, 2 },
                    { 2, 125.00m, new DateTime(2026, 9, 11, 18, 22, 0, 0, DateTimeKind.Unspecified), 1, 4 },
                    { 3, 125.00m, new DateTime(2026, 9, 14, 9, 47, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 4, 110.00m, new DateTime(2026, 9, 12, 14, 10, 0, 0, DateTimeKind.Unspecified), 2, 3 },
                    { 5, 110.00m, new DateTime(2026, 9, 16, 20, 35, 0, 0, DateTimeKind.Unspecified), 2, 2 },
                    { 6, 140.00m, new DateTime(2026, 9, 13, 11, 28, 0, 0, DateTimeKind.Unspecified), 3, 6 },
                    { 7, 140.00m, new DateTime(2026, 9, 17, 13, 2, 0, 0, DateTimeKind.Unspecified), 3, 2 },
                    { 8, 95.00m, new DateTime(2026, 9, 14, 15, 55, 0, 0, DateTimeKind.Unspecified), 4, 2 },
                    { 9, 90.00m, new DateTime(2026, 9, 15, 10, 19, 0, 0, DateTimeKind.Unspecified), 5, 5 },
                    { 10, 85.00m, new DateTime(2026, 9, 16, 16, 40, 0, 0, DateTimeKind.Unspecified), 6, 3 },
                    { 11, 105.00m, new DateTime(2026, 9, 18, 12, 11, 0, 0, DateTimeKind.Unspecified), 7, 4 },
                    { 12, 135.00m, new DateTime(2026, 9, 19, 17, 26, 0, 0, DateTimeKind.Unspecified), 8, 8 },
                    { 13, 135.00m, new DateTime(2026, 9, 20, 9, 33, 0, 0, DateTimeKind.Unspecified), 8, 2 },
                    { 14, 130.00m, new DateTime(2026, 9, 20, 19, 58, 0, 0, DateTimeKind.Unspecified), 9, 3 },
                    { 15, 115.00m, new DateTime(2026, 9, 28, 14, 45, 0, 0, DateTimeKind.Unspecified), 10, 2 },
                    { 16, 120.00m, new DateTime(2026, 9, 27, 10, 5, 0, 0, DateTimeKind.Unspecified), 11, 2 },
                    { 17, 125.00m, new DateTime(2026, 9, 28, 16, 40, 0, 0, DateTimeKind.Unspecified), 12, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieActors_ActorId",
                table: "MovieActors",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieLocations_LocationId",
                table: "MovieLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_DirectorId",
                table: "Movies",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_ProductionCompanyId",
                table: "Movies",
                column: "ProductionCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Screenings_MovieId",
                table: "Screenings",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Screenings_ScreenId",
                table: "Screenings",
                column: "ScreenId");

            migrationBuilder.CreateIndex(
                name: "IX_Screens_CinemaId_ScreenNumberOrName",
                table: "Screens",
                columns: new[] { "CinemaId", "ScreenNumberOrName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Studios_ProductionCompanyId",
                table: "Studios",
                column: "ProductionCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSales_ScreeningId",
                table: "TicketSales",
                column: "ScreeningId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieActors");

            migrationBuilder.DropTable(
                name: "MovieLocations");

            migrationBuilder.DropTable(
                name: "ProductionRecords");

            migrationBuilder.DropTable(
                name: "Studios");

            migrationBuilder.DropTable(
                name: "TicketSales");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Screenings");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Screens");

            migrationBuilder.DropTable(
                name: "Directors");

            migrationBuilder.DropTable(
                name: "ProductionCompanies");

            migrationBuilder.DropTable(
                name: "Cinemas");
        }
    }
}
