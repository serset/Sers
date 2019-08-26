using System;

namespace App.AuthCenter.Logical.Contract.Token
{
    public class AuthToken
    {

        public long userId;

        public string name;


        /// <summary>
        /// RefreshToken
        /// </summary>
        public string rt { get; set; }


        /// <summary>
        /// AccessToken
        /// </summary>
        public string at { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime addTime;

        public DateTime at_ExpiresTime;
        public DateTime rt_ExpiresTime;

    }
}
