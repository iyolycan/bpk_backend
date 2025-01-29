using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;
using Serilog;

namespace Ajinomoto.Arc.Business.Modules
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IDomainService _domainService;
        private readonly IMasterDataService _masterDataService;

        public InvoiceService(IDomainService domainService, IMasterDataService masterDataService)
        {
            _domainService = domainService;
            _masterDataService = masterDataService;
        }

        public async Task<InvoiceResponse?> GetInvoice(string invoiceNumber)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new InvoiceResponse();

                    result = (from a in _domainService.GetAllInvoice()
                              join cust in _domainService.GetAllCustomer() on a.CustomerCode equals cust.CustomerCode into ab
                              from b in ab.DefaultIfEmpty()
                              where a.InvoiceNumber == invoiceNumber
                              select new InvoiceResponse
                              {
                                  InvoiceNumber = a.InvoiceNumber,
                                  InvoiceDate = a.InvoiceDate.ToString(ConfigConstants.S_FORMAT_DATE),
                                  CustomerCode = a.CustomerCode,
                                  CustomerName = b.Name,
                                  Amount = a.Amount
                              }).FirstOrDefault();

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: GetInvoice(), invoiceNumber: {invoiceNumber}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }

        public async Task<ValidateMultipleInvoiceResponse> ValidateMultipleInvoices(List<string> invoices)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = new ValidateMultipleInvoiceResponse
                    {
                        ValidationMessage = new List<string>(),
                        Data = new List<ValidateMultipleInvoiceDetailResponse>()
                    };

                    var data = new List<ValidateMultipleInvoiceDetailResponse>();
                    foreach (var item in invoices)
                    {
                        var newItem = new ValidateMultipleInvoiceDetailResponse
                        {
                            InvoiceNumber = item,
                            IsNotExist = false,
                            IsUsed = false
                        };

                        data.Add(newItem);
                    }

                    var existInvoice = _domainService.GetAllInvoice()
                        .Where(x => invoices.Contains(x.InvoiceNumber))
                        .Select(x => x.InvoiceNumber)
                        .ToList();

                    var notExistInvoice = invoices.Where(x => !existInvoice.Contains(x));
                    foreach (var item in notExistInvoice)
                    {
                        var found = data.Single(x => x.InvoiceNumber == item);
                        found.IsNotExist = true;
                    }

                    if (notExistInvoice.Any())
                    {
                        result.ValidationMessage.Add("Invoice " + string.Join(", ", notExistInvoice.ToArray()) + " is not exist.");
                    }

                    var usedInBpk = _masterDataService.GetAllActiveBpkDetail()
                        .Where(x => x.InvoiceNumber != null && existInvoice.Contains(x.InvoiceNumber))
                        .Select(x => x.InvoiceNumber)
                        .ToList();
                    foreach (var item in usedInBpk)
                    {
                        var found = data.Single(x => x.InvoiceNumber == item);
                        found.IsUsed = true;
                    }

                    if (usedInBpk.Any())
                    {
                        result.ValidationMessage.Add("Invoice " + string.Join(", ", usedInBpk.ToArray()) + " already used in other BPK.");
                    }

                    var dataInvoice = (from a in data.Where(x => !x.IsNotExist && !x.IsUsed)
                                       join b in _domainService.GetAllInvoice() on a.InvoiceNumber equals b.InvoiceNumber
                                       join customer in _domainService.GetAllCustomer() on b.CustomerCode equals customer.CustomerCode into bc
                                       from c in bc.DefaultIfEmpty()
                                       select new ValidateMultipleInvoiceDetailResponse
                                       {
                                           InvoiceNumber = a.InvoiceNumber,
                                           InvoiceDate = b.InvoiceDate.ToString(ConfigConstants.S_FORMAT_DATE),
                                           CustomerCode = b.CustomerCode,
                                           CustomerName = c.Name,
                                           Amount = b.Amount,
                                           IsUsed = false,
                                           IsNotExist = false
                                       }).ToList();

                    foreach (var item in dataInvoice)
                    {
                        var found = data.Single(x => x.InvoiceNumber == item.InvoiceNumber);
                        found.InvoiceDate = item.InvoiceDate;
                        found.CustomerCode = item.CustomerCode;
                        found.CustomerName = item.CustomerName;
                        found.Amount = item.Amount;
                    }

                    result.Data = data;

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: ValidateMultipleInvoices(), invoices: {invoices}" +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }
    }
}
