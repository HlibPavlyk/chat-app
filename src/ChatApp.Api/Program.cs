using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Application.Services;
using ChatApp.Infrastructure.Extensions;
using ChatApp.Infrastructure.Hubs;
using ChatApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

/*builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter() ());
    });*/

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddDbConnection(builder.Configuration);

builder.Services.AddSingleton<IChatHubRepository, ChatHubRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatHubService, ChatHubService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.MapHub<ChatHub>("hub");

app.Run();
