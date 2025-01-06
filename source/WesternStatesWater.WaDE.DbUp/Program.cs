using DbUp;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using WesternStatesWater.WaDE.Common;

namespace WesternStatesWater.WaDE.DbUp
{
    class Program
    {
        static IConfiguration Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "ConnectionStrings:WadeDatabase", "Server=localhost;Initial Catalog=WaDE2Test;TrustServerCertificate=True;User=sa;Password=DevP@ssw0rd!;Encrypt=False;" }
                    })
                    .AddUserSecrets("0233c5d4-6e7e-4ba4-997b-313518edcce4")
                    .AddEnvironmentVariables();

                return builder.Build();
            }
        }

        static void Main(string[] args)
        {
            (var connectionString, var rebuild, var force) = ParseParameters(args);

            //if no connection was provided, check the environment variable
            connectionString = string.IsNullOrEmpty(connectionString)
                ? Configuration["ConnectionStrings:WadeDatabase"]
                : connectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new WaDEException("Connection string not found.");
            }

            EnsureDatabase.For.SqlDatabase(connectionString);

            if (rebuild)
            {
                ClearDb(connectionString, force);
            }

            UpdateDb(connectionString);

            return;
        }

        private static (string ConnectionString, bool Rebuild, bool Force) ParseParameters(string[] args)
        {
            var connectionString = string.Empty;
            var rebuild = false;
            var force = false;

            if (args != null &&
                args.Length > 0)
            {
                rebuild = args[0].Equals("rebuild", StringComparison.InvariantCultureIgnoreCase);

                if (args.Length > 1)
                {
                    connectionString = args[1];

                    if (args.Length > 2)
                    {
                        force = args[2].Equals("force", StringComparison.InvariantCultureIgnoreCase);
                    }
                }
            }

            return (connectionString, rebuild, force);
        }

        private static void ClearDb(string connectionString, bool force)
        {
            var doClear = true;

            if (!force &&
                Environment.UserInteractive)
            {
                //ask user if they really want this
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("###User Interactive Mode Question###");
                Console.WriteLine($"Do you want to rebuild the database ({connectionString})? yes/no");
                Console.ResetColor();

                var response = Console.ReadLine();

                if (response.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                {
                    doClear = true;
                }
                else
                {
                    doClear = false;
                }

                Console.WriteLine($"doClear: {doClear}");
            }

            if (doClear)
            {
                Console.WriteLine($"Clearing database: {connectionString}");

                ClearSchema(connectionString, "Core");
                ClearSchema(connectionString, "CVs");
                ClearSchema(connectionString, "Input");
                ClearSchema(connectionString, "dbo");

                Console.WriteLine("Database cleared.");
            }
            else
            {
                Console.WriteLine("Database not cleared.");
            }
        }

        private static void ClearSchema(string connectionString, string schema)
        {
            Console.WriteLine($"Clearing schema: {schema}");

            // script came from http://stackoverflow.com/a/32776552
            var dropProcedure = @"IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'spDropSchema')
                BEGIN
                    DROP PROCEDURE spDropSchema;
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
                ORDER BY NAME;

                --column master keys
                SELECT @Sql = @Sql + 'DROP COLUMN MASTER KEY ' + QUOTENAME(NAME) + ';' + CHAR(13)
                FROM sys.column_master_keys
                ORDER BY NAME;

                --user defined types
                SELECT @Sql = @Sql + 'DROP TYPE '+ QUOTENAME(s.Name) +'.' + QUOTENAME(t.NAME) + ';' + CHAR(13)
                FROM sys.types t inner join
                     sys.schemas s on t.schema_id = s.schema_id
                WHERE t.is_user_defined = 1;

                EXECUTE sp_executesql @Sql;
                ";

            var execProcedure = $"exec spDropSchema '{schema}';";

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
        }

        private static void UpdateDb(string connectionString)
        {
            var migrator = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithTransaction()
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .WithExecutionTimeout(TimeSpan.FromSeconds(3600))
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
        }
    }
}