using System.ComponentModel;
using ChatApp.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

/*builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter() ());
    });*/

builder.Services.AddDbConnection(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
