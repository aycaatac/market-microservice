using MailService.API.Data;
using MailService.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MailService.API.Services
{
    public class MailServiceImp : IMailService
    {
        private DbContextOptions<AppMailDbContext> dbOptions;

        public MailServiceImp(DbContextOptions<AppMailDbContext> options)
        {
            this.dbOptions = options;
        }

        public async Task MailCartAndLog(ShoppingCartDto cartDto)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested ");
            message.AppendLine("<br/>Total " + cartDto.CartHeader.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");
            foreach (var item in cartDto.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + " x " + item.ProductCount);
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }

        public async Task RegisterLog(string emailAddress)
        {
            await RegisterLogImp(emailAddress);
        }
        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                MailLogger mailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };

                await using var db = new AppMailDbContext(dbOptions);
                await db.MailLoggers.AddAsync(mailLog);
                await db.SaveChangesAsync();

                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
        }

        public async Task<bool> RegisterLogImp(string emailAddress)
        {
            try
            {
                MailLogger mailLog = new()
                {
                    Email = emailAddress,
                    EmailSent = DateTime.Now,
                    Message = "A new user was registered! The email address of the user is: " + emailAddress
                };

                await using var db = new AppMailDbContext(dbOptions);
                await db.MailLoggers.AddAsync(mailLog);
                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
        }
    }
}
