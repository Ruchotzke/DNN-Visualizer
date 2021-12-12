using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DNN_V2
{
    [System.Serializable]
    public enum ActivationFunction
    {
        SIGMOID,
        RELU,
        SOFTMAX
    }

    public static class ActivationFunctionExtensions
    {
        /// <summary>
        /// Calculate the output of this activation function.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Evaluate(this ActivationFunction func, float value)
        {
            switch (func)
            {
                case ActivationFunction.SIGMOID:
                    return 1.0f / (1.0f + Mathf.Exp(-value));
                case ActivationFunction.RELU:
                    return Mathf.Max(0.0f, value);
                case ActivationFunction.SOFTMAX:
                    Debug.LogError("CANT EVALUATE SOFTMAX ON A SINGLE OUTPUT");
                    return float.NaN; 
                default:
                    return 0.0f;
            }
        }

        public static float[] EvaluateMultiple(this ActivationFunction func, float[] values)
        {
            if (func != ActivationFunction.SOFTMAX) return null;

            float[] ret = new float[values.Length];
            float sum = 0.0f;
            for (int i = 0; i < values.Length; i++)
            {
                float exp = Mathf.Exp(values[i]);
                ret[i] = exp;
                sum += exp;
            }

            for(int i = 0; i < values.Length; i++)
            {
                ret[i] /= sum;
            }

            return ret;
        }

        /// <summary>
        /// Calculate the value of the derivative of this activation function.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Derivative(this ActivationFunction func, float value)
        {
            switch (func)
            {
                case ActivationFunction.SIGMOID:
                    return value * (1.0f - value);
                default:
                    return 0.0f;
            }
        }
    }
}
