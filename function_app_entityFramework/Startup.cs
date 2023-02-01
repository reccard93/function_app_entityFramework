using function_app_entityFramework.Data;
using function_app_entityFramework.models;
using function_app_entityFramework.Repositories;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using MyClassLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            builder.Services.AddScoped<IToDoListRepository, ToDoListRepository>();
            builder.Services.AddScoped<IDependentsRepository, DependentsRepository>();


        }
    }
}
