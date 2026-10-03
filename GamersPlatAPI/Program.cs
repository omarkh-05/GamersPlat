using Bussiness;
using Bussiness.BLL;
using Bussiness.Helpers;
using Bussiness.Interfaces;
using Data.DLL;
using Data.EF;
using DataLayer;
using GamersPlatAPI.Authorization;
using GamersPlatAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

// Add services to the container.s
builder.Services.AddControllers();
builder.Services.AddDbContext<GamersPlatDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("GamersPlat")));

builder.Services.AddScoped<IAuthService, AuthBLL>();
builder.Services.AddScoped<IUser, UserBLL>();
builder.Services.AddScoped<IUserRole, UserRoleBLL>();
builder.Services.AddScoped<IRoles, RoleBLL>();
builder.Services.AddScoped<AdminBLL>();
// Register other business layer services so controllers can inject them
builder.Services.AddScoped<CenterBLL>();
builder.Services.AddScoped<BookingBLL>();
builder.Services.AddScoped<CenterImageBLL>();
builder.Services.AddScoped<NotificationBLL>();
builder.Services.AddScoped<OfferBLL>();
builder.Services.AddScoped<TournamentBLL>();
builder.Services.AddScoped<TournamentPlayerBLL>();
builder.Services.AddScoped<ReviewBLL>();
builder.Services.AddScoped<ResourcesTypeBLL>();
builder.Services.AddScoped<OwnerBLL>();
builder.Services.AddScoped<DeviceBLL>();
builder.Services.AddScoped<SessionBLL>();
builder.Services.AddScoped<SessionParticipantBLL>();
builder.Services.AddScoped<ServiceBLL>();
builder.Services.AddScoped<GameBLL>();
builder.Services.AddScoped<PlayerBLL>();
builder.Services.AddScoped<StaffBLL>();
builder.Services.AddScoped<StaffRoleBLL>();
builder.Services.AddScoped<AuthHelpers>();
builder.Services.AddScoped<RefreshTokenBLL>();
builder.Services.AddScoped<PasswordResetTokenBLL>();
builder.Services.AddScoped<CountryBLL>();
builder.Services.AddScoped<UserDLL>();
builder.Services.AddScoped<RoleDLL>();
builder.Services.AddScoped<UserRoleDLL>();
builder.Services.AddScoped<AuthDLL>();
builder.Services.AddScoped<RefreshTokenDLL>();
builder.Services.AddScoped<TournamentDLL>();
builder.Services.AddScoped<TournamentPlayerDLL>();
builder.Services.AddScoped<SessionDLL>();
builder.Services.AddScoped<SessionParticipantDLL>();
builder.Services.AddScoped<StaffDLL>();
builder.Services.AddScoped<StaffRoleDLL>();
builder.Services.AddScoped<OfferDLL>();
builder.Services.AddScoped<ResourcesTypeDLL>();
builder.Services.AddScoped<ReviewDLL>();
builder.Services.AddScoped<NotificationDLL>();
builder.Services.AddScoped<AdminDLL>();
builder.Services.AddScoped<ServiceDLL>();
builder.Services.AddScoped<DeviceDLL>();
builder.Services.AddScoped<CenterDLL>();
builder.Services.AddScoped<CityDLL>();
builder.Services.AddScoped<CountryDLL>();
builder.Services.AddScoped<AuditLogDLL>();
builder.Services.AddScoped<CenterImageDLL>();
builder.Services.AddScoped<CenterInvitationDLL>();
builder.Services.AddScoped<EmailVerificationTokenDLL>();
builder.Services.AddScoped<EmailVerificationTokenBLL>();
builder.Services.AddScoped<GameDLL>();
builder.Services.AddScoped<GameBLL>();
builder.Services.AddScoped<OwnerDLL>();
builder.Services.AddScoped<OwnerBLL>();
builder.Services.AddScoped<PasswordResetTokenDLL>();
builder.Services.AddScoped<PasswordResetTokenBLL>();
builder.Services.AddScoped<PlayerDLL>();
builder.Services.AddScoped<PlayerBLL>();
builder.Services.AddScoped<PointTransactionDLL>();
builder.Services.AddScoped<PointTransactionBLL>();
builder.Services.AddScoped<BookingDLL>();
// Interface -> implementation registrations
builder.Services.AddScoped<ICenterService, CenterBLL>();
builder.Services.AddScoped<IResourcesTypeService, ResourcesTypeBLL>();
builder.Services.AddScoped<ITournamentService, TournamentBLL>();
builder.Services.AddScoped<ITournamentPlayerService, TournamentPlayerBLL>();
builder.Services.AddScoped<IOfferService, OfferBLL>();
builder.Services.AddScoped<IReviewService, ReviewBLL>();
builder.Services.AddScoped<INotificationService, NotificationBLL>();
builder.Services.AddScoped<ICenterImageService, CenterImageBLL>();
builder.Services.AddScoped<IBookingService, BookingBLL>();
builder.Services.AddScoped<IOwnerService, OwnerBLL>();
builder.Services.AddScoped<CityBLL>();

#region Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();

