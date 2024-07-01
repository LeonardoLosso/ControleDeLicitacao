using ControleDeLicitacao.API.Middleware;
using ControleDeLicitacao.API.Registradores;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//-----------[BUILDER]------------
var connString = builder.Configuration.GetConnectionString("Connection");
var logConnString = builder.Configuration.GetConnectionString("LogConnection");

//conex�o com o banco de dados
builder.Services.AddContexts(connString);
builder.Services.AddDbContext<LogContext>(opts => opts.UseSqlServer(logConnString));

//configura��es de usuario
builder.Services.AddUserConfig();

//registro de servi�os
builder.Services.AddAppServices();

//registro de repositorios
builder.Services.AddInfraRepositories();

//registro de mapeamentos
builder.Services.AddMappers();

//configura��es gerais da API
builder.Services.AddOthersConfig();

//-----------[APPLICATION]------------

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LogMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
