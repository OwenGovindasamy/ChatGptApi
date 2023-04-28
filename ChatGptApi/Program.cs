using ChatGptApi.Interfaces;
using ChatGptApi.Logic.Chat;
using ChatGptApi.Logic.Image;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using ChatGptApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using ChatGptApi.TokenConfig;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
//Get property value called ChatGptApiContextConnection from the appsettings.json file
var connectionString = builder.Configuration.GetConnectionString("ChatGptApiContextConnection") ?? throw new InvalidOperationException("Connection string 'ChatGptApiContextConnection' not found.");

builder.Services.AddDbContext<ChatGptApiContext>(options => options.UseSqlServer(connectionString));

//Allow weaker passwords
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<ChatGptApiContext>();

//Secure key used for generating jwt token
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bgwnhzhcoejxupilcsxbkrkvbquanakz"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(opt =>
     {
         opt.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuerSigningKey = true,
             IssuerSigningKey = key,
             ValidateIssuer = false,
             ValidateAudience = false
         };
     });

var configuration = builder.Configuration;

//Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Injecting interfaces
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddScoped<IImageLogic, ImageLogic>();
builder.Services.AddScoped<IChatLogic, ChatLogic>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<UserManager<IdentityUser>>();

//Configuring swagger to give us a pop up for bearer token
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatGptApi", Version = "v1" });

    var security = new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", Array.Empty<string>() }
                };
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "insert Bearer space {{ token }}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                        },
                        new List<string>()
                    }
                });
});

builder.Services.ConfigureSwaggerGen(options =>
{
    options.CustomSchemaIds(x => x.FullName);
});

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllers();

app.Run();
