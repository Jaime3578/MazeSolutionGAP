using MazeSolution.Entities;
using System.Collections.Generic;


namespace MazeSolution.Service
{
    public interface IAStartPathFinding
    {
        List<Location> GetPath(int[,] matrix, int x, int y, int x1, int y1);
    }
}
