using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Data;
using Ajinomoto.Arc.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using System.Data;

namespace Ajinomoto.Arc.Business
{
    public partial class DomainService : IDomainService
    {
        private readonly DataContext _dataContext;

        public DomainService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IQueryable<AppAction> GetAllAppAction()
        {
            return _dataContext.AppActions;
        }

        public IQueryable<AppConfig> GetAllAppConfig()
        {
            return _dataContext.AppConfigs;
        }

        public IQueryable<AppUser> GetAllAppUser()
        {
            return _dataContext.AppUsers;
        }

        public IQueryable<AppUserArea> GetAllAppUserArea()
        {
            return _dataContext.AppUserAreas;
        }

        public IQueryable<AppUserBranch> GetAllAppUserBranch()
        {
            return _dataContext.AppUserBranches;
        }

        public IQueryable<Area> GetAllArea()
        {
            return _dataContext.Areas;
        }

        public IQueryable<Bpk> GetAllBpk()
        {
            return _dataContext.Bpks;
        }

        // public IQueryable<BpkMasterStatus> GetAllMasterStatusBpk()
        // {
        //     return _dataContext.BpkMasterStatuses;
        // }

        public IQueryable<BpkDetail> GetAllBpkDetail()
        {
            return _dataContext.BpkDetails;
        }

        public IQueryable<BpkHistory> GetAllBpkHistory()
        {
            return _dataContext.BpkHistories;
        }

        public IQueryable<BpkStatus> GetAllBpkStatus()
        {
            return _dataContext.BpkStatuses;
        }

        public IQueryable<Branch> GetAllBranch()
        {
            return _dataContext.Branches;
        }

        public IQueryable<ClearingStatus> GetAllClearingStatus()
        {
            return _dataContext.ClearingStatuses;
        }

        public IQueryable<Customer> GetAllCustomer()
        {
            return _dataContext.Customers;
        }

        public IQueryable<DataLevel> GetAllDataLevel()
        {
            return _dataContext.DataLevels;
        }

        public IQueryable<IncomingPayment> GetAllIncomingPayment()
        {
            return _dataContext.IncomingPayments;
        }

        public IQueryable<IncomingPaymentCutOff> GetAllIncomingPaymentCutOff()
        {
            return _dataContext.IncomingPaymentCutOffs;
        }

        public IQueryable<IncomingPaymentNonSpm> GetAllIncomingPaymentNonSpm()
        {
            return _dataContext.IncomingPaymentNonSpms;
        }

        public IQueryable<IncomingPaymentView> GetAllIncomingPaymentView()
        {
            return _dataContext.IncomingPaymentViews;
        }

        public IQueryable<Invoice> GetAllInvoice()
        {
            return _dataContext.Invoices;
        }

        public IQueryable<KpiProperty> GetAllKpiProperty()
        {
            return _dataContext.KpiProperties;
        }

        public IQueryable<KpiSummary> GetAllKpiSummary()
        {
            return _dataContext.KpiSummaries;
        }

        public IQueryable<Potongan> GetAllPotongan()
        {
            return _dataContext.Potongans;
        }

        public IQueryable<PotonganType> GetAllPotonganType()
        {
            return _dataContext.PotonganTypes;
        }

        public IQueryable<Role> GetAllRole()
        {
            return _dataContext.Roles;
        }

        public IQueryable<RoleArea> GetAllRoleArea()
        {
            return _dataContext.RoleAreas;
        }

        public IQueryable<RoleBranch> GetAllRoleBranch()
        {
            return _dataContext.RoleBranches;
        }

        public IQueryable<Segment> GetAllSegment()
        {
            return _dataContext.Segments;
        }

        public IQueryable<SegmentKpiProperty> GetAllSegmentKpiProperty()
        {
            return _dataContext.SegmentKpiProperties;
        }

        public IQueryable<Source> GetAllSource()
        {
            return _dataContext.Sources;
        }

        public IQueryable<TemplateUploadType> GetAllTemplateUploadType()
        {
            return _dataContext.TemplateUploadTypes;
        }

        public IQueryable<UserView> GetAllUserView()
        {
            return _dataContext.UserViews;
        }

        public IDbContextTransaction BeginTransaction()
        {
            try
            {
                IDbContextTransaction transaction = _dataContext.Database.BeginTransaction(IsolationLevel.Serializable);

                return transaction;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: BeginTransaction(), Message: {ex}");
                throw;
            }
        }


    }
}