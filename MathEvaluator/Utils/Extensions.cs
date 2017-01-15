using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEvaluator.Utils
{
    static class Extensions
    {
        public static bool Not(this bool arg)
        {
            return !arg;
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotNullOrEmpty(this string s)
        {
            return s.IsNullOrEmpty().Not();
        }

        public static bool IsNull(this object arg)
        {
            return arg == null;
        }
        public static bool IsNotNull(this object arg)
        {
            return arg.IsNull().Not();
        }

        public static T To<T>(this object arg)
        {
            return (T)arg;
        }

        public static bool TryNext<T>(this IEnumerator<T> enumerator, out T item)
        {
            if (enumerator.MoveNext().Not())
            {
                item = default(T);
                return false;
            }
            item = enumerator.Current;
            return true;
        }
        public static T Next<T>(this IEnumerator<T> enumerator)
        {
            if (enumerator.MoveNext().Not())
            {
                return default(T);                
            }
            return enumerator.Current;            
        }
    }
}
