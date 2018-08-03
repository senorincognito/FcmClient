namespace FcmClient
{
    public class FcmSendResult
    {
        public FcmSendResult(string[] successfulSends, string[] failedSends)
        {
            SuccessfulSends = successfulSends;
            FailedSends = failedSends;
        }
        public FcmSendResult()
        {
            SuccessfulSends = new string[0];
            FailedSends = new string[0];
        }
        public string[] SuccessfulSends { get; set; }
        public string[] FailedSends { get; set; }
    }
}
