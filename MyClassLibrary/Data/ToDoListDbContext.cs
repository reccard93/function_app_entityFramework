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

        public ToDoListDbContext(DbContextOptions<ToDoListDbContext> options, IConfiguration config)
            : base(options)
        {
            _config = config;
        }

        public DbSet<ToDoList> ToDoList { get; set; } = null!;
        public DbSet<Dipendenti> Dipendenti { get; set; } = null!;

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
