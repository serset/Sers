package Sers.Mq.SocketMq;


import Sers.Core.Module.Log.Logger;
import Sers.Core.Util.Data.ArraySegment;
import Sers.Core.Util.Data.ByteData;
import Sers.Core.Util.Threading.LongTaskHelp;

import java.io.*;
import java.net.Socket;
import java.util.List;
import java.util.concurrent.LinkedBlockingQueue;

public class SocketConnect implements  Closeable   {

    public interface IOnDisconnected { void onDisconnected(SocketConnect obj); }
    public IOnDisconnected onDisconnected;

    //region (x.1)成员
    Socket socket = null;
    InputStream is = null;
    OutputStream os = null;
    DataInputStream input;
    DataOutputStream output;

    public boolean isConnected(){
        return null != socket;
    }

    public boolean testIsConnected(){
    	 return  null != socket;
//        return  null != socket && socketIsConnected(socket);
    }


    /**
     * 判断是否断开连接，断开返回true,没有返回false
     * @param socket
     * @return
     */
    private static boolean socketIsConnected(Socket socket){
        try{
            socket.sendUrgentData(0xFF);//发送1个字节的紧急数据，默认情况下，服务器端没有开启紧急数据处理，不影响正常通信
            return true;
        }catch(Exception se){
            return false;
        }
    }


    //endregion


    //region (x.2)Init

    /**
     * 初始化并开启后台线程
      * @param socket
     * @param workThreadCount
     * @return
     */
    public boolean init(Socket socket, int workThreadCount) {

        this.socket = socket;

        try {
            is = socket.getInputStream();
            input = new DataInputStream(is);

            os = socket.getOutputStream();
            output=new DataOutputStream(os);

            taskToDealMsg.action=()->taskToDealMsg();
            taskToDealMsg.threadCount =workThreadCount;
            taskToDealMsg.start();



            taskToReceiveMsg.action=()->taskToReceiveMsg();
            taskToReceiveMsg.start();


            taskToSendMsg.action=()->taskToSendMsg();
            taskToSendMsg.start();

            return  true;
        }catch (IOException e) {
//            e.printStackTrace();
            Logger.Error(e);
        }
        return false;
    }
    //endregion

    //region (x.3)Close

    @Override
    public  void close(){

        if (!isConnected()) { return; }

        //region(x.1)
        try{
            taskToSendMsg.stop();

            taskToReceiveMsg.stop();
        } catch (Exception e) {
            Logger.Error(e);
        }
        //endregion

        //region (x.2)清理释放连接资源
        try {
            if(null!=input){
                input.close();
                
            }

            if(null!=output){
                output.flush();
                output.close();
               
            }

            if(null!=is){
                is.close();
                
            }

            if(null!=os){
                //os.flush();
                os.close();
               
            }


            if(null!=socket) {
//                socket.shutdownInput();//关闭输入流
//                socket.shutdownOutput();
                socket.close();           
            }

        } catch (IOException e) {
            Logger.Error(e);
        }finally {
			input = null;
			output = null;
			is = null;
			os = null;
			socket = null;
		}
        //endregion

        //(x.3)
        try {
            if(onDisconnected!=null) onDisconnected.onDisconnected(this);
        } catch (Exception e) {
            Logger.Error(e);
        }
    }
    //endregion

    //region (x.4)收到消息 和 发送消息

    public interface IOnGetMsg{
        void onGetMsg(ArraySegment msg) throws Exception;
    }

    public IOnGetMsg onGetMsg;


    public void sendMsg(List<ArraySegment> data) throws Exception {
        if (!isConnected())
        {
            //TODO:
            throw new Exception("[lith_190628_001]socket is closed");
        }
        msgToSend.put(data);
    }
    //endregion


    //region (x.5)后台消息线程


    //region (x.x.1)后台处理接收到的消息线程 TaskToDealMsg


    LongTaskHelp taskToDealMsg = new LongTaskHelp();

    //阻塞队列，FIFO
     LinkedBlockingQueue<ArraySegment> msgToDeal = new LinkedBlockingQueue<ArraySegment>();




    void taskToDealMsg()
    {
        while (true)
        {
            try
            {

                while (true)
                {
                    ArraySegment arr=msgToDeal.take();
//                    if(arr!=null)
                    //阻塞获取
                    onGetMsg.onGetMsg(arr);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }

    void cacheReceivedMsg(byte[] msg) throws InterruptedException {
            msgToDeal.put(new ArraySegment(msg));
    }



    //endregion

    //region (x.x.2)后台接收消息线程 TaskToReceiveMsg
    LongTaskHelp taskToReceiveMsg = new LongTaskHelp();

    void taskToReceiveMsg()
	{
		try {
			while (isConnected()) {

				cacheReceivedMsg(ReadMsg());
			}
		} catch (Exception e) {
			Logger.Error(e);
		}
		close();

	}
    //endregion


    //region (x.x.3)后台发送消息线程 TaskToSendMsg
    LongTaskHelp taskToSendMsg = new LongTaskHelp();

    //阻塞队列，FIFO
    LinkedBlockingQueue<List<ArraySegment>> msgToSend = new LinkedBlockingQueue<List<ArraySegment>>();

	void taskToSendMsg() {
		try {
			while (isConnected()) {

				WriteMsg(msgToSend.take());
			}
		} catch (Exception e) {
			Logger.Error(e);
		}
		close();
	}

    //endregion


     //endregion



    //region (x.6)第一层封装 ReadMsg WriteMsg


    //线程不安全

    byte[] ReadMsg_bytesLen=new byte[4];
    ArraySegment WriteMsg_SegLen= new ArraySegment(ReadMsg_bytesLen);
    /*
        消息块格式：
            第一部分(len)    数据长度，4字节 Int32类型
            第二部分(data)   原始数据，长度由第二部分指定

    */
    byte[]   ReadMsg() throws Exception {

        ReadMsg_Receive(ReadMsg_bytesLen);

        int len=WriteMsg_SegLen.ReadInt32(0);
        byte[] data=new byte[len];
        ReadMsg_Receive(data);
        return  data;
    }

    void ReadMsg_Receive(byte[] data) throws Exception {
        int len=data.length;
        int readedCount = 0;
        int curCount=0;
        do{     
            curCount=input.read(data,readedCount,len - readedCount);
        	 
            if (curCount == 0)
            {
                throw new Exception("[lith_190418_002]socket is closed.");
            }
            readedCount +=curCount;
        } while (readedCount < len);
    }



    ArraySegment WriteMsg_bytesLen= new ArraySegment(new byte[4]);
    void WriteMsg(List<ArraySegment> data) throws Exception {

		// (x.1) write len
		int len = ByteData.ByteDataLen(data);
		WriteMsg_bytesLen.WriteInt32(len, 0);
		output.write(WriteMsg_bytesLen.array, WriteMsg_bytesLen.offset, WriteMsg_bytesLen.count);

		// (x.2) write data
		for (ArraySegment item : data) {
			output.write(item.array, item.offset, item.count);
		}
	}
	// endregion

}
