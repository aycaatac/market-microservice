using EmailService.API.Models;
using Microsoft.EntityFrameworkCore;


namespace EmailService.API.Data
{
    public class AppEmailDbContext : DbContext
    {
        public AppEmailDbContext(DbContextOptions<AppEmailDbContext> options) : base(options)
        {
        }

        public DbSet<EmailLogger> EmailLoggers { get; set; }


    }
}