using Mongo.Services.EmailAPI.Model.Dto;
using Mongo.Services.EmailAPI.Models.Dto;

namespace Mongo.Services.EmailAPI.Utils.IUtils
{
    public interface IEmailService
    {
        Task EmailAndLogCart(CartDto cart);
        Task EmailAndLogUser(UserDto user);
    }
}
