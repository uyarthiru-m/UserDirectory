using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UserDirectory.Api.Data;
using UserDirectory.Api.Models;

namespace UserDirectory.Tests;

public sealed class DatabaseInitializerTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    [Fact]
    public async Task Initialize_AddsLocalAccountTableToExistingUserDatabase()
    {
        await _connection.OpenAsync();
        await using (var command = _connection.CreateCommand())
        {
            command.CommandText =
                """
                CREATE TABLE "Users" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
                    "Name" TEXT NOT NULL,
                    "Age" INTEGER NOT NULL,
                    "City" TEXT NOT NULL,
                    "State" TEXT NOT NULL,
                    "Pincode" TEXT NOT NULL
                );
                """;
            await command.ExecuteNonQueryAsync();
        }

        await using var database = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options);
        DatabaseInitializer.Initialize(database);
        DatabaseInitializer.Initialize(database);

        database.LocalAccounts.Add(new LocalAccount
        {
            Email = "test@example.com",
            NormalizedEmail = "TEST@EXAMPLE.COM",
            PasswordHash = "test-hash",
            CreatedAtUtc = DateTime.UtcNow
        });
        await database.SaveChangesAsync();

        Assert.Equal(1, await database.LocalAccounts.CountAsync());
        Assert.False(await database.Users.AnyAsync());
    }

    public void Dispose() => _connection.Dispose();
}
