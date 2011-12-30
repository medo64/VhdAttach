using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

[DataContract()]
internal class SettingsRequestData {

    private static readonly DataContractJsonSerializer DataSerializer = new DataContractJsonSerializer(typeof(SettingsRequestData));


    public SettingsRequestData(bool contextMenuAttach, bool contextMenuAttachReadOnly, bool contextMenuDetach, bool contextMenuDetachDrive, string[] autoAttachList) {
        this.ContextMenuAttach = contextMenuAttach;
        this.ContextMenuAttachReadOnly = contextMenuAttachReadOnly;
        this.ContextMenuDetach = contextMenuDetach;
        this.ContextMenuDetachDrive = contextMenuDetachDrive;
        this.AutoAttachList = autoAttachList;
    }


    [DataMember()]
    public bool ContextMenuAttach { get; private set; }

    [DataMember()]
    public bool ContextMenuAttachReadOnly { get; private set; }

    [DataMember()]
    public bool ContextMenuDetach { get; private set; }

    [DataMember()]
    public bool ContextMenuDetachDrive { get; private set; }

    [DataMember()]
    public string[] AutoAttachList { get; private set; }


    public byte[] ToJson() {
        using (var stream = new MemoryStream()) {
            DataSerializer.WriteObject(stream, this);
            return stream.ToArray();
        }
    }

    public static SettingsRequestData FromJson(byte[] json) {
        using (var stream = new MemoryStream(json)) {
            return DataSerializer.ReadObject(stream) as SettingsRequestData;
        }
    }

}
