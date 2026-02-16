using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

namespace ZenBlog.API.SeriLog;

public static class SerilogSetup
{
    public static void AddAppSerilog(this WebApplicationBuilder builder)
    {
        var connectionString =
            builder.Configuration.GetConnectionString("ZenBlogConnection")
            ?? throw new InvalidOperationException("Connection string 'LetsSpeakLawConnection' don't found.");

        var columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            ["message"] = new RenderedMessageColumnWriter(NpgsqlDbType.Text),
            ["message_template"] = new MessageTemplateColumnWriter(NpgsqlDbType.Text),
            ["level"] = new LevelColumnWriter(true, NpgsqlDbType.Varchar),
            ["raise_date"] = new TimestampColumnWriter(NpgsqlDbType.Timestamp),
            ["exception"] = new ExceptionColumnWriter(NpgsqlDbType.Text),
            ["properties"] = new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb),
            ["machine_name"] = new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text)
        };

        builder.Host.UseSerilog((ctx, services, lc) => lc
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.PostgreSQL(
                connectionString: connectionString,
                tableName: "logs",
                columnOptions: columnWriters,
                needAutoCreateTable: true
            )
        );
    }
}
