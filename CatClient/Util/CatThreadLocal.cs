using System;
using System.Collections;
using System.Threading;
using System.Web;

namespace Org.Unidal.Cat.Util
{
    public class CatThreadLocal<T>
    {
        //private readonly Hashtable _mValues = new Hashtable(1024);

        private ThreadLocal<T> threadLocal = new ThreadLocal<T>();

        public T Value
        {
            get
            {
                if (null != HttpContext.Current)
                {
                    return (T)(HttpContext.Current.Items[CatConstants.CAT_CONTEXT]);
                }
                else
                {
                    return threadLocal.Value;
                }
            }
            set
            {
                if (null != HttpContext.Current)
                {
                    HttpContext.Current.Items[CatConstants.CAT_CONTEXT] = value;
                }
                else
                {
                    threadLocal.Value = value;
                }
            }
        }

        public void Remove()
        {
            threadLocal.Value = default(T);
        }
    }
}