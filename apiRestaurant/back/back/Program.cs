using back.Models;
using back.Routes;
using back.Services.Personnes;
using back.Services.PersonnesRestaurantsAimer;
using back.Services.PersonnesRestaurantsHistorique;
using back.Services.Protections;
using back.Services.Restaurants;
using back.Services.TypesRestaurant;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using System.Reflection;
using back.CachePolicyCustom;
using back.Services.Mdps;
using back.Services.Logs;
using back.Services.Jwts;

var builder = WebApplication.CreateBuilder(args);

string cheminCleRsa = builder.Configuration.GetValue<string>("cheminRsa")!;

RSA rsa = RSA.Create();

// creer la cle une seule fois
if (!File.Exists(cheminCleRsa))
{
    // cree un fichier bin pour signer le JWT
    var clePriver = rsa.ExportRSAPrivateKey();
    File.WriteAllBytes(cheminCleRsa, clePriver);
}

// recupere la clé
rsa.ImportRSAPrivateKey(File.ReadAllBytes(cheminCleRsa), out _);

// permet de savoir si on a le bon role pour pouvoir y acceder
// nom => a donner dans .RequireAuthorization("nom")
builder.Services.AddAuthorizationBuilder()
  .AddPolicy("admin", x => x.RequireRole("admin").RequireClaim("idPersonne"))
  .AddPolicy("utilisateur", x => x.RequireRole("utilisateur").RequireClaim("idPersonne"));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        // se qu'on veut valider ou non
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    // permet de valider le chiffrement du JWT en definissant la cle utilisé
                    option.Configuration = new OpenIdConnectConfiguration
                    {
                        SigningKeys = { new RsaSecurityKey(rsa) }
                    };

                    // pour avoir les clé valeur normal comme dans les claims
                    // par defaut ajouter des Uri pour certain truc comme le "sub"
                    option.MapInboundClaims = false;
                });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    // genere un XML et permet de voir le sumary dans swagger pour chaque fonctions dans le controller
    string xmlNomFichier = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlNomFichier));

    // ajout d'une option pour mettre le token en mode Bearer
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // ou le trouver
        In = ParameterLocation.Header,

        // description
        Description = "Token",

        // nom dans le header
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",

        // JWT de type Bearer
        Scheme = "Bearer"
    });

    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },

            new string[]{}
        }
    });
});

string connexionString = builder.Configuration.GetConnectionString("defaut")!;

builder.Services.AddDbContext<BddContext>(x =>
{
    try
    {
        x.UseMySql(connexionString, ServerVersion.AutoDetect(connexionString));
    }
    catch
    {
        Results.Problem("", statusCode: 503);
    }
});

builder.Services
    .AddSingleton<IProtectionService, ProtectionService>()
    .AddTransient<IMdpService, MdpService>()
    .AddTransient<IJwtService>(x => new JwtService(rsa, "moi"))
    .AddScoped<ILogService, LogService>()
    .AddScoped<IPersonneService, PersonneService>()
    .AddScoped<ITypeRestaurantService, TypeRestaurantService>()
    .AddScoped<IRestaurantService, RestaurantService>()
    .AddScoped<IPersonneRestaurantAimerService, PersonneRestaurantAimerService>()
    .AddScoped<IPersonneRestaurantHistoriqueService, PersonneRestaurantHistoriqueService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddOutputCache(x =>
{
    // cache actif avec le authorize
    // permet de supprimer un élement du cache et non le cache complet
    x.AddPolicy("parIdPersonne", new CachePolicyVarieParId("idPersonne", "personne"));
    x.AddPolicy("parIdRestaurant", new CachePolicyVarieParId("idRestaurant", "restaurant"));
    x.AddPolicy("parIdTypeRestaurant", new CachePolicyVarieParId("idTypeRestaurant", "typeRestaurant"));    
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x => x.DefaultModelsExpandDepth(-1));
}

// l'ordre est important
app.UseAuthentication();
app.UseAuthorization();

// doit etre au dessus de toutes les routes
app.UseOutputCache();

app.MapGroup("/personne").AjouterRoutePersonne();
app.MapGroup("/typeRestaurant").AjouterRouteTypeRestaurant();
app.MapGroup("/restaurant").AjouterRouteRestaurant();
app.MapGroup("/log").AjouterRouteLog();
app.MapGroup("/personneRestaurantAimer").AjouterRoutePersonneRestoAimer();
app.MapGroup("/personneRestaurantHistorique").AjouterRoutePersonneRestoHistorique();

app.Run();

// Scaffold-DbContext "server=localhost;database=bdd;User=root;Pwd=;" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -Force
