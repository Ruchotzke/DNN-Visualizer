using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DNNElements
{

    /// <summary>
    /// An implementation of a layer in "linked list" style.
    /// </summary>
    [System.Serializable]
    public class Layer
    {
        /* Layer Properties */
        public int inputSize;
        public int size;
        public float[] weights;    /* Weights are stored in output major order - i.e. index = outputIndex * inputSize + weightIndex */
        public float bias;
        public ActivationFunction activationFunction;

        public Layer(int inputSize, int size, float bias = 0.0f, ActivationFunction activationFunction = ActivationFunction.RELU)
        {
            /* Default Applications */
            this.inputSize = inputSize;
            this.size = size;
            this.bias = bias;
            this.activationFunction = activationFunction;

            /* Randomly generate weights to initialize them */
            weights = new float[inputSize * size];
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Random.value;
            }
        }

        public float[] ComputeInference(float[] input)
        {
            /* Compute a MAC for each output element (output stationary) */
            float[] output = new float[size];
            for (int outputIndex = 0; outputIndex < output.Length; outputIndex++)
            {
                /* MAC */
                output[outputIndex] = bias;
                for (int weightIndex = 0; weightIndex < inputSize; weightIndex++)
                {
                    output[outputIndex] += weights[outputIndex * inputSize + weightIndex] * input[weightIndex];
                }
            }

            /* Apply the activation function */
            if(activationFunction == ActivationFunction.RELU)
            {
                for (int i = 0; i < output.Length; i++)
                {
                    if (output[i] < 0) output[i] = 0;
                }
            }
            else if(activationFunction == ActivationFunction.SOFTMAX)
            {
                /* Compute numerator terms and sum */
                float sum = 0.0f;
                for (int i = 0;i < output.Length; i++)
                {
                    float exp = Mathf.Exp(output[i]);
                    sum += exp;
                    output[i] = exp;
                }

                /* Divide by the sum */
                for (int i = 0; i < output.Length; i++)
                {
                    output[i] /= sum;
                }
            }

            return output;
        }
    }

    [System.Serializable]
    public enum ActivationFunction
    {
        RELU,
        SOFTMAX
    }

}
