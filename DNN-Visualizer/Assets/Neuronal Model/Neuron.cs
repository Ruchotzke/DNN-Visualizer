using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNN_V2;
using UnityEngine.UI;
using TMPro;

namespace neuronal
{
    public class Neuron : MonoBehaviour
    {
        /* Properties */
        [Header("Properties")]
        public List<Neuron> Incoming = new List<Neuron>();
        public List<float> Weights = new List<float>();
        public List<Neuron> Outgoing = new List<Neuron>();
        public float Bias;
        public ActivationFunction Activation;
        public bool IsInputNeuron;
        public bool IsOutputNeuron;

        /* State */
        public float Output
        {
            get { return _output; }
            set { _output = value;
                activationLabel?.SetText(value); 
                }
        }
        private float _output;
        public float Error;
        public int InferenceStepCount = 0;
        public int BackpropStepCount = 0;

        [Header("Graphics")]
        public SpriteRenderer Center;
        public TextMeshProUGUI Name;
        public Label activationLabel;

        public bool AttemptInference()
        {
            /* If we are an input neuron, we don't do inference. */
            if (IsInputNeuron) return true;

            /* We only want to do an inference if all of our ancestors are done */
            foreach(var neuron in Incoming)
            {
                if (neuron.InferenceStepCount <= this.InferenceStepCount) return false; //one of our ancestors hasn't yet fired.
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
            InferenceStepCount++;
            return true;
        }
    
        public bool AttemptBackpropagation()
        {
            /* If all of our descendants have a valid error, we can update our own error */
            foreach(var neuron in Outgoing)
            {
                if (neuron.BackpropStepCount <= this.BackpropStepCount) return false;  //someone downstream isn't done
            }

            /* Everyone downstream is done. We can update our values and error now */
            Error = 0.0f;
            foreach(var descendant in Outgoing)
            {
                Error += descendant.Weights[descendant.Incoming.IndexOf(this)] * descendant.Error;
            }
            Error *= Activation.Derivative(Output);

            /* We did it. let the parent script know */
            BackpropStepCount++;
            return true;
        }

        /// <summary>
        /// Update the weights and biases of this neuron according to 
        /// this neuron's error.
        /// </summary>
        /// <param name="learningRate">How fast should error alter weights and biases.</param>
        public void UpdateParams(float learningRate)
        {
            /* Weights are adjusted by their derivative (their activation) */
            for(int i = 0; i < Weights.Count; i++)
            {
                Weights[i] -= Incoming[i].Output * Error * learningRate;
            }

            /* Biases are constant, and their only variant is the error itself */
            Bias -= Error * learningRate;
        }
    }
}

