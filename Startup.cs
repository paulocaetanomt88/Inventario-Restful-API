using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventarioRestfulAPI.Models;
using InventarioRestfulAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace InventarioRestfulAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // O m�todo ConfigureServices() � respons�vel por definir os servi�os que a aplica��o vai usar,
        // incluindo recursos da plataforma como ASP .NET Core MVC e Entity Framework.
        // O par�metro IServiceCollection permite configurar diferentes tipos de servi�os seja por cria��o de objeto
        // ou correspond�ncia a uma interface espec�fica e suporta os lifetimes: Transient, Scoped e Singleton.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IProdutoRepository, ProdutoRepository>(); // Scoped: Criado uma vez por solicita��o.
            services.AddControllers();

            // servi�o de Autentica��o no IServicesCollection informando o seu tipo (bearer)
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;

                // Definimos SaveToken igual a true e com isso o token ser� armazenado no contexto HTTP e poderemos acessar o token no controller quando precisarmos;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    // Habilitamos a valida��o da Audience e do Issuer, obtendo os valores definidos no arquivo de configura��o;
                    ValidateIssuer = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidIssuer = Configuration["Jwt:Issuer"],

                    // passamos a chave de seguran�a usada quando criamos o token jwt
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });
        }

        // Este m�todo � chamado pelo tempo de execu��o usado para configurar o pipeline de solicita��o HTTP.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseStatusCodePages();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // O valor HSTS padr�o � de 30 dias. Voc� pode querer mudar isso para cen�rios de produ��o, consulte https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();


            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
