using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DNN_V2;
public class DNNLayout : MonoBehaviour
{
    [Header("Layout")]
    public Transform LayerContainer;

    [Header("Prefabs")]
    public Transform pf_Layer;
    public Neuron pf_Neuron;

    DNN dnn;

    private void Awake()
    {
        /* Set up a basic DNN */
        dnn = FindObjectOfType<DNN>();
        dnn.layers.Add(new InputLayer(2, dnn));
        dnn.layers.Add(new Layer(2, 3, ActivationFunction.SIGMOID));
        dnn.layers.Add(new Layer(3, 2, ActivationFunction.SIGMOID));
        dnn.input = new float[2] { 0.1f, 0.7f};
    }

    private void Start()
    {
        DisplayDNN();
    }

    void DisplayDNN()
    {
        for(int layerid = 0; layerid < dnn.layers.Count; layerid++)
        {
            var layerTransform = Instantiate(pf_Layer, LayerContainer);
            for(int i = 0; i < dnn.layers[layerid].Neurons.Length; i++)
            {
                Instantiate(pf_Neuron, layerTransform);
            }
        }
    }
}
