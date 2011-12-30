using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

[DataContract()]
internal class AttachRequestData {

    private static readonly DataContractJsonSerializer DataSerializer = new DataContractJsonSerializer(typeof(AttachRequestData));


    public AttachRequestData(string path, bool mountReadOnly, bool initializeDisk) {
        this.Path = path;
        this.MountReadOnly = mountReadOnly;
        this.InitializeDisk = initializeDisk;
    }


    [DataMember()]
    public string Path { get; private set; }

    [DataMember()]
    public bool MountReadOnly { get; private set; }

    [DataMember()]
    public bool InitializeDisk { get; private set; }


    public byte[] ToJson() {
        using (var stream = new MemoryStream()) {
            DataSerializer.WriteObject(stream, this);
            return stream.ToArray();
        }
    }

    public static AttachRequestData FromJson(byte[] json) {
        using (var stream = new MemoryStream(json)) {
            return DataSerializer.ReadObject(stream) as AttachRequestData;
        }
    }

}
