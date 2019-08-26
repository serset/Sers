package Sers.Mq.SocketMq;

import Sers.Core.Module.Log.Logger;
import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Util.Data.ArraySegment;
import Sers.Core.Util.Threading.LongTaskHelp;

import java.net.Socket;
import java.util.ArrayList;
import java.util.List;

public class ClientMq {


    //region (x.0) 成员
    public ClientMqConfig config;

    MqConnect _mqConnect = new MqConnect();

    public void  OnDisconnected_Set(SocketConnect.IOnDisconnected onDisconnected){
        _mqConnect.OnDisconnected_Set(onDisconnected);
    }

    public boolean IsConnected(){
        return _mqConnect.isConnected();
    }

    public ClientMq()
    {
        
    }

    //endregion



    //region (x.1) Connect Close

    //region Connect


    public boolean Connect() throws Exception {
        //服务已启动
        if (_mqConnect.isConnected()) return false;

         //region (x.1)连接服务器
        Logger.Info("[消息队列 Socket Client] 连接服务器,host：" + config.host + " port:" + config.port);
        Socket client;
        try
        {
            client = new Socket(config.host, config.port);
        }
        catch (Exception ex)
        {
            //服务启动失败
            Logger.Error("[消息队列 Socket Client] 连接服务器 出错", ex);
            return false;
        }
        //endregion


        //(x.2) init _mqConnect
        if (!_mqConnect.Init(client,config))
        {
            _mqConnect.close();
            Logger.Info("[消息队列 Socket Client] 无法连接服务器。");
            return false;
        }

        //(x.3)发送身份验证
        if (!checkSecretKey()) return false;

        //(x.4) start back thread for ping
        ping_BackThread.action =  ()-> Ping_Thread();
        ping_BackThread.start();

        Logger.Info("[消息队列 Socket Client] 已连接服务器。");
        return true;
    }

    //region checkSecretKey

    private boolean checkSecretKey() throws Exception {
        //发送身份验证
        Logger.Info("[消息队列 Socket Client] 发送身份验证请求...");

        byte[] req=Serialization.Instance.stringToBytes(config.secretKey);

        List<ArraySegment> reqData=new ArrayList<ArraySegment>();
        reqData.add(new ArraySegment(req));

        ArraySegment reply = SendRequest(reqData);
        String strRet=Serialization.Instance.bytesToString(reply);
        if ("true".equals(strRet))
        {
            Logger.Info("[消息队列 Socket Client] 身份验证通过");
            return true;
        }
        Logger.Info("[消息队列 Socket Client] 身份验证失败");
        return false;

    }
    //endregion


    //endregion

    //region Close
    public void close()
    {
        if (!IsConnected()) return;

        Logger.Info("[消息队列 Socket Client] 准备断开连接");
        try
        {
            ping_BackThread.stop();
        }
        catch (Exception ex)
        {
            Logger.Error("[消息队列 Socket Client] 准备断开连接 出错",ex);
        }

        try
        {
            _mqConnect.close();
        }
        catch (Exception ex)
        {
            Logger.Error("[消息队列 Socket Client] 准备断开连接 出错", ex);
        }
        Logger.Info("[消息队列 Socket Client] 已断开连接。");
    }


    //endregion

    //endregion


    //region (x.2) Message

    public void SendMessage(List<ArraySegment> msgData) throws Exception {
        _mqConnect.SendMessage(msgData);
    }

    public  void OnReceiveMessage_Set(MqConnect.IOnReceiveMessage onReceiveMessage) {
        _mqConnect.OnReceiveMessage=onReceiveMessage;
    }
    //endregion


    //region (x.3) ReqRep

    public  void OnReceiveRequest_Set(MqConnect.IOnReceiveRequest onReceiveRequest) {
        _mqConnect.OnReceiveRequest=onReceiveRequest;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="requestData"></param>
    /// <returns></returns>
    public ArraySegment SendRequest(List<ArraySegment> requestData) throws Exception {
        ArraySegment replyData=new ArraySegment();
        boolean flag = _mqConnect.SendRequest(requestData, replyData);
        return replyData;
    }
    //endregion


     //region (x.4) Ping_Thread

    LongTaskHelp ping_BackThread = new LongTaskHelp();
    void Ping_Thread() {
        while (true)
        {
            try
            {
                while (true)
                {
                    boolean disconnected = true;
                    try
                    {
                        disconnected = !_mqConnect.Ping();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    if (disconnected)
                    {
                        try
                        {
                            _mqConnect.close();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                        return;
                    }
                    Thread.sleep(config.pingInterval);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
    //endregion

}
