using Ajinomoto.Arc.Data.Models;
using EFCore.BulkExtensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ajinomoto.Arc.Business;

public partial class DomainService 
{
    public void UpdateBulkBpk(List<Bpk> models)
    {
        try
        {
            _dataContext.BulkUpdate(models);
        }
        catch (Exception ex)
        {
            Log.Logger.Error($"Method: UpdateBulkBpk(), " +
                $"example 10 model: {string.Join(",", models.Take(10).Select(x => x.BpkId).ToArray())}, " +
                $"Message: {ex.Message}");
            throw;
        }
    }

    public void UpdateBulkBpkDetail(List<BpkDetail> models)
    {
        try
        {
            _dataContext.BulkUpdate(models);
        }
        catch (Exception ex)
        {
            Log.Logger.Error($"Method: UpdateBulkBpkDetail(), " +
                $"example 10 model: {string.Join(",", models.Take(10).Select(x => x.BpkDetailId).ToArray())}, " +
                $"Message: {ex.Message}");
            throw;
        }
    }


    public void UpdateBulkIncomingPayment(List<IncomingPayment> models)
    {
        try
        {
            _dataContext.BulkUpdate(models);
        }
        catch (Exception ex)
        {
            Log.Logger.Error($"Method: UpdateBulkBpkDetail(), " +
                $"example 10 model: {string.Join(",", models.Take(10).Select(x => x.IncomingPaymentId).ToArray())}, " +
                $"Message: {ex.Message}");
            throw;
        }
    }
}
