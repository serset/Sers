using Sers.Core.Extensions;
using Sers.Core.Util.ConfigurationManager;
using StackExchange.Redis;
using System;

namespace Sers.Core.Db.Redis
{
    public class DbRedis:IDisposable
    {
        protected ConnectionMultiplexer redis { get; set; }
        protected IDatabase db { get; set; }

        protected string defaultConnectString = ConfigurationManager.Instance.Get<string>("ConnectionStrings", "Redis");

        public DbRedis(string connection=null, int db = -1)
        {
            redis = ConnectionMultiplexer.Connect(connection?? defaultConnectString);
            this.db = redis.GetDatabase(db);          
        }

        public bool ChangeDataBase(int db = -1)
        {
            if (null!=redis)
            {
                this.db = redis.GetDatabase(db);
                return true;
            }
            return false;
        }

        public bool Set(object value, params string[] keys)
        {
            return Set(value, (TimeSpan?)null, keys);             
        }
        public bool Set(object value, TimeSpan? expiry = null, params string[] keys)
        {
            string str= value.Serialize();            
            return db.StringSet(BuildKey(keys),str, expiry);
        }

        public bool Set(object value, DateTime? expiry = null, params string[] keys)
        {
            return Set(value, null== expiry?null:(expiry-DateTime.Now),keys);
        }


        public T Get<T>(params string [] keys)
        {
            string str = db.StringGet(BuildKey(keys));
            return str.Deserialize<T>();
        }

 
        public bool Delete(params string[] keys)
        {
            return db.KeyDelete(BuildKey(keys));
        }

        public string BuildKey(params string[] keys)
        {
            return String.Join(":", keys);
        }

        public void Dispose()
        {
            if (null != redis)
            {
                redis.Dispose();
                redis = null;
            }            
        }
    }
}
