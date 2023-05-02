using ChatGPTLineBot.Configs;
using ChatGPTLineBot.Services;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Config>(builder.Configuration.GetSection("Config"));
builder.Services.AddSingleton<ChatGPTService>();
builder.Logging.AddNLog("Config/NLog.config");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseRouting();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
