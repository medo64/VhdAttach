using System.ServiceModel;

internal class WcfPipeClient {

    public static byte[] Execute(string action, byte[] data) {
        try {
            if (ServiceProxy == null) { CreateServiceProxy(); }
            return ServiceProxy.Execute(action, data);
        } catch (CommunicationException) {
            try {
                CreateServiceProxy(); //try to recreate proxy
                return ServiceProxy.Execute(action, data);
            } catch (CommunicationException ex) {
                return (new ResponseData(ExitCodes.CannotExecute, "Communication error.\n" + ex.Message)).ToJson();
            }
        }
    }


    private static IPipeService ServiceProxy;

    private static void CreateServiceProxy() {
        ServiceProxy = ChannelFactory<IPipeService>.CreateChannel(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/VhdAttach/Service"));
    }

}
