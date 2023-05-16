

namespace KD_Search
{
    using System;
    using System.Runtime.CompilerServices;


    public struct HyperRect<T>
        where T : IComparable<T>
    {

        private T[] minPoint;


        private T[] maxPoint;


        public T[] MinPoint
        {
            get
            {
                return this.minPoint;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this.minPoint = new T[value.Length];
                value.CopyTo(this.minPoint, 0);
            }
        }


        public T[] MaxPoint
        {
            get
            {
                return this.maxPoint;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this.maxPoint = new T[value.Length];
                value.CopyTo(this.maxPoint, 0);
            }
        }


        public static HyperRect<T> Infinite(int dimensions, T positiveInfinity, T negativeInfinity)
        {
            var rect = default(HyperRect<T>);

            rect.MinPoint = new T[dimensions];
            rect.MaxPoint = new T[dimensions];

            for (var dimension = 0; dimension < dimensions; dimension++)
            {
                rect.MinPoint[dimension] = negativeInfinity;
                rect.MaxPoint[dimension] = positiveInfinity;
            }

            return rect;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] GetClosestPoint(T[] toPoint)
        {
            var closest = new T[toPoint.Length];

            for (var dimension = 0; dimension < toPoint.Length; dimension++)
            {
                if (this.minPoint[dimension].CompareTo(toPoint[dimension]) > 0)
                {
                    closest[dimension] = this.minPoint[dimension];
                }
                else if (this.maxPoint[dimension].CompareTo(toPoint[dimension]) < 0)
                {
                    closest[dimension] = this.maxPoint[dimension];
                }
                else
                {
                    // Point is within rectangle, at least on this dimension
                    closest[dimension] = toPoint[dimension];
                }
            }

            return closest;
        }


        public HyperRect<T> Clone()
        {
            // For a discussion of why we don't implement ICloneable
            // see http://stackoverflow.com/questions/536349/why-no-icloneablet
            var rect = default(HyperRect<T>);
            rect.MinPoint = this.MinPoint;
            rect.MaxPoint = this.MaxPoint;
            return rect;
        }
    }
}