// Register Swagger generator and customize its behavior.
builder.Services.AddSwaggerGen(static options =>
{
    // ===============================
    // 1) Define the JWT Bearer security scheme
    // ===============================
    //
    // This tells Swagger that our API uses JWT Bearer authentication
    // through the HTTP Authorization header.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // The name of the HTTP header where the token will be sent.
        Name = "Authorization",
        // Indicates this is an HTTP authentication scheme.
        Type = SecuritySchemeType.Http,
        // Specifies the authentication scheme name.
        // Must be exactly "Bearer" for JWT Bearer tokens.
        Scheme = "Bearer",
        // Optional metadata to describe the token format.
        BearerFormat = "JWT",
        // Specifies that the token is sent in the request header.
        In = ParameterLocation.Header,
        // Text shown in Swagger UI to guide the user.
        Description = "Enter: Bearer {your JWT token}"
    });

    // ===============================
    // 2) Require the Bearer scheme for secured endpoints
    // ===============================
    //
    // This tells Swagger that endpoints protected by [Authorize]
    // require the Bearer token defined above.
    /* options.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
         {
             new OpenApiSecurityScheme
             {
                 // Reference the previously defined "Bearer" security scheme.
                 Reference = new BaseOpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                 }
             },
             // No scopes are required for JWT Bearer authentication.
             // This array is empty because JWT does not use OAuth scopes here.
             new string[] {}
         }
     });*/
});
//builder.Services.AddSwaggerGen();
#endregion


#region Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("GamersPlatAPIpolicy", policy =>
    {
        policy.WithOrigins()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // السماح بالوصول مع credentials
    });
});
#endregion


#region Authentication
var secretKey = builder.Configuration["GamersPlat_JKey"];
if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new Exception("JWT secret key is not configured.");
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Ensures the token was issued by a trusted issuer.
            ValidateIssuer = true,
            // Ensures the token is intended for this API (audience check).
            ValidateAudience = true,
            // Ensures the token has not expired.
            ValidateLifetime = true,
            // Ensures the token signature is valid and was signed by the API.
            ValidateIssuerSigningKey = true,
            // The expected issuer value (must match the issuer used when creating the JWT).
            ValidIssuer = "GamersPlat",
            // The expected audience value (must match the audience used when creating the JWT).
            ValidAudience = "GamersPlatUsers",
            // The secret key used to validate the JWT signature.
            // This must be the same key used when generating the token.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["accessToken"];

                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });
#endregion


#region Authorization
builder.Services.AddSingleton<IAuthorizationHandler, ClientOwnerOrAdminHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientOwnerOrAdmin", policy =>
        policy.Requirements.Add(new ClientOwnerOrAdminRequirement()));
});

#endregion


#region RateLimit
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthLimiter", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(2),
                QueueLimit = 0
            });
    });
});
#endregion


var app = builder.Build();
// Seed roles at startup
// Seed roles at startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<GamersPlatDbContext>();

        var roles = new[] { "Admin", "Owner", "Player" };

        foreach (var rn in roles)
        {
            if (!db.Roles.Any(r => r.RoleName == rn))
            {
                db.Roles.Add(new Data.Role { RoleName = rn });
            }
        }

        db.SaveChanges();
    }
    catch
    {
        // Ignore seeding errors at startup to avoid blocking the app; admin can inspect logs
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //  app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("GamersPlatAPIpolicy");

#region RateLimit 
app.UseRateLimiter();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
    {
        // سجل فقط الرسالة، لا تسلسل context
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Too many login attempts. Please try again later.");
    }
});
#endregion

app.UseAuthentication();
// Global error response middleware
app.UseMiddleware<ErrorResponseMiddleware>();

app.UseAuthorization();

// Monitor And Logging
// ✅ Step 6: Global 403 logging middleware (place it HERE)
//  ممكن احط في كل اندبوينت لوج برضو خاص او زي هيك عادي
app.Use(async (context, next) =>
{
    await next();

    // لاحظ أنه يمكن تسجيل أكثر من حالة
    int statusCode = context.Response.StatusCode;

    if (statusCode == StatusCodes.Status401Unauthorized || statusCode == StatusCodes.Status403Forbidden || statusCode == StatusCodes.Status404NotFound || statusCode == StatusCodes.Status400BadRequest)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var path = context.Request.Path.ToString();
        var reason = statusCode == 401 ? "Unauthorized" : "Forbidden";

        string logMessage = $"Security Alert: {reason}. UserId={userId}, Path={path}, IP={ip}";

        // سجل في Logger أول
        app.Logger.LogWarning(logMessage);

        // سجل في Event Viewer
        //   try
        //   {
        //       if (!System.Diagnostics.EventLog.SourceExists("GymSystemAPI"))
        //       {
        //           System.Diagnostics.EventLog.CreateEventSource("GymSystemAPI", "Application");
        //       }
        //       System.Diagnostics.EventLog.WriteEntry("GymSystemAPI", logMessage,  System.Diagnostics.EventLogEntryType.Warning);
        //   }
        //   catch (Exception ex)
        //   {
        //       app.Logger.LogError("Failed to write security log to Event Viewer: {0}", ex.Message);
        //   }
    }
});

app.MapControllers();

app.Run();
