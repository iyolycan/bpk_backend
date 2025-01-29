using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;

namespace Ajinomoto.Arc.Common.Extensions
{
    public static class CustomExtensions
    {
        public static ServiceResponse<T> GenerateResponse<T>(this T result)
        {
            if (result != null)
            {
                var serviceResponse = new ServiceResponse<T>()
                {
                    Data = result,
                    Succeeded = true,
                    ErrorMessage = null,
                    Code = MessageConstants.S_RESPONSE_STATUS_CODE_OK
                };

                return serviceResponse;
            }

            var errorResponse = new ServiceResponse<T>()
            {
                Data = result,
                Succeeded = false,
                ErrorMessage = MessageConstants.S_DATA_NOT_FOUND,
                Code = MessageConstants.S_RESPONSE_STATUS_CODE_NOT_FOUND
            };

            return errorResponse;
        }

        public static ServiceResponse<IEnumerable<T>> GenerateResponse<T>(this IEnumerable<T> result)
        {
            if (result != null)
            {
                var serviceResponse = new ServiceResponse<IEnumerable<T>>()
                {
                    Data = result,
                    Succeeded = true,
                    ErrorMessage = null,
                    Code = MessageConstants.S_RESPONSE_STATUS_CODE_OK
                };

                return serviceResponse;
            }

            var errorResponse = new ServiceResponse<IEnumerable<T>>()
            {
                Data = result,
                Succeeded = false,
                ErrorMessage = MessageConstants.S_DATA_NOT_FOUND,
                Code = MessageConstants.S_RESPONSE_STATUS_CODE_NOT_FOUND
            };

            return errorResponse;
        }
        public static string ToFixedString(this double num)
        {
            var decimalMultiplier = 10000; // 4 decimal point
            var result = string.Format("{0:N2}", (Math.Truncate(num * decimalMultiplier) / decimalMultiplier));

            return result;
        }

        public static DateOnly? ToDbDatetime(this string? str, string formatTime)
        {
            if (string.IsNullOrEmpty(str)) return null;

            var result = DateOnly.ParseExact(str, formatTime, null);

            return result;
        }

        public static string ToHexString(this Guid guidId)
        {
            return "0x" + guidId.ToString("N");
        }
    }
}
