using TDL.Client.Audit;

namespace TDL.Client.Queue.Abstractions.Response
{
    public class ValidResponse : IResponse
    {
        public string Id { get; set; }

        public object Result { get; }

        public ValidResponse(
            string id,
            object result)
        {
            Id = id;
            Result = result;
        }

        public string AuditText => "resp = "+Result.ToDisplayableResponse();
    }
}
