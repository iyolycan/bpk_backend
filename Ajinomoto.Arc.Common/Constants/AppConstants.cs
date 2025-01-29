namespace Ajinomoto.Arc.Common.Constants
{
    public class ConfigConstants
    {
        public const int N_DEFAULT_PAGESIZE = 10;
        public const int N_DEFAULT_PAGE = 1;

        public const int N_INIT_REVISION = 0;

        public const string S_DOC = "DOC";
        public const string S_FORMAT_DATE = "dd-MM-yyyy";

        public const string S_ACCOUNTING_FORMAT = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";
        public const string S_SAP_DATE_FORMAT = "dd.MM.yyyy";

        public const string S_DEFAULT_PASSWORD = "12345";
        public const string S_RESET_PASSWORD_MODULE = "reset_password";

        public const string S_SYSTEM = "SYSTEM";
        public const string S_NO = "NO";

        public const string S_SAP_EXPORT_FILE = "{0}{1}.xlsx";
    }

    public class AppInternalUser
    {
        public const string S_SYSTEM_USER = "SYSTEM";
    }

    public class AppSource
    {
        public const string S_WEB_APP = "WEBAPP";
        public const string S_TASK_SCHEDULER = "TASK SCHEDULER";
    }

    public class BpkConstants
    {
        public const string S_BPK_NOT_CREATED = "BPK Not Created";
        public const string S_BPK_NOT_FOUND = "BPK Not Found";
        public const string S_FORM_TRANSFER = "Transfer";
        public const string S_HISTORY_CREATED = "Created";
    }

    public class ExcelConstant
    {
        public const string S_STRING_CONVERTER = "'";
    }
}
