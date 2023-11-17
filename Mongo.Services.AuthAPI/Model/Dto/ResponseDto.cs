namespace Mongo.Services.AuthAPI.Model.Dto
{
    public class ResponseDto
    {
        public object? Result { get; set; }
        public bool IsSuccessful { get; set; } = true;
        public string message { get; set; } = "";
    }
}
