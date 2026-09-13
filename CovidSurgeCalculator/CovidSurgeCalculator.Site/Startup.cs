using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CovidSurgeCalculator.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Contract.Requires(configuration != null);

            Configuration = configuration;
            UseSession = configuration.GetValue<bool>("UseSession");
        }

        public IConfiguration Configuration { get; }
        public bool UseSession { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddMemoryCache();
            services.AddHostedService<ModelCacheHostedService>();
            if (UseSession)
            {
                services.AddDistributedMemoryCache();
                services.AddSession(options =>
                {
                    options.Cookie.Name = ".CovidSurgeCalculator.Session";
                    options.Cookie.IsEssential = true;
                });
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            Contract.Requires(env != null);

            if (env.IsDevelopment())
            {
                logger.LogInformation("In development environment");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                logger.LogInformation("In production");
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            if (UseSession)
            {
                app.UseSession();
            }
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}