using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class LayerLayout : MonoBehaviour
{
    #region SERIALIZED
    [Header("Prefabs")]
    public GameObject pf_Layer;
    public GameObject pf_Neuron;
    public UILineRenderer pf_LineRenderer;

    [Header("Containers")]
    public Transform LayerContainer;
    public Transform LineRendererContainer;
    #endregion

    #region PROPERTIES
    DNN dnn;
    List<List<Transform>> neurons = new List<List<Transform>>();
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
        neurons.Add(new List<Transform>());
        for (int i = 0; i < dnn.layers[0].inputSize; i++)
        {
            var neuron = Instantiate(pf_Neuron, inputLayer);
            neuron.name = "I" + i;
            neurons[0].Add(neuron.transform);
        }

        /* Next generate all hidden layers */
        int layerCount = 0;
        foreach(var layer in dnn.layers)
        {
            /* Generate a new layer */
            Transform layerInstance = Instantiate(pf_Layer, LayerContainer).transform;
            layerInstance.name = "Layer " + layerCount++;
            neurons.Add(new List<Transform>());

            /* Add neurons for this layer */
            for (int i = 0; i < layer.size; i++)
            {
                var neuron = Instantiate(pf_Neuron, layerInstance);
                neuron.name = "N" + i;
                neurons[neurons.Count - 1].Add(neuron.transform);
            }
        }

        /* Finally, densely connect all neurons */
        for(int i = 0; i < neurons.Count - 1; i++)
        {
            /* For each layer, connect all outgoing neurons to the next layer's incoming neurons */
            for(int outNeuron = 0; outNeuron < neurons[i].Count; outNeuron++)
            {
                for(int inNeuron = 0; inNeuron < neurons[i+1].Count; inNeuron++)
                {
                    Debug.Log("LINE: " + neurons[i][outNeuron].GetComponent<RectTransform>().anchoredPosition + ", " + neurons[i + 1][inNeuron].GetComponent<RectTransform>().anchoredPosition + ": " + neurons[i][outNeuron].name + ", " + neurons[i+1][inNeuron].name);
                    var lr = Instantiate(pf_LineRenderer, LineRendererContainer);
                    lr.Points = new Vector2[2] { neurons[i][outNeuron].GetComponent<RectTransform>().anchoredPosition, neurons[i + 1][inNeuron].GetComponent<RectTransform>().anchoredPosition };
                    lr.SetAllDirty();
                }
            }
        }
    }
}
