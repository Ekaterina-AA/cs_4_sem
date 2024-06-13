using System;
using System.IO;
using System.Linq;
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
        SuperMatrixMultiplier matrixMultiplier = new SuperMatrixMultiplier();

        int[,] matrix1 = matrixMultiplier.GenerateMatrix(1000, 50000);
        int[,] matrix2 = matrixMultiplier.GenerateMatrix(50000, 2);

        matrixMultiplier.WriteMatrixToFile(matrix1, "matrix1.txt");
        matrixMultiplier.WriteMatrixToFile(matrix2, "matrix2.txt");

        int[,] readMatrix1 = matrixMultiplier.ReadMatrixFromFile("matrix1.txt");
        int[,] readMatrix2 = matrixMultiplier.ReadMatrixFromFile("matrix2.txt");

        int[,] resultMatrix = matrixMultiplier.MultiplyMatrices(readMatrix1, readMatrix2);

        await matrixMultiplier.WriteMatrixResultToFileAsync(resultMatrix, "resultMatrix.txt");
    }
}

