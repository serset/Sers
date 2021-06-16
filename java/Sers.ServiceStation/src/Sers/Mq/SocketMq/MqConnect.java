package Sers.Mq.SocketMq;

import Sers.Core.Module.Log.Logger;
import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Util.Common.CommonHelp;
import Sers.Core.Util.Data.ArraySegment;
import Sers.Core.Util.Threading.AutoResetEvent;

import java.io.Closeable;
import java.net.Socket;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ConcurrentHashMap;

public class MqConnect implements Closeable {



    //region (x.0) 成员

    public MqConnectConfig config ;

    public void  OnDisconnected_Set(SocketConnect.IOnDisconnected onDisconnected){
        socketConnect.onDisconnected=onDisconnected;
    }



    SocketConnect socketConnect = new SocketConnect();

    public boolean isConnected(){
        return socketConnect.isConnected();
    }


    public MqConnect()
    {
        socketConnect.onGetMsg = data-> Socket_OnGetMsg(data);

        OnReceiveMessage=msg->{};


        OnReceiveRequest= (ArraySegment req)->{ return  new  ArrayList<ArraySegment>();};
    }

    //endregion

    //region (x.1) Init Close

    //region (x.x.1) Init

    /// <summary>
    /// 初始化并开启接收发送等线程
    /// </summary>
    /// <param name="client"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public boolean Init(Socket client, MqConnectConfig config) throws Exception {
        this.config = config;

        if (!socketConnect.init(client, config.workThreadCount))
        {
            return false;
        }

        if (!Ping())
        {
            socketConnect.close();
            return false;
        }
        return true;

    }
    //endregion

    //region (x.x.2) Close

    @Override
    public void close() {
        try
        {
            if (isConnected())
            {
                socketConnect.close();
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    //endregion


    //endregion

    //region (x.2) socket 底层接口


    void Socket_OnGetMsg(ArraySegment data) throws Exception {

        byte msgType = data.Get(0);
        byte requestType = data.Get(1);

        ArraySegment msgData = data.Slice(2);
        switch (msgType)
        {
            case EMsgType.reply:
            {
                long reqKey=msgData.ReadLong(0);
                ArraySegment replyData=data.Slice(10);
                RequestManage_OnGetMsg(reqKey, replyData);
                return;
            }
            case EMsgType.request:
            {
                long reqKey=msgData.ReadLong(0);
                ArraySegment requestData=data.Slice(10);

                List<ArraySegment> replyData = RequestManage_OnReceiveRequest(requestType, requestData);

                if(replyData!=null && replyData.size()!=0) {

                    //region PackageReqRepFrame
                    List<ArraySegment> repFrame=replyData;
                    repFrame.add(0, new ArraySegment(new byte[8]).WriteLong(reqKey,0) );
                    //endregion

                    Socket_SendData(EMsgType.reply, requestType, repFrame);
                }
                return;
            }
            case EMsgType.message:
            {
                OnReceiveMessage.OnReceiveMessage(msgData);
                return;
            }
        }
    }


    void Socket_SendData(byte msgType, byte requestType, List<ArraySegment> data) throws Exception {
        data.add(0, new ArraySegment(new byte[]{msgType,requestType }));
        socketConnect.sendMsg(data);
    }
    //endregion



    //region (x.3) Message

    public interface IOnReceiveMessage{
        void OnReceiveMessage(ArraySegment msg);
    }

    public void SendMessage(List<ArraySegment> msgData) throws Exception {
        byte requestType = 0;
        Socket_SendData(EMsgType.message, requestType,msgData);
    }
    public IOnReceiveMessage OnReceiveMessage ;
    //endregion





    final static  String MqVersion = "Sers.Mq.Socket.v1";

    //region  (x.4) RequestManage

    List<ArraySegment> RequestManage_OnReceiveRequest(byte requestType, ArraySegment requestData)
    {
        switch (requestType)
        {
            case ERequestType.app:
            {
                //app
                return OnReceiveRequest.OnReceiveRequest(requestData);
            }
            case ERequestType.ping:
            {
                String mqVersion = Serialization.Instance.bytesToString(requestData);
                if (!MqVersion.equals(mqVersion))
                {
                    return null;
                }
                ArrayList<ArraySegment> byteData = new ArrayList<ArraySegment>();
                byteData.add(requestData);
                return byteData;
            }
        }
        return null;
    }


    class RequestInfo
    {
        public ArraySegment replyData;
        public AutoResetEvent mEvent = new AutoResetEvent(false);
    }

    final ConcurrentHashMap<Long, RequestInfo> RequestManage_RequestMap = new ConcurrentHashMap<Long, RequestInfo>();


    boolean RequestManage_SendRequest(List<ArraySegment> requestData, ArraySegment replyData, byte requestType, int millisecondsTimeout) throws Exception {
        boolean success = false;
        long reqKey = CommonHelp.NewGuidLong();


        RequestInfo requestInfo = new RequestInfo();

        try
        {

            RequestManage_RequestMap.put(reqKey,requestInfo);

            //region PackageReqRepFrame
            List<ArraySegment> reqRepFrame=requestData;
            reqRepFrame.add(0, new ArraySegment(new byte[8]).WriteLong(reqKey,0) );
            //endregion

//            requestInfo.mEvent.Reset();

            //SendRequest
            Socket_SendData(EMsgType.request, requestType, reqRepFrame);

            success = requestInfo.mEvent.waitOne(millisecondsTimeout);

            if(success){
                replyData.CopyFrom(requestInfo.replyData);
            }
        }
        finally
        {
            if (!success) RequestManage_RequestMap.remove(reqKey);
        }
        return success;

    }


    void RequestManage_OnGetMsg(long reqKey, ArraySegment replyData)
    {
        RequestInfo requestInfo=RequestManage_RequestMap.remove(reqKey);
        if (null!=requestInfo)
        {
            requestInfo.replyData = replyData;
            requestInfo.mEvent.set();
        }
    }


    //endregion


    //region (x.5) ReqRep

    public interface IOnReceiveRequest{

        List<ArraySegment> OnReceiveRequest(ArraySegment req);
    }

    public IOnReceiveRequest OnReceiveRequest;

    public boolean SendRequest(List<ArraySegment> requestData, ArraySegment replyData) throws Exception {
        return RequestManage_SendRequest(requestData, replyData, ERequestType.app, config.requestTimeout);
    }
    //endregion

    //region (x.6) Ping

    public boolean Ping() throws Exception {
        int retry = config.pingRetryCount;
        while (!Ping_Try())
        {
            if ((--retry) <= 0)
            {
                return false;
            }
        }
        return true;
    }


    static final byte[] MqVersion_ba = Serialization.Instance.stringToBytes(MqVersion);


    boolean Ping_Try() throws Exception {

        if (!socketConnect.testIsConnected())
        {
            return false;
        }

        List<ArraySegment> ping_ConnectData=new ArrayList<ArraySegment>();
        ping_ConnectData.add(new ArraySegment(MqVersion_ba));

        ArraySegment replyData=new ArraySegment();
        if (RequestManage_SendRequest(ping_ConnectData, replyData, ERequestType.ping, config.pingTimeout))
        {
            String mqVersion = Serialization.Instance.bytesToString(replyData);
            return MqVersion.equals(mqVersion);
        }
        return false;
    }
    //endregion


}

