namespace Cathode.Common.Protocol
{
    public enum ApiErrorCode
    {
        Unknown,
        InternalError,
        UnsupportedApiVersion,
        InvalidApiVersion,
        ValidationFailed,
        BadRequest,
        Unauthorised,
        Forbidden,
        NotFound,
        UnsupportedMediaType,
        AlreadyInUse,
        NotConfigured,
        AlreadyActivated
    }
}