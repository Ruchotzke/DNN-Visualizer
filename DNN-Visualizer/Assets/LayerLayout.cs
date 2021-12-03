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

    [Header("Containers")]
    public Transform LayerContainer;
    public Transform LineRendererContainer;
    #endregion

    #region PROPERTIES
    DNN dnn;
    List<List<Neuron>> neurons = new List<List<Neuron>>();
    #endregion

    private void Awake()
    {
        dnn = FindObjectOfType<DNN>();
    }

    private void Start()
    {
        UpdateLayout();
    }

    public void UpdateLayout()
    {
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
    }
}
