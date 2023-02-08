using MyClassLibrary.Data;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using MyClassLibrary.UnitOfWork;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace function_app_entityFramework
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddDbContext<ToDoListDbContext>(options =>
            {
                options.UseSqlServer(GetConnectionStringFromKeyVault(config));
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        private static string GetConnectionStringFromKeyVault(IConfiguration config)
        {
            try
            {
                string keyVaultUrl = config["KeyVaultUrl"];
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));

                var secret = keyVaultClient.GetSecretAsync(keyVaultUrl, "ConnectionStringa").GetAwaiter().GetResult();
                return secret.Value;
            }
            catch (Exception ex)
            {
                // Logga l'eccezione o gestiscila come desideri
                Console.WriteLine("Errore durante la recupero della connection string dal Key Vault: " + ex.Message);
                throw;
            }
        }
    }
}
