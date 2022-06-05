using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PXLFunds.Data;
using PXLFunds.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PXLFunds
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //Dbcontext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ApplicationDbContext")));

            //Identity
            //External Login
            //External login
            //services.AddAuthentication()
            //    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            //    {
            //        options.Authority = "https://localhost:9001";
            //        options.ClientId = "test.client.id";
            //        options.ClientSecret = "test.client.secret";
            //        options.ResponseType = OpenIdConnectResponseType.Code;
            //        options.SaveTokens = true;
            //        options.Scope.Add("email");
            //        options.GetClaimsFromUserInfoEndpoint = true;
            //    });

            //Program Services
            services.AddScoped<ISeedDataRepository, SeedDataRepository>();
            services.AddScoped<IUserLoginRepository, UserLoginRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            SeedData(app);
        }
        private void SeedData(IApplicationBuilder app)
        {
            IServiceProvider serviceProvider = app.ApplicationServices.CreateScope().ServiceProvider;
            var seedDataService = serviceProvider.GetRequiredService<ISeedDataRepository>();
            seedDataService.Initialise();
        }
    }
}
