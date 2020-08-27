using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

 
namespace  Sers.CL.Zmq.FullDuplex.Zmq
{
	/// <summary>
	/// Sends and receives messages, single frames and byte frames across ZeroMQ.
	/// </summary>
	public class ZSocket : IDisposable
	{
		  
		private ZContext _context;

		private IntPtr _socketPtr;

		private ZSocketType _socketType;
		
		/// <summary>
		/// Create a <see cref="ZSocket"/> instance.
		/// You are using ZContext.Current!
		/// </summary>
		/// <returns><see cref="ZSocket"/></returns>
		public ZSocket(ZSocketType socketType) : this (ZContext.Current, socketType) { }

		/// <summary>
		/// Create a <see cref="ZSocket"/> instance.
		/// </summary>
		/// <returns><see cref="ZSocket"/></returns>
		public ZSocket(ZContext context, ZSocketType socketType)
		{
			_context = context;
			_socketType = socketType;

		 
			if (!Initialize(out var error))
			{
                throw new Exception("zmq error: errno " + error);
            }
		}

		protected ZSocket() { }

		protected bool Initialize(out int error)
		{
			error = 0;

			if (IntPtr.Zero == (_socketPtr = zmq.socket(_context.ContextPtr, (Int32)_socketType)))
			{
                error = zmq.errno();
                return false;
			}
			return true;
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="ZSocket"/> class.
		/// </summary>
		~ZSocket()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ZSocket"/>, and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{			 
				Close(out var error);
			}
		}

		/// <summary>
		/// Close the current socket.
		/// </summary>
		public void Close()
		{
		 
			if (!Close(out var error))
			{
                throw new Exception("zmq error: errno " + error);
            }
		}

		/// <summary>
		/// Close the current socket.
		/// </summary>
		public bool Close(out int error)
		{
			error = 0;
			if (_socketPtr == IntPtr.Zero) return true;

			if (-1 == zmq.close(_socketPtr))
			{
                error = zmq.errno();
                return false;
			}
			_socketPtr = IntPtr.Zero;
			return true;
		}

	 

	 

	 

		/// <summary>
		/// Bind the specified endpoint.
		/// </summary>
		/// <param name="endpoint">A string consisting of a transport and an address, formatted as <c><em>transport</em>://<em>address</em></c>.</param>
		public void Bind(string endpoint)
		{
            if (-1 == zmq.bind(_socketPtr, endpoint))
            {
                int errno = zmq.errno();
                zmq.ThrowErrno(errno);
            }         
        }
         

		/// <summary>
		/// Connect the specified endpoint.
		/// </summary>
		/// <param name="endpoint">A string consisting of a transport and an address, formatted as <c><em>transport</em>://<em>address</em></c>.</param>
		public void Connect(string endpoint)
		{
            if (-1 == zmq.connect(_socketPtr, endpoint))
            {
                int errno = zmq.errno();               
                zmq.ThrowErrno(errno);
            } 
        }

		/// <summary>
		/// Disconnect the specified endpoint.
		/// </summary>
		public void Disconnect(string endpoint)
		{			 
			if (!Disconnect(endpoint, out var errno))
			{
                zmq.ThrowErrno(errno);    
            }
		}

        /// <summary>
        /// Disconnect the specified endpoint.
        /// </summary>
        /// <param name="endpoint">A string consisting of a transport and an address, formatted as <c><em>transport</em>://<em>address</em></c>.</param>
        /// <param name="errno"></param>
		public bool Disconnect(string endpoint, out int errno)
		{
            errno =  0;

            if (-1 == zmq.disconnect(_socketPtr, endpoint))
            {
                errno = zmq.errno();
                return false;
            }
            return true;
		}


        #region OptionBytes

        public byte[] Identity
        {
            get { return GetOptionBytes(ZSocketOption.IDENTITY); }
            set { SetOptionBytes(ZSocketOption.IDENTITY, value); }
        }
        // From options.hpp: unsigned char identity [256];
        private const int MaxBinaryOptionSize = 256;

        public byte[] GetOptionBytes(ZSocketOption option)
        {
            byte[] value;

            int optionLength = MaxBinaryOptionSize;
            using (var optionValue = new DispoIntPtr(optionLength))
            {
                GetOption(option, optionValue.Ptr, ref optionLength);
                value = new byte[optionLength];
                Marshal.Copy(optionValue.Ptr, value, 0, optionLength);
            }
            return value;
        }

        public void SetOptionBytes(ZSocketOption option, byte[] value)
        {

            int optionLength = /* Marshal.SizeOf(typeof(byte)) * */ value.Length;
            using (var optionValue = new DispoIntPtr(optionLength))
            {
                Marshal.Copy(value, 0, optionValue.Ptr, optionLength);

                while (-1 == zmq.setsockopt(this._socketPtr, (int)option, optionValue.Ptr, optionLength))
                {
                    int errno = zmq.errno();
                    if (errno == (int)ZError.EINTR)
                    {
                        continue;
                    }
                    zmq.ThrowErrno(errno);
                }          
            }
        }
        #endregion

        #region option int32
        public bool ReceiveMore
        {
            get { return GetOptionInt32(ZSocketOption.RCVMORE,out int value) && value == 1; }
        }

        static readonly int Int32OptionLength = Marshal.SizeOf(typeof(Int32));

       

        public bool GetOptionInt32(ZSocketOption option, out Int32 value)
        {          
            value = default(Int32);         
            int optionLength = Int32OptionLength;
            using (var optionValue = new DispoIntPtr(optionLength))
            {
                GetOption(option, optionValue.Ptr, ref optionLength);
                
                value = Marshal.ReadInt32(optionValue.Ptr);  
            }
            return true;
        }
        private void GetOption(ZSocketOption option, IntPtr optionValue, ref int optionLength)
        {
          
            using (var optionLengthP = new DispoIntPtr(IntPtr.Size))
            {
                if (IntPtr.Size == 4)
                    Marshal.WriteInt32(optionLengthP.Ptr, optionLength);
                else if (IntPtr.Size == 8)
                    Marshal.WriteInt64(optionLengthP.Ptr, (long)optionLength);
                else
                    throw new PlatformNotSupportedException();

                while (-1 == zmq.getsockopt(this._socketPtr, (int)option, optionValue, optionLengthP.Ptr))
                {
                    int errno = zmq.errno();
                    if (errno == (int)ZError.EINTR)
                    {
                        continue;
                    }                 
                    zmq.ThrowErrno(errno);
                }

                if (IntPtr.Size == 4)
                    optionLength = Marshal.ReadInt32(optionLengthP.Ptr);
                else if (IntPtr.Size == 8)
                    optionLength = (int)Marshal.ReadInt64(optionLengthP.Ptr);
                else
                    throw new PlatformNotSupportedException();
            }
 
        }
        #endregion

        public List<byte[]>  ReceiveMessage()
        {          
            var message = new List<byte[]>();

            do
            {
                zmq.ReceiveMessage(_socketPtr, out var frame, (int)ZSocketFlags.More);
                message.Add(frame);

            } while (this.ReceiveMore);
            return message;
        }



        public void SendMessage(byte[][] message)
        {
            var t = 0;
            for (; t < message.Length-1; t++)
            {
                zmq.SendMessage(_socketPtr, message[t], (int)ZSocketFlags.More);
            }
            zmq.SendMessage(_socketPtr, message[t]);
        }

    }
}