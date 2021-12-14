using System.Collections.Generic;
using UnityEngine;

namespace DNN_V2
{

    /// <summary>
    /// A simple container for a neural network.
    /// Only supports binary classification / probability output.
    /// </summary>
    public class DNN : MonoBehaviour
    {
        public List<Layer> layers = new List<Layer>();
        public float[] input;
        Dataset dataset;
        public float learningRate = 0.1f;

        [SerializeField] LineRenderer lr_loss;
        [SerializeField] LineRenderer lr_acc;

        private void Awake()
        {
            ///* Seed the random for testing */

            ///* Get the dataset */
            //dataset = Dataset.Instance;

            ///* Create a model */
            //layers.Add(new Layer(8, 12, ActivationFunction.SIGMOID));
            //layers.Add(new Layer(12, 120, ActivationFunction.SIGMOID));
            //layers.Add(new Layer(120, 8, ActivationFunction.SIGMOID));
            //layers.Add(new Layer(8, 2, ActivationFunction.SIGMOID));

            /////* TEST: Array inputs */

            //const int SET_SIZE = 10;
            //List<float> lossOverTime = new List<float>();
            //List<float> accuracyOverTime = new List<float>();
            //for (int epoch = 0; epoch < 100; epoch++)
            //{
            //    /* Perform inferences and backprop */
            //    for (int i = 0; i < SET_SIZE; i++)
            //    {
            //        input = dataset.Values[i].input;
            //        float[] inference = DoInference();
            //        DoBackpropagation(inference, dataset.Values[i].output);
            //    }

            //    /* Profile this round of training */
            //    float loss = 0.0f;
            //    float acc = (float)SET_SIZE;
            //    for (int i = 0; i < SET_SIZE - 1; i++)
            //    {
            //        input = dataset.Values[i].input;
            //        float[] inference = DoInference();
            //        loss += CrossEntropyLoss(inference, dataset.Values[i].output);

            //        for (int j = 0; j < inference.Length; j++)
            //        {
            //            if (inference[j] != dataset.Values[i].output[j])
            //            {
            //                acc -= 1.0f;
            //                break;
            //            }
            //        }
            //    }
            //    loss /= SET_SIZE;
            //    acc /= SET_SIZE;
            //    lossOverTime.Add(loss);
            //    accuracyOverTime.Add(acc);
            //    Debug.Log("Epoch " + epoch + " complete. Loss: " + loss + " Accuracy: " + acc);

            //}

            ///* display the loss and accuracy over time */
            //Vector3[] points = new Vector3[accuracyOverTime.Count];
            //for (int i = 0; i < accuracyOverTime.Count; i++)
            //{
            //    points[i] = new Vector3(Mathf.Lerp(-8.0f, 8.0f, i / (float)(accuracyOverTime.Count - 1)), 2.0f * accuracyOverTime[i], 0.0f);
            //}
            //lr_acc.positionCount = points.Length;
            //lr_acc.SetPositions(points);

            //points = new Vector3[lossOverTime.Count];
            //for (int i = 0; i < lossOverTime.Count; i++)
            //{
            //    points[i] = new Vector3(Mathf.Lerp(-8.0f, 8.0f, i / (float)(lossOverTime.Count - 1)), 2.0f * lossOverTime[i], 0.0f);
            //}
            //lr_loss.positionCount = points.Length;
            //lr_loss.SetPositions(points);

        }

        public float[] DoInference()
        {
            /* Move forward through layers */
            float[] intermediate = layers[0].Inference(); //should be an input layer

            //Debug.Log("[" + string.Join(", ", intermediate) + "]");
            for(int i = 1; i < layers.Count - 1; i++)
            {
                intermediate.CopyTo(layers[i].LayerInput, 0);
                intermediate = layers[i].Inference();
                //Debug.Log("[" + string.Join(", ", intermediate) + "]");
            }

            /* Return the final layer's output */
            intermediate.CopyTo(layers[layers.Count - 1].LayerInput, 0);
            //Debug.Log("[" + string.Join(", ", layers[layers.Count - 1].Inference()) + "]");
            return layers[layers.Count - 1].Inference();
        }

        public void DoBackpropagation(float[] output, float[] actual)
        {
            /* Start with the complete output */
            float[] outputError = new float[output.Length];
            for (int i = 0; i < output.Length; i++)
            {
                outputError[i] = layers[layers.Count - 1].Neurons[i].activationFunction.Derivative(layers[layers.Count - 1].Neurons[i].Output) * CrossEntropyLossDerivative(output[i], actual[i]);
                layers[layers.Count - 1].Neurons[i].UpdateParameters(outputError[i], learningRate);
            }
            

            /* Update the hidden layers */
            /* For each layer moving backwards, calculate error */
            /* We are always fully connected, meaning we can sum the errors of a layer to get its effect on the next layer */
            float[] previousError = outputError;
            for (int layer = layers.Count - 2; layer > 0; layer--) //> 0, we don't want to impact the input layer
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

        public float CrossEntropyLoss(float[] output, float[] actual)
        {
            float total = 0.0f;
            for (int i = 0; i < output.Length; i++)
            {
                if (actual[i] != 0.0f)
                {
                    total += actual[i] * Mathf.Log(output[i]);
                }
            }

            return -1 * total;
        }

        public float CrossEntropyLossDerivative(float output, float actual)
        {
            return -1f * actual / output;
        }

        public float LossDerivative(float output, float actual)
        {
            return -1.0f * (actual / output - (1 - actual) / (1 - output));
        }

        public float[] LossDerivative(float[] output, float[] actual)
        {
            float[] losses = new float[output.Length];
            for(int i = 0; i < output.Length; i++)
            {
                losses[i] = LossDerivative(output[i], actual[i]);
            }

            return losses;
        }

        public float BatchedLossDerivative(List<float> outputs, List<float> actual)
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
