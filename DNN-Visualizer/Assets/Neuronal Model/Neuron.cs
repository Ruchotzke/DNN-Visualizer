using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNN_V2;
using UnityEngine.UI;

namespace neuronal
{
    public class Neuron : MonoBehaviour
    {
        /* Properties */
        public List<Neuron> Incoming = new List<Neuron>();
        public List<float> Weights = new List<float>();
        public List<Neuron> Outgoing = new List<Neuron>();
        public float Bias;
        public ActivationFunction Activation;
        public bool IsInputNeuron;
        public bool IsOutputNeuron;

        /* State */
        public float Output;
        public int StepCount = 0;

        public SpriteRenderer Center;

        public bool AttemptInference()
        {
            /* If we are an input neuron, we don't do inference. */
            if (IsInputNeuron) return true;

            /* We only want to do an inference if all of our ancestors are done */
            foreach(var neuron in Incoming)
            {
                if (neuron.StepCount <= this.StepCount) return false; //one of our ancestors hasn't yet fired.
            }

            /* We are ready to perform inference */
            float accumulation = Bias;
            for(int i = 0; i < Incoming.Count; i++)
            {
                accumulation += Incoming[i].Output * Weights[i];
            }
            accumulation = Activation.Evaluate(accumulation);

            /* We completed our inference. Update our state */
            Output = accumulation;
            StepCount++;
            return true;
        }
    }
}

