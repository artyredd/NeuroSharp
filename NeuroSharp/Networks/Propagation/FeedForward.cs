using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Propagation
{
    public class FeedForward<T> : IPropogator<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Propogates wights to the right node. Adds the biases to the right node. Then executes the activation function on the right node member-wise
        /// <code>
        /// Complexity: ~ O(n³) Exact: O(n³) + O(2nm)
        /// </code>
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="Right"></param>
        /// <param name="Activator"></param>
        /// <returns></returns>
        public IMatrix<T> Forward(INode<T> Node, IMatrix<T> Matrix, IActivationFunction<T> Activator)
        {
            var result = Node.Weights * Matrix;

            result += Node.Biases;

            result.PerformMemberWise((ref T val) => val = Activator.Function(ref val));

            return result;
        }

        /// <summary>
        /// Propogates wights to the right node. Adds the biases to the right node. Then executes the activation function on the right node member-wise
        /// <code>
        /// Complexity: ~ O(n³) Exact: O(n³) + O(2nm)
        /// </code>
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="Right"></param>
        /// <param name="Activator"></param>
        /// <returns></returns>
        public IMatrix<T> Forward(IMatrix<T> Node, IMatrix<T> Matrix, IActivationFunction<T> Activator)
        {
            var result = Node * Matrix;

            result.PerformMemberWise((ref T val) => val = Activator.Function(ref val));

            return result;
        }

        public (IMatrix<T> WeightDelta, IMatrix<T> BiasDelta) Backward(IMatrix<T> Output, IMatrix<T> Error, IMatrix<T> PreviousLayerOutput, double TrainingRate, IActivationFunction<T> Activator)
        {
            IMatrix<T> OutputGradient = MatrixOperations<T>.PerformNonMutableOperation(Output, Activator.Derivative);

            OutputGradient %= Error;

            OutputGradient *= TrainingRate;

            IMatrix<T> BiasDelta = OutputGradient.Duplicate();

            OutputGradient *= PreviousLayerOutput.Transpose();

            IMatrix<T> WeightDelta = OutputGradient;

            return (WeightDelta, BiasDelta);
        }
    }
}
