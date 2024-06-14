using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SuperMatrixMultiplier
{
    public int[,] GenerateMatrix(int rows, int columns)
    {
        Random random = new Random();
        int[,] matrix = new int[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                matrix[i, j] = random.Next(1, 10); 
            }
        }

        return matrix;
    }

    public void WriteMatrixToFile(int[,] matrix, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    writer.Write(matrix[i, j] + " ");
                }
                writer.WriteLine();
            }
        }
    }

    public int[,] ReadMatrixFromFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        int rows = lines.Length;
        int columns = lines[0].Split().Length;

        int[,] matrix = new int[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            string[] rowElements = lines[i].Split();
            for (int j = 0; j < columns; j++)
            {
                int temp;
                if (int.TryParse(rowElements[j], out temp))
                    matrix[i, j] = temp;
                else
                    continue;
            }
        }

        return matrix;
    }

    public int[,] MultiplyMatrices(int[,] matrix1, int[,] matrix2)
    {
        int rows1 = matrix1.GetLength(0);
        int columns1 = matrix1.GetLength(1) - 1;
        int columns2 = matrix2.GetLength(1) - 1;

        int[,] result = new int[rows1, columns2];

        Parallel.For(0, rows1, i =>
        {
            for (int j = 0; j < columns2; j++)
            {
                for (int k = 0; k < columns1; k++)
                {
                    result[i, j] += matrix1[i, k] * matrix2[k, j];
                }
            }
        });

        return result;
    }

    public int[,] MultiplyMatrices_delegate(Func<int, int, int> del1, Func<int, int, int> del2, int rows, int columns1, int columns2)
    {
        int[,] result = new int[rows, columns2];

        Parallel.For(0, rows, i =>
        {
            for (int j = 0; j < columns2; j++)
            {
                for (int k = 0; k < columns1; k++)
                {
                    var temp1 = del1(i,j);
                    var temp2 = del2(i,k);
                    result[i, j] += temp1*temp2;
                }
            }
        });
        return result;
    }

    public void MultiplyMatrices_delegate(Func<int, int, int> del1, Func<int, int, int> del2, int rows, int columns1, int columns2, string filePath)
    {
        object lockObject = new object();

        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            Parallel.For(0, rows, i =>
            {
                for (int j = 0; j < columns2; j++)
                {
                    int tempResult = 0;
                    for (int k = 0; k < columns1; k++)
                    {
                        var temp1 = del1(i, j);
                        var temp2 = del2(i, k);
                        tempResult += temp1 * temp2;
                    }
                    byte[] bytes = BitConverter.GetBytes(tempResult);

                    lock (lockObject)
                    {
                        long position = i * columns2 * (sizeof(int)) + j * (sizeof(int));
                        fs.Seek(position, SeekOrigin.Begin);
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            });
        }
    }

    public async Task WriteMatrixResultToFileAsync(int[,] matrix, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    await writer.WriteAsync(matrix[i, j] + " ");
                }
                await writer.WriteLineAsync();
            }
        }
    }
}

class Program
{
    static async Task Main()
    {
        int matrixMaker1(int i, int j)
        {
            return i+j;
        }
        int matrixMaker2(int i, int j)
        {
            return i-j;
        }
        Func<int, int, int> del1 = matrixMaker1;
        Func<int, int, int> del2 = matrixMaker2;
        int rows = 10, columns1 = 10, columns2 = 10;

        SuperMatrixMultiplier matrixMultiplier = new SuperMatrixMultiplier();

        matrixMultiplier.MultiplyMatrices_delegate(matrixMaker1, matrixMaker2, rows, columns1, columns2, "resultMatrix.dat");
        //int[,] resultMatrix = matrixMultiplier.MultiplyMatrices_delegate(matrixMaker1, matrixMaker2, rows, columns1, columns2);
        //int[,] matrix1 = matrixMultiplier.GenerateMatrix(1000, 50000);
        //int[,] matrix2 = matrixMultiplier.GenerateMatrix(50000, 2);
        //
        //matrixMultiplier.WriteMatrixToFile(matrix1, "matrix1.txt");
        //matrixMultiplier.WriteMatrixToFile(matrix2, "matrix2.txt");
        //
        //int[,] readMatrix1 = matrixMultiplier.ReadMatrixFromFile("matrix1.txt");
        //int[,] readMatrix2 = matrixMultiplier.ReadMatrixFromFile("matrix2.txt");
        //
        //int[,] resultMatrix = matrixMultiplier.MultiplyMatrices(readMatrix1, readMatrix2);
        //
        //await matrixMultiplier.WriteMatrixResultToFileAsync(resultMatrix, "resultMatrix.txt");
        int i = 0;
        using (BinaryReader reader = new BinaryReader(File.Open("resultMatrix.dat", FileMode.Open)))
        {
            using (StreamWriter writer = new StreamWriter("resultMatrix.txt"))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    int value = reader.ReadInt32();

                    writer.Write(value + " ");
                    i++;
                    if (i == columns2)
                    {
                        i = 0;
                        writer.WriteLine();
                    }
                    
                }
            }
        }
    }
}

