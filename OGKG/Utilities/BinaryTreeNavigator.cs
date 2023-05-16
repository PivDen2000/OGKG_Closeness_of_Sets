

namespace KD_Search
{
    using System;

    using static BinaryTreeNavigation;


    public class BinaryTreeNavigator<TPoint, TNode>
    {

        private readonly TPoint[] pointArray;

        private readonly TNode[] nodeArray;


        public int Index { get; }

        public BinaryTreeNavigator<TPoint, TNode> Left
            =>
                LeftChildIndex(this.Index) < this.pointArray.Length - 1
                    ? new BinaryTreeNavigator<TPoint, TNode>(this.pointArray, this.nodeArray, LeftChildIndex(this.Index))
                    : null;


        public BinaryTreeNavigator<TPoint, TNode> Right
               =>
                   RightChildIndex(this.Index) < this.pointArray.Length - 1
                       ? new BinaryTreeNavigator<TPoint, TNode>(this.pointArray, this.nodeArray, RightChildIndex(this.Index))
                       : null;


        public BinaryTreeNavigator<TPoint, TNode> Parent => this.Index == 0 ? null : new BinaryTreeNavigator<TPoint, TNode>(this.pointArray, this.nodeArray, ParentIndex(this.Index));


        public TPoint Point => this.pointArray[this.Index];


        public TNode Node => this.nodeArray[this.Index];

        public BinaryTreeNavigator(TPoint[] pointArray, TNode[] nodeArray, int index = 0)
        {
            this.Index = index;
            this.pointArray = pointArray;
            this.nodeArray = nodeArray;
        }
    }
}
