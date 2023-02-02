using MyClassLibrary.Data;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using MyClassLibrary.UnitOfWork;

namespace function_app_entityFramework
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            var config = new ConfigurationBuilder()
                .Build();
            builder.Services.AddDbContext<ToDoListDbContext>(options =>
                options.UseSqlServer(connectionString));
            //builder.Services.AddSingleton(typeof(IRepository<,>), typeof(Repository<,>));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


        }
    }
}
