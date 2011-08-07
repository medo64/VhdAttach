using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

[DataContract()]
internal class ResponseData {

    private static readonly DataContractJsonSerializer DataSerializer = new DataContractJsonSerializer(typeof(ResponseData));


    public ResponseData(int exitCode, string message) {
        this.ExitCode = exitCode;
        this.Message = message;
    }


    [DataMember()]
    public int ExitCode { get; private set; }

    [DataMember()]
    public string Message { get; private set; }


    public byte[] ToJson() {
        using (var stream = new MemoryStream()) {
            DataSerializer.WriteObject(stream, this);
            return stream.ToArray();
        }
    }

    public static ResponseData FromJson(byte[] json) {
        using (var stream = new MemoryStream(json)) {
            return DataSerializer.ReadObject(stream) as ResponseData;
        }
    }

}
