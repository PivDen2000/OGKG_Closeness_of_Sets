

namespace KD_Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using static BinaryTreeNavigation;

    [Serializable]
    public class KDTree<TDimension, TNode>
        where TDimension : IComparable<TDimension>
    {
        public int Count { get; }

        public int Dimensions { get; }

        public TDimension[][] InternalPointArray { get; }

        public TNode[] InternalNodeArray { get; }

        public Func<TDimension[], TDimension[], double> Metric { get; set; }

        public BinaryTreeNavigator<TDimension[], TNode> Navigator
            => new BinaryTreeNavigator<TDimension[], TNode>(this.InternalPointArray, this.InternalNodeArray);

        private TDimension MaxValue { get; }

        private TDimension MinValue { get; }

   
        public KDTree(
            int dimensions,
            TDimension[][] points,
            TNode[] nodes,
            Func<TDimension[], TDimension[], double> metric,
            TDimension searchWindowMinValue = default(TDimension),
            TDimension searchWindowMaxValue = default(TDimension))
        {
            // Attempt find the Min/Max value if null.
            if (searchWindowMinValue.Equals(default(TDimension)))
            {
                var type = typeof(TDimension);
                this.MinValue = (TDimension)type.GetField("MinValue").GetValue(type);
            }
            else
            {
                this.MinValue = searchWindowMinValue;
            }

            if (searchWindowMaxValue.Equals(default(TDimension)))
            {
                var type = typeof(TDimension);
                this.MaxValue = (TDimension)type.GetField("MaxValue").GetValue(type);
            }
            else
            {
                this.MaxValue = searchWindowMaxValue;
            }

            var elementCount = (int)Math.Pow(2, (int)(Math.Log(points.Length) / Math.Log(2)) + 1);
            this.Dimensions = dimensions;
            this.InternalPointArray = Enumerable.Repeat(default(TDimension[]), elementCount).ToArray();
            this.InternalNodeArray = Enumerable.Repeat(default(TNode), elementCount).ToArray();
            this.Metric = metric;
            this.Count = points.Length;
            this.GenerateTree(0, 0, points, nodes);
        }
        public Tuple<TDimension[], TNode>[] NearestNeighbors(TDimension[] point, int neighbors)
        {
            var nearestNeighborList = new BoundedPriorityList<int, double>(neighbors, true);
            var rect = HyperRect<TDimension>.Infinite(this.Dimensions, this.MaxValue, this.MinValue);
            this.SearchForNearestNeighbors(0, point, rect, 0, nearestNeighborList, double.MaxValue);

            return nearestNeighborList.ToResultSet(this);
        }

        private void GenerateTree(
            int index,
            int dim,
            IReadOnlyCollection<TDimension[]> points,
            IEnumerable<TNode> nodes)
        {

           var zippedList = points.Zip(nodes, (p, n) => new { Point = p, Node = n });


            var sortedPoints = zippedList.OrderBy(z => z.Point[dim]).ToArray();


            var medianPoint = sortedPoints[points.Count / 2];
            var medianPointIdx = sortedPoints.Length / 2;

            this.InternalPointArray[index] = medianPoint.Point;
            this.InternalNodeArray[index] = medianPoint.Node;


            var leftPoints = new TDimension[medianPointIdx][];
            var leftNodes = new TNode[medianPointIdx];
            Array.Copy(sortedPoints.Select(z => z.Point).ToArray(), leftPoints, leftPoints.Length);
            Array.Copy(sortedPoints.Select(z => z.Node).ToArray(), leftNodes, leftNodes.Length);

            // 2nd group: Points after the median
            var rightPoints = new TDimension[sortedPoints.Length - (medianPointIdx + 1)][];
            var rightNodes = new TNode[sortedPoints.Length - (medianPointIdx + 1)];
            Array.Copy(
                sortedPoints.Select(z => z.Point).ToArray(),
                medianPointIdx + 1,
                rightPoints,
                0,
                rightPoints.Length);
            Array.Copy(sortedPoints.Select(z => z.Node).ToArray(), medianPointIdx + 1, rightNodes, 0, rightNodes.Length);


            var nextDim = (dim + 1) % this.Dimensions; // select next dimension

            if (leftPoints.Length <= 1)
            {
                if (leftPoints.Length == 1)
                {
                    this.InternalPointArray[LeftChildIndex(index)] = leftPoints[0];
                    this.InternalNodeArray[LeftChildIndex(index)] = leftNodes[0];
                }
            }
            else
            {
                this.GenerateTree(LeftChildIndex(index), nextDim, leftPoints, leftNodes);
            }

            // Do the same for the right points
            if (rightPoints.Length <= 1)
            {
                if (rightPoints.Length == 1)
                {
                    this.InternalPointArray[RightChildIndex(index)] = rightPoints[0];
                    this.InternalNodeArray[RightChildIndex(index)] = rightNodes[0];
                }
            }
            else
            {
                this.GenerateTree(RightChildIndex(index), nextDim, rightPoints, rightNodes);
            }
        }

        private void SearchForNearestNeighbors(
            int nodeIndex,
            TDimension[] target,
            HyperRect<TDimension> rect,
            int dimension,
            BoundedPriorityList<int, double> nearestNeighbors,
            double maxSearchRadiusSquared)
        {
            if (this.InternalPointArray.Length <= nodeIndex || nodeIndex < 0
                || this.InternalPointArray[nodeIndex] == null)
            {
                return;
            }


            var dim = dimension % this.Dimensions;

            var leftRect = rect.Clone();
            leftRect.MaxPoint[dim] = this.InternalPointArray[nodeIndex][dim];

            var rightRect = rect.Clone();
            rightRect.MinPoint[dim] = this.InternalPointArray[nodeIndex][dim];

            var compare = target[dim].CompareTo(this.InternalPointArray[nodeIndex][dim]);

            var nearerRect = compare <= 0 ? leftRect : rightRect;
            var furtherRect = compare <= 0 ? rightRect : leftRect;

            var nearerNode = compare <= 0 ? LeftChildIndex(nodeIndex) : RightChildIndex(nodeIndex);
            var furtherNode = compare <= 0 ? RightChildIndex(nodeIndex) : LeftChildIndex(nodeIndex);


            this.SearchForNearestNeighbors(
                nearerNode,
                target,
                nearerRect,
                dimension + 1,
                nearestNeighbors,
                maxSearchRadiusSquared);


            var closestPointInFurtherRect = furtherRect.GetClosestPoint(target);
            var distanceSquaredToTarget = this.Metric(closestPointInFurtherRect, target);

            if (distanceSquaredToTarget.CompareTo(maxSearchRadiusSquared) <= 0)
            {
                if (nearestNeighbors.IsFull)
                {
                    if (distanceSquaredToTarget.CompareTo(nearestNeighbors.MaxPriority) < 0)
                    {
                        this.SearchForNearestNeighbors(
                            furtherNode,
                            target,
                            furtherRect,
                            dimension + 1,
                            nearestNeighbors,
                            maxSearchRadiusSquared);
                    }
                }
                else
                {
                    this.SearchForNearestNeighbors(
                        furtherNode,
                        target,
                        furtherRect,
                        dimension + 1,
                        nearestNeighbors,
                        maxSearchRadiusSquared);
                }
            }


            distanceSquaredToTarget = this.Metric(this.InternalPointArray[nodeIndex], target);
            if (distanceSquaredToTarget.CompareTo(maxSearchRadiusSquared) <= 0)
            {
                nearestNeighbors.Add(nodeIndex, distanceSquaredToTarget);
            }
        }
    }

}
