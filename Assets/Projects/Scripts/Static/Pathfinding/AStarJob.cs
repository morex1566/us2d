using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Static.Pathfinding
{
    [BurstCompile]
    public struct AStarJob : IJob
    {
        public int2 startPos;

        public int2 endPos;

        public int gridWidth;

        public int gridHeight;

        public NativeArray<PathfindingNode> nodes;

        public NativeList<int2> resultPath;

        public void Execute()
        {
            int startIndex = GetIndex(startPos.x, startPos.y);
            int endIndex = GetIndex(endPos.x, endPos.y);

            if (startIndex < 0 || endIndex < 0 || startIndex >= nodes.Length || endIndex >= nodes.Length) return;

            // 노드 초기화
            for (int i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                node.ResetCosts();
                nodes[i] = node;
            }

            var startNode = nodes[startIndex];
            startNode.gCost = 0;
            startNode.hCost = CalculateDistance(startPos, endPos);
            nodes[startIndex] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startIndex);

            // 8방향 오프셋: 루프 밖에서 한 번만 할당
            NativeArray<int2> neighborOffsets = new NativeArray<int2>(8, Allocator.Temp);
            neighborOffsets[0] = new int2(0, 1);
            neighborOffsets[1] = new int2(0, -1);
            neighborOffsets[2] = new int2(1, 0);
            neighborOffsets[3] = new int2(-1, 0);
            neighborOffsets[4] = new int2(1, 1);
            neighborOffsets[5] = new int2(1, -1);
            neighborOffsets[6] = new int2(-1, 1);
            neighborOffsets[7] = new int2(-1, -1);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestFCostNodeIndex(openList);
                var currentNode = nodes[currentNodeIndex];

                if (currentNodeIndex == endIndex)
                {
                    RetracePath(endIndex);
                    break;
                }

                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }
                closedList.Add(currentNodeIndex);

                // 이웃 탐색
                for (int i = 0; i < neighborOffsets.Length; i++)
                {
                    int2 neighborPos = currentNode.position + neighborOffsets[i];
                    if (!IsValidPos(neighborPos)) continue;

                    int neighborIndex = GetIndex(neighborPos.x, neighborPos.y);
                    if (IsListContains(closedList, neighborIndex) || !nodes[neighborIndex].isWalkable) continue;

                    int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode.position, neighborPos);
                    if (tentativeGCost < nodes[neighborIndex].gCost)
                    {
                        var neighborNode = nodes[neighborIndex];
                        neighborNode.gCost = tentativeGCost;
                        neighborNode.hCost = CalculateDistance(neighborPos, endPos);
                        neighborNode.parentIndex = currentNodeIndex;
                        nodes[neighborIndex] = neighborNode;

                        if (!IsListContains(openList, neighborIndex))
                        {
                            openList.Add(neighborIndex);
                        }
                    }
                }
            }

            // 할당된 메모리 일괄 해제
            neighborOffsets.Dispose();
            openList.Dispose();
            closedList.Dispose();
        }

        private bool IsListContains(NativeList<int> list, int value)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i] == value) return true;
            return false;
        }

        private int GetLowestFCostNodeIndex(NativeList<int> openList)
        {
            int lowestIndex = openList[0];
            for (int i = 1; i < openList.Length; i++)
            {
                if (nodes[openList[i]].fCost < nodes[lowestIndex].fCost)
                {
                    lowestIndex = openList[i];
                }
            }
            return lowestIndex;
        }

        private void RetracePath(int endIndex)
        {
            int curr = endIndex;
            while (curr != -1)
            {
                resultPath.Add(nodes[curr].position);
                curr = nodes[curr].parentIndex;
            }
        }

        private int CalculateDistance(int2 a, int2 b)
        {
            int dx = math.abs(a.x - b.x);
            int dy = math.abs(a.y - b.y);
            int remaining = math.abs(dx - dy);
            return 14 * math.min(dx, dy) + 10 * remaining;
        }

        private bool IsValidPos(int2 pos) => pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;

        private int GetIndex(int x, int y) => x + y * gridWidth;
    }
}