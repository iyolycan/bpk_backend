using Ajinomoto.Arc.Api.Authorization;
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.DtoModels;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

namespace Ajinomoto.Arc.Api.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportFacade _reportFacade;

        public ReportController(IReportFacade reportFacade)
        {
            _reportFacade = reportFacade;
        }

        [HttpGet("bpk")]
        public async Task<FileResult> GenerateBpkReport(Guid incomingPaymentId)
        {
            XLWorkbook wb = await _reportFacade.GenerateBpkReport(incomingPaymentId);
            string fileName = "ExportBPK_" + incomingPaymentId + ".xlsx";

            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        // [Route("IncomingPaymentList")]
        // [HttpPost]
        // public async Task<List<FileResult>> GenerateIncomingPaymentListReport(IncomingPaymentListRequest param)
        // {
        //     XLWorkbook wb = await _reportFacade.GenerateIncomingPaymentListReport(param);
        //     string fileName = "IncomingPaymentList-Report.xlsx";

        //     using (MemoryStream stream = new MemoryStream())
        //     {
        //         wb.SaveAs(stream);

        //         return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //     }
        // }

        [Route("IncomingPaymentList")]
        [HttpPost]
        public async Task<FileResult> GenerateIncomingPaymentListReport(IncomingPaymentListRequest param)
        {
            // Call the service to generate the report, which now returns a single XLWorkbook
            XLWorkbook wb = await _reportFacade.GenerateIncomingPaymentListReport(param);
            
            if (wb == null)
            {
                // Handle the case where the workbook generation failed
                // You can return a different response or handle errors as necessary
                // For example, return NotFound or BadRequest based on your needs
                return null; // Or return an appropriate error response
            }

            string fileName = "IncomingPaymentList-Report.xlsx";

            using (MemoryStream stream = new MemoryStream())
            {
                // Save the workbook to the memory stream
                wb.SaveAs(stream);
                
                // Reset the position of the stream to the beginning
                stream.Position = 0;

                // Return the file result
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }


        [Route("Kpi")]
        [HttpGet]
        public async Task<FileResult> GenerateKpiReport(string period, int? picId)
        {
            XLWorkbook wb = await _reportFacade.GenerateKpiReport(period, picId);
            string fileName = "KPI-Report-" + period + ".xlsx";

            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
