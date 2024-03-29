﻿using static Sers.Hardware.Env.LinuxHelp;

namespace Sers.Hardware.Env
{


    public class DeviceInfo
    {

        /// <summary>
        /// 机器key码，同一台机器key码不变
        /// </summary>
        public string deviceKey { get; set; }

        public string OSPlatform;

        /// <summary>
        /// 系统架构
        /// </summary>
        public string OSArchitecture;
        /// <summary>
        /// 系统名称
        /// </summary>
        public string OSDescription;
        /// <summary>
        /// 进程架构
        /// </summary>
        public string ProcessArchitecture;
        /// <summary>
        /// 是否64位操作系统
        /// </summary>
        public bool Is64BitOperatingSystem;
        /// <summary>
        /// CPU CORE
        /// </summary>
        public int ProcessorCount;
        /// <summary>
        /// HostName
        /// </summary>
        public string MachineName;
        /// <summary>
        /// Version
        /// </summary>
        public string OSVersion;

        /// <summary>
        /// 内存相关的
        /// </summary>
        public long WorkingSet;



        /// <summary>
        /// CPU序列号
        /// </summary>
        public string ProcessorID;



        /// <summary>
        /// 描述linux系统的信息（若不为Linux则为null)
        /// </summary>
        public ServerInfo linux_ServerInfo;

    }
}
