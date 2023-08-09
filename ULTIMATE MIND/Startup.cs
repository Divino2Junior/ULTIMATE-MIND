using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ULTIMATE_MIND.Arquitetura.Util;
using Microsoft.Extensions.Options;
using System;

namespace ULTIMATE_MIND
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddAuthentication(Constantes.CookieUser)
                   .AddCookie(Constantes.CookieUser, options =>
                   {
                       options.AccessDeniedPath = new PathString("/");
                       options.Cookie = new CookieBuilder
                       {
                           //Domain = "",
                           HttpOnly = true,
                           Name = Constantes.UsuarioLogado,
                           Path = "/",
                           SameSite = SameSiteMode.Lax,
                           SecurePolicy = CookieSecurePolicy.SameAsRequest
                       };

                       options.LoginPath = new PathString("/");
                       options.ReturnUrlParameter = "RequestPath";
                       options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Altera o tempo de expiração
                       options.SlidingExpiration = true; // Habilita a renovação do cookie a cada solicitação
                       options.Cookie.MaxAge = TimeSpan.FromMinutes(60); // Define a idade máxima do cookie
                       options.Cookie.IsEssential = true;
                   });

            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.ValueLengthLimit = int.MaxValue;
                x.MemoryBufferThreshold = int.MaxValue;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Adiciona a autenticação ao pipeline de solicitação

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}
