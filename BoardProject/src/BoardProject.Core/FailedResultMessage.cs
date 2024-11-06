namespace BoardProject.Core
{
    public static class FailedResultMessage
    {
        public const string RequestValidation = "Validation of the request has failed.";
        public const string Unprocessable = "Unprocessable entity.";
        public const string NotFound = "Data not found.";
        public const string Exception = "Exception has been caught.";
        public const string NotProcessedDueToConfigSettings = "Config value or values do not allow to process the request.";
    }
}