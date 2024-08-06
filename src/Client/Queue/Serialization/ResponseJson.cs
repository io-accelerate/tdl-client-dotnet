using Newtonsoft.Json;
using TDL.Client.Queue.Abstractions.Response;

namespace TDL.Client.Queue.Serialization
{
    public class ResponseJson
    {
        [JsonProperty("result")]
        public object Result { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public static string Serialize(IResponse response) =>
            JsonConvert.SerializeObject(From(response));

        private static ResponseJson From(IResponse response) =>
            new ResponseJson
            {
                Result = response.Result,
                Error = null,
                Id = response.Id
            };
    }
}
