namespace Sers.Core.Module.PubSub
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
