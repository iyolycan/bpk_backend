using Microsoft.AspNetCore.Http;

namespace Ajinomoto.Arc.Common.DtoModels
{
    public class ImportIncomingPaymentRequest
    {
        public int SourceId { get; set; }
        public int PicId { get; set; }
        public int SegmentId { get; set; }
        public IFormFile FileUpload { get; set; }
    }
}
