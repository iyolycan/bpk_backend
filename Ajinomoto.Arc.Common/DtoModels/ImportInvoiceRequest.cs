using Microsoft.AspNetCore.Http;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class ImportInvoiceRequest
    {
        public IFormFile FileUpload { get; set; }
    }
}
