namespace TDL.Client.Queue.Abstractions.Response
{
    public class FatalErrorResponse : IResponse
    {
        public string Id => "error";

        public object Result { get; }

        public FatalErrorResponse(string message)
        {
            Result = message;
        }

        public string AuditText => $@"{Id} = ""{Result}"", (NOT PUBLISHED)";
    }
}
