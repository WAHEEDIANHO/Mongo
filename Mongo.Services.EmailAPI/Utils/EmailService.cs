using Microsoft.EntityFrameworkCore;
using Mongo.Services.EmailAPI.Data;
using Mongo.Services.EmailAPI.Model.Dto;
using Mongo.Services.EmailAPI.Models;
using Mongo.Services.EmailAPI.Models.Dto;
using Mongo.Services.EmailAPI.Utils.IUtils;
using System.Text;

namespace Mongo.Services.EmailAPI.Utils
{
    public class EmailService : IEmailService
    {

        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public EmailService(DbContextOptions<AppDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task EmailAndLogCart(CartDto cart)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested");
            message.AppendLine("<br/>Total " + cart.CartHeader.cartTotal);
                message.Append("<br/>");
            message.Append("<ul>");
            foreach(var item in cart.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.product.Name + " x " + item.Count);
                message.Append("</li>");
                message.Append("</ul>");
            }

            await LogAndMail(message.ToString(), cart.CartHeader.Email);
        }

        public async Task EmailAndLogUser(UserDto user)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>User Register successful");

            await LogAndMail(message.ToString(), user.email);
        }
        public async Task EmailAndLogOrder(RewardsDto reward)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>User Register successful");

            await LogAndMail(message.ToString(), reward.UserId);
        }

        private async Task<bool> LogAndMail(string message, string email)
        {
            EmailLogger emailLogger = new EmailLogger()
            {
                Email = email,
                Message = message,
                EmailSent = DateTime.Now,
            };

            await using var _db = new AppDbContext(_dbContextOptions);
            _db.EmailLoggers.Add(emailLogger);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
