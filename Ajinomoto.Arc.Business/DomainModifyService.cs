using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Data.Models;
using Serilog;

namespace Ajinomoto.Arc.Business
{
    public partial class DomainService : IDomainService
    {
        public void UpdateAppConfig(AppConfig model)
        {
            try
            {
                _dataContext.AppConfigs.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateConfigGeneral(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public int InsertAppUser(AppUser model)
        {
            try
            {
                _dataContext.AppUsers.Add(model);

                return model.AppUserId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertAppUser(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateAppUser(AppUser model)
        {
            try
            {
                _dataContext.AppUsers.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateAppUser(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public Guid InsertAppUserArea(AppUserArea model)
        {
            try
            {
                _dataContext.AppUserAreas.Add(model);

                return model.AppUserAreaId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertAppUserArea(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteAppUserArea(AppUserArea model)
        {
            try
            {
                _dataContext.AppUserAreas.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteAppUserArea(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public Guid InsertAppUserBranch(AppUserBranch model)
        {
            try
            {
                _dataContext.AppUserBranches.Add(model);

                return model.AppUserBranchId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertAppUserBranch(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteAppUserBranch(AppUserBranch model)
        {
            try
            {
                _dataContext.AppUserBranches.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteAppUserBranch(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public int InsertArea(Area model)
        {
            try
            {
                _dataContext.Areas.Add(model);

                return model.AreaId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertArea(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateArea(Area model)
        {
            try
            {
                _dataContext.Areas.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateArea(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteArea(Area model)
        {
            try
            {
                _dataContext.Areas.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteArea(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public Guid InsertBpk(Bpk model)
        {
            try
            {
                _dataContext.Bpks.Add(model);

                return model.BpkId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertBpk(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateBpk(Bpk model)
        {
            try
            {
                _dataContext.Bpks.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateBpk(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteBpk(Bpk model)
        {
            try
            {
                _dataContext.Bpks.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteBpk(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public Guid InsertBpkDetail(BpkDetail model)
        {
            try
            {
                _dataContext.BpkDetails.Add(model);

                return model.BpkDetailId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertBpkDetail(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateBpkDetail(BpkDetail model)
        {
            try
            {
                _dataContext.BpkDetails.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateBpkDetail(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteBpkDetail(BpkDetail model)
        {
            try
            {
                _dataContext.BpkDetails.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteBpkDetail(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public Guid InsertBpkHistory(BpkHistory model)
        {
            try
            {
                _dataContext.BpkHistories.Add(model);

                return model.BpkId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertBpkHistory(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateBpkHistory(BpkHistory model)
        {
            try
            {
                _dataContext.BpkHistories.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateBpkHistory(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteBpkHistory(BpkHistory model)
        {
            try
            {
                _dataContext.BpkHistories.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteBpkHistory(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public int InsertBranch(Branch model)
        {
            try
            {
                _dataContext.Branches.Add(model);

                return model.BranchId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertBranch(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateBranch(Branch model)
        {
            try
            {
                _dataContext.Branches.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateBranch(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteBranch(Branch model)
        {
            try
            {
                _dataContext.Branches.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteBranch(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public Guid InsertIncomingPayment(IncomingPayment model)
        {
            try
            {
                _dataContext.IncomingPayments.Add(model);

                return model.IncomingPaymentId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertIncomingPayment(), model: {model}, Message: {ex}");
                throw;
            }
        }
        public Guid InsertInvoiceDetails(InvoiceDetails model)
        {
            try
            {
                _dataContext.InvoiceDetails.Add(model);

                return Guid.Parse(model.InvoiceDetailsId);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertIncomingPayment(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateIncomingPayment(IncomingPayment model)
        {
            try
            {
                _dataContext.IncomingPayments.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateIncomingPayment(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteIncomingPayment(IncomingPayment model)
        {
            try
            {
                _dataContext.IncomingPayments.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteIncomingPayment(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public Guid InsertIncomingPaymentCutOff(IncomingPaymentCutOff model)
        {
            try
            {
                _dataContext.IncomingPaymentCutOffs.Add(model);

                return model.IncomingPaymentCutOffId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertConfigArCutOff(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateIncomingPaymentCutOff(IncomingPaymentCutOff model)
        {
            try
            {
                _dataContext.IncomingPaymentCutOffs.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateConfigArCutOff(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public Guid InsertIncomingPaymentNonSpm(IncomingPaymentNonSpm model)
        {
            try
            {
                _dataContext.IncomingPaymentNonSpms.Add(model);

                return model.IncomingPaymentNonSpmId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertIncomingPaymentNonSpm(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateIncomingPaymentNonSpm(IncomingPaymentNonSpm model)
        {
            try
            {
                _dataContext.IncomingPaymentNonSpms.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateIncomingPaymentNonSpm(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public Guid InsertPotongan(Potongan model)
        {
            try
            {
                _dataContext.Potongans.Add(model);

                return model.PotonganId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertPotongan(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdatePotongan(Potongan model)
        {
            try
            {
                _dataContext.Potongans.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdatePotongan(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeletePotongan(Potongan model)
        {
            try
            {
                _dataContext.Potongans.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeletePotongan(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public int InsertPotonganType(PotonganType model)
        {
            try
            {
                _dataContext.PotonganTypes.Add(model);

                return model.PotonganTypeId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertPotonganType(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdatePotonganType(PotonganType model)
        {
            try
            {
                _dataContext.PotonganTypes.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdatePotonganType(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeletePotonganType(PotonganType model)
        {
            try
            {
                _dataContext.PotonganTypes.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeletePotonganType(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public int InsertRole(Role model)
        {
            try
            {
                _dataContext.Roles.Add(model);

                return model.RoleId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertRole(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateRole(Role model)
        {
            try
            {
                _dataContext.Roles.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateRole(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteRole(Role model)
        {
            try
            {
                _dataContext.Roles.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteRole(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public Guid InsertRoleArea(RoleArea model)
        {
            try
            {
                _dataContext.RoleAreas.Add(model);

                return model.RoleAreaId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertRoleArea(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateRoleArea(RoleArea model)
        {
            try
            {
                _dataContext.RoleAreas.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateRoleArea(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteRoleArea(RoleArea model)
        {
            try
            {
                _dataContext.RoleAreas.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteRoleArea(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public Guid InsertRoleBranch(RoleBranch model)
        {
            try
            {
                _dataContext.RoleBranches.Add(model);

                return model.RoleBranchId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertRoleBranch(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateRoleBranch(RoleBranch model)
        {
            try
            {
                _dataContext.RoleBranches.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateRoleBranch(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteRoleBranch(RoleBranch model)
        {
            try
            {
                _dataContext.RoleBranches.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteRoleBranch(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public int InsertSegment(Segment model)
        {
            try
            {
                _dataContext.Segments.Add(model);

                return model.SegmentId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertSegment(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateSegment(Segment model)
        {
            try
            {
                _dataContext.Segments.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateSegment(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteSegment(Segment model)
        {
            try
            {
                _dataContext.Segments.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteSegment(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public int InsertSource(Source model)
        {
            try
            {
                _dataContext.Sources.Add(model);

                return model.SourceId;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: InsertSource(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void UpdateSource(Source model)
        {
            try
            {
                _dataContext.Sources.Update(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: UpdateSource(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void DeleteSource(Source model)
        {
            try
            {
                _dataContext.Sources.Remove(model);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: DeleteSource(), model: {model}, Message: {ex}");
                throw;
            }
        }

        public void SaveChanges()
        {
            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: SaveChanges(), Message: {ex}");
                throw;
            }
        }
    }
}
