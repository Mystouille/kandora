using System;
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.utils
{
    public class ISequenceComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            var xl = x.ToList();
            var yl = y.ToList();
            int interCount = x.Intersect(y).Count();
            if (xl.Count != yl.Count || interCount != xl.Intersect(xl).Count() || interCount != yl.Intersect(yl).Count())
            {
                return false;
            }
            for(int i = 0; i< xl.Count(); i++)
            {
                if(!xl[i].Equals(yl[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(IEnumerable<T> obj)
        {
            // Will not throw an OverflowException
            unchecked
            {
                return obj.Where(e => e != null).Select(e => e.GetHashCode()).Aggregate(17, (a, b) => 23 * a + b);
            }
        }
    }
}
