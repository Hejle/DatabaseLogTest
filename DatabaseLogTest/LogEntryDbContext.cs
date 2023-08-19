using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DatabaseLogTest.Models
{
    public class LogEntryDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public LogEntryDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:LogDatabase"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>().HasKey(x => x.Id);
        }
    }
}