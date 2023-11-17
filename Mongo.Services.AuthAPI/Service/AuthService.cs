using Microsoft.AspNetCore.Identity;
using Mongo.MessageBus;
using Mongo.Services.AuthAPI.Data;
using Mongo.Services.AuthAPI.Model;
using Mongo.Services.AuthAPI.Model.Dto;
using Mongo.Services.AuthAPI.Service.IService;

namespace Mongo.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator,
            IMessageBus messageBus, IConfiguration configuration)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _messageBus = messageBus;
            _configuration = configuration;

        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginUser)
        {
            var user = _db.ApplicationUsers.First(u => u.Email.ToLower() == loginUser.username.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginUser.password);
            if (user == null || !isValid) {
                return new LoginResponseDto() { user = null, token="" };
            }

            var roles = await _userManager.GetRolesAsync(user); 
            var token = _jwtTokenGenerator.GenerateToken(user, roles);
            
            
            UserDto userDto = new ()
            {
                email = user.Email,
                id = user.Id,
                name = user.Name,   
                phoneNumber = user.PhoneNumber
            };

            return new LoginResponseDto()
            {
                user = userDto,
                token = token
            };

        }

        public async Task<string> Register(RegisterRequestDto registerRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registerRequestDto.email,
                Email = registerRequestDto.email,
                NormalizedEmail = registerRequestDto.email,
                Name = registerRequestDto.name,
                PhoneNumber = registerRequestDto.phoneNumebr
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerRequestDto.password);

                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registerRequestDto.email);
                    UserDto userDto = new()
                    {
                        email = userToReturn.Email,
                        name = userToReturn.Name,
                        phoneNumber = userToReturn.PhoneNumber,
                        id = userToReturn.Id
                    };

                   
                    await _messageBus.PubishMessge(userDto, _configuration.GetValue<string>("TopicAndQueueName:UserRegisterQueue"));
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }catch (Exception ex)
            {

            }
            return "Error";
        }

        public async Task<bool> AssignRole(string email, string role)
        {
            try
            {
                var user = _db.ApplicationUsers.First(u => u.Email.ToLower() == email.ToLower());
                Console.WriteLine(email, role);
                if (user != null)
                {
                    if(!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                    {
                        _roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                    }
                    await _userManager.AddToRoleAsync(user, role);
                    return true;
                }
                throw new Exception("unable to assign role");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
