using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    public static class MatrixOperations<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        public delegate T TwoElementOperation<U>(ref U left, ref U right);
        public delegate T SingleElementOperation<U>(ref U MatrixElement);

        public static IMatrix<T> PerformMutableOperation(IMatrix<T> matrix, SingleElementOperation<T> Operation)
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

        public static IMatrix<T> PerformTwoSidedMutableOperation(IMatrix<T> Left, IMatrix<T> Right, TwoElementOperation<T> Operation)
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

        public static Span<T> MultiplyMatrices(IMatrix<T> Left, IMatrix<T> Right, TwoElementOperation<T> Multiplier, TwoElementOperation<T> Adder)
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

        public static IMatrix<T> PerformNonMutableOperation(IMatrix<T> matrix, SingleElementOperation<T> Operation)
        {
            // duplicate the matrix to verify that we are not mutating the original matrix
            IMatrix<T> Dupe = matrix.Duplicate();

            // perform the possible mytating operation on all items int he matrix
            PerformMutableOperation(Dupe, Operation);

            // return the duplicated matrix and not the original
            return Dupe;
        }

        public static IMatrix<T> PerformTwoSidedNonMutableOperation(IMatrix<T> Left, IMatrix<T> Right, TwoElementOperation<T> Operation)
        {
            // duplicate the matrix to verify that we are not mutating the original matrix
            IMatrix<T> Dupe = Left.Duplicate();

            // perform the possible mytating operation on all items int he matrix
            PerformTwoSidedMutableOperation(Dupe, Right, Operation);

            // return the duplicated matrix and not the original
            return Dupe;
        }

        public static class Operators
        {
            public static IMatrix<T> Multiply(sbyte Value, IMatrix<T> Right)
            {
                if (Right.SByteOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.SByteOperations.ReferenceMultiplier(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Multiply), typeof(sbyte).Name, typeof(T).Name);
            }
            public static IMatrix<T> Multiply(byte Value, IMatrix<T> Right)
            {
                if (Right.ByteOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.ByteOperations.ReferenceMultiplier(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Multiply), typeof(byte).Name, typeof(T).Name);
            }
            public static IMatrix<T> Multiply(short Value, IMatrix<T> Right)
            {
                if (Right.ShortOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.ShortOperations.ReferenceMultiplier(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Multiply), typeof(short).Name, typeof(T).Name);
            }
            public static IMatrix<T> Multiply(int Value, IMatrix<T> Right)
            {
                if (Right.IntegerOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.IntegerOperations.ReferenceMultiplier(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Multiply), typeof(int).Name, typeof(T).Name);
            }
            public static IMatrix<T> Multiply(long Value, IMatrix<T> Right)
            {
                if (Right.LongOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.LongOperations.ReferenceMultiplier(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Multiply), typeof(long).Name, typeof(T).Name);
            }
            public static IMatrix<T> Multiply(double Value, IMatrix<T> Right)
            {
                if (Right.DoubleOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.DoubleOperations.ReferenceMultiplier(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Multiply), typeof(double).Name, typeof(T).Name);
            }
            public static IMatrix<T> Multiply(float Value, IMatrix<T> Right)
            {
                if (Right.FloatOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.FloatOperations.ReferenceMultiplier(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Multiply), typeof(float).Name, typeof(T).Name);
            }
            public static IMatrix<T> Multiply(decimal Value, IMatrix<T> Right)
            {
                if (Right.DecimalOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.DecimalOperations.ReferenceMultiplier(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Multiply), typeof(decimal).Name, typeof(T).Name);
            }

            public static IMatrix<T> Subtract(sbyte Value, IMatrix<T> Right)
            {
                if (Right.SByteOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.SByteOperations.ReferenceSubtractor(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Subtract), typeof(sbyte).Name, typeof(T).Name);
            }
            public static IMatrix<T> Subtract(byte Value, IMatrix<T> Right)
            {
                if (Right.ByteOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.ByteOperations.ReferenceSubtractor(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Subtract), typeof(byte).Name, typeof(T).Name);
            }
            public static IMatrix<T> Subtract(short Value, IMatrix<T> Right)
            {
                if (Right.ShortOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.ShortOperations.ReferenceSubtractor(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Subtract), typeof(short).Name, typeof(T).Name);
            }
            public static IMatrix<T> Subtract(int Value, IMatrix<T> Right)
            {
                if (Right.IntegerOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.IntegerOperations.ReferenceSubtractor(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Subtract), typeof(int).Name, typeof(T).Name);
            }
            public static IMatrix<T> Subtract(long Value, IMatrix<T> Right)
            {
                if (Right.LongOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.LongOperations.ReferenceSubtractor(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Subtract), typeof(long).Name, typeof(T).Name);
            }
            public static IMatrix<T> Subtract(double Value, IMatrix<T> Right)
            {
                if (Right.DoubleOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.DoubleOperations.ReferenceSubtractor(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Subtract), typeof(double).Name, typeof(T).Name);
            }
            public static IMatrix<T> Subtract(float Value, IMatrix<T> Right)
            {
                if (Right.FloatOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.FloatOperations.ReferenceSubtractor(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Subtract), typeof(float).Name, typeof(T).Name);
            }
            public static IMatrix<T> Subtract(decimal Value, IMatrix<T> Right)
            {
                if (Right.DecimalOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.DecimalOperations.ReferenceSubtractor(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Subtract), typeof(decimal).Name, typeof(T).Name);
            }

            public static IMatrix<T> Add(sbyte Value, IMatrix<T> Right)
            {
                if (Right.SByteOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.SByteOperations.ReferenceAdder(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Add), typeof(sbyte).Name, typeof(T).Name);
            }
            public static IMatrix<T> Add(byte Value, IMatrix<T> Right)
            {
                if (Right.ByteOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.ByteOperations.ReferenceAdder(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Add), typeof(byte).Name, typeof(T).Name);
            }
            public static IMatrix<T> Add(short Value, IMatrix<T> Right)
            {
                if (Right.ShortOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.ShortOperations.ReferenceAdder(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Add), typeof(short).Name, typeof(T).Name);
            }
            public static IMatrix<T> Add(int Value, IMatrix<T> Right)
            {
                if (Right.IntegerOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.IntegerOperations.ReferenceAdder(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Add), typeof(int).Name, typeof(T).Name);
            }
            public static IMatrix<T> Add(long Value, IMatrix<T> Right)
            {
                if (Right.LongOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.LongOperations.ReferenceAdder(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Add), typeof(long).Name, typeof(T).Name);
            }
            public static IMatrix<T> Add(double Value, IMatrix<T> Right)
            {
                if (Right.DoubleOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.DoubleOperations.ReferenceAdder(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Add), typeof(double).Name, typeof(T).Name);
            }
            public static IMatrix<T> Add(float Value, IMatrix<T> Right)
            {
                if (Right.FloatOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.FloatOperations.ReferenceAdder(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Add), typeof(float).Name, typeof(T).Name);
            }
            public static IMatrix<T> Add(decimal Value, IMatrix<T> Right)
            {
                if (Right.DecimalOperations != null)
                {
                    return PerformNonMutableOperation(Right, Right.DecimalOperations.ReferenceAdder(Value));
                }
                throw Exceptions.InvalidMatrix_Operation(nameof(Add), typeof(decimal).Name, typeof(T).Name);
            }

            public static IMatrix<T> Add(IMatrix<T> Left, IMatrix<T> Right)
            {
                // if both matrices are the same shape we can perform member-wise addition and subtraction
                if (Left.Columns == Right.Columns && Left.Rows == Right.Rows)
                {
                    IMatrix<T> Dupe = Left.Duplicate();
                    PerformTwoSidedMutableOperation(Dupe, Right, Dupe.SameTypedOperations.TwoRefenceAdder);
                    return Dupe;
                }
                // since the two matrices are different sizes we must perform matrix addition
                throw Exceptions.UnexpectedMatrixShape<T>(Left, Right, $"Expected congruent matrices(same rows and columns)", $"Only congruent matrices can be added memberwise.");
            }
            public static IMatrix<T> Subtract(IMatrix<T> Left, IMatrix<T> Right)
            {
                // if both matrices are the same shape we can perform member-wise addition and subtraction
                if (Left.Columns == Right.Columns && Left.Rows == Right.Rows)
                {
                    IMatrix<T> Dupe = Left.Duplicate();
                    PerformTwoSidedMutableOperation(Dupe, Right, Dupe.SameTypedOperations.TwoRefenceSubtractor);
                    return Dupe;
                }
                // since the two matrices are different sizes we must perform matrix addition
                throw Exceptions.UnexpectedMatrixShape<T>(Left, Right, $"Congruent(same rows and columns)", $"Only congruent matrices can be added element wise using the + operator.");
            }
            public static IMatrix<T> Multiply(IMatrix<T> Left, IMatrix<T> Right)
            {
                if (Left.Columns == Right.Rows)
                {
                    //verify that the two matrices are able to be multiplied
                    // two matracies can only be multiplied if the number of columns of the first matrix is the same as the number of rows in the second matrix
                    Span<T> Result = MultiplyMatrices(Left, Right, Left.SameTypedOperations.TwoValueMultiplier, Left.SameTypedOperations.TwoRefenceAdder);

                    // multiplied matrices are always squareish
                    return new BaseMatrix<T>(Left.Rows, Right.Columns, Result, false)
                    {
                        SByteOperations = Left.SByteOperations,
                        ByteOperations = Left.ByteOperations,
                        ShortOperations = Left.ShortOperations,
                        IntegerOperations = Left.IntegerOperations,
                        LongOperations = Left.LongOperations,
                        FloatOperations = Left.FloatOperations,
                        DoubleOperations = Left.DoubleOperations,
                        DecimalOperations = Left.DecimalOperations,
                        SameTypedOperations = Left.SameTypedOperations,
                    };
                }
                throw Exceptions.IncompatibleMatrixShapeForMultiplication(Left, Right);
            }
            public static IMatrix<T> Modulo(IMatrix<T> Left, IMatrix<T> Right)
            {
                if (Left.Columns == Right.Columns && Left.Rows == Right.Rows)
                {
                    IMatrix<T> clone = Left.Duplicate();

                    PerformTwoSidedMutableOperation(clone, Right, clone.SameTypedOperations.TwoReferenceMultiplier);

                    return clone;
                }
                throw Exceptions.UnexpectedMatrixShape<T>(Left, Right, $"Congruent(same rows and columns)", "You can only perform member-wise multiplication of two matrices if they are the same shape and size(congruent).");
            }
        }
    }
}
