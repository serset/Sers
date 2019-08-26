using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Mq.Zmq.ClrZmq.RouteDealer
{
    enum EMsgType : byte
    {
        /// <summary>
        /// (1) client -> server  
        ///   [client.request]
        ///   request in client:  [               ,  requestGuid,   requestData,    MsgTypeFrame=[MsgType=request][RequestType:1 byte]      ]
        ///   request in server:  [ clientConnGuid,  requestGuid,   requestData,    MsgTypeFrame=[MsgType=request][RequestType:1 byte]      ]
        /// 
        /// 
        /// (2) server -> client  
        ///   [server.request]
        ///   request in server:  [ clientConnGuid,  requestGuid,   requestData,    MsgTypeFrame=[MsgType=request][RequestType:1 byte]      ]
        ///   request in client:  [                  requestGuid,   requestData,    MsgTypeFrame=[MsgType=request][RequestType:1 byte]      ] 
        /// 
        /// </summary>
        request,


        /// <summary>
        /// (1) client -> server    
        ///   [server.reply]
        ///   reply in server:  [  clientConnGuid,  requestGuid,    replyData  , MsgTypeFrame=[MsgType=reply][RequestType:1 byte]      ]
        ///   reply in client:  [                   requestGuid,    replyData  , MsgTypeFrame=[MsgType=reply][RequestType:1 byte]      ]
        ///   
        /// 
        /// (2) server -> client  
        ///   [client.reply]
        ///   reply in client:  [                   requestGuid,    replyData  , MsgTypeFrame=[MsgType=reply][RequestType:1 byte]      ]
        ///   reply in server:  [  clientConnGuid,  requestGuid,    replyData  , MsgTypeFrame=[MsgType=reply][RequestType:1 byte]      ] 
        /// 
        /// </summary>
        reply,

        /// <summary>
        /// 
        /// </summary>
        message
    }
}
