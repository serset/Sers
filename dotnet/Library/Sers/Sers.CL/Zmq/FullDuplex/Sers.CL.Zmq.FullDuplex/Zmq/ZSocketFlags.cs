using System;

namespace Sers.CL.Zmq.FullDuplex.Zmq
{
    [Flags]
    public enum ZSocketFlags : int
    {
        /// <summary>
        /// No socket flags are specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The operation should be performed in non-blocking mode.
        /// </summary>
        DontWait = 1,

        /// <summary>
        /// The message being sent is a multi-part message, and that further message parts are to follow.
        /// </summary>
        More = 2
    }
}
