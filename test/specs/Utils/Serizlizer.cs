using System.IO;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;

namespace TDL.Test.Specs.Utils
{
    /// <summary>
    /// JSON serializer for request bodies using Newtonsoft.Json
    /// </summary>
    public class NewtonsoftJsonSerializer : IRestSerializer
    {
        private readonly JsonSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewtonsoftJsonSerializer"/> class.
        /// </summary>
        public NewtonsoftJsonSerializer()
        {
            ContentType = ContentType.Json;
            _serializer = new JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewtonsoftJsonSerializer"/> class with custom Json.NET settings.
        /// </summary>
        /// <param name="serializer">The custom Json.NET serializer.</param>
        public NewtonsoftJsonSerializer(JsonSerializer serializer)
        {
            ContentType = ContentType.Json;
            _serializer = serializer;
        }

        /// <summary>
        /// Serialize the object as JSON
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON as String</returns>
        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';

                    _serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Deserialize the JSON to an object
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize</typeparam>
        /// <param name="response">The response containing JSON string</param>
        /// <returns>Deserialized object</returns>
        public T Deserialize<T>(RestResponse response)
        {
            using (var stringReader = new StringReader(response.Content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return _serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public string Serialize(Parameter parameter)
        {
            return Serialize(parameter.Value);
        }


        public ISerializer   Serializer           => (ISerializer)this;
        public IDeserializer Deserializer         => (IDeserializer)this;
        
        /// <summary>
        /// Content type for serialized content
        /// </summary>
        public ContentType ContentType { get; set; } = ContentType.Json;

        public DataFormat    DataFormat           => DataFormat.Json;
        public string[]      AcceptedContentTypes => ContentType.JsonAccept;
        public SupportsContentType SupportsContentType 
            => contentType => contentType.Value.EndsWith("json", StringComparison.InvariantCultureIgnoreCase);


        public string? SerializeBody(object? body)
        {
            return body == null ? null : Serialize(body);
        }


    }
}
