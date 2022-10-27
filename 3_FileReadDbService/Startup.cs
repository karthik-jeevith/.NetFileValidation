using Confluent.Kafka;
using FileReadFromDb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileReadFromDb
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
            var consumerConfig = new ConsumerConfig();
            Configuration.Bind("consumer", consumerConfig);
            services.AddSingleton<ConsumerConfig>(consumerConfig);

            var producerConfig = new ProducerConfig();
            Configuration.Bind("producer", producerConfig);
            services.AddSingleton<ProducerConfig>(producerConfig);


            services.AddControllers();

            services.AddSingleton<DapperContext>();

            services.AddScoped<IJsonValidatorService, JsonValidatorService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Validating API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("Swagger/v1/swagger.json", "upload xml API V1");

                c.RoutePrefix = String.Empty;  // route url as main
            });
        }
    }
}
