using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DNN_V2
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

        virtual public float[] Inference()
        {
            float[] ret = new float[Neurons.Length];

            for (int i = 0; i < Neurons.Length; i++)
            {
                ret[i] = Neurons[i].Output;
            }

            return ret;
        }
    }

    public class InputLayer : Layer
    {
        DNN dnn;
        public InputLayer(int size, DNN parent) : base(0, size, ActivationFunction.SIGMOID)
        {
            dnn = parent;
        }

        public override float[] Inference()
        {
            return dnn.input;
        }
    }
}
