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
    public List<Layer> layers = new List<Layer>();
    ColorDataset dataset;
    #endregion

    private void Awake()
    {
        /* Generate Default Network */
        layers.Add(new Layer(3, 8, 0, ActivationFunction.RELU));
        layers.Add(new Layer(8, 12, 0, ActivationFunction.RELU));
        layers.Add(new Layer(12, 8, 0, ActivationFunction.SOFTMAX));

        /* DEBUG: SEED THE RANDOM */
        Random.InitState(0);

        /* Load training data */
        ColorDataset dataset = new ColorDataset();
        dataset.AddDataset("TrainingData");

        /* Try an inference */
        foreach(var data in dataset.values.Keys) { input = data; break; } //only way to "index" a dictionary. Stupid. A newer version of .net would let you do this normally
        var intermediates = DoInference();
        Debug.Log("Inference Complete. Cross entropy loss: " + CrossEntropy(intermediates[intermediates.Count - 1], dataset.values[input].oneHot));
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
            if(truth[i] != 0.0f)
            {
                total += truth[i] * Mathf.Log(probability[i], 2);
            }
        }

        return -1 * total;
    }
}
