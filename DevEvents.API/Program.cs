using DevEvents.API.Mappers;
using DevEvents.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.OpenApi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//Manter a Lista em memória numa mesma instancia durante a execução da API.
//builder.Services.AddDbContext<DevEventsDbContext>(o => o.UseInMemoryDatabase("DevEventsDb"));


//Salvar em BD com as configurações do appsettings.json
var connectionSring = builder.Configuration.GetConnectionString("DevEventsCs");
builder.Services.AddDbContext<DevEventsDbContext>(o => o.UseSqlServer(connectionSring));

//O Sistema rastreia todos os perfis programados do Automapper passando somente 1.
builder.Services.AddAutoMapper(typeof(DevEventProfile).Assembly);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DevEvents.API",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name= "ChrystianDev",
            Email = "chrystianmote@hotmail.com",
            Url = new Uri("https://www.linkedin.com/in/chrystianmote/")

        }
    });
    var xmlFile = "DevEvents.API.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

});

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
