using Ajinomoto.Arc.Data.Models;

namespace Ajinomoto.Arc.Business.Modules
{
    public partial class MasterDataService
    {
        public IQueryable<AppUser> GetAllActiveAppUser()
        {
            var result = _domainService.GetAllAppUser().Where(x => x.IsActive);

            return result;
        }

        public IQueryable<Area> GetAllActiveArea()
        {
            var result = _domainService.GetAllArea().Where(x => x.IsActive);

            return result;
        }

        public IQueryable<BpkDetail> GetAllActiveBpkDetail()
        {
            var result = _domainService.GetAllBpkDetail().Where(x => x.DeletedFlag == false);

            return result;
        }

        public IQueryable<Branch> GetAllActiveBranch()
        {
            var result = _domainService.GetAllBranch().Where(x => x.IsActive);

            return result;
        }

        public IQueryable<PotonganType> GetAllActivePotonganType()
        {
            var result = _domainService.GetAllPotonganType().Where(x => x.IsActive);

            return result;
        }

        public IQueryable<Segment> GetAllActiveSegment()
        {
            var result = _domainService.GetAllSegment().Where(x => x.IsActive);

            return result;
        }

        public IQueryable<Source> GetAllActiveSource()
        {
            var result = _domainService.GetAllSource().Where(x => x.IsActive);

            return result;
        }

    }
}
