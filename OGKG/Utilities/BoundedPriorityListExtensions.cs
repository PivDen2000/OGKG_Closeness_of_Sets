

namespace KD_Search
{
    using System;
    using System.Runtime.CompilerServices;


    public static class BoundedPriorityListExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tuple<TDimension[], TNode>[] ToResultSet<TPriority, TDimension, TNode>(
           this BoundedPriorityList<int, TPriority> list,
           KDTree<TDimension, TNode> tree)
           where TDimension : IComparable<TDimension>
           where TPriority : IComparable<TPriority>
        {
            var array = new Tuple<TDimension[], TNode>[list.Count];
            for (var i = 0; i < list.Count; i++)
            {
                array[i] = new Tuple<TDimension[], TNode>(
                    tree.InternalPointArray[list[i]],
                    tree.InternalNodeArray[list[i]]);
            }

            return array;
        }
    }
}
