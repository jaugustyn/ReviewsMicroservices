using Core.Entities;
using Core.Entities.Models;
using Core.Enumerations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Services.Services;

namespace Infrastructure.Data;

public static class SeedDatabase<T> where T : class, IEntityBase
{
    public static void Seed(IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));

        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<IMongoDbContext>();

            if (typeof(T) == typeof(User))
                SeedUsers(context.Users);
            else if (typeof(T) == typeof(Review))
                SeedReviews(context.Reviews);
            else if (typeof(T) == typeof(Rating))
                SeedReviewRatings(context.Ratings);
            else if (typeof(T) == typeof(Comment)) SeedComments(context.Comments);
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine("Cannot seed database. Timeout. Check database connection.\n" + ex);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    private static void SeedUsers(IMongoCollection<User> collection)
    {
        if (!collection.AsQueryable().Any()) collection.InsertMany(GetSeedUsers());
    }

    private static void SeedReviews(IMongoCollection<Review> collection)
    {
        if (!collection.AsQueryable().Any()) collection.InsertMany(GetSeedReviews());
    }

    private static void SeedReviewRatings(IMongoCollection<Rating> collection)
    {
        if (!collection.AsQueryable().Any()) collection.InsertMany(GetSeedReviewRatings());
    }

    private static void SeedComments(IMongoCollection<Comment> collection)
    {
        if (!collection.AsQueryable().Any()) collection.InsertMany(GetSeedComments());
    }

    private static IEnumerable<Comment> GetSeedComments()
    {
        return new List<Comment>()
        {
            new()
            {
                Id = new Guid(),
                UserId = new Guid("b679ab75-976d-4584-a1fc-e95bd65b89e5"),
                ReviewId = new Guid("d0adc59c-cf85-44e7-86c2-aa8c76ee4c64"),
                CreatedAt = DateTimeOffset.Now,
                Text = "Komentarz 1"
            },
            new()
            {
                Id = new Guid(),
                UserId = new Guid("b679ab75-976d-4584-a1fc-e95bd65b89e5"),
                ReviewId = new Guid("d0adc59c-cf85-44e7-86c2-aa8c76ee4c64"),
                CreatedAt = DateTimeOffset.Now,
                Text = "Komentarz 2"
            },
            new()
            {
                Id = new Guid(),
                UserId = new Guid("ff4065ea-d922-4c6a-a5b1-0f33650c02d9"),
                ReviewId = new Guid("d0adc59c-cf85-44e7-86c2-aa8c76ee4c64"),
                CreatedAt = DateTimeOffset.Now,
                Text = "Komentarz 3"
            }
        };
    }

    private static IEnumerable<User> GetSeedUsers()
    {
        var passwordService = new PasswordService();
        var password = passwordService.HashPassword("zaq1@WSX", out var salt);

        return new List<User>()
        {
            new()
            {
                Id = new Guid("b679ab75-976d-4584-a1fc-e95bd65b89e5"),
                CreatedAt = DateTimeOffset.Now,
                Username = "Janek51236",
                Email = "user@example.pl",
                Role = Role.User,
                FirstName = "Janek",
                LastName = "Maj",
                Birthday = new DateTime(1999, 8, 13),
                PasswordHash = password,
                PasswordSalt = Convert.ToHexString(salt)
            },
            new()
            {
                Id = new Guid("ff4065ea-d922-4c6a-a5b1-0f33650c02d9"),
                CreatedAt = DateTimeOffset.Now,
                Username = "MaciekAdmin",
                Email = "admin@example.pl",
                Role = Role.Administrator,
                FirstName = "Maciek",
                LastName = "Marzec",
                Birthday = new DateTime(1996, 2, 21),
                PasswordHash = password,
                PasswordSalt = Convert.ToHexString(salt)
            }
        };
    }

    private static IEnumerable<Review> GetSeedReviews()
    {
        return new List<Review>()
        {
            new()
            {
                Id = new Guid("d0adc59c-cf85-44e7-86c2-aa8c76ee4c64"),
                CreatedAt = DateTimeOffset.Now,
                Title = "Ciekawa recenzja",
                Text = "Ciekawy content",
                UserId = new Guid("ff4065ea-d922-4c6a-a5b1-0f33650c02d9")
            },
            new()
            {
                Id = new Guid("06f97564-7c6c-4f31-8880-b608cdf34f40"),
                CreatedAt = DateTimeOffset.Now,
                Title = "Niezła recenzja",
                Text = "Niezły content",
                UserId = new Guid("b679ab75-976d-4584-a1fc-e95bd65b89e5")
            }
        };
    }

    private static IEnumerable<Rating> GetSeedReviewRatings()
    {
        return new List<Rating>()
        {
            new()
            {
                Id = new Guid(),
                ReviewId = new Guid("d0adc59c-cf85-44e7-86c2-aa8c76ee4c64"),
                UserId = new Guid("ff4065ea-d922-4c6a-a5b1-0f33650c02d9"),
                Value = 5
            },
            new()
            {
                Id = new Guid(),
                ReviewId = new Guid("d0adc59c-cf85-44e7-86c2-aa8c76ee4c64"),
                UserId = new Guid("b679ab75-976d-4584-a1fc-e95bd65b89e5"),
                Value = 1
            }
        };
    }
}