using System;
using System.ServiceModel;

static class WcfPipeServer {

    private static readonly EndpointAddress ServiceAddress = new EndpointAddress("net.pipe://localhost/VhdAttach/Service");
    private static readonly ServiceHost _host = new ServiceHost(new PipeService(), ServiceAddress.Uri);

    public static void Start() {
        _host.AddServiceEndpoint(typeof(IPipeService), new NetNamedPipeBinding(), ServiceAddress.Uri);
        _host.Open();
    }

    public static void Stop() {
        if (_host.State != CommunicationState.Closed) {
            _host.Close();
        }
    }

}
