using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyClassLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace function_app_entityFramework
{
    public class ToDoListDbContextFactory : IDesignTimeDbContextFactory<ToDoListDbContext>
    {
        public ToDoListDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ToDoListDbContext>();
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            string connectionString = GetConnectionString(config);
            builder.UseSqlServer(connectionString);
            return new ToDoListDbContext(builder.Options, config);
        }

        private static string GetConnectionString(IConfiguration config)
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
