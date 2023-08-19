using DatabaseLogTest.Models;
using Dumpify;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using System.Data;

IConfiguration configuration = default!;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services => services.AddDbContext<LogEntryDbContext>())
    .ConfigureAppConfiguration(configurationBuilder =>
    {
        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
        configurationBuilder.AddJsonFile("appsettings.json", optional: true);
        configurationBuilder.AddCommandLine(args);
        configurationBuilder.AddUserSecrets<Program>(); //Connectionstring is located in the user secrets
        configuration = configurationBuilder.Build(); //Builds and sets the configuration for later use
    })
    .ConfigureLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        loggingBuilder.AddNLog(CreateLogConfiguration(configuration));
    })
    .Build();

ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogTrace("Logging just works");
logger.LogInformation("Can also log with multiple loglevels");

LogWithException(logger);
var logEntries = host.Services.GetRequiredService<LogEntryDbContext>();

logEntries.LogEntries.OrderBy(x => x.DateTime).Last().Dump();

void LogWithException(ILogger<Program> logger)
{
    try
    {
        logger.LogTrace("Working carefully...");
        throw new Exception("Work failed");
    }
    catch (Exception e)
    {
        logger.LogError(exception: e, message: e.Message);
    }
}


static LoggingConfiguration CreateLogConfiguration(IConfiguration configuration)
{
    var logConfig = new LoggingConfiguration();

    var consoleTarget = new ConsoleTarget
    {
        Name = "ConsoleTarget",
        Layout = "${longdate} | ${level:uppercase=true} | ${logger} | ${message} | ${exception}",
        WriteBuffer = true,
        DetectConsoleAvailable = true,
    };
    logConfig.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, consoleTarget);
    logConfig.AddTarget(consoleTarget);

    var connectionString = configuration["ConnectionStrings:LogDatabase"];
    if (string.IsNullOrEmpty(connectionString)) throw new Exception("You need to set connectionstring for the database in order to configure the database logging");

    DatabaseTarget dbTarget = CreateDatabaseTarget(connectionString);

    logConfig.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, dbTarget);
    logConfig.AddTarget(dbTarget);

    //This will make NLog throw excpetions for errors in log-configuration.
    //This should be false on a production environment
    LogManager.ThrowExceptions = true;
    LogManager.ThrowConfigExceptions = true;
    return logConfig;
}

static DatabaseTarget CreateDatabaseTarget(string connectionString)
{
    DatabaseParameterInfo param;
    var dbTarget = new DatabaseTarget()
    {
        Name = "DatabaseTarget",
        ConnectionString = connectionString,
        CommandText = "AddLog",
        CommandType = CommandType.StoredProcedure
    };
    param = new DatabaseParameterInfo
    {
        Name = "@id",
        Layout = "${guid}",
        DbType = "Guid"
    };
    dbTarget.Parameters.Add(param);
    param = new DatabaseParameterInfo
    {
        Name = "@logtime",
        Layout = "${date}"
    };
    dbTarget.Parameters.Add(param);
    param = new DatabaseParameterInfo
    {
        Name = "@level",
        Layout = "${level:uppercase=true}"
    };
    dbTarget.Parameters.Add(param);
    param = new DatabaseParameterInfo
    {
        Name = "@source",
        Layout = "${logger}"
    };
    dbTarget.Parameters.Add(param);
    param = new DatabaseParameterInfo
    {
        Name = "@message",
        Layout = "${message}"
    };
    dbTarget.Parameters.Add(param);
    param = new DatabaseParameterInfo
    {
        Name = "@exception",
        Layout = "${exception}"
    };
    dbTarget.Parameters.Add(param);
    return dbTarget;
}