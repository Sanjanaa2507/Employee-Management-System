using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Entity;
using EmployeeManagementSystem.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ======================= SERVICES =======================

// Controllers
builder.Services.AddControllers();

// ---------- OpenAPI + Scalar (WITH JWT SUPPORT) ----------
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter: Bearer {your JWT token}"
        };

        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        return Task.CompletedTask;
    });
});

// ---------- CORS ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCrosOrigin", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ---------- DATABASE ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ---------- REPOSITORIES ----------
builder.Services.AddScoped<IRepository<Department>, Repository<Department>>();
builder.Services.AddScoped<IRepository<Employee>, Repository<Employee>>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddScoped<IRepository<Leave>, Repository<Leave>>();
builder.Services.AddScoped<IRepository<Attendance>, Repository<Attendance>>();

// ---------- HELPERS ----------
builder.Services.AddScoped<UserHelper>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// ---------- EMAIL + BACKGROUND JOB ----------
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<DailyAbsenceChecker>();
builder.Services.AddHostedService<DailyAbsenceWorker>();

// ======================= AUTHENTICATION =======================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"]!)
        ),

        ClockSkew = TimeSpan.Zero
    };

    // 🔥 Prevent HTML redirect on auth failure
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\":\"Unauthorized\"}");
        }
    };
});

builder.Services.AddAuthorization();

// ======================= APP =======================

var app = builder.Build();

// ---------- DATA SEED ----------
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var seedHelper = new DataSeedHelper(dbContext);
    seedHelper.InsertData();
    
}

// ---------- DEV TOOLS ----------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// ---------- MIDDLEWARE ----------
app.UseCors("AllowCrosOrigin");

app.UseHttpsRedirection();

app.UseAuthentication(); // MUST be before Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
