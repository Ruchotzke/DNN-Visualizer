using DNNElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class LayerLayout : MonoBehaviour
{
    #region SERIALIZED
    [Header("Prefabs")]
    public GameObject pf_Layer;
    public Neuron pf_Neuron;
    public UILineRenderer pf_LineRenderer;
    public Label pf_Label;

    [Header("Containers")]
    public Transform LayerContainer;
    public Transform LineRendererContainer;
    public Transform LabelContainer;

    [Header("Elements")]
    public Image ColorDisplay;
    #endregion

    #region PROPERTIES
    DNN dnn;
    List<List<Neuron>> neurons = new List<List<Neuron>>();
    List<Label> outputs = new List<Label>();
    #endregion

    public void InitializeLayout(DNN dnn)
    {
        /* Remember the dnn that called this */
        this.dnn = dnn;

        /* First generate an input layer */
        Transform inputLayer = Instantiate(pf_Layer, LayerContainer).transform;
        inputLayer.name = "Input layer";
        neurons.Add(new List<Neuron>());
        for (int i = 0; i < dnn.layers[0].inputSize; i++)
        {
            var neuron = Instantiate(pf_Neuron, inputLayer);
            neuron.layer = null;
            neuron.index = i;
            neuron.name = "I" + i;
            neurons[0].Add(neuron);
        }

        /* Next generate all hidden layers */
        int layerCount = 0;
        foreach(var layer in dnn.layers)
        {
            /* Generate a new layer */
            Transform layerInstance = Instantiate(pf_Layer, LayerContainer).transform;
            layerInstance.name = "Layer " + layerCount++;
            neurons.Add(new List<Neuron>());

            /* Add neurons for this layer */
            for (int i = 0; i < layer.size; i++)
            {
                var neuron = Instantiate(pf_Neuron, layerInstance);
                neuron.layer = layer;
                neuron.index = i;
                neuron.name = "N" + i;
                neurons[neurons.Count - 1].Add(neuron);
            }

            /* Unity UI quirk - update the layout positions before the end of the frame */
            LayoutRebuilder.ForceRebuildLayoutImmediate(LayerContainer.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(layerInstance.GetComponent<RectTransform>());
        }

        /* Finally, densely connect all neurons */
        for (int i = 0; i < neurons.Count - 1; i++)
        {
            /* For each layer, connect all outgoing neurons to the next layer's incoming neurons */
            for(int outNeuron = 0; outNeuron < neurons[i].Count; outNeuron++)
            {
                for(int inNeuron = 0; inNeuron < neurons[i+1].Count; inNeuron++)
                {
                    Neuron startNeuron = neurons[i][outNeuron];
                    Neuron endNeuron = neurons[i + 1][inNeuron];
                    Vector3 start = startNeuron.transform.position;
                    Vector3 end = endNeuron.transform.position;

                    var lr = Instantiate(pf_LineRenderer, LineRendererContainer);
                    
                    lr.Points = new Vector2[2] { start, end };
                    lr.SetAllDirty();

                    neurons[i][outNeuron].AddLine(lr);
                }
            }
        }

        /* Generate a label for each output */
        int output = 0;
        foreach(var neuron in neurons[neurons.Count - 1])
        {
            var label = Instantiate(pf_Label, LabelContainer);
            label.outputIndex = output++;
            label.name.text = ((ColorChoices)label.outputIndex).ToString();
            outputs.Add(label);
        }
    }

    public void UpdateLayout(List<float[]> layerData)
    {
        /* Set the input color */
        ColorDisplay.color = new Color(layerData[0][0], layerData[0][1], layerData[0][2]);

        /* Color the line segments by their corresponding activation */
        for(int layerIndex = 0; layerIndex < neurons.Count - 1; layerIndex++)
        {
            int neuronIndex = 0;
            Debug.Log(layerIndex);
            foreach (var neuron in neurons[layerIndex])
            {
                float[] weights = dnn.layers[layerIndex].weights;
                neuron.UpdateColors(layerData[layerIndex][neuronIndex], weights);
                neuronIndex += 1;
            }
        }

        /* Set the output label values */
        for (int i = 0; i < outputs.Count; i++)
        {
            outputs[i].percent.text = layerData[layerData.Count - 1][i].ToString("f2");
        }
    }
}
