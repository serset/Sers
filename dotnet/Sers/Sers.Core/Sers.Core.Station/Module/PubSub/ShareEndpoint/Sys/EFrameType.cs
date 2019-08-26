using System;
using System.Collections.Generic;
using System.Text;


namespace Sers.Core.Module.PubSub.ShareEndpoint.Sys
{
    public enum EFrameType : byte
    {
        /// <summary>
        ///  publish,msgTitle,msgData
        /// </summary>
        publish,
        /// <summary>
        /// subscribe,msgTitle
        /// </summary>
        subscribe,
        /// <summary>
        /// subscribeCancel,msgTitle
        /// </summary>
        subscribeCancel,
        /// <summary>
        /// message,msgTitle,msgData
        /// </summary>
        message
    }
}
