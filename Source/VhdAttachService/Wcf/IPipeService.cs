using System.ServiceModel;

[ServiceContract(Namespace = "http://jmedved.com/VhdAttach")]
interface IPipeService {

    [OperationContract]
    byte[] Execute(string action, byte[] data);

}
