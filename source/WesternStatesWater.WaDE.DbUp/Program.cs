using DbUp;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Reflection;

namespace WesternStatesWater.WaDE.DbUp
{
    class Program
    {
        static IConfiguration Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .AddEnvironmentVariables();

                return builder.Build();
            }
        }

        static void Main(string[] args)
        {
            var connectionString = Configuration["WadeDatabase"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string environment variable missing.");
            }

            EnsureDatabase.For.SqlDatabase(connectionString);

            if (args.Length > 0 &&
                args[0].Equals("rebuild", StringComparison.InvariantCultureIgnoreCase))
            {
                ClearDb(connectionString);
            }

            UpdateDb(connectionString);

            return;
        }

        private static void ClearDb(string connectionString)
        {
            // script came from http://stackoverflow.com/a/32776552
            var dropProcedure = @"IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'spDropSchema')
                BEGIN
                    DROP  PROCEDURE  spDropSchema;
                END";

            var createProcedure = @"CREATE PROCEDURE spDropSchema(@Schema nvarchar(200))
                AS
                DECLARE @Sql NVARCHAR(MAX) = '';
                            
                --constraints
                SELECT @Sql = @Sql + 'ALTER TABLE '+ QUOTENAME(@Schema) + '.' + QUOTENAME(t.name) + ' DROP CONSTRAINT ' + QUOTENAME(f.name)  + ';' + CHAR(13)
                FROM sys.tables t 
                    inner join sys.foreign_keys f on f.parent_object_id = t.object_id 
                    inner join sys.schemas s on t.schema_id = s.schema_id
                WHERE s.name = @Schema
                ORDER BY t.name;
                            
                --tables
                SELECT @Sql = @Sql + 'DROP TABLE '+ QUOTENAME(@Schema) +'.' + QUOTENAME(TABLE_NAME) + ';' + CHAR(13)
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = @Schema AND TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_NAME;
                            
                --views
                SELECT @Sql = @Sql + 'DROP VIEW '+ QUOTENAME(@Schema) +'.' + QUOTENAME(TABLE_NAME) + ';' + CHAR(13)
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = @Schema AND TABLE_TYPE = 'VIEW'
                ORDER BY TABLE_NAME;
                            
                --procedures
                SELECT @Sql = @Sql + 'DROP PROCEDURE '+ QUOTENAME(@Schema) +'.' + QUOTENAME(ROUTINE_NAME) + ';' + CHAR(13)
                FROM INFORMATION_SCHEMA.ROUTINES
                WHERE ROUTINE_SCHEMA = @Schema AND ROUTINE_TYPE = 'PROCEDURE'
                ORDER BY ROUTINE_NAME;
                            
                --functions
                SELECT @Sql = @Sql + 'DROP FUNCTION '+ QUOTENAME(@Schema) +'.' + QUOTENAME(ROUTINE_NAME) + ';' + CHAR(13)
                FROM INFORMATION_SCHEMA.ROUTINES
                WHERE ROUTINE_SCHEMA = @Schema AND ROUTINE_TYPE = 'FUNCTION'
                ORDER BY ROUTINE_NAME;
                            
                --sequences
                SELECT @Sql = @Sql + 'DROP SEQUENCE '+ QUOTENAME(@Schema) +'.' + QUOTENAME(SEQUENCE_NAME) + ';' + CHAR(13)
                FROM INFORMATION_SCHEMA.SEQUENCES
                WHERE SEQUENCE_SCHEMA = @Schema
                ORDER BY SEQUENCE_NAME;

                --column encryption keys
                SELECT @Sql = @Sql + 'DROP COLUMN ENCRYPTION KEY ' + QUOTENAME(NAME) + ';' + CHAR(13)
                FROM sys.column_encryption_keys
                ORDER BY NAME

                --column master keys
                SELECT @Sql = @Sql + 'DROP COLUMN MASTER KEY ' + QUOTENAME(NAME) + ';' + CHAR(13)
                FROM sys.column_master_keys
                ORDER BY NAME
                EXECUTE sp_executesql @Sql;
                ";

            var execProcedure = @"exec spDropSchema 'dbo';";

            Console.WriteLine($"Clearing database: {connectionString}");

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(dropProcedure, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand(createProcedure, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand(execProcedure, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand(dropProcedure, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Database cleared");
        }

        private static void UpdateDb(string connectionString)
        {
            var migrator = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithTransaction()
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .WithExecutionTimeout(TimeSpan.FromSeconds(300))
                .LogToConsole()
                .Build();

            var result = migrator.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();

                if (Environment.UserInteractive)
                {
                    Console.ReadLine();
                }

                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();

            if (Environment.UserInteractive)
            {
                Console.ReadLine();
            }
        }
    }
}
