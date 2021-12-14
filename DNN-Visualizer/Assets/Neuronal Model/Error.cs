using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace neuronal
{
    public class Error
    {
        /// <summary>
        /// Compute the mean squared error of the inputs.
        /// </summary>
        /// <param name="predicted">The predicted value.</param>
        /// <param name="expected">The actual value.</param>
        /// <returns>A positive float representing the error.</returns>
        public static float MSE(float predicted, float expected)
        {
            return (predicted - expected) * (predicted - expected);
        }

        /// <summary>
        /// Compute the derivative of a MSE loss function.
        /// **NOTE: My derivative may be incorrect. Use with caution***
        /// </summary>
        /// <param name="predicted">The predicted value.</param>
        /// <param name="expected">The actual value.</param>
        /// <returns>The derivative of MSE with respect to the predicted value.</returns>
        public static float MSEDerivative(float predicted, float expected)
        {
            return 2 * (predicted - expected);
        }

        public static float MSE(List<float> predicted, List<float> expected)
        {
            float error = 0.0f;
            for(int i = 0; i < predicted.Count; i++)
            {
                error += MSE(predicted[i], expected[i]);
            }

            return error / expected.Count;
        }
    }
}

