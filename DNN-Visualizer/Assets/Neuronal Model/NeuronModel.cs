using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DNN_V2;

namespace neuronal
{
    public class NeuronModel : MonoBehaviour
    {
        public const int MAX_ITERATIONS = 1000;
        public const float LEARNING_RATE = 0.01f;

        /* NOTE: INPUT/OUTPUT NEURON INDICES ARE SORTED BY Y POSITION (HIGH Y == LOW INDEX) */
        public List<Neuron> inputNeurons = new List<Neuron>();
        public List<Neuron> outputNeurons = new List<Neuron>();
        
        /* CONTAINS ALL NEURONS EXCEPT FOR INPUT NEURONS, WHICH AREN'T REALLY NEURONS */
        public List<Neuron> neuronList = new List<Neuron>();

        /* LAST USED DATAPOINT */
        public float[] lastInput;       /* last input used in the system */
        public float[] lastOutput;      /* corresponding correct output for last input */

        public void DoInference(float[] inputs)
        {
            /* First set up the input neurons */
            for(int i = 0; i < inputs.Length; i++)
            {
                inputNeurons[i].Output = inputs[i];
                inputNeurons[i].InferenceStepCount += 1;
            }

            /* We can now perform an inference on the remaining neurons */
            int attemptCount = 0;
            List<Neuron> incomplete = new List<Neuron>();
            incomplete.AddRange(neuronList);
            while(attemptCount < MAX_ITERATIONS && incomplete.Count > 0)
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

            if (attemptCount >= MAX_ITERATIONS) Debug.LogError("Inference Incomplete. Max iterations reached.");
        }

        public void DoBackpropagation(float[] expectedOutputs)
        {
            /* First set up the output neuron errors */
            /* We will use MSE for our model */
            for(int i = 0; i < expectedOutputs.Length; i++)
            {
                /* An output neuron's error is the loss derivative multiplied by the derivative of the output activation function */
                outputNeurons[i].Error = outputNeurons[i].Activation.Derivative(outputNeurons[i].Output) * Error.MSEDerivative(outputNeurons[i].Output, expectedOutputs[i]);
                outputNeurons[i].BackpropStepCount += 1; /* These nodes are complete */
            }

            /* We can now perform backprop on the remaining neurons iteratively */
            int attemptCount = 0;
            List<Neuron> incomplete = new List<Neuron>();
            incomplete.AddRange(neuronList);
            incomplete.RemoveAll(neuron => neuron.IsOutputNeuron); //output neurons are done. Input neurons are already absent from this list as well.
            while (attemptCount < MAX_ITERATIONS && incomplete.Count > 0)
            {
                /* Attempt to do an error calculation on each neuron. */
                List<Neuron> completed = new List<Neuron>();
                foreach (var neuron in incomplete)
                {
                    bool didError = neuron.AttemptBackpropagation();
                    if (didError) completed.Add(neuron);
                }

                /* Remove completed neurons from the pool */
                foreach (var neuron in completed)
                {
                    incomplete.Remove(neuron);
                }

                /* This attempt is complete */
                attemptCount++;
            }

            if (attemptCount >= MAX_ITERATIONS) {
                Debug.LogError("Backpropagation Incomplete. Max iterations reached");
                Debug.LogError("MISSING NODES: " + string.Join(", ", incomplete));
                return;
            }

            /* Finally, if backprop worked out, we can update all parameters */
            foreach(var neuron in neuronList)
            {
                neuron.UpdateParams(LEARNING_RATE);
            }
        }
    }
}

