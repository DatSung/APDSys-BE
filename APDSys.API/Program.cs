using ADPSys.DataAccess.Context;
using ADPSys.DataAccess.IRepository;
using ADPSys.DataAccess.Repository;
using APDSys.Model.Domain;
using APDSys.Service.IService;
using APDSys.Service.Mappings;
using APDSys.Service.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Configure JSON serialization options to include a converter for Enum values, 
    // ensuring they are serialized as strings when returned from API controllers.
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Add db service
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("local");
    options.UseSqlServer(connectionString);
});

// Add automapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILogService, LogService>();

// Add Identity
builder.Services
    // AddIdentity configures the application's identity system, specifying ApplicationUser as the user type 
    .AddIdentity<ApplicationUser, IdentityRole>()
    // And IdentityRole as the role type. It also sets up Entity Framework stores for user and role management 
    .AddEntityFrameworkStores<ApplicationDbContext>()
    // Using ApplicationDbContext, and adds default token providers for password reset, email confirmation, etc.
    .AddDefaultTokenProviders();

// Config Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});

// Add AuthenticationSchema and JwtBearer
builder.Services
    .AddAuthentication(options =>
    {
        // Set up JWT bearer authentication as the default scheme for authentication, authentication challenge, 
        // and authentication. This ensures that JWT bearer authentication is used throughout the application.
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Configure JWT bearer authentication options to save tokens, disable HTTPS metadata validation, 
        // and set up token validation parameters including issuer, audience, and the signing key derived 
        // from the application's configuration settings.
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
