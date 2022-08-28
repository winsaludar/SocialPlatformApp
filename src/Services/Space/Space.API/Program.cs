using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Space.API.Middlewares;
using Space.Common.Helpers;
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

var app = builder.Build();
EnableMiddlewares(app);

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