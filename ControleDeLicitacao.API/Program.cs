using ControleDeLicitacao.API.Middleware;
using ControleDeLicitacao.API.Registradores;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//-----------[BUILDER]------------
var connString = builder.Configuration.GetConnectionString("Connection");

//conexão com o banco de dados
builder.Services.AddContexts(connString);

//configurações de usuario
builder.Services.AddUserConfig();

//registro de serviços
builder.Services.AddAppServices();

//registro de repositorios
builder.Services.AddInfraRepositories();

//registro de mapeamentos
builder.Services.AddMappers();

//configurações gerais da API
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

//app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
