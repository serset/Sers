using System;



namespace Sers.CL.Zmq.FullDuplex.Zmq
{
    /// <summary>
    /// Creates <see cref="ZSocket"/> instances within a process boundary.
    /// </summary>
    public sealed class ZContext : IDisposable
    {

        public static readonly ZContext Current = new ZContext();


        /// <summary>
        /// Create a <see cref="ZContext"/> instance.
        /// </summary>
        /// <returns><see cref="ZContext"/></returns>
        public ZContext()
        {
            _contextPtr = zmq.ctx_new();

            if (_contextPtr == IntPtr.Zero)
            {
                throw new InvalidProgramException("zmq_ctx_new");
            }
        }



        ~ZContext()
        {
            Dispose(false);
        }

        private IntPtr _contextPtr;

        /// <summary>
        /// Gets a handle to the native ZeroMQ context.
        /// </summary>
        public IntPtr ContextPtr
        {
            get { return _contextPtr; }
        }




        /// <summary>
        /// Shutdown the ZeroMQ context.
        /// </summary>
        public bool Shutdown()
        {
            if (_contextPtr == IntPtr.Zero)
                return true;

            while (-1 == zmq.ctx_shutdown(_contextPtr))
            {
                int errno = zmq.errno();
                if (errno == (int)ZError.EINTR)
                {
                    continue;
                }
                return false;
            }

            // don't _contextPtr = IntPtr.Zero;
            return true;
        }

        /// <summary>
        /// Terminate the ZeroMQ context.
        /// </summary>
        public void Terminate()
        {

            if (!Terminate(out int error))
            {
                throw new Exception("zmq error: errno " + error);
            }
        }

        /// <summary>
        /// Terminate the ZeroMQ context.
        /// </summary>
        public bool Terminate(out int error)
        {
            error = 0;

            if (_contextPtr == IntPtr.Zero)
                return true;

            while (-1 == zmq.ctx_term(_contextPtr))
            {
                int errno = zmq.errno();
                if (errno == (int)ZError.EINTR)
                {
                    continue;
                }
                // Maybe ZError.EFAULT

                return false;
            }

            _contextPtr = IntPtr.Zero;
            return true;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                Terminate(out var _);
            }
        }

        private void EnsureNotDisposed()
        {
            if (_contextPtr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}