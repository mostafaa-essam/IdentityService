using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Services.AccountService;
using IdentityService.Services.EmailService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Identity :-
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
//Set an hour For The Token to be expired:-
//builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
//                    options.TokenLifespan = TimeSpan.FromHours(1));
//Jwt:-
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Token"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
//Email:-
builder.Services.AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection("Email"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Token:Issuer"],
        ValidAudience = builder.Configuration["Token:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Token:SigningKey")),
        ClockSkew = TimeSpan.Zero
    };
});
//DataBase
var ConnectionString = builder.Configuration.GetConnectionString("SQLConnection") ??
                                         throw new InvalidOperationException("Connectionstring is not corret");
builder.Services.AddDbContext<ApplicationDbContext>(
    options=>options.UseSqlServer(ConnectionString));
//Services
builder.Services.AddScoped<IAccountService,AccountService>();
builder.Services.AddScoped<IEmailService,EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
