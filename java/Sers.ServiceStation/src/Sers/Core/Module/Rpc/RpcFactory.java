package Sers.Core.Module.Rpc;


public class RpcFactory {

    public static final RpcFactory Instance = new RpcFactory();


    public interface ICreate<T>{
    	public T create();
    }

    public ICreate<RpcContext> CreateRpcContext =
        () -> new RpcContext();


    public ICreate<RpcContextData> CreateRpcContextData =
        () -> new RpcContextData();
        
}
