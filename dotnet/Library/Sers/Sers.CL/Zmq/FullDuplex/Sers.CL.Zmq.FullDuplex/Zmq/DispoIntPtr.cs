namespace Sers.CL.Zmq.FullDuplex.Zmq
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed class DispoIntPtr : IDisposable
    {

        public DispoIntPtr(int size)
        {
            Ptr = Marshal.AllocHGlobal(size);
            isAllocated = true;
        }

        private bool isAllocated;

        public IntPtr Ptr { get; private set; }



        ~DispoIntPtr()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            // TODO: instance ThreadStatic && do ( o == null ? return : ( lock(o, ms), check threadId, .. ) ) 
            IntPtr handle = Ptr;
            if (handle != IntPtr.Zero)
            {
                if (isAllocated)
                {
                    Marshal.FreeHGlobal(handle);
                    isAllocated = false;
                }
                Ptr = IntPtr.Zero;
            }
        }





    }
}