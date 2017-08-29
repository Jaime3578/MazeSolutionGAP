using System.IO;


namespace MazeSolution.Service
{
    public interface IFileReading
    {
        int[,] CreateMatrixFromFile(TextReader text);
    }
}
