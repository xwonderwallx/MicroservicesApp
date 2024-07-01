using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UserService.Data;
using UserService.Interfaces;
using UserService.Services;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure DBContext
            builder.Services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register MessageConsumer
            builder.Services.AddHostedService<MessageConsumer>();

            // Register AuthServiceClient for HTTP requests to AuthService with mTLS
            var certsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "certs");
            builder.Services.AddHttpClient<IAuthServiceClient, AuthServiceClient>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["AuthService:BaseUrl"]);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(new X509Certificate2(
                    Path.Combine(certsFolderPath, "client.crt"),
                    Path.Combine(certsFolderPath, "client.key"))); // Corrected from "server.client" to "client.key"
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // Implement proper validation in production
                return handler;
            });

            // Configure Kestrel for mTLS
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureHttpsDefaults(listenOptions =>
                {
                    listenOptions.ServerCertificate = new X509Certificate2(
                        Path.Combine(certsFolderPath, "server.crt"),
                        Path.Combine(certsFolderPath, "server.key"));
                    listenOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    listenOptions.ClientCertificateValidation = (cert, chain, errors) => true; // Adjust this for production
                });
            });

            // Configure JWT Authentication
            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Register ProfileService with its interface
            builder.Services.AddScoped<IProfileService, ProfileService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
