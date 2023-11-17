using Microsoft.AspNetCore.Identity;

namespace Mongo.Services.AuthAPI.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
