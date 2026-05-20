using System.Text;
using DotNetEnv;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Infrastructure.Persistence.EFC.Repositories;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.Services;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;
using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Hashing.BCrypt.Services;
using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Persistence.EFC.Repositories;
using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Pipeline.Middleware.Components;
using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Services;
using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Tokens.JWT.Configuration;
using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Tokens.JWT.Services;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.ACL;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.ACL.Services;
using FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.OutboundServices.ACL;
using FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Profiles.Infrastructure.Persistence.EFC.Repositories;
using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.ACL;
using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.ACL.Services;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Services;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Infrastructure.Persistence.EFC;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.OutboundServices.ACL;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Infrastructure.Persistence.EFC.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.ACL;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.ACL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Namespace aliases to disambiguate multiple IExternalProfileService across BCs
using RsvOutbound = FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;
using BrkOutbound = FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.OutboundServices;

Env.Load("../.env");

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("Connection string not found.");

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

// Configure CORS — origins driven by environment variable for deployment flexibility.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "FullStackFury.EduSpacePlatform.API",
            Version = "v1",
            Description = "Eduspace Platform API",
            TermsOfService = new Uri("https://acme-learning.com/tos"),
            Contact = new OpenApiContact
            {
                Name = "Eduspace",
                Email = "contact@fullstackfury.com"
            },
            License = new OpenApiLicense
            {
                Name = "Apache 2.0",
                Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
            }
        });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token into field",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
    c.EnableAnnotations();
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    else if (builder.Environment.IsProduction())
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Error);
});

//Shared BC
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Profiles BC
builder.Services.AddScoped<IAdminProfileRepository, AdminProfileRepository>();
builder.Services.AddScoped<ITeacherProfileRepository, TeacherProfileRepository>();
builder.Services.AddScoped<IAdminProfileCommandService, AdminProfileCommandService>();
builder.Services.AddScoped<IAdminProfileQueryService, AdminProfileQueryService>();
builder.Services.AddScoped<ITeacherProfileCommandService, TeacherProfileCommandService>();
builder.Services.AddScoped<ITeacherQueryService, TeacherProfileQueryService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();
builder.Services.AddScoped<IExternalIamService, ExternalIamService>();
builder.Services.AddScoped<IAccountCommandService, AccountCommandService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<IAccountQueryService, AccountQueryService>();
builder.Services.AddScoped<IProfilesContextFacade, ProfilesContextFacade>();

// IAM — auth infrastructure
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IActivationTokenRepository, ActivationTokenRepository>();

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportCommandService, ReportCommandService>();
builder.Services.AddScoped<IReportQueryService, ReportQueryService>();

// BreakdownManagement BC — ACL outbound services
builder.Services.AddScoped<BrkOutbound.IExternalProfileService, BrkOutbound.ExternalProfileServices>();
builder.Services.AddScoped<BrkOutbound.IExternalResourceService, BrkOutbound.ExternalResourceService>();

//Reservation Scheduling
builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();
builder.Services.AddScoped<IMeetingCommandService, MeetingCommandService>();
builder.Services.AddScoped<IMeetingQueryService, MeetingQueryService>();
builder.Services.AddScoped<RsvOutbound.IExternalClassroomService, RsvOutbound.ExternalClassroomServices>();
builder.Services.AddScoped<RsvOutbound.IExternalProfileService, RsvOutbound.ExternalProfileService>();

// Spaces and Resource BC
// Classrooms
builder.Services
    .AddScoped<FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.OutboundServices.ACL.
        IExternalProfileService, ExternalProfileService>();
builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();
builder.Services.AddScoped<IClassroomCommandService, ClassroomCommandService>();
builder.Services.AddScoped<IClassroomQueryService, ClassroomQueryService>();
// Resources
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IResourceCommandService, ResourceCommandService>();
builder.Services.AddScoped<IResourceQueryService, ResourceQueryService>();

builder.Services.AddScoped<ISpacesAndResourceManagementFacade, SpacesAndResourceManagementFacade>();

// Shared Areas
builder.Services.AddScoped<ISharedAreaRepository, SharedAreaRepository>();
builder.Services.AddScoped<ISharedAreaCommandService, SharedAreaCommandService>();
builder.Services.AddScoped<ISharedAreaQueryService, SharedAreaQueryService>();

//Token Settings Configuration
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();

// Email delivery via SendGrid — required in every environment (local + prod).
if (string.IsNullOrWhiteSpace(builder.Configuration["SENDGRID_API_KEY"]))
    throw new InvalidOperationException(
        "SENDGRID_API_KEY is not configured. Set it in .env or environment variables.");
builder.Services.AddScoped<IEmailService, EmailService>();

// JWT authentication — validates tokens issued by TokenService
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenSettings = builder.Configuration.GetSection("TokenSettings").Get<TokenSettings>()
            ?? throw new InvalidOperationException("TokenSettings missing from configuration.");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = tokenSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = tokenSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
