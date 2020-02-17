using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.CL.Zmq.FullDuplex.Zmq
{
    public enum ZError:int
    {
                EPERM = 1,
                ENOENT = 2,
                ESRCH = 3,
                EINTR = 4,
                EIO = 5,
                ENXIO = 6,
                E2BIG = 7,
                ENOEXEC = 8,
                EBADF = 9,
                ECHILD = 10,
                EAGAIN = 11,
                ENOMEM = 12,
                EACCES = 13,
                EFAULT = 14,
                ENOTBLK = 15,
                EBUSY = 16,
                EEXIST = 17,
                EXDEV = 18,
                ENODEV = 19,
                ENOTDIR = 20,
                EISDIR = 21,
                EINVAL = 22,
                ENFILE = 23,
                EMFILE = 24,
                ENOTTY = 25,
                ETXTBSY = 26,
                EFBIG = 27,
                ENOSPC = 28,
                ESPIPE = 29,
                EROFS = 30,
                EMLINK = 31,
                EPIPE = 32,
                EDOM = 33,
                ERANGE = 34, // 34

                ENOTSUP = 129,
                EPROTONOSUPPORT = 135,
                ENOBUFS = 119,
                ENETDOWN = 116,
                EADDRINUSE = 100,
                EADDRNOTAVAIL = 101,
                ECONNREFUSED = 107,
                EINPROGRESS = 112,
                ENOTSOCK = 128,
                EMSGSIZE = 115,
                // as of here are differences to nanomsg
                EAFNOSUPPORT = 102,
                ENETUNREACH = 118,
                ECONNABORTED = 106,
                ECONNRESET = 108,
                ENOTCONN = 126,
                ETIMEDOUT = 138,
                EHOSTUNREACH = 110,
                ENETRESET = 117,

                /*  Native ZeroMQ error codes. */
                //EFSM = HAUSNUMERO + 51,
                //ENOCOMPATPROTO = HAUSNUMERO + 52,
                //ETERM = HAUSNUMERO + 53,
                //EMTHREAD = HAUSNUMERO + 54
           
}
}
