using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Static.Pathfinding
{
    public static class Pathfinding
    {
        /// <summary>
        /// Request a path from start to end position.
        /// Returns a JobHandle. resultPath will contain the positions in reverse order (goal to start).
        /// </summary>
        public static JobHandle RequestPath(Tilemap tilemap, int2 start, int2 end, NativeList<int2> pathResult)
        {
            BoundsInt bounds = tilemap.cellBounds;
            int width = bounds.size.x;
            int height = bounds.size.y;

            // Use Allocator.Persistent or TempJob depending on usage. 
            // Here we use TempJob and the caller should ideally handle disposal or use [DeallocateOnJobCompletion]
            NativeArray<PathfindingNode> nodes = new NativeArray<PathfindingNode>(width * height, Allocator.TempJob);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3Int tilePos = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);
                    bool isWalkable = tilemap.HasTile(tilePos);
                    
                    int index = x + y * width;
                    nodes[index] = new PathfindingNode
                    {
                        position = new int2(x, y),
                        isWalkable = isWalkable
                    };
                }
            }

            var job = new AStarJob
            {
                startPos = start - new int2(bounds.xMin, bounds.yMin),
                endPos = end - new int2(bounds.xMin, bounds.yMin),
                gridWidth = width,
                gridHeight = height,
                nodes = nodes,
                resultPath = pathResult
            };

            // Schedule the job
            JobHandle handle = job.Schedule();
            
            // Dispose nodes after job completes
            nodes.Dispose(handle);

            return handle;
        }
    }
}
