using NeuroSharp.Propagation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    public class NeuralNetwork
    {
        /// <summary>
        /// Name of the network.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The nodes that process and prepare input data for the hidden nodes.
        /// </summary>
        public INode<double> InputNodes { get; set; }

        /// <summary>
        /// The nodes that prepare data for the output nodes to make a decision.
        /// </summary>
        public INode<double>[] HiddenNodes { get; set; }

        /// <summary>
        /// The activation function that should be used to train this network. Changing this after training has begun will drastically change results.
        /// </summary>
        public IActivationFunction<double> Activator { get; set; } = new Activation.Sigmoid();

        /// <summary>
        /// The speed that this network should train at. Range [ 0d - 1d ] where 1d is maximum speed.
        /// </summary>
        public double TrainingRate { get; set; } = 1.0d;

        /// <summary>
        /// The <see cref="IPropogator{T}"/> that should be used to propogate forward and backward.
        /// </summary>
        public IPropogator<double> Propogator { get; set; } = new FeedForward<double>();

        private readonly Random rng = new();

        public NeuralNetwork() { }

        public NeuralNetwork(int Inputs, int Outputs, params int[] HiddenLayers)
        {
            INode<double> CreateNode(int Rows, int Columns)
            {
                return new Node<double>()
                {
                    Weights = new Double.Matrix(Rows, Columns, Helpers.NextDouble),
                    Biases = new Double.Matrix(Rows, 1, Helpers.NextDouble)
                };
            }
            if (HiddenLayers?.Length is null or 0)
            {
                /*
                    Since no hidden layers were specified hook directly to outputs
                    inputs -> o -> o -> Output
                */
                InputNodes = CreateNode(Outputs, Inputs);
            }
            else
            {
                /*
                    Since hidden layers were specified create and order them correctly
                    inputs -> o -> o ... -> o -> output
                */
                InputNodes = CreateNode(HiddenLayers[0], Inputs);

                HiddenNodes = new INode<double>[HiddenLayers.Length];

                for (int i = 0; i < HiddenLayers.Length; i++)
                {
                    // these matrices could be potentially gigantic, avoid using span here until benchmarks can be performed to verify the viability of contiguous blocks of memory
                    int nextLayer = i + 1;
                    if (nextLayer >= HiddenLayers.Length)
                    {
                        HiddenNodes[i] = CreateNode(Outputs, HiddenLayers[i]);
                        break;
                    }
                    HiddenNodes[i] = CreateNode(HiddenLayers[nextLayer], HiddenLayers[i]);
                }
            }
        }

        public IMatrix<double> TrainingInput(IEnumerable<double> Inputs, IEnumerable<double> Expected)
        {
            // convert incoming enumerable data such as arrays etc.. to matrices
            IMatrix<double> inputs = new Double.Matrix(InputNodes.Weights.Columns, 1, Inputs);

            IMatrix<double> expected = new Double.Matrix(Expected.Count(), 1, Expected);

            return TrainingInput(inputs, expected);
        }

        public IMatrix<double> TrainingInput(IMatrix<double> Inputs, IMatrix<double> Expected)
        {
            // make sure training data matches sceheme
            if (Expected.Rows != HiddenNodes[^1].Weights.Rows)
            {
                throw Networks.Exceptions.InconsistentTrainingDataScheme(HiddenNodes[^1].Weights.Rows, Expected.Rows);
            }
            if (Inputs.Rows != InputNodes.Weights.Rows)
            {
                throw Networks.Exceptions.InconsistentTrainingDataScheme(InputNodes.Weights.Rows, Inputs.Rows);
            }

            IMatrix<double> hidden = Propogator.PropogateForward(InputNodes, Inputs, Activator);

            IMatrix<double> outputs = Propogator.PropogateForward(HiddenNodes[0], hidden, Activator);

            // total error for final output
            IMatrix<double> OutputErrors = Expected - outputs;

            IMatrix<double> OutputGradient = MatrixOperations<double>.PerformNonMutableOperation(outputs, Activator.Derivative);

            OutputGradient %= OutputErrors;

            OutputGradient *= TrainingRate;

            HiddenNodes[0].Biases += OutputGradient;

            OutputGradient *= hidden.Transpose();

            HiddenNodes[0].Weights += OutputGradient;

            IMatrix<double> InputGradient = MatrixOperations<double>.PerformNonMutableOperation(hidden, Activator.Derivative);

            IMatrix<double> HiddenErrors = (HiddenNodes[0].Weights.Transpose() * OutputErrors).Duplicate();

            InputGradient %= HiddenErrors;

            InputGradient *= TrainingRate;

            InputNodes.Biases += InputGradient;

            InputGradient *= Inputs.Transpose();

            InputNodes.Weights += InputGradient;

            return default;
        }

        public IMatrix<double> CheckInput(IEnumerable<double> Inputs)
        {
            IMatrix<double> inputs = new Double.Matrix(InputNodes.Weights.Columns, 1, Inputs);
            return CheckInput(inputs);
        }

        public IMatrix<double> CheckInput(IMatrix<double> Inputs)
        {
            IMatrix<double> propogated = Propogator.PropogateForward(InputNodes, Inputs, Activator);

            for (int i = 0; i < HiddenNodes.Length; i++)
            {
                propogated = Propogator.PropogateForward(HiddenNodes[i], propogated, Activator);
            }

            return propogated;
        }
    }
}
