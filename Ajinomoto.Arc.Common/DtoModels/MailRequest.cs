using Microsoft.AspNetCore.Http;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class MailRequest
    {
        public List<string> ToEmail { get; set; }
        public List<string> Cc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
