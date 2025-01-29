using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.Enums;

namespace Ajinomoto.Arc.Business.Facades
{
    public class DropdownFacade : IDropdownFacade
    {
        private readonly IMasterDataService _masterDataService;

        public DropdownFacade(IMasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlArea(string filter)
        {
            var result = await _masterDataService.GetDdlArea(filter);

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlBranch()
        {
            var result = await _masterDataService.GetDdlBranch();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlCustomer()
        {

            var result = await _masterDataService.GetDdlCustomer();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlCustomer(string? filter)
        {

            var result = await _masterDataService.GetDdlCustomer(filter);

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlDataLevel()
        {
            var result = await _masterDataService.GetDdlDataLevel();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = null
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlInvoice(string filter)
        {
            var result = await _masterDataService.GetDdlInvoice(filter);

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = null
            };

            return errorResponse;
        }
        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlInvoiceByCustomer(string customerCode)
        {
            var result = await _masterDataService.GetDdlInvoiceByCustomer(customerCode);

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = null
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlKpiProperty()
        {
            var result = await _masterDataService.GetDdlKpiProperty();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlKpiPropertyCurrent()
        {
            var result = await _masterDataService.GetDdlKpiProperty();

            var validCurrent = new List<int>
            {
                (int)KpiPropertyEnum.ClearingAr,
                (int)KpiPropertyEnum.CreateInvoice,
                (int)KpiPropertyEnum.UploadInvoice
            };

            result = result.Where(x => validCurrent.Contains(Convert.ToInt32(x.Value))).ToList();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlKpiPropertyTotal()
        {
            var result = await _masterDataService.GetDdlKpiProperty();

            var validCurrent = new List<int>
            {
                (int)KpiPropertyEnum.BpkReceived,
                (int)KpiPropertyEnum.ArTransaction
            };

            result = result.Where(x => validCurrent.Contains(Convert.ToInt32(x.Value))).ToList();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlPotonganType()
        {
            var result = await _masterDataService.GetDdlPotonganType();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlRole()
        {
            var result = await _masterDataService.GetDdlRole();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlSegment()
        {
            var result = await _masterDataService.GetDdlSegment();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlSource()
        {
            var result = await _masterDataService.GetDdlSource();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlTemplateUploadType()
        {
            var result = await _masterDataService.GetDdlTemplateUploadType();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlUser()
        {
            var result = await _masterDataService.GetDdlUser();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }

        public async Task<ResultBase<IEnumerable<DropdownDto>>> GetDdlUserCoec()
        {
            var result = await _masterDataService.GetDdlUserCoec();

            if (result != null)
            {
                return new ResultBase<IEnumerable<DropdownDto>>()
                {
                    Success = true,
                    Message = MessageConstants.S_SUCCESSFULLY,
                    Model = result
                };
            }

            var errorResponse = new ResultBase<IEnumerable<DropdownDto>>()
            {
                Success = false,
                Message = MessageConstants.S_DATA_NOT_FOUND,
                Model = result
            };

            return errorResponse;
        }
    }
}
