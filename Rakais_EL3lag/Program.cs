
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Rakais_EL3lag.Models;
using Rakais_EL3lag.Seed;
using System.Text;



namespace Rakais_EL3lag
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            builder.Services.AddDbContext<RakaizContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Rakaiz"))
            );

            builder.Services.AddAuthentication(options => {
                //Check JWT Token Header
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //[authrize]
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//unauth
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>//verified key
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:IssuerIP"],

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecritKey"]))

                };
            });
            builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<RakaizContext>().AddDefaultTokenProviders();
            // Add services to the container.

            builder.Services.AddControllers();
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }

            builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(wwwrootPath));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseAuthorization();


            app.MapControllers();
            await DbSeeder.SeedAdminAsync(app.Services);

            app.Run();
        }
    }
}
