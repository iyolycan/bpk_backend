using Ajinomoto.Arc.Common.Constants;

namespace Ajinomoto.Arc.Business.Helper
{
    public static class BusinessHelper
    {
        public static string GetSapFilePath(string sharingFolder, string interfaceNumber, DateOnly paymentDate)
        {
            string fileName = string.Format(ConfigConstants.S_SAP_EXPORT_FILE, 
                interfaceNumber, 
                paymentDate.ToString("ddMMyyyy"));

            Directory.CreateDirectory(sharingFolder);
            var filePath = Path.Combine(sharingFolder, fileName);

            return filePath;
        }

        public static string GetBpkNumber(DateTime createdAt, string interfaceNumber)
        {
            return
                createdAt.ToString("ddMMyyyy") + ConfigConstants.S_DOC + interfaceNumber;
        }
    }
}
