namespace Ajinomoto.Arc.Common.Enums
{
    public enum AppActionEnum
    {
        Created = 1,
        SaveAsDraft,
        Submitted,
        Clearing,
        RequestForRevision,
        ReviseApprove,
        ReviseReject,
        OpenClearing
    }

    public enum AppUserEnum
    {
        Administrator = 1
    }

    public enum AppConfigEnum
    {
        DAYS_TO_BPK_REMINDER = 1,
        DAYS_RE_SEND_EMAIL,
        MAX_PEMBULATAN,
        MAX_EXPORT_PAYMENT_LIST
    }

    public enum BranchEnum
    {
        Cjkt = 1,
        CSub,
        CMes
    }

    public enum BpkStatusEnum
    {
        BpkNotCreated = 1,
        BpkDraft,
        BpkSubmitted,
        BpkRejected,
        BpkRevised
    }

    public enum ClearingStatusEnum
    {
        NotYet = 1,
        ClearingOK,
        ClearingFailed,
    }

    public enum DataLevelEnum
    {
        AllData = 1,
        BranchLevel,
        AreaLevel
    }

    public enum IncomingPaymentColumn
    {
        PaymentDate,
        CustomerName
    }

    public enum KpiPropertyEnum
    {
        ArTransaction = 1,
        BpkReceived,
        ClearingAr,
        UploadInvoice,
        CreateInvoice
    }

    public enum PotonganTypeEnum
    {
        Return = 1,
        Promosi,
        CreditMemo,
        DebitMemo,
        ChargePO,
        BankCharge,
        Pembulatan,
        LebihBayar,
        KurangBayar
    }

    public enum RoleEnum
    {
        Administrator = 1,
        Coec,
        AdminCJkt,
        AdminCSub,
        AdminCMes,
        AdminArea,
        AdminSpm
    }

    public enum SegmentEnum
    {
        Spm = 1,
        PromosiPta,
        Df,
        Distributor,
        Sdl,
        Industry,
        OffSales,
        Bread,
        ECommerce,
        Amina,
        DfUploadInv,
        ArNexPtAiInvoice,
        ArNexPtAiClearing
    }

    public enum SortingDirection
    {
        Ascending = 1,
        Descending
    }

    public enum SourceEnum
    {
        BcaVa = 1,
        BriVa,
        Bca3500,
        Cpu,
        BotPtAsi,
        Maybank,
        Bri,
        Domestic,
        Export
    }

    public enum StatusCodeEnum
    {
        Ok = 201,
        Accepted = 202,
        Unauthorized = 401,
        Forbidden = 403,
        Error = 500,
        GatewayTimeout = 503,
        TokenInValid = 190,
        BadRequest = 400,
        NotFound = 404,
        NotAcceptable = 406,
        InvalidData = 422
    };

    public enum TemplateUploadTypeEnum
    {
        TemplateUpload01 = 1,
        TemplateUpload02,
        TemplateUpload03,
        TemplateUpload04,
        TemplateUpload05,
        TemplateUpload06,
        TemplateUpload07,
        TemplateUpload08,
        TemplateUpload09,
        TemplateUpload10,
        TemplateUpload11,
        TemplateUpload12,
        TemplateUpload13
    }
}
