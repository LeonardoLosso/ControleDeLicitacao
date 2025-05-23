﻿using ControleDeLicitacao.App.Mappings;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.App.Services.Cadastros.User;
using ControleDeLicitacao.App.Services.Documentos;
using ControleDeLicitacao.App.Services.Documentos.Baixa;
using ControleDeLicitacao.App.Services.Logger;
using ControleDeLicitacao.App.Upload.Services;
using ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;
using ControleDeLicitacao.Infrastructure;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using ControleDeLicitacao.Infrastructure.Persistence.Interface;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ControleDeLicitacao.API.Registradores;

public static class Registradores
{
    public static IServiceCollection AddContexts(this IServiceCollection services, string connString)
    {
        services.AddDbContext<EntidadeContext>(
            opts => opts.UseSqlServer(
                connString, 
                sqlOptions => sqlOptions.EnableRetryOnFailure()));

        services.AddDbContext<ItemContext>(
            opts => opts.UseSqlServer(
                connString, 
                sqlOptions => sqlOptions.EnableRetryOnFailure()));

        services.AddDbContext<UsuarioContext>(
            opts => opts.UseSqlServer(
                connString, 
                sqlOptions => sqlOptions.EnableRetryOnFailure()));

        services.AddDbContext<BaixaContext>(
            opts => opts.UseSqlServer(
                connString, 
                sqlOptions => sqlOptions.EnableRetryOnFailure()));

        return services;
    }

    public static IServiceCollection AddUserConfig(this IServiceCollection services)
    {

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes("F4ca)uNa0FaC4.4T3n74t1VAnAO3xI27E")),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

        
        services
            .AddIdentity<Usuario, IdentityRole<int>>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
            })
            .AddEntityFrameworkStores<UsuarioContext>()
            .AddUserStore<CustomUserStore>()
            .AddDefaultTokenProviders();

        return services;
    }
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IHostedService, StartupTask>();
        services.AddScoped<EntidadeService>();
        services.AddScoped<ItemService>();
        services.AddScoped<UsuarioService>();
        services.AddSingleton<TokenService>();
        services.AddSingleton<UserKeeper>();
        services.AddScoped<BaixaService>();
        services.AddScoped<BaixaPoliciaService>();
        services.AddScoped<EmpenhoService>();
        services.AddScoped<NotaService>();
        services.AddScoped<UploadService>();
        services.AddHttpClient<RequestService>();

        return services;
    }

    public static IServiceCollection AddInfraRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<DbContext, EntidadeContext>();
        services.AddScoped<EntidadeRepository>();
        services.AddScoped<ItemRepository>();
        services.AddScoped<BaixaRepository>();

        return services;
    }

    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(EntidadeMapping).Assembly);
        services.AddAutoMapper(typeof(ItemMapping).Assembly);
        services.AddAutoMapper(typeof(UsuarioMapping).Assembly);
        services.AddAutoMapper(typeof(ReajusteMapping).Assembly);
        services.AddAutoMapper(typeof(BaixaMapping).Assembly);

        return services;
    }

    public static IServiceCollection AddOthersConfig(this IServiceCollection services)
    {

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        services.AddControllers(options =>
        {
            options.RespectBrowserAcceptHeader = true;
            options.ReturnHttpNotAcceptable = true;
        }).AddXmlSerializerFormatters()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        })
        .AddNewtonsoftJson();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
