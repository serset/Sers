#region << 版本注释 - v1 >>
/*
 * ========================================================================
 * 版本：v1
 * 时间：2021-03-19
 * 作者：Lith
 * 邮箱：serset@yeah.net
 * 
 * ========================================================================
*/
#endregion




namespace Vit.Core.Util.Threading
{
    /// <summary>
    /// 切换线程时不传递数据     
    /// </summary>
    public class ThreadCache<T>
    {

        readonly System.Threading.ThreadLocal<T> _AsyncLocal = new System.Threading.ThreadLocal<T>();

       
        public T Value
        {
            get
            {              
                return _AsyncLocal.Value;
            }
            set
            {               
                _AsyncLocal.Value = value;
            }
        }
    }
   
}
