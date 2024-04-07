//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Newtonsoft.Json;
    using STJ;
    using JsonSerializer = Newtonsoft.Json.JsonSerializer;

    /// <summary>
    /// The default Cosmos JSON.NET serializer.
    /// </summary>
    internal sealed class CosmosJsonDotNetSerializer : CosmosSerializer
    {
        private static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            TypeInfoResolver = InternalDocumentContext.Default
        };

        /// <summary>
        /// Create a serializer that uses the JSON.net serializer
        /// </summary>
        /// <remarks>
        /// This is internal to reduce exposure of JSON.net types so
        /// it is easier to convert to System.Text.Json
        /// </remarks>
        internal CosmosJsonDotNetSerializer()
        {
        }

        /// <summary>
        /// Create a serializer that uses the JSON.net serializer
        /// </summary>
        /// <remarks>
        /// This is internal to reduce exposure of JSON.net types so
        /// it is easier to convert to System.Text.Json
        /// </remarks>
        [Obsolete("You'll need to provide your own STJ", true)]
        internal CosmosJsonDotNetSerializer(CosmosSerializationOptions cosmosSerializerOptions)
        {
            if (cosmosSerializerOptions == null)
            {
                return;
            }
        }

        /// <summary>
        /// Create a serializer that uses the JSON.net serializer
        /// </summary>
        /// <remarks>
        /// This is internal to reduce exposure of JSON.net types so
        /// it is easier to convert to System.Text.Json
        /// </remarks>
        [Obsolete("You'll need to provide your own STJ", true)]
        internal CosmosJsonDotNetSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
        }

        /// <summary>
        /// Convert a Stream to the passed in type.
        /// </summary>
        /// <typeparam name="T">The type of object that should be deserialized</typeparam>
        /// <param name="stream">An open stream that is readable that contains JSON</param>
        /// <returns>The object representing the deserialized stream</returns>
        public override T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)stream;
                }

                using (TextReader tr = new StreamReader(stream, leaveOpen: true))
                {
                    stream.Position = 0;
                }

                return System.Text.Json.JsonSerializer.Deserialize<T>(stream, this.serializerOptions);
                /*
                using (StreamReader sr = new StreamReader(stream))
                {
                    using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
                    {
                        JsonSerializer jsonSerializer = this.GetSerializer();
                        return jsonSerializer.Deserialize<T>(jsonTextReader);
                    }
                }*/
            }
        }

        /// <summary>
        /// Converts an object to a open readable stream
        /// </summary>
        /// <typeparam name="T">The type of object being serialized</typeparam>
        /// <param name="input">The object to be serialized</param>
        /// <returns>An open readable stream containing the JSON of the serialized object</returns>
        public override Stream ToStream<T>(T input)
        {
            MemoryStream streamPayload = new MemoryStream();
            System.Text.Json.JsonSerializer.Serialize(streamPayload, input, this.serializerOptions);
            /*
            using (StreamWriter streamWriter = new StreamWriter(streamPayload, encoding: CosmosJsonDotNetSerializer.DefaultEncoding, bufferSize: 1024, leaveOpen: true))
            {
                using (JsonWriter writer = new JsonTextWriter(streamWriter))
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.None;
                    JsonSerializer jsonSerializer = this.GetSerializer();
                    jsonSerializer.Serialize(writer, input);
                    writer.Flush();
                    streamWriter.Flush();
                }
            }*/

            streamPayload.Position = 0;
            return streamPayload;
        }
    }
}
