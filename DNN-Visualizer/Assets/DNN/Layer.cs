using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DNN
{
    public class Layer
    {
        public Neuron[] Neurons;
        public float[] LayerInput;

        public Layer(int inputSize, int outputSize, ActivationFunction activationFunction)
        {
            LayerInput = new float[inputSize]; //all neurons can share an input array
            Neurons = new Neuron[outputSize];
            for(int i = 0; i < outputSize; i++)
            {
                Neurons[i] = new Neuron(ref LayerInput, activationFunction);
            }
        }

        public float[] Inference()
        {
            float[] ret = new float[Neurons.Length];

            for (int i = 0; i < Neurons.Length; i++)
            {
                ret[i] = Neurons[i].Output;
            }

            return ret;
        }
    }
}
