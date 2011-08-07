using System.ServiceModel;

class WcfPipeClient {

    private static readonly EndpointAddress ServiceAddress = new EndpointAddress("net.pipe://localhost/VhdAttach/Service");
    private static readonly IPipeService ServiceProxy = ChannelFactory<IPipeService>.CreateChannel(new NetNamedPipeBinding(), ServiceAddress);

    public static byte[] Execute(string action, byte[] data) {
        try {
            return ServiceProxy.Execute(action, data);
        } catch (CommunicationException) {
            return (new ResponseData(ExitCodes.CannotExecute, "Communication error.")).ToJson();
        }
    }
}
