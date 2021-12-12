using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DNN
{
    public enum ActivationFunction
    {
        SIGMOID
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
                default:
                    return 0.0f;
            }
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
