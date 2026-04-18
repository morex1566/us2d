using Unity.Mathematics;

namespace Static.Pathfinding
{
    public struct PathfindingNode
    {
        public int2 position;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
        public int parentIndex;
        public bool isWalkable;

        public void ResetCosts()
        {
            gCost = int.MaxValue;
            hCost = 0;
            parentIndex = -1;
        }
    }
}
