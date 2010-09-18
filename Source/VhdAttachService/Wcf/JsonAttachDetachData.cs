using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

[DataContract()]
internal class JsonAttachDetachData {

    private static readonly DataContractJsonSerializer DataSerializer = new DataContractJsonSerializer(typeof(JsonAttachDetachData));


    public JsonAttachDetachData(string path) {
        this.Path = path;
    }


    [DataMember()]
    public string Path { get; private set; }


    public byte[] ToJson() {
        using (var stream = new MemoryStream()) {
            DataSerializer.WriteObject(stream, this);
            return stream.ToArray();
        }
    }

    public static JsonAttachDetachData FromJson(byte[] json) {
        using (var stream = new MemoryStream(json)) {
            return DataSerializer.ReadObject(stream) as JsonAttachDetachData;
        }
    }

}
