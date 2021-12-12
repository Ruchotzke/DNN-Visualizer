using System.Collections.Generic;
using UnityEngine;

namespace DNN
{

    /// <summary>
    /// A simple container for a neural network.
    /// Only supports binary classification / probability output.
    /// </summary>
    public class DNN : MonoBehaviour
    {
        List<Layer> layers = new List<Layer>();
        float[] input;
        Dataset dataset;
        float learningRate = 0.1f;

        [SerializeField] LineRenderer lr_loss;
        [SerializeField] LineRenderer lr_acc;

        private void Awake()
        {
            /* Seed the random for testing */

            /* Get the dataset */
            dataset = Dataset.Instance;

            /* Create a model */
            layers.Add(new Layer(8, 12, ActivationFunction.SIGMOID));
            layers.Add(new Layer(12, 120, ActivationFunction.SIGMOID));
            layers.Add(new Layer(120, 8, ActivationFunction.SIGMOID));
            layers.Add(new Layer(8, 2, ActivationFunction.SIGMOID));
            layers.Add(new Layer(2, 1, ActivationFunction.SIGMOID));

            ///* TEST: Array inputs */
            //input = new float[8]{ 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};
            //Debug.Log(DoInference());
            //input = new float[8] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
            //Debug.Log(DoInference());


            /* Compute an inference */
            List<float> lossOverTime = new List<float>();
            List<float> accuracyOverTime = new List<float>();
            for (int i = 0; i < 40; i++)
            {
                int TEST_INPUTS = 6;
                /* Do training on all inputs */
                List<float> outputs = new List<float>();
                List<float> actual = new List<float>();
                for (int inputIndex = 0; inputIndex < TEST_INPUTS; inputIndex++)
                {
                    input = dataset.Values[inputIndex].input;
                    float inference = DoInference();
                    outputs.Add(inference);
                    actual.Add(dataset.Values[inputIndex].output);
                    DoBackpropagation(outputs, actual);
                    outputs.Clear();
                    actual.Clear();
                }

                /* Compute output values */
                float loss = 0.0f;
                int correct = 0;
                for (int testIndex = 0; testIndex < TEST_INPUTS; testIndex++)
                {
                    input = dataset.Values[testIndex].input;
                    float inference = DoInference();
                    Debug.Log("Input: [" + string.Join(", ", input) + "]    Output: " + inference);
                    //Debug.Log(testIndex + "   " + Loss(inference, dataset.Values[testIndex].output));
                    loss += Loss(inference, dataset.Values[testIndex].output);
                    if (Mathf.Round(inference) == dataset.Values[testIndex].output) correct += 1;
                }
                loss /= TEST_INPUTS;

                lossOverTime.Add(loss);
                accuracyOverTime.Add((float)correct / (float)TEST_INPUTS);
                Debug.Log("Accuracy: " + (float)correct / (float)TEST_INPUTS);
            }

            /* display the loss and accuracy over time */
            Vector3[] points = new Vector3[accuracyOverTime.Count];
            for (int i = 0; i < accuracyOverTime.Count; i++)
            {
                points[i] = new Vector3(Mathf.Lerp(-8.0f, 8.0f, i / (float)(accuracyOverTime.Count - 1)), 2.0f * accuracyOverTime[i], 0.0f);
            }
            lr_acc.positionCount = points.Length;
            lr_acc.SetPositions(points);

            points = new Vector3[lossOverTime.Count];
            for (int i = 0; i < lossOverTime.Count; i++)
            {
                points[i] = new Vector3(Mathf.Lerp(-8.0f, 8.0f, i / (float)(lossOverTime.Count - 1)), 2.0f * lossOverTime[i], 0.0f);
            }
            lr_loss.positionCount = points.Length;
            lr_loss.SetPositions(points);

        }

        public float DoInference()
        {
            /* Move forward through layers */
            float[] intermediate = input;
            //Debug.Log("[" + string.Join(", ", intermediate) + "]");
            for(int i = 0; i < layers.Count - 1; i++)
            {
                intermediate.CopyTo(layers[i].LayerInput, 0);
                intermediate = layers[i].Inference();
                //Debug.Log("[" + string.Join(", ", intermediate) + "]");
            }

            /* Return the final layer's output */
            intermediate.CopyTo(layers[layers.Count - 1].LayerInput, 0);
            //Debug.Log("[" + string.Join(", ", layers[layers.Count - 1].Inference()) + "]");
            return layers[layers.Count - 1].Inference()[0];
        }

        public void DoBackpropagation(List<float> output, List<float> actual)
        {
            /* Start with the complete output */
            float outputError = layers[layers.Count - 1].Neurons[0].activationFunction.Derivative(layers[layers.Count - 1].Neurons[0].Output) * LossDerivative(output, actual);
            layers[layers.Count - 1].Neurons[0].UpdateParameters(outputError, learningRate);

            /* Update the hidden layers */
            /* For each layer moving backwards, calculate error */
            /* We are always fully connected, meaning we can sum the errors of a layer to get its effect on the next layer */
            float[] previousError = new float[1] { outputError };
            for (int layer = layers.Count - 2; layer >= 0; layer--)
            {
                float[] currError = new float[layers[layer].Neurons.Length];
                /* For each neuron in the layer, compute a local output error */
                for (int neuronidx = 0; neuronidx < layers[layer].Neurons.Length; neuronidx++)
                {
                    /* Calculate the error */
                    Neuron neuron = layers[layer].Neurons[neuronidx];
                    float neuronError = 0.0f;
                    float activationDerivative = neuron.activationFunction.Derivative(neuron.Output);
                    for(int weight = 0; weight < layers[layer + 1].Neurons.Length; weight++)
                    {
                        neuronError += activationDerivative * layers[layer + 1].Neurons[weight].Weights[neuronidx] * previousError[weight];
                    }

                    /* Apply the error */
                    neuron.UpdateParameters(neuronError, learningRate);
                    currError[neuronidx] = neuronError;
                }

                /* update the outer error variable now that this layer is done */
                previousError = currError;
            }
            
        }

        public float Loss(float output, float actual)
        {
            return -1 * (actual * Mathf.Log(output) + (1-actual)*Mathf.Log(1 - output));
        }

        public float LossDerivative(float output, float actual)
        {
            return -1.0f * (actual / output - (1 - actual) / (1 - output));
        }

        public float LossDerivative(List<float> outputs, List<float> actual)
        {
            float error = 0.0f;
            for(int i = 0; i < outputs.Count; i++)
            {
                error += (actual[i] / outputs[i] - (1 - actual[i]) / (1 - outputs[i]));
            }

            return -1.0f * error / outputs.Count;
        }
    }
}
