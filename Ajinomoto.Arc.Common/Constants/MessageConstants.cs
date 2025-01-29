namespace Ajinomoto.Arc.Common.Constants
{
    public static class MessageConstants
    {
        public const string S_RESPONSE_STATUS_CODE_OK = "200";
        public const string S_RESPONSE_STATUS_CODE_NOT_FOUND = "404";
        public const string S_RESPONSE_STATUS_CODE_INTERNAL_SERVER_ERROR = "500";

        public const string S_NOT_AUTHORIZE = "Unauthorized";
        public const string S_REQUEST_INVALID = "The Request is invalid";

        public const string S_DATA_NOT_FOUND = "Data Not found!";
        public const string S_UNEXPECTED_ERROR_OCCURRED = "Unexpected Error Occurred!";
        public const string S_SUCCESSFULLY = "Successfully!";
        public const string S_SOMETHING_ERROR = "Something error!";
        public const string S_LOGIN_ERROR = "Login error!";
        public const string S_SECRET_KEY_NOT_CORRECT = "Secret key is not correct!";
        public const string S_EMAIL_FORGOT_PASSWORD = "Please check your email to change password";

        // Save BPK
        public const string S_INVOICE_NOT_AVAILABLE = "Invoice Number {0} not available";
        public const string S_POTONGAN_NOT_AVAILABLE = "Potongan Number {0} already exist";
        public const string S_POTONGAN_ONLY_ONE = "Bank Charge or Pembulatan can only be one.";
        public const string S_POTONGAN_NUMBER_MAX_RETURN_PROMOSI = "Return or Promosi Potongan Number must be less than or equal to 17.";
        public const string S_POTONGAN_NUMBER_MAX_DEFAULT = "Potongan Number must be less than or equal to 50.";
        public const string S_BPK_SAVED = "BPK Saved";
        public const string S_BPK_SUBMITTED = "BPK Submitted";
        public const string S_BPK_REJECTED = "BPK Rejected";
        public const string S_BPK_REQUEST_FOR_REVISION = "BPK Request For Revised";
        public const string S_BPK_NOT_REQUEST_FOR_REVISION = "BPK is not Request For Revised";
        public const string S_BPK_DIFFERENCE_NOT_ZERO = "Difference not 0";
        public const string S_BPK_NOT_FOUND = "BPK Not Found.";
        public const string S_EXPORT_SAP_CREATED = "Export SAP File Created";
        public const string S_BPK_REVISE_APPROVE = "BPK Request For Revised Approve";
        public const string S_BPK_REVISE_REJECT = "BPK Request For Revised Rejected";
        public const string S_PEMBULATAN_MAX_INVALID = "Maximum/Minimum Pembulatan +/-{0}";
        public const string S_BPK_REQUEST_FILE_NOT_EXIST = "Cannot request. File Clearing not exist.";

        //Incoming Payment
        public const string S_INCOMINGPAYMENT_UPLOAD = "Incoming Payment Uploaded";
        public const string S_PERIOD_CUT_OFF_NOT_FOUND = "Period CutOff Not Found.";
        public const string S_INCOMING_PAYMENT_NOT_CLEAR = "Incoming Payment not Clearing yet";
        public const string S_INCOMING_PAYMENT_CLEARING_MANUAL_FAILED = "Cannot Open Clearing Clearing Manual";
        public const string S_INCOMING_PAYMENT_OPEN_CLEARING = "Incoming Payment has been Open Clearing";
        public const string S_INCOMING_PAYMENT_NOT_FOUND = "Incoming Payment Not Found.";
        public const string S_INCOMING_PAYMENT_REMOVED = "Incoming Payment Removed";

        // Import incoming payment
        public const string S_INCORRECT_TEMPLATE = "Please use the correct template";
        public const string S_CLEARING_NUMBER_DIFF = "Clearing number in Excel Cell {0} is different with Clearing number of Interface Number {1} ({2}) in App";
        public const string S_CLEARING_MANUAL_SUBMITTED_FILE_NOT_EXIST = "Cannot Clearing Manual for Excel Cell {0}, file clearing not exist";

        // Area
        public const string S_AREA_NOT_FOUND = "Area Not Found.";
        public const string S_AREA_EXIST = "Area Already Exist";
        public const string S_AREA_SAVED = "Area Saved";
        public const string S_AREA_IS_USED = "You cannot remove, Area already used in transaction. Try to deactivate it instead";
        public const string S_AREA_REMOVED = "Area Removed";

        // Branch
        public const string S_BRANCH_NOT_FOUND = "Branch Not Found.";
        public const string S_BRANCH_EXIST = "Branch Already Exist";
        public const string S_BRANCH_SAVED = "Branch Saved";
        public const string S_BRANCH_IS_CORE = "You cannot remove Core Branch. Try to deactivate it instead";
        public const string S_BRANCH_IS_USED = "You cannot remove, Branch already used in transaction. Try to deactivate it instead";
        public const string S_BRANCH_REMOVED = "Branch Removed";

        // Segment
        public const string S_SEGMENT_NOT_FOUND = "Segment Not Found.";
        public const string S_SEGMENT_EXIST = "Segment Already Exist";
        public const string S_SEGMENT_SAVED = "Segment Saved";
        public const string S_SEGMENT_IS_CORE = "You cannot remove Core Segment. Try to deactivate it instead";
        public const string S_SEGMENT_IS_USED = "You cannot remove, Segment already used in transaction. Try to deactivate it instead";
        public const string S_SEGMENT_REMOVED = "Segment Removed";

        // Source
        public const string S_SOURCE_NOT_FOUND = "Source Not Found.";
        public const string S_SOURCE_EXIST = "Source Already Exist";
        public const string S_SOURCE_SAVED = "Source Saved";
        public const string S_SOURCE_IS_CORE = "You cannot remove Core Source. Try to deactivate it instead";
        public const string S_SOURCE_IS_USED = "You cannot remove, Source already used in transaction. Try to deactivate it instead";
        public const string S_SOURCE_REMOVED = "Source Removed";

        // User
        public const string S_USER_NOT_FOUND = "User Not Found";
        public const string S_USER_EXIST = "User Already Exist";
        public const string S_USER_SAVED = "User Saved";
        public const string S_INVALID_EMAIL = "User Email Invalid";
        public const string S_USER_IS_ADMIN = "You cannot edit Administrator account";


        // Potongan Type
        public const string S_POTONGAN_TYPE_NOT_FOUND = "Potongan Type Not Found";
        public const string S_POTONGAN_TYPE_EXIST = "Potongan Type Already Exist";
        public const string S_POTONGAN_TYPE_SAVED = "Potongan Type Saved";
        public const string S_POTONGAN_TYPE_IS_CORE = "You cannot remove Core Potongan Type. Try to deactivate it instead";
        public const string S_POTONGAN_TYPE_IS_USED = "You cannot remove, Potongan Type already used in transaction. Try to deactivate it instead";
        public const string S_POTONGAN_TYPE_REMOVED = "Potongan Type Removed";

        // Role
        public const string S_ROLE_NOT_FOUND = "Role Not Found";
        public const string S_ROLE_EXIST = "Role Already Exist";
        public const string S_ROLE_SAVED = "Role Saved";
        public const string S_ROLE_IS_CORE = "You cannot remove Core Role";
        public const string S_ROLE_IS_USED = "You cannot remove, Role already used in transaction";
        public const string S_ROLE_REMOVED = "Role Removed";

        // Forgot Password
        public const string S_USERNAME_NOT_FOUND = "Username Not Found";
        public const string S_NOT_AJINOMOTO_DOMAIN = "Not valid Ajinomoto email";
        public const string S_EMAIL_NOT_FOUND = "Email Not Found";
        public const string S_EMAIL_SENT = "Email Sent";
        public const string S_INVALID_LINK = "Invalid Link";
        public const string S_VALID_LINK = "Valid Link";
        public const string S_PASSWORD_CHANGES = "Password has been change";
        public const string S_USERNAME_NOT_FOUND_INACTIVE = "Username Not Found or Not Active";


        // Config
        public const string S_CONFIG_NOT_FOUND = "Config Not Found";

    }
}
