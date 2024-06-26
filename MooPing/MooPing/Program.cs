using Carter;
using MooPing.Components;
using NpgsqlTypes;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using Serilog.Sinks.PostgreSQL;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Havit.Blazor.Components.Web;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using MooPing.AuthenticationStateSyncer;
using MooPing.Database;
using Microsoft.EntityFrameworkCore;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

#region Services
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();

builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.AddCssBundle("/css/site.css", "scss/site.scss");
    pipeline.MinifyJsFiles("js/**/*.js", "js/**/*.js");
});

builder.Services.AddDbContext<MooPingDbContext>(options =>
            options.UseNpgsql(builder.Configuration["MooPing:SupabaseConnectionString"]));

//Havit Blazor Services
builder.Services.AddHxServices();
builder.Services.AddHxMessenger();
builder.Services.AddHxMessageBoxHost();

//Auth0
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
#endregion

#region Logging

// Configure serilog logging
//Connection string is from Secret Manager. (Right-click on project and select "Manage User Secrets")
var logConnectionString = builder.Configuration["MooPing:SupabaseConnectionString"];

if (!string.IsNullOrEmpty(logConnectionString))
{
    IDictionary<string, ColumnWriterBase> columnOptions = new Dictionary<string, ColumnWriterBase>
{
    { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
    { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
    { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
    { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
    { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
    { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
    { "props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
    { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") }
};

    var loggerConfiguration = new LoggerConfiguration().Filter.ByExcluding(le => Matching.FromSource("Microsoft").Invoke(le)
                         && (le.Level == LogEventLevel.Verbose
                         || le.Level == LogEventLevel.Debug
                         || le.Level == LogEventLevel.Information))
                        .WriteTo.PostgreSQL(
                                    connectionString: logConnectionString,
                                    columnOptions: columnOptions,
                                    needAutoCreateTable: true,
                                    schemaName: "log",
                                    tableName: "Serilog"
                                    ).Enrich.FromLogContext();

    var logger = loggerConfiguration.CreateLogger();
    builder.Services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddSerilog(logger);
    });
}
#endregion

#region Auth0 Login
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.Scope = "openid profile email";
});
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

#region Pipelines
app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
#endregion

app.MapCarter(); //Map Api

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MooPing.Client._Imports).Assembly);

app.Run();
