using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace neuronal
{
    public class NeuronModel : MonoBehaviour
    {
        public List<Neuron> inputNeurons = new List<Neuron>();
        public List<Neuron> neuronList = new List<Neuron>();

        public void DoInference(float[] inputs)
        {
            /* First set up the input neurons */
            for(int i = 0; i < inputs.Length; i++)
            {
                inputNeurons[i].Output = inputs[i];
                inputNeurons[i].StepCount += 1;
            }

            /* We can now perform an inference on the remaining neurons */
            int attemptCount = 0;
            List<Neuron> incomplete = new List<Neuron>();
            incomplete.AddRange(neuronList);
            while(attemptCount < 1000 && incomplete.Count > 0)
            {
                /* Attempt to do an inference on each neuron. */
                List<Neuron> completed = new List<Neuron>();
                foreach(var neuron in incomplete)
                {
                    bool didInference = neuron.AttemptInference();
                    if (didInference) completed.Add(neuron);
                }
                
                /* Remove completed neurons from the pool */
                foreach(var neuron in completed)
                {
                    incomplete.Remove(neuron);
                }

                /* This attempt is complete */
                attemptCount++;
            }
        }
    }
}

