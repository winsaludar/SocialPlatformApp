using Authentication.API.Middlewares;
using Authentication.Domain.Repositories;
using Authentication.Persistence;
using Authentication.Persistence.Models;
using Authentication.Persistence.Repositories;
using Authentication.Services;
using Authentication.Services.Abstraction;
using EventBus.Core.Abstractions;
using EventBus.RabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
AddDatabase(builder);
AddAuthentication(builder);
AddMiddlewares(builder);
AddDependencies(builder);
AddEventBus(builder);

var app = builder.Build();
EnableMiddlewares(app);

app.Run();

void AddDatabase(WebApplicationBuilder builder)
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AuthenticationDbContext>(options => options.UseSqlServer(connectionString));
}

void AddAuthentication(WebApplicationBuilder builder)
{
    // Configure token validation parameters 
    var tokenValidationParameters = new TokenValidationParameters
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
    builder.Services.AddSingleton(tokenValidationParameters);

    // Configure Identity
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<AuthenticationDbContext>()
        .AddDefaultTokenProviders();

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
        options.TokenValidationParameters = tokenValidationParameters;
    });
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
}

void AddEventBus(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<IRabbitMQPersistentConnection>(x =>
    {
        var logger = x.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

        var factory = new ConnectionFactory()
        {
            HostName = builder.Configuration["EventBus:Hostname"],
            Port = int.Parse(builder.Configuration["EventBus:Port"]),
            UserName = builder.Configuration["EventBus:Username"],
            Password = builder.Configuration["EventBus:Password"],
        };

        int retryCount = int.Parse(builder.Configuration["EventBus:RetryCount"]);

        return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
    });

    builder.Services.AddSingleton<IEventBus, EventBusRabbitMQ>(x =>
    {
        string subscriptionClientName = builder.Configuration["EventBus:SubscriptionClientName"];
        var rabbitMQPersistentConnection = x.GetRequiredService<IRabbitMQPersistentConnection>();
        var logger = x.GetRequiredService<ILogger<EventBusRabbitMQ>>();

        int retryCount = 5;
        if (!string.IsNullOrEmpty(builder.Configuration["EventBus:RetryCount"]))
        {
            retryCount = int.Parse(builder.Configuration["EventBus:RetryCount"]);
        }

        return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, retryCount);
    });
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