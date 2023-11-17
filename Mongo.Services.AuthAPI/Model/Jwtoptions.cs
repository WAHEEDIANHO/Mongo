namespace Mongo.Services.AuthAPI.Model
{
    public class Jwtoptions
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;  
        public string Audience { get; set; } = string.Empty;     

    }
}
