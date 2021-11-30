using DNNElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNN : MonoBehaviour
{
    #region INPUTS
    [SerializeField] Color input = Color.red;
    #endregion

    #region PROPERTIES
    List<Layer> layers = new List<Layer>();
    #endregion

    private void Awake()
    {
        /* Generate Default Network */
        layers.Add(new Layer(3, 8, 0, ActivationFunction.RELU));
        layers.Add(new Layer(8, 12, 0, ActivationFunction.RELU));
        layers.Add(new Layer(12, 4, 0, ActivationFunction.SOFTMAX));

        /* Try an inference */
        var intermediates = DoInference();
        Debug.Log("Intermediates count: " + intermediates.Count);
        for (int i = 0; i < intermediates.Count; i++)
        {
            string print = "[";
            for (int j = 0; j < intermediates[i].Length; j++)
            {
                print += intermediates[i][j] + ", ";
            }

            Debug.Log(print + "]");
        }
    }

    /// <summary>
    /// Compute the inference output, along with intermediate values for display.
    /// </summary>
    /// <returns>A list of float[] containing intermediate outputs.</returns>
    List<float[]> DoInference()
    {
        List<float[]> intermediates = new List<float[]>();
        float[] initial = new float[3] { input.r, input.g, input.b };
        intermediates.Add(initial);
        for (int i = 0; i < layers.Count; i++)
        {
            initial = layers[i].ComputeInference(initial);
            intermediates.Add(initial);
        }

        return intermediates;
    }
}
