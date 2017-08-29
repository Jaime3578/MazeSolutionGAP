using System;
using System.Collections.Generic;
using System.Linq;
using MazeSolution.Entities;

namespace MazeSolution.Service
{
    public class AStarPathfinding : IAStartPathFinding
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<Location> GetPath(int[,] matrix, int x, int y, int x1, int y1)
        {
            log.Info("Getting path from matrix init");
            var openList = new List<Location>();
            var closedList = new List<Location>();
           
            // start by adding the original position to the open list           
            closedList = ProcessLists(openList,closedList, x, y, x1, y1, matrix);
            log.Info("Getting path done!");
            return closedList;
        }

        private List<Location> ProcessLists(List<Location> openList, List<Location> closedList, int x, int y, int x1, int y1, int[,] matrix)
        {
            log.Info("Proccesing lists");
            Location current = null;
            var start = new Location { X = x, Y = y };
            var target = new Location { X = x1, Y = y1 };
            openList.Add(start);
            int g = 0;

            while (openList.Count > 0)
            {
                // get the square with the lowest F score
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                // add the current square to the closed list
                closedList.Add(current);

                // remove it from the open list
                openList.Remove(current);

                // if we added the destination to the closed list, we've found a path
                if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)
                    break;

                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, matrix);
                g++;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    // if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) != null)
                        continue;

                    // if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) == null)
                    {
                        // compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // and add it to the open list
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + adjacentSquare.H < adjacentSquare.F)
                        {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }

            return closedList;
        }

        /// <summary>
        /// Get Adjacent squaers from a point
        /// </summary>
        /// <param name="x">c coord</param>
        /// <param name="y">y coord</param>
        /// <param name="map">maze  matrix</param>
        /// <returns>valid adjacent squares</returns>
        private List<Location> GetWalkableAdjacentSquares(int x, int y, int[,] map)
        {
            var proposedLocations = new List<Location>();

            if (y > 0)
            {
                proposedLocations.Add(new Location { X = x, Y = y - 1 });
            }
            if ((y + 1) <= map.GetLength(1))
            {
                proposedLocations.Add(new Location { X = x, Y = y + 1 });
            }
            if (x > 0)
            {
                proposedLocations.Add(new Location { X = x - 1, Y = y });
            }
            if ((x + 1) < map.GetLength(0))
            {
                proposedLocations.Add(new Location { X = x + 1, Y = y });
            }

            return proposedLocations.Where(l => map[l.X,l.Y] == 1 || map[l.X,l.Y] == 3).ToList();
        }

        private int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }
    }
}
