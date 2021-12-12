using DNNElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNNElements
{
    public class DNN : MonoBehaviour
    {
        #region INPUTS
        [SerializeField] float[] input;
        #endregion

        #region PROPERTIES
        public List<Layer> layers = new List<Layer>();
        ColorDataset dataset;
        LayerLayout layout;
        #endregion

        private void Awake()
        {
            /* Generate toy binary network */
            layers.Add(new Layer(2, 2, null, ActivationFunction.SIGMOID));
            layers.Add(new Layer(2, 1, null, ActivationFunction.SIGMOID));

            /* XOR Network */
            input = new float[] { 1.0f, 0.0f };
            var intermediates = DoInference();

            Debug.Log(string.Format("Inference Complete: output={0}", intermediates[intermediates.Count - 1][0]));
            DoBackpropagation(intermediates, new float[] { 1.0f });

        }

        /// <summary>
        /// Compute the inference output, along with intermediate values for display.
        /// </summary>
        /// <returns>A list of float[] containing intermediate outputs.</returns>
        List<float[]> DoInference()
        {
            List<float[]> intermediates = new List<float[]>();
            float[] initial = input;
            intermediates.Add(initial);
            for (int i = 0; i < layers.Count; i++)
            {
                initial = layers[i].ComputeInference(initial);
                intermediates.Add(initial);
            }

            return intermediates;
        }

        List<float[]> DoBackpropagation(List<float[]> intermediate, float[] actualOutput)
        {
            /* Calculate the error for our output */
            float error = CrossEntropy(intermediate[intermediate.Count - 1], actualOutput);

            /* Calculate the back propagation for the output node */
            float outputError = layers[layers.Count - 1].activationFunction.Derivative(intermediate[intermediate.Count - 1][0]) * error;

            /* Return the deltas */
            return null;
        }

        //List<float[]> DoBackpropagation(List<float[]> intermediate, float[] actualOutput)
        //{
        //    /* Step 1: We always have a cross entropy output */
        //    float[] dLdY = new float[actualOutput.Length];
        //    for(int i = 0; i < dLdY.Length; i++)
        //    {
        //        dLdY[i] = -actualOutput[i] / intermediate[intermediate.Count - 1][i];
        //    }
        //    Debug.Log("dLdY: " + string.Join(", ", dLdY));

        //    /* Step 2: Calculate the softmax derivatives */
        //    float[,] dYdG = new float[intermediate[intermediate.Count - 1].Length, intermediate[intermediate.Count - 1].Length];
        //    for(int y = 0; y < dYdG.GetLength(0); y++)
        //    {
        //        for(int g = 0; g < dYdG.GetLength(1); g++)
        //        {
        //            if(y == g)
        //            {
        //                dYdG[y, g] = intermediate[intermediate.Count - 1][y] * (1f - intermediate[intermediate.Count - 1][y]);
        //            }
        //            else
        //            {
        //                dYdG[y, g] = -intermediate[intermediate.Count - 1][y] * intermediate[intermediate.Count - 1][g];
        //            }
        //        }
        //    }
        //    Log2DArray(dYdG);

        //    /* Step 3: Calculate change in layer 2 weights and biases */
        //    float[] dLdW = new float[layers[layers.Count - 1].weights.Length];
        //    for(int output = 0; output < layers[layers.Count - 1].size; output++)
        //    {
        //        for(int input = 0; input < layers[layers.Count - 1].inputSize; input++)
        //        {
        //            for(int y = 0; y < layers[layers.Count - 1].size; y++)
        //            {
        //                /* Each weight is only updated by it's corresponding output */
        //                dLdW[input + output * layers[layers.Count - 1].inputSize] += dLdY[y] * dYdG[y, output] * intermediate[intermediate.Count - 2][input];
        //            }
        //        }
        //    }
        //    Debug.Log("dLdW: " + string.Join(", ", dLdW));

        //    /* Step 4: Calculate change in layer 1 weights and biases */



        //    return null;
        //}

        /// <summary>
        /// Calculate the cross-entropy loss for a given set of probabilities.
        /// </summary>
        /// <param name="probability">The output probabilities.</param>
        /// <param name="truth">The true values.</param>
        /// <returns>The cross entropy loss between the two vectors.</returns>
        float CrossEntropy(float[] probability, float[] truth)
        {
            float total = 0.0f;
            for (int i = 0; i < probability.Length; i++)
            {
                if (truth[i] != 0.0f)
                {
                    total += truth[i] * Mathf.Log(probability[i]);
                }
            }

            return -1 * total;
        }

        void Log2DArray(float[,] array)
        {
            Debug.Log("[");
            for (int x = 0; x < array.GetLength(0); x++)
            {
                string line = "[";
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    line += array[x, y] + ", ";
                }
                Debug.Log(line + "]");
            }
            Debug.Log("]");
        }
    }
}

