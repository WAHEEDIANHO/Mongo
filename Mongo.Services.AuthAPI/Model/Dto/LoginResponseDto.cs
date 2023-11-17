namespace Mongo.Services.AuthAPI.Model.Dto
{
    public class LoginResponseDto
    {
        public UserDto user { get; set; }
        public string token { get; set; }
    }
}
