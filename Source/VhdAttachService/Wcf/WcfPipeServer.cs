using System;
using System.ServiceModel;

static class WcfPipeServer {

    private static readonly EndpointAddress ServiceAddress = new EndpointAddress("net.pipe://localhost/VhdAttach/Service");
    private static ServiceHost _host;

    public static void Start() {
        if ((_host != null) && (_host.State != CommunicationState.Closed)) { _host.Close(); }
        _host = new ServiceHost(new PipeService(), ServiceAddress.Uri);
        _host.AddServiceEndpoint(typeof(IPipeService), new NetNamedPipeBinding(), ServiceAddress.Uri);
        _host.Open();
    }

    public static CommunicationState State { get { return _host.State; } }

    public static void Stop() {
        if (_host.State != CommunicationState.Closed) {
            _host.Close();
        }
    }

}
