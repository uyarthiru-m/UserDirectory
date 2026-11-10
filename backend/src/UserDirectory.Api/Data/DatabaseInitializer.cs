using Microsoft.EntityFrameworkCore;

namespace UserDirectory.Api.Data;

public static class DatabaseInitializer
{
    public static void Initialize(AppDbContext database)
    {
        database.Database.EnsureCreated();

        // Add account storage to databases created before local authentication was introduced.
        database.Database.ExecuteSqlRaw(
            """
            CREATE TABLE IF NOT EXISTS "LocalAccounts" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_LocalAccounts" PRIMARY KEY AUTOINCREMENT,
                "Email" TEXT NOT NULL,
                "NormalizedEmail" TEXT NOT NULL,
                "PasswordHash" TEXT NOT NULL,
                "CreatedAtUtc" TEXT NOT NULL
            );
            """);
        database.Database.ExecuteSqlRaw(
            """CREATE UNIQUE INDEX IF NOT EXISTS "IX_LocalAccounts_NormalizedEmail" ON "LocalAccounts" ("NormalizedEmail");""");
    }
}
