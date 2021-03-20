using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RefactorThis.Controllers;
using RefactorThis.Middleware;
using RefactorThis.Repositories;
using RefactorThis.Validators;

namespace RefactorThis
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddScoped<IProductController, ProductControllerImpl>();
            services.AddScoped<IOptionController, ProductOptionControllerImpl>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductOptionRepository, ProductOptionRepository>();

            services.AddScoped<ICreateOrUpdateProductRequestValidator, CreateOrUpdateProductRequestValidator>();
            services.AddScoped<ICreateOrUpdateOptionRequestValidator, CreateOrUpdateOptionRequestValidator>();

            services.AddResponseCompression();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiddleware<ExceptionResponseMiddleware>();

            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
