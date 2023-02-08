using MyClassLibrary.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace MyClassLibrary.Data
{
    public class ToDoListDbContext : DbContext
    {
        private readonly IConfiguration _config;
        public ToDoListDbContext()
        {

        }
        public ToDoListDbContext(DbContextOptions<ToDoListDbContext> options, IConfiguration config)
            : base(options)
        {
            _config = config;
        }

        public DbSet<ToDoList> ToDoList { get; set; } = null!;
        public DbSet<Dipendenti> Dipendenti { get; set; } = null!;

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


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = GetConnectionStringFromKeyVault(_config);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDoList>()
                .HasOne(t => t.Dipendente)
                .WithMany(d => d.Tasks)
                .HasForeignKey(t => t.DipendenteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
