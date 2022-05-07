namespace Sers.Core.Module.PubSub
{
    public enum EFrameType : byte
    {
        /// <summary>
        ///  publish, msgTitle, msgData
        /// </summary>
        publish = 0,
        /// <summary>
        /// subscribe, msgTitle
        /// </summary>
        subscribe = 1,
        /// <summary>
        /// unSubscribe, msgTitle
        /// </summary>
        unSubscribe = 2,
        /// <summary>
        /// message, msgTitle, msgData
        /// </summary>
        message = 3
    }
}
