using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using ReraCore.Helper;
using ReraCore.Interfaces;
using ReraRepo.Repositories;
using ReraRepo.Services;
using ReraWebPortal.Middleware;
using System.Net;
using System.Text;
using static ReraWebPortal.Controllers.ProjectController;

var builder = WebApplication.CreateBuilder(args);


// ---------------- Controllers ----------------
builder.Services.AddControllers();

// ---------------- Swagger ----------------
builder.Services.AddEndpointsApiExplorer();
//cookie
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This sets the minimum SameSite mode applied to all cookies
    options.MinimumSameSitePolicy = SameSiteMode.None;

    // This controls whether to apply the cookie policy globally
    options.OnAppendCookie = context =>
    {
        context.CookieOptions.SameSite = SameSiteMode.None;
        context.CookieOptions.Secure = true;
        context.CookieOptions.HttpOnly = false;
        context.CookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(1);
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Rera Web Portal API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token with Bearer prefix"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Add JWT Authentication for Service2
builder.Services.
    AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"]?.ToString(),
            ValidAudience = builder.Configuration["Jwt:Audience"]?.ToString(),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecurityKey"]?.ToString()))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireClaim("scope", builder.Configuration["Jwt:Scope"]?.ToString());
    });
});
object value = builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});
string[] allowedMethods = ["GET", "POST"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("corepolicy", policy =>
    {
        policy.WithOrigins("http://10.70.232.192", 
            "http://localhost:5024", 
            "http://localhost:5000", 
            "http://localhost:4200", 
            "http://localhost:4558", 
            "https://reraanalytics.devitsandbox.com/",
            "http://10.70.232.233", 
            "https://10.70.232.233", 
            "https://reraweb.devitsandbox.com",
            "http://reraweb.devitsandbox.com",
            "http://172.18.181.20", 
            "https://172.18.181.20",
            "https://172.21.124.93",
            "http://172.21.124.93",
            "http://rera.rajasthan.gov.in",
            "https://rera.rajasthan.gov.in")
              .WithMethods(allowedMethods)
              .WithHeaders("Content-Type", "Accept", "Authorization", "no-loader", "X-API-KEY")
              .WithExposedHeaders("X-AuthToken", "no-loader", "Content-Disposition")
              .AllowCredentials();
    });
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});
var RootPath = builder.Environment.ContentRootPath;
ConfigurationHelper.Configure(builder.Configuration, RootPath);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<UserStore>();
builder.Services.AddSingleton<ConnectionStore>();
builder.Services.AddSingleton<SavedQueryStore>();
builder.Services.AddSingleton<SqlServerDatabaseService>();
builder.Services.AddSingleton<DatabaseServiceFactory>();

builder.Services.AddHostedService<ReraBackgroundService>();


var app = builder.Build();
app.UseCors("corepolicy");

app.Use(async (context, next) =>
{
    //security
    // Remove unwanted headers
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-AspNet-Version");
    context.Response.Headers.Remove("X-AspNetMvc-Version");

    // the X-Powered-By header is an HTTP response header that typically indicates which web framework or technology is being used to serve the request
    context.Response.Headers.Remove("X-Powered-By");

    //dummy set
    context.Response.Headers["Server"] = "noinfo";
    context.Response.Headers["X-AspNet-Version"] = "noinfo";
    context.Response.Headers["X-AspNetMvc-Version"] = "noinfo";
    context.Response.Headers["X-Powered-By"] = "noinfo";

    // Set 'X-Content-Type-Options' header to 'nosniff' to prevent MIME sniffing
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";

    // Set the 'X-Frame-Options' header to 'DENY' to prevent clickjacking
    context.Response.Headers["X-Frame-Options"] = "DENY"; // Or "SAMEORIGIN"

    // Content-Security-Policy (CSP) header (restricts sources of content)
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self';script-src 'self' 'unsafe-inline' 'unsafe-eval';style-src 'self' 'unsafe-inline';img-src 'self' data: blob:;font-src 'self' data:;connect-src 'self';object-src 'none';frame-ancestors 'none';base-uri 'self';form-action 'self';";

    //end security

    //handle allow methos
    if (!allowedMethods.Contains(context.Request.Method))
    {
        context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
        return;//short-circut
    }

    //use context in other
    HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
    CommonFuncationHelper.Configure(context,httpContextAccessor);
    //next
    await next();
});
// ---------------- Middleware ----------------
app.UseCookiePolicy();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service1 API v1"));
}
app.UseForwardedHeaders();

app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
