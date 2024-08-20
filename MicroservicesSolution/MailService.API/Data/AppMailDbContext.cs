using MailService.API.Models;
using Microsoft.EntityFrameworkCore;


namespace MailService.API.Data
{
    public class AppMailDbContext : DbContext
    {
        public AppMailDbContext(DbContextOptions<AppMailDbContext> options) : base(options)
        {
        }

        public DbSet<MailLogger> MailLoggers { get; set; }

    }
}