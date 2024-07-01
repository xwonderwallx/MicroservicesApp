using UserService.Enums;

namespace UserService.Models.Responses
{
    public class ServiceResponse
    {
        public ResponseType ResponseType { get; set; }
        public string ResponseMessage { get; set; }
        public object AdditionalObject { get; set; }
    }
}
