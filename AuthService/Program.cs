using AuthService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            AddServicesToContainer(builder);

            // Configure Kestrel for mTLS
            string certsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "certs");
            ConfigureKestrelForMTLS(certsFolderPath, builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void AddServicesToContainer(WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddDbContext<AuthContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();

            ConfigureJWT(builder);

            builder.Services.AddScoped<Services.AuthService>();
        }

        private static void ConfigureJWT(WebApplicationBuilder builder)
        {
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
        }

        private static void ConfigureKestrelForMTLS(string certsFolderPath, WebApplicationBuilder builder)
        {
            // Configure Kestrel for mTLS
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureHttpsDefaults(listenOptions =>
                {
                    listenOptions.ServerCertificate = new X509Certificate2(
                       $"{certsFolderPath}/server.crt",
                       $"{certsFolderPath}/server.key");
                    listenOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    listenOptions.ClientCertificateValidation = (cert, chain, errors) => true; // Adjust this for production
                });
            });
        }
    }
}
