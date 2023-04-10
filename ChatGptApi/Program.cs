using ChatGptApi.Interfaces;
using ChatGptApi.Logic.Chat;
using ChatGptApi.Logic.Image;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChatGptApi.Data;
using ChatGptApi.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ChatGptApiContextConnection") ?? throw new InvalidOperationException("Connection string 'ChatGptApiContextConnection' not found.");

builder.Services.AddDbContext<ChatGptApiContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ChatGptApiUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ChatGptApiContext>();
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddScoped<IImageLogic, ImageLogic>();
builder.Services.AddScoped<IChatLogic, ChatLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
