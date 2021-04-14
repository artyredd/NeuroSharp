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

        public static BaseMatrix<T> PerformOperationMemberwise<T>(BaseMatrix<T> matrix, SingleElementOperation<T> Operation) where T : unmanaged, IComparable<T>, IEquatable<T>
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

        public static BaseMatrix<T> PerformTwoSidedOperationMemberwise<T>(BaseMatrix<T> Left, BaseMatrix<T> Right, TwoElementOperation<T> Operation)
            where T : unmanaged, IComparable<T>, IEquatable<T>
        {
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
        public static Span<T> MultiplyMatrices<T>(BaseMatrix<T> Left, BaseMatrix<T> Right, TwoElementOperation<T> Multiplier, TwoElementOperation<T> Adder)
            where T : unmanaged, IComparable<T>, IEquatable<T>
        {
            // to multiply two matrices we calc the dot product of the first row of the left with the first column of the right
            // we can either tranpose the whole matrix or use referenced mapped indices

            // try ref mapping
            T[] FinalMatrixArray = new T[Left.Rows * Right.Columns];

            Span<T> FinalMatrixSpan = new(FinalMatrixArray);

            int currentIndex = 0;

            for (int LeftRow = 0; LeftRow < Left.Rows; LeftRow++)
            {
                // create a block of memory for the row
                Span<T> Row = new(Left[LeftRow]);

                // becuase the transposed matrix is has mapped idicies this is called right row even though we are multiplying the column
                // also note that the transposed values are, again, mapped and are not actually rows therefor to avoid copying of arrays we reference them using
                // index mapping to a 'tranposed shape' and we can't create a contiguous block of memory for them since they can't be added to a single array
                for (int RightRow = 0; RightRow < Right.Columns; RightRow++)
                {
                    // this is the value of the result of multiplying all values and adding them together
                    T RowValue = default;

                    for (int LeftColumn = 0; LeftColumn < Left.Columns; LeftColumn++)
                    {
                        // since c# doesn't let us constrain T to value types we have to assign the operation from the top-level object that knows what types these are since we can't use the _Multiply(*) operator on multi-typed generics
                        T multiplied = Multiplier(ref Row[LeftColumn], ref Right.Transposed[RightRow, LeftColumn]);

                        // add the result of the product of the two matrix elements to the sum of the row
                        Adder(ref RowValue, ref multiplied);
                    }

                    // save the sum into the array
                    FinalMatrixSpan[currentIndex++] = RowValue;
                }
            }

            return FinalMatrixSpan;
        }
    }
}
