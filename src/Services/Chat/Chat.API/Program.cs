using Chat.API.Hubs;
using Chat.API.Middlewares;
using Chat.Application.IntegrationEvents;
using Chat.Application.Validators;
using Chat.Domain.SeedWork;
using Chat.Infrastructure;
using Chat.Infrastructure.Repositories;
using EventBus.Core;
using EventBus.Core.Abstractions;
using EventBus.RabbitMQ;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
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
    builder.Services.Configure<ChatDbSettings>(builder.Configuration.GetSection(nameof(ChatDbSettings)));
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

        // Sending the access token in the request header is required due to
        // a limitation in Browser APIs. We restrict it to only calls to the
        // SignalR hub in this code.
        // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
        // for more information about security considerations when using
        // the query string to transmit the access token.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Headers["access_token"];

                // Make sure the request if for our chat hub
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hubs/chat")))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });
}

void AddAuthorization(WebApplicationBuilder builder)
{
}

void AddMiddlewares(WebApplicationBuilder builder)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(builder.Configuration["CORSPolicy:Name"], policy =>
        {
            policy.WithOrigins(builder.Configuration["CORSPolicy:Origins"])
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    builder.Services.AddControllers();
    builder.Services.AddSignalR();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

void AddDependencies(WebApplicationBuilder builder)
{
    builder.Services.AddMediatR(typeof(Chat.Application.AssemblyReference));
    builder.Services.AddTransient<ExceptionHandlingMiddleware>();

    builder.Services.AddScoped<IValidatorManager, ValidatorManager>();

    builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
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

    app.UseCors(builder.Configuration["CORSPolicy:Name"]);

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHub<ChatHub>("/hubs/chat/{serverId}");
}

void ConfigureEventBus(WebApplication app)
{
    var eventBus = app.Services.GetService<IEventBus>();
    eventBus?.Subscribe<UserRegisteredSuccessfulIntegrationEvent, UserRegisteredSuccessfulIntegrationEventHandler>();
}