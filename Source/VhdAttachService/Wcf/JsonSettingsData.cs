using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

[DataContract()]
internal class JsonSettingsData {

    private static readonly DataContractJsonSerializer DataSerializer = new DataContractJsonSerializer(typeof(JsonSettingsData));


    public JsonSettingsData(bool contextMenuAttach, bool contextMenuDetach, bool contextMenuDetachDrive, string[] autoAttachList) {
        this.ContextMenuAttach = contextMenuAttach;
        this.ContextMenuDetach = contextMenuDetach;
        this.ContextMenuDetachDrive = contextMenuDetachDrive;
        this.AutoAttachList = autoAttachList;
    }


    [DataMember()]
    public bool ContextMenuAttach { get; private set; }

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

    public static JsonSettingsData FromJson(byte[] json) {
        using (var stream = new MemoryStream(json)) {
            return DataSerializer.ReadObject(stream) as JsonSettingsData;
        }
    }

}
