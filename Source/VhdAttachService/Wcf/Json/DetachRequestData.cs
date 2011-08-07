using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

[DataContract()]
internal class DetachRequestData {

    private static readonly DataContractJsonSerializer DataSerializer = new DataContractJsonSerializer(typeof(DetachRequestData));


    public DetachRequestData(string path) {
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

    public static DetachRequestData FromJson(byte[] json) {
        using (var stream = new MemoryStream(json)) {
            return DataSerializer.ReadObject(stream) as DetachRequestData;
        }
    }

}
