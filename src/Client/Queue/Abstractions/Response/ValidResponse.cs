using TDL.Client.Audit;

namespace TDL.Client.Queue.Abstractions.Response
{
    public class ValidResponse : IResponse
    {
        public string Id { get; set; }

        public object Result { get; }

        public bool IsError => false;

        public ValidResponse(
            string id,
            object result)
        {
            Id = id;
            Result = result;
        }
    }
}
