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
        public float[] bias;       /* Each "neuron" has its own bias */
        public ActivationFunction activationFunction;

        public Layer(int inputSize, int size, float[] bias = null, ActivationFunction activationFunction = ActivationFunction.RELU)
        {
            /* Default Applications */
            this.inputSize = inputSize;
            this.size = size;
            this.activationFunction = activationFunction;
            if(bias == null)
            {
                this.bias = new float[size];
            }
            else
            {
                this.bias = bias;
            }

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
                output[outputIndex] = bias[outputIndex];
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
            else if(activationFunction == ActivationFunction.SIGMOID)
            {
                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = 1.0f / (1.0f + Mathf.Exp(-1.0f * output[i]));
                }
            }

            return output;
        }
    }

    [System.Serializable]
    public enum ActivationFunction
    {
        RELU,
        SOFTMAX,
        SIGMOID
    }

    public static class ActivationFunctionExtensions
    {
        public static float Derivative(this ActivationFunction func, float input)
        {
            switch (func)
            {
                case ActivationFunction.RELU:
                    return 0.0f;
                case ActivationFunction.SOFTMAX:
                    return 0.0f;
                case ActivationFunction.SIGMOID:
                    return input * (1 - input);
                default:
                    return 0.0f;
            }
        }
    }

}
