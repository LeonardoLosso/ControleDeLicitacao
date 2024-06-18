//using ControleDeLicitacao.API.Registradores;
using ControleDeLicitacao.API.Middleware;
using ControleDeLicitacao.API.Registradores;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//--------------------
var connString = builder.Configuration.GetConnectionString("Connection");

//conexão com o banco de dados
builder.Services.AddDbContext<EntidadeContext>(opts =>
opts.UseSqlServer(connString));

//builder.Services.AddUserDbContext(connString);
//falta criar authorization e authentication
//builder.Services.AddUserConfig();

//builder.Services.AddUserServices();
builder.Services.AddAppServices();
//---------------------

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
