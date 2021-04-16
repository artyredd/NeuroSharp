using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    public static class MatrixOperations
    {
        public delegate T TwoElementOperation<T>(ref T left, ref T right);
        public delegate T SingleElementOperation<T>(ref T MatrixElement);

        public static IMatrix<T> PerformOperationMemberwise<T>(IMatrix<T> matrix, SingleElementOperation<T> Operation) where T : unmanaged, IComparable<T>, IEquatable<T>
        {
            for (int row = 0; row < matrix.Rows; row++)
            {
                Span<T> Row = new(matrix._Matrix[row]);
                for (int column = 0; column < matrix.Columns; column++)
                {
                    Operation(ref Row[column]);
                }
            }

            return matrix;
        }

        public static IMatrix<T> PerformTwoSidedOperationMemberwise<T>(IMatrix<T> Left, IMatrix<T> Right, TwoElementOperation<T> Operation)
            where T : unmanaged, IComparable<T>, IEquatable<T>
        {
            if (Left.Rows != Right.Rows || Left.Columns != Right.Columns)
            {
                throw Exceptions.UnexpectedMatrixShape(Left.Rows, Left.Columns, "Congruent", $"Actual shape: {Right.Rows}x{Right.Columns}. Only congruent matrices can have operations ran on them concurrently.");
            }
            // this function should only be used when the two matrices are the same shape(same row and column count)
            for (int row = 0; row < Left.Rows; row++)
            {
                Span<T> LeftRow = new(Left[row]);
                Span<T> RightRow = new(Right[row]);
                for (int column = 0; column < Left.Columns; column++)
                {
                    Operation(ref LeftRow[column], ref RightRow[column]);
                }
            }
            return Left;
        }
        public static Span<T> MultiplyMatrices<T>(IMatrix<T> Left, IMatrix<T> Right, TwoElementOperation<T> Multiplier, TwoElementOperation<T> Adder)
            where T : unmanaged, IComparable<T>, IEquatable<T>
        {
            // to multiply two matrices we calc the dot product of the first row of the left with the first column of the right
            // we can either tranpose the whole matrix or use referenced mapped indices

            // try ref mapping
            T[] FinalMatrixArray = new T[Left.Rows * Right.Columns];

            Span<T> FinalMatrixSpan = new(FinalMatrixArray);

            // transpose the matrix to avoid using slow indice mapping
            IMatrix<T> Transposed = Left.Transpose();

            // once the matrix is transposed we can avoid math to mapp rows to columns
            // store the shapes of the matrxes to avoid slowness from using properties that have null checks
            (int Rows, int Columns) LeftShape = (Transposed.Rows, Transposed.Columns);
            (int Rows, int Columns) RightShape = (Right.Rows, Right.Columns);
            for (int RowIndex = 0; RowIndex < LeftShape.Rows; RowIndex++)
            {
                // store both rows directly in contigous memory
                Span<T> LeftSpan = new(Transposed[RowIndex]);
                Span<T> RightSpan = new(Right[RowIndex]);

                int index = 0;
                for (int LeftColumn = 0; LeftColumn < LeftShape.Columns; LeftColumn++)
                {
                    for (int RightColumn = 0; RightColumn < RightShape.Columns; RightColumn++)
                    {
                        // multiply the two values and store them in a tmp var
                        T Multiplied = Multiplier(ref LeftSpan[LeftColumn], ref RightSpan[RightColumn]);

                        // add the value to the final array
                        Adder(ref FinalMatrixSpan[index++], ref Multiplied);
                    }
                }
            }

            return FinalMatrixSpan;
        }
    }
}
