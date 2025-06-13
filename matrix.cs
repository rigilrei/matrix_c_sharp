public class Matrix
{
    protected int[,] mas; // элементы матрицы
    public int Rows { get; }
    public int Cols { get; }

    public Matrix(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        mas = new int[rows, cols];
    }

    public int this[int i, int j]
    {
        get => mas[i, j]; // возвращает эл мас
        set => mas[i, j] = value; // установка значения
    }

    public void Print()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                Console.Write($"{mas[i, j],4}");
            }
            Console.WriteLine();
        }
    }
}

    public class SquareMatrix : Matrix
{
    public SquareMatrix(int size) : base(size, size) { }

    public double Determinant()
    {
        var matrix = ToDoubleArray();
        return DeterminantRecursive(matrix);
    }

    private double DeterminantRecursive(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        if (n == 1) return matrix[0, 0];
        if (n == 2) return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

        double det = 0;
        for (int p = 0; p < n; p++)
        {
            var subMatrix = CreateSubMatrix(matrix, p); // подматрица - 1 строка и 1 столбец
            det += matrix[0, p] * DeterminantRecursive(subMatrix) * (p % 2 == 0 ? 1 : -1);
        }
        return det;
    }

    private double[,] CreateSubMatrix(double[,] matrix, int excludeColumn)
    {
        int n = matrix.GetLength(0);
        var result = new double[n - 1, n - 1];
        for (int i = 1; i < n; i++)
        {
            for (int j = 0, col = 0; j < n; j++)
            {
                if (j == excludeColumn) continue;
                result[i - 1, col++] = matrix[i, j];
            }
        }
        return result;
    }

    public SquareMatrix Transpose()
    {
        var result = new SquareMatrix(Rows); // матрица сейм размера
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                result[j, i] = mas[i, j];
            }
        }
        return result;
    }

    public SquareMatrix Inverse()
    {
        double[,] augmented = new double[Rows, 2 * Cols]; // для расширенной матрицы
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                augmented[i, j] = mas[i, j]; //  коп в левую половину расшир матрицы
                augmented[i, j + Cols] = (i == j) ? 1 : 0; // правая половина заполняется единичной матрицой
            }
        }

        for (int k = 0; k < Rows; k++) // ступ вид
        {
            double pivot = augmented[k, k];
            if (pivot == 0) // глав диаг
                throw new InvalidOperationException("Матрица вырожденная и не имеет обратной.");

            for (int j = 0; j < 2 * Cols; j++)
                augmented[k, j] /= pivot;

            for (int i = 0; i < Rows; i++)
            {
                if (i == k) continue;
                double factor = augmented[i, k];
                for (int j = 0; j < 2 * Cols; j++)
                    augmented[i, j] -= factor * augmented[k, j];
            }
        }

        var result = new SquareMatrix(Rows);
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                result[i, j] = (int)augmented[i, j + Cols];

        return result;
    }

    public SquareMatrix UpperTriangular()
    {
        var result = new SquareMatrix(Rows);
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                result[i, j] = mas[i, j];

        for (int k = 0; k < Rows; k++)
        {
            // Проверка на нулевой элемент на диагонали, замена строк если нужно
            if (result[k, k] == 0)
            {
                for (int i = k + 1; i < Rows; i++)
                {
                    if (result[i, k] != 0)
                    {
                        SwapRows(result, k, i);
                        break;
                    }
                }
            }

            // Приведение элементов ниже главной диагонали к нулю
            for (int i = k + 1; i < Rows; i++)
            {
                if (result[i, k] != 0)
                {
                    double factor = (double)result[i, k] / result[k, k];
                    for (int j = k; j < Cols; j++)
                    {
                        result[i, j] -= (int)(factor * result[k, j]);
                    }
                }
            }
        }

        return result;
    }

    public int Trace() // s глав диаг 
    {
        int trace = 0;
        for (int i = 0; i < Rows; i++)
        {
            trace += mas[i, i];
        }
        return trace;
    }

    private void SwapRows(SquareMatrix matrix, int row1, int row2)
    {
        for (int j = 0; j < matrix.Cols; j++)
        {
            int temp = matrix[row1, j];
            matrix[row1, j] = matrix[row2, j];
            matrix[row2, j] = temp;
        }
    }

    private double[,] ToDoubleArray()
    {
        var result = new double[Rows, Cols];
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                result[i, j] = mas[i, j];
        return result;
    }
}

public class MatrixGenerator
{
    public static void Main(string[] args)
    {
        int size = 3; // размер кв матрицы
        Random random = new Random();

        // кв матрица
        var squareMatrix = new SquareMatrix(size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                squareMatrix[i, j] = random.Next(1, 101); // рандом числа от 1 до 100
            }
        }

        Console.WriteLine("Исходная матрица:");
        squareMatrix.Print();

        Console.WriteLine($"\nОпределитель: {squareMatrix.Determinant()}");

        Console.WriteLine("\nТранспонированная матрица:");
        squareMatrix.Transpose().Print();

        Console.WriteLine("\nВерхнетреугольная форма:");
        squareMatrix.UpperTriangular().Print();

        Console.WriteLine($"\nСлед матрицы: {squareMatrix.Trace()}");

        try
        {
            Console.WriteLine("\nОбратная матрица:");
            squareMatrix.Inverse().Print();
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine($"\nОшибка: {e.Message}");
        }
    }
}




