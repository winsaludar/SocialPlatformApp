using EventBus.Core;
using EventBus.Core.Abstractions;
using EventBus.RabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using Space.API.Middlewares;
using Space.Common.Helpers;
using Space.Common.IntegrationEvents.EventHandlers;
using Space.Common.IntegrationEvents.Events;
using Space.Domain.Helpers;
using Space.Domain.Repositories;
using Space.Persistence;
using Space.Persistence.Repositories;
using Space.Services;
using Space.Services.Abstraction;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
AddDatabase(builder);
AddAuthentication(builder);
AddAuthorization(builder);
AddMiddlewares(builder);
AddDependencies(builder);
RegisterEventBus(builder);

var app = builder.Build();
EnableMiddlewares(app);
ConfigureEventBus(app);

app.Run();

void AddDatabase(WebApplicationBuilder builder)
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<SpaceDbContext>(options => options.UseSqlServer(connectionString));
}

void AddAuthentication(WebApplicationBuilder builder)
{
    // Configure JWT Authentication Scheme
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
}

void AddAuthorization(WebApplicationBuilder builder)
{
}

void AddMiddlewares(WebApplicationBuilder builder)
{
    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

void AddDependencies(WebApplicationBuilder builder)
{
    builder.Services.AddTransient<ExceptionHandlingMiddleware>();
    builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
    builder.Services.AddScoped<IServiceManager, ServiceManager>();
    builder.Services.AddScoped<IHelperManager, HelperManager>();
}

void RegisterEventBus(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<IRabbitMQPersistentConnection>(x =>
    {
        var logger = x.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

        var factory = new ConnectionFactory()
        {
            HostName = builder.Configuration["EventBus:Hostname"],
            DispatchConsumersAsync = true,
            Port = int.Parse(builder.Configuration["EventBus:Port"]),
            UserName = builder.Configuration["EventBus:Username"],
            Password = builder.Configuration["EventBus:Password"],
        };

        int retryCount = int.Parse(builder.Configuration["EventBus:RetryCount"]);

        return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
    });

    builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

    builder.Services.AddSingleton<IEventBus, EventBusRabbitMQ>(x =>
    {
        string subscriptionClientName = builder.Configuration["EventBus:SubscriptionClientName"];
        var rabbitMQPersistentConnection = x.GetRequiredService<IRabbitMQPersistentConnection>();
        var logger = x.GetRequiredService<ILogger<EventBusRabbitMQ>>();
        var serviceScopeFactory = x.GetRequiredService<IServiceScopeFactory>();
        var subscriptionManager = x.GetRequiredService<IEventBusSubscriptionsManager>();

        int retryCount = 5;
        if (!string.IsNullOrEmpty(builder.Configuration["EventBus:RetryCount"]))
        {
            retryCount = int.Parse(builder.Configuration["EventBus:RetryCount"]);
        }

        return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, serviceScopeFactory, subscriptionManager, subscriptionClientName, retryCount);
    });

    builder.Services.AddTransient<UserRegisteredSuccessfulIntegrationEventHandler>();
}

void EnableMiddlewares(WebApplication app)
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
}

void ConfigureEventBus(WebApplication app)
{
    var eventBus = app.Services.GetService<IEventBus>();

    eventBus?.Subscribe<UserRegisteredSuccessfulIntegrationEvent, UserRegisteredSuccessfulIntegrationEventHandler>();
}