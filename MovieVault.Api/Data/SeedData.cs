using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Domain.Models;

namespace MovieVault.Api.Data;

public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Director>().HasData(
            new Director { DirectorId = 1, Name = "Greta Halvorsen", DateOfBirth = new DateTime(1975, 4, 12), Nationality = "Norwegian" },
            new Director { DirectorId = 2, Name = "Marcus Webb", DateOfBirth = new DateTime(1962, 11, 30), Nationality = "British" },
            new Director { DirectorId = 3, Name = "Aiko Tanaka", DateOfBirth = new DateTime(1980, 7, 3), Nationality = "Japanese" }
        );

        modelBuilder.Entity<ProductionCompany>().HasData(
            new ProductionCompany { ProductionCompanyId = 1, Name = "Northlight Pictures", FoundingYear = 1994, Country = "USA" },
            new ProductionCompany { ProductionCompanyId = 2, Name = "Harbour Row Films", FoundingYear = 2003, Country = "United Kingdom" },
            new ProductionCompany { ProductionCompanyId = 3, Name = "Kitsune Studios", FoundingYear = 1988, Country = "Japan" }
        );

        modelBuilder.Entity<Studio>().HasData(
            new Studio { StudioId = 1, ProductionCompanyId = 1, Name = "Northlight Stage A", City = "Los Angeles", Country = "USA", SoundStages = 6, MaxStageCapacity = 400 },
            new Studio { StudioId = 2, ProductionCompanyId = 1, Name = "Northlight Stage B", City = "Atlanta", Country = "USA", SoundStages = 4, MaxStageCapacity = 250 },
            new Studio { StudioId = 3, ProductionCompanyId = 2, Name = "Harbour Row Docks", City = "London", Country = "United Kingdom", SoundStages = 3, MaxStageCapacity = 180 },
            new Studio { StudioId = 4, ProductionCompanyId = 3, Name = "Kitsune Main Lot", City = "Kyoto", Country = "Japan", SoundStages = 5, MaxStageCapacity = 320 }
        );

        modelBuilder.Entity<Actor>().HasData(
            new Actor { ActorId = 1, Name = "Elena Ruiz", DateOfBirth = new DateTime(1988, 2, 19), Nationality = "Spanish", Gender = "Female" },
            new Actor { ActorId = 2, Name = "Tom Brannigan", DateOfBirth = new DateTime(1979, 9, 5), Nationality = "Irish", Gender = "Male" },
            new Actor { ActorId = 3, Name = "Priya Nair", DateOfBirth = new DateTime(1992, 12, 11), Nationality = "Indian", Gender = "Female" },
            new Actor { ActorId = 4, Name = "Karl Ostermann", DateOfBirth = new DateTime(1968, 6, 23), Nationality = "German", Gender = "Male" },
            new Actor { ActorId = 5, Name = "Mei Chen", DateOfBirth = new DateTime(1995, 3, 8), Nationality = "Chinese", Gender = "Female" },
            new Actor { ActorId = 6, Name = "Dario Fontana", DateOfBirth = new DateTime(1983, 10, 27), Nationality = "Italian", Gender = "Male" },
            new Actor { ActorId = 7, Name = "Sofia Lindqvist", DateOfBirth = new DateTime(1990, 5, 14), Nationality = "Swedish", Gender = "Female" },
            new Actor { ActorId = 8, Name = "Owen Blackwood", DateOfBirth = new DateTime(1972, 1, 16), Nationality = "Canadian", Gender = "Male" }
        );

        modelBuilder.Entity<Location>().HasData(
            new Location { LocationId = 1, Name = "Old Harbour Pier", City = "Bergen", Country = "Norway", LocationType = "street" },
            new Location { LocationId = 2, Name = "Grand Central Library", City = "London", Country = "United Kingdom", LocationType = "building" },
            new Location { LocationId = 3, Name = "Kiyomizu Hillside", City = "Kyoto", Country = "Japan", LocationType = "landscape" },
            new Location { LocationId = 4, Name = "Backlot Village Set", City = "Los Angeles", Country = "USA", LocationType = "set" },
            new Location { LocationId = 5, Name = "Ponte Vecchio Approach", City = "Florence", Country = "Italy", LocationType = "street" }
        );

        modelBuilder.Entity<Cinema>().HasData(
            new Cinema { CinemaId = 1, Name = "Palace Cinema", City = "Aarhus", StreetAddress = "Skolegade 12", OpeningYear = 1954 },
            new Cinema { CinemaId = 2, Name = "Metro Screens", City = "Copenhagen", StreetAddress = "Vesterbrogade 88", OpeningYear = 1998 },
            new Cinema { CinemaId = 3, Name = "Kino Nord", City = "Aalborg", StreetAddress = "Boulevarden 4", OpeningYear = 2011 }
        );

        modelBuilder.Entity<Screen>().HasData(
            new Screen { ScreenId = 1, CinemaId = 1, ScreenNumberOrName = "Screen 1", SeatingCapacity = 220, ScreenType = "IMAX" },
            new Screen { ScreenId = 2, CinemaId = 1, ScreenNumberOrName = "Screen 2", SeatingCapacity = 140, ScreenType = "Standard" },
            new Screen { ScreenId = 3, CinemaId = 1, ScreenNumberOrName = "Screen 3", SeatingCapacity = 90, ScreenType = "Standard" },
            new Screen { ScreenId = 4, CinemaId = 2, ScreenNumberOrName = "Hall A", SeatingCapacity = 300, ScreenType = "IMAX" },
            new Screen { ScreenId = 5, CinemaId = 2, ScreenNumberOrName = "Hall B", SeatingCapacity = 160, ScreenType = "3D" },
            new Screen { ScreenId = 6, CinemaId = 3, ScreenNumberOrName = "Screen 1", SeatingCapacity = 120, ScreenType = "Standard" }
        );

        modelBuilder.Entity<Movie>().HasData(
            new Movie { MovieId = 1, DirectorId = 1, ProductionCompanyId = 1, Title = "The Winter Crossing", ReleaseDate = new DateTime(2023, 11, 17), RuntimeMinutes = 138, AgeRating = "12", Status = "released" },
            new Movie { MovieId = 2, DirectorId = 1, ProductionCompanyId = 1, Title = "Salt and Iron", ReleaseDate = new DateTime(2021, 6, 4), RuntimeMinutes = 121, AgeRating = "15", Status = "released" },
            new Movie { MovieId = 3, DirectorId = 2, ProductionCompanyId = 2, Title = "A Quiet Inheritance", ReleaseDate = new DateTime(2022, 9, 23), RuntimeMinutes = 109, AgeRating = "7", Status = "released" },
            new Movie { MovieId = 4, DirectorId = 3, ProductionCompanyId = 3, Title = "Lanterns Over Kyoto", ReleaseDate = new DateTime(2024, 2, 9), RuntimeMinutes = 126, AgeRating = "12", Status = "released" },
            new Movie { MovieId = 5, DirectorId = 2, ProductionCompanyId = 3, Title = "The Longest Night", RuntimeMinutes = 115, AgeRating = "15", Status = "in production" },
            new Movie { MovieId = 6, Title = "The Glass Orchard", Status = "planned" }
        );

        modelBuilder.Entity<ProductionRecord>().HasData(
            new ProductionRecord { MovieId = 1, ProductionBudget = 48000000.00m, ShootingStartDate = new DateTime(2022, 3, 1), ShootingEndDate = new DateTime(2022, 8, 19) },
            new ProductionRecord { MovieId = 2, ProductionBudget = 22500000.00m, ShootingStartDate = new DateTime(2020, 5, 11), ShootingEndDate = new DateTime(2020, 9, 30) },
            new ProductionRecord { MovieId = 3, ProductionBudget = 9800000.00m, ShootingStartDate = new DateTime(2021, 8, 2), ShootingEndDate = new DateTime(2021, 11, 12) },
            new ProductionRecord { MovieId = 4, ProductionBudget = 31000000.00m, ShootingStartDate = new DateTime(2023, 1, 16), ShootingEndDate = new DateTime(2023, 6, 28) },
            new ProductionRecord { MovieId = 5, ProductionBudget = 27400000.00m, ShootingStartDate = new DateTime(2025, 2, 3) }
        );

        modelBuilder.Entity<MovieActor>().HasData(
            new MovieActor { MovieId = 1, ActorId = 1, CharacterName = "Ingrid Solberg", RoleType = "lead" },
            new MovieActor { MovieId = 1, ActorId = 2, CharacterName = "Captain Hale", RoleType = "lead" },
            new MovieActor { MovieId = 1, ActorId = 4, CharacterName = "Dr. Neumann", RoleType = "supporting" },
            new MovieActor { MovieId = 1, ActorId = 7, CharacterName = "Young Ingrid", RoleType = "supporting" },
            new MovieActor { MovieId = 1, ActorId = 6, CharacterName = "Ferry Passenger", RoleType = "cameo" },
            new MovieActor { MovieId = 2, ActorId = 1, CharacterName = "Marisol", RoleType = "lead" },
            new MovieActor { MovieId = 2, ActorId = 8, CharacterName = "Sheriff Doyle", RoleType = "supporting" },
            new MovieActor { MovieId = 3, ActorId = 3, CharacterName = "Anjali Rao", RoleType = "lead" },
            new MovieActor { MovieId = 3, ActorId = 4, CharacterName = "Mr. Pike", RoleType = "supporting" },
            new MovieActor { MovieId = 4, ActorId = 5, CharacterName = "Hana", RoleType = "lead" },
            new MovieActor { MovieId = 4, ActorId = 3, CharacterName = "Vaidehi", RoleType = "supporting" },
            new MovieActor { MovieId = 5, ActorId = 2, CharacterName = "Unnamed Soldier", RoleType = "lead" },
            new MovieActor { MovieId = 5, ActorId = 6, CharacterName = "Luca", RoleType = "supporting" }
        );

        modelBuilder.Entity<MovieLocation>().HasData(
            new MovieLocation { MovieId = 1, LocationId = 1, ShootingDays = 24 },
            new MovieLocation { MovieId = 1, LocationId = 4, ShootingDays = 31 },
            new MovieLocation { MovieId = 1, LocationId = 2, ShootingDays = 9 },
            new MovieLocation { MovieId = 2, LocationId = 4, ShootingDays = 18 },
            new MovieLocation { MovieId = 3, LocationId = 2, ShootingDays = 27 },
            new MovieLocation { MovieId = 4, LocationId = 3, ShootingDays = 35 },
            new MovieLocation { MovieId = 5, LocationId = 5, ShootingDays = 12 },
            new MovieLocation { MovieId = 5, LocationId = 1, ShootingDays = 7 }
        );

        modelBuilder.Entity<Screening>().HasData(
            new Screening { ScreeningId = 1, MovieId = 1, ScreenId = 1, StartsAt = new DateTime(2026, 9, 18, 19, 0, 0), TicketPrice = 125.00m },
            new Screening { ScreeningId = 2, MovieId = 1, ScreenId = 2, StartsAt = new DateTime(2026, 9, 18, 21, 30, 0), TicketPrice = 110.00m },
            new Screening { ScreeningId = 3, MovieId = 1, ScreenId = 4, StartsAt = new DateTime(2026, 9, 19, 18, 0, 0), TicketPrice = 140.00m },
            new Screening { ScreeningId = 4, MovieId = 2, ScreenId = 1, StartsAt = new DateTime(2026, 9, 19, 16, 0, 0), TicketPrice = 95.00m },
            new Screening { ScreeningId = 5, MovieId = 2, ScreenId = 6, StartsAt = new DateTime(2026, 9, 20, 20, 0, 0), TicketPrice = 90.00m },
            new Screening { ScreeningId = 6, MovieId = 3, ScreenId = 3, StartsAt = new DateTime(2026, 9, 20, 17, 15, 0), TicketPrice = 85.00m },
            new Screening { ScreeningId = 7, MovieId = 3, ScreenId = 5, StartsAt = new DateTime(2026, 9, 21, 19, 45, 0), TicketPrice = 105.00m },
            new Screening { ScreeningId = 8, MovieId = 4, ScreenId = 4, StartsAt = new DateTime(2026, 9, 21, 20, 30, 0), TicketPrice = 135.00m },
            new Screening { ScreeningId = 9, MovieId = 4, ScreenId = 1, StartsAt = new DateTime(2026, 9, 22, 18, 30, 0), TicketPrice = 130.00m },
            new Screening { ScreeningId = 10, MovieId = 4, ScreenId = 2, StartsAt = new DateTime(2026, 10, 2, 19, 0, 0), TicketPrice = 115.00m },
            new Screening { ScreeningId = 11, MovieId = 1, ScreenId = 6, StartsAt = new DateTime(2026, 11, 6, 19, 0, 0), TicketPrice = 120.00m },
            new Screening { ScreeningId = 12, MovieId = 4, ScreenId = 5, StartsAt = new DateTime(2026, 11, 14, 20, 0, 0), TicketPrice = 125.00m },
            new Screening { ScreeningId = 13, MovieId = 2, ScreenId = 3, StartsAt = new DateTime(2026, 12, 4, 18, 30, 0), TicketPrice = 95.00m },
            new Screening { ScreeningId = 14, MovieId = 3, ScreenId = 4, StartsAt = new DateTime(2027, 1, 15, 19, 30, 0), TicketPrice = 110.00m }
        );

        modelBuilder.Entity<TicketSale>().HasData(
            new TicketSale { TicketSaleId = 1, ScreeningId = 1, SaleDateTime = new DateTime(2026, 9, 10, 12, 4, 0), TicketCount = 2, PricePerTicket = 125.00m },
            new TicketSale { TicketSaleId = 2, ScreeningId = 1, SaleDateTime = new DateTime(2026, 9, 11, 18, 22, 0), TicketCount = 4, PricePerTicket = 125.00m },
            new TicketSale { TicketSaleId = 3, ScreeningId = 1, SaleDateTime = new DateTime(2026, 9, 14, 9, 47, 0), TicketCount = 1, PricePerTicket = 125.00m },
            new TicketSale { TicketSaleId = 4, ScreeningId = 2, SaleDateTime = new DateTime(2026, 9, 12, 14, 10, 0), TicketCount = 3, PricePerTicket = 110.00m },
            new TicketSale { TicketSaleId = 5, ScreeningId = 2, SaleDateTime = new DateTime(2026, 9, 16, 20, 35, 0), TicketCount = 2, PricePerTicket = 110.00m },
            new TicketSale { TicketSaleId = 6, ScreeningId = 3, SaleDateTime = new DateTime(2026, 9, 13, 11, 28, 0), TicketCount = 6, PricePerTicket = 140.00m },
            new TicketSale { TicketSaleId = 7, ScreeningId = 3, SaleDateTime = new DateTime(2026, 9, 17, 13, 2, 0), TicketCount = 2, PricePerTicket = 140.00m },
            new TicketSale { TicketSaleId = 8, ScreeningId = 4, SaleDateTime = new DateTime(2026, 9, 14, 15, 55, 0), TicketCount = 2, PricePerTicket = 95.00m },
            new TicketSale { TicketSaleId = 9, ScreeningId = 5, SaleDateTime = new DateTime(2026, 9, 15, 10, 19, 0), TicketCount = 5, PricePerTicket = 90.00m },
            new TicketSale { TicketSaleId = 10, ScreeningId = 6, SaleDateTime = new DateTime(2026, 9, 16, 16, 40, 0), TicketCount = 3, PricePerTicket = 85.00m },
            new TicketSale { TicketSaleId = 11, ScreeningId = 7, SaleDateTime = new DateTime(2026, 9, 18, 12, 11, 0), TicketCount = 4, PricePerTicket = 105.00m },
            new TicketSale { TicketSaleId = 12, ScreeningId = 8, SaleDateTime = new DateTime(2026, 9, 19, 17, 26, 0), TicketCount = 8, PricePerTicket = 135.00m },
            new TicketSale { TicketSaleId = 13, ScreeningId = 8, SaleDateTime = new DateTime(2026, 9, 20, 9, 33, 0), TicketCount = 2, PricePerTicket = 135.00m },
            new TicketSale { TicketSaleId = 14, ScreeningId = 9, SaleDateTime = new DateTime(2026, 9, 20, 19, 58, 0), TicketCount = 3, PricePerTicket = 130.00m },
            new TicketSale { TicketSaleId = 15, ScreeningId = 10, SaleDateTime = new DateTime(2026, 9, 28, 14, 45, 0), TicketCount = 2, PricePerTicket = 115.00m },
            new TicketSale { TicketSaleId = 16, ScreeningId = 11, SaleDateTime = new DateTime(2026, 9, 27, 10, 5, 0), TicketCount = 2, PricePerTicket = 120.00m },
            new TicketSale { TicketSaleId = 17, ScreeningId = 12, SaleDateTime = new DateTime(2026, 9, 28, 16, 40, 0), TicketCount = 3, PricePerTicket = 125.00m }
        );
    }
}
