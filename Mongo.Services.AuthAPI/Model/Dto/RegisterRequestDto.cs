namespace Mongo.Services.AuthAPI.Model.Dto
{
    public class RegisterRequestDto
    {
        public string name {  get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phoneNumebr { get; set; }
        public string role { get; set; }
    }
}
