using System;
using System.Data;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Collections.Generic;

namespace skiing
{
    class MainClass
    {
        private static int[] possibleCellsX = new int[] { 1, -1, 0, 0 };
        private static int[] possibleCellsY = new int[] {0, 0, 1, -1};
        private static readonly string textFile = "C:\\Users\\nelso\\Downloads\\skirsesort.kitzbuehel\\map.txt";

        public static void Main(string[] args)
        {
            int[,] matrix = new int[,] { { 4, 8, 7, 3 }, 
                                          { 2, 5, 9, 3 }, 
                                          { 6, 3, 2, 5 }, 
                                          { 4, 4, 1, 6 } };

            (var matrixMap, var row, var col) = getMatrixFromFile();
            longestPath(matrixMap, row, col);
            Console.WriteLine();
            longestPath(matrix, 4, 4);
        }
        
        public static (int, List<int>) longestPathRecursive(int i, int j, int[,] matrix, int[,] matrixM, List<int>[,] bestPathM, int row, int col) 
        {
            if (matrixM[i, j] == 0) 
            {
                var best = 0;
                var bestPath = new List<int>();

                for (int k = 0; k < 4; k++)
                {
                    var nexti = i + possibleCellsX[k];
                    var nextj = j + possibleCellsY[k];

                    if (nexti >= 0 && nexti < row && 
                        nextj >= 0 && nextj< col &&
                        matrix[nexti, nextj] < matrix[i, j])
                    {
                        var n = 0;
                        var path = new List<int>();
                        (n, path) = longestPathRecursive(nexti, nextj, matrix, matrixM, bestPathM, row, col);
                        if (n > best)
                        {
                            best = n;
                            bestPath = path;
                        }
                    }
                }

                var bestPathAux = new List<int> { matrix[i, j] };
                foreach(var val in bestPath)
                {
                    bestPathAux.Add(val);
                }
                matrixM[i, j] = best + 1;
                bestPathM[i, j] = bestPathAux;
                return (matrixM[i, j], bestPathM[i, j]);
            } else
            {
                return (matrixM[i, j], bestPathM[i, j]);
            }
        }

        public static void longestPath(int[,] matrix, int row, int col)
        {
            var matrixM = new int[row, col];
            var bestPathM = createbestPathM(row, col);
            var nArray = new List<int>();
            var pathArray = new List<List<int>>();

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    var n = 0;
                    var path = new List<int>();
                    (n, path) = longestPathRecursive(i, j, matrix, matrixM, bestPathM, row, col);
                    nArray.Add(n);
                    pathArray.Add(path);
                }
            }

            var nMax = nArray.Max();
            var listIndices = findIndices(nMax, nArray);
            var indexMax = 0;
            var deepest = -1;

            foreach(var index in listIndices)
            {
                var deep = pathArray[index][0] - pathArray[index][pathArray[index].Count-1];
                if (deep > deepest)
                {
                    indexMax = index;
                    deepest = deep;
                }
            }

            Console.Write("Calculated path = ");
            foreach(var val in pathArray[indexMax])
            {
                Console.Write(val);
                Console.Write(" ");
            }
            Console.WriteLine();
            Console.WriteLine("Length of calculated path = " + nMax);
            Console.WriteLine("Drop of calculated path = " + deepest);
            //Console.WriteLine();
            //printMatrix(matrixM, row, col);
        }

        public static List<int> findIndices(int max, List<int> list)
        {
            var listIndices = new List<int>();
  
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == max)
                {
                    listIndices.Add(i);
                }
            }

            return listIndices;
        }

        public static List<int>[,] createbestPathM(int row, int col) 
        {
            var bestPathM = new List<int>[row,col];
            
            for (int i = 0; i < row; i++) 
            {
                for (int j = 0; j < col; j++)
                {
                    bestPathM[i, j] = new List<int>();
                }
            }

            return bestPathM;
        }

        public static void printMatrix(int[,] matrix, int row, int col) 
        {
            for (int i = 0; i < row; i++) 
            { 
                for (int j = 0; j < col; j++)
                {
                    Console.Write(matrix[i, j]);
                }
                Console.WriteLine();
            }
        }

        public static (int[,], int, int) getMatrixFromFile()
        {
            var row = 0;
            var col = 0;
            int[,] matrix = {};

            if (File.Exists(textFile))
            { 
                string[] lines = File.ReadAllLines(textFile);
                row = int.Parse(lines[0].Split(" ").ToList()[0]);
                col = int.Parse(lines[0].Split(" ").ToList()[1]);
                matrix = new int[row, col];
                for (int i = 1; i < lines.Length; i++) 
                {
                    var values = lines[i].Split(" ").ToList();
                    for (int j = 0; j < col; j++)
                    {
                        matrix[i-1, j] = int.Parse(values[j]); 
                    }
                }
            }

            return (matrix, row, col);
        }
    }
}