using System;
using System.Collections.Generic;
using System.IO;

namespace MazeSolution.Service
{
    public class FileReading : IFileReading
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int hSpace = 4;

        /// <summary>
        /// create adjacency matrix from text file
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int[,] CreateMatrixFromFile(TextReader text)
        {
            log.Info("Create matrix from file");
            int x = 0;
            int y = 0;
            int i = 0;
            int j = 0;

            List<String> normalizedMatrix = getMatrixDimensions(text, ref x, ref y);

            int[,] matrix = new int[x,y];

            foreach (string line in normalizedMatrix)
            {
                char[] array = line.ToCharArray();
                j = 0;
            
                foreach(char c in array)
                {
                    matrix[i, j] = c.Equals('1') ? 1 : 0;
                    j++;
                }

                i++;
            }
            log.Info("Matrix from file done!");
            return matrix;
        }
    
        /// <summary>
        /// Get Dimensions of matrix
        /// </summary>
        /// <param name="text"></param>
        private List<String> getMatrixDimensions(TextReader text, ref int x, ref int y)
        {
            String line;
            List<String> sb = new List<String>();
            log.Info("Creating and cutting blank spaces");
            while ((line = text.ReadLine()) != null)
            {
                if (IsValidLine(line))
                {
                    x = x + 1;
                    string split = line.Replace("\tF","1");
                    split = split.Replace("\t","0");
                    y = (split.Length-hSpace) > y ? (split.Length-hSpace) : y;
                    sb.Add(split.Substring(4,split.Length-hSpace));
                }

            }

            return sb;
        }

        /// <summary>
        /// Checks if a line is not a empty line
        /// </summary>
        private bool IsValidLine(string line)
        {
            if (line.Contains("F"))
            {
                return true;
            }

            return false;
        }
    }
}
