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
        public const bool USE_SLOW_INFERENCE = true;

        /* NOTE: INPUT/OUTPUT NEURON INDICES ARE SORTED BY Y POSITION (HIGH Y == LOW INDEX) */
        public List<Neuron> inputNeurons = new List<Neuron>();
        public List<Neuron> outputNeurons = new List<Neuron>();
        
        /* CONTAINS ALL NEURONS EXCEPT FOR INPUT NEURONS, WHICH AREN'T REALLY NEURONS */
        public List<Neuron> neuronList = new List<Neuron>();

        /* LAST USED DATAPOINT */
        public float[] lastInput;       /* last input used in the system */
        public float[] lastOutput;      /* corresponding correct output for last input */

        InputManager manager;
        private void Awake()
        {
            manager = GameObject.FindObjectOfType<InputManager>();
        }

        public List<ModelAction> DoInference(float[] inputs, int initialTimestamp = 0)
        {
            /* Generate a list of actions to return */
            List<ModelAction> actions = new List<ModelAction>();

            /* First set up the input neurons */
            int timestamp = initialTimestamp;
            for(int i = 0; i < inputs.Length; i++)
            {
                inputNeurons[i].Output = inputs[i];
                inputNeurons[i].InferenceStepCount += 1;
                actions.Add(new ModelAction(ModelActionType.CALCULATED_OUTPUT, inputNeurons[i], null, timestamp));
            }

            /* We can now perform an inference on the remaining neurons */
            int attemptCount = 0;
            List<Neuron> incomplete = new List<Neuron>();
            incomplete.AddRange(neuronList);
            incomplete.RemoveAll(n => n.IsInputNeuron); //input neurons are done!
            while(attemptCount < MAX_ITERATIONS && incomplete.Count > 0)
            {
                /* Increment the timestamp */
                timestamp += 1;

                /* Attempt to do an inference on each neuron. */
                List<Neuron> completed = new List<Neuron>();
                foreach(var neuron in incomplete)
                {
                    /* SLOW INFERENCE makes sure layers can only be computed one "layer" at a time */
                    /* If slow inference is off, inference will usually take place in one loop, ruining the visual */
                    /* Slow inference forces a neuron to compute only if its predecessor executed in an earlier loop */
                    if (USE_SLOW_INFERENCE)
                    {
                        bool skipfornow = false;
                        foreach(var pred in neuron.Incoming)
                        {
                            /* One of our predecessors completed. That implies we SHOULD NOT complete this loop */
                            if (completed.Contains(pred))
                            {
                                skipfornow = true;
                                break;
                            }
                        }
                        if (skipfornow) continue; //we will come back to this node in the next loop
                    }

                    /* Attempt inference */
                    bool didInference = neuron.AttemptInference();
                    if (didInference)
                    {
                        completed.Add(neuron);
                        actions.Add(new ModelAction(ModelActionType.CALCULATED_OUTPUT, neuron, null, timestamp));
                    }
                }
                
                /* Remove completed neurons from the pool */
                foreach(var neuron in completed)
                {
                    incomplete.Remove(neuron);
                }

                /* This attempt is complete */
                attemptCount++;
            }

            if (attemptCount >= MAX_ITERATIONS)
            {
                Debug.LogError("Inference Incomplete. Max iterations reached");
                Debug.LogError("MISSING NODES: " + string.Join(", ", incomplete));
                return actions;
            }

            return actions;
        }

        public List<ModelAction> DoBackpropagation(float[] expectedOutputs, int initialTimestamp = 0)
        {
            /* Actions */
            List<ModelAction> actions = new List<ModelAction>();

            /* First set up the output neuron errors */
            /* We will use MSE for our model */
            int timestamp = initialTimestamp;
            for(int i = 0; i < expectedOutputs.Length; i++)
            {
                /* An output neuron's error is the loss derivative multiplied by the derivative of the output activation function */
                outputNeurons[i].Error = outputNeurons[i].Activation.Derivative(outputNeurons[i].Output) * Error.MSEDerivative(outputNeurons[i].Output, expectedOutputs[i]);
                outputNeurons[i].BackpropStepCount += 1; /* These nodes are complete */
                actions.Add(new ModelAction(ModelActionType.CALCULATED_ERROR, outputNeurons[i], null, timestamp));
            }

            /* We can now perform backprop on the remaining neurons iteratively */
            int attemptCount = 0;
            List<Neuron> incomplete = new List<Neuron>();
            incomplete.AddRange(neuronList);
            incomplete.RemoveAll(n => n.IsInputNeuron); //input neurons are not trainable (no parameters used)
            incomplete.RemoveAll(neuron => neuron.IsOutputNeuron); //output neurons are done. Input neurons are already absent from this list as well.
            while (attemptCount < MAX_ITERATIONS && incomplete.Count > 0)
            {
                /* Timestamp */
                timestamp += 1;

                /* Attempt to do an error calculation on each neuron. */
                List<Neuron> completed = new List<Neuron>();
                foreach (var neuron in incomplete)
                {
                    bool didError = neuron.AttemptBackpropagation();
                    if (didError)
                    {
                        completed.Add(neuron);
                        actions.Add(new ModelAction(ModelActionType.CALCULATED_ERROR, neuron, null, timestamp));
                    }
                }

                /* Remove completed neurons from the pool */
                foreach (var neuron in completed)
                {
                    incomplete.Remove(neuron);
                }

                /* This attempt is complete */
                attemptCount++;
            }
            timestamp += 1;

            if (attemptCount >= MAX_ITERATIONS) {
                Debug.LogError("Backpropagation Incomplete. Max iterations reached");
                Debug.LogError("MISSING NODES: " + string.Join(", ", incomplete));
                return actions;
            }

            /* Finally, if backprop worked out, we can update all parameters */
            foreach (var neuron in neuronList)
            {
                neuron.UpdateParams(LEARNING_RATE);
                actions.Add(new ModelAction(ModelActionType.UPDATED_PARAMS, neuron, null, timestamp));
            }

            return actions;
        }

        public (float loss, float acc) GetStats(bool updateGUI)
        {
            /* Get accuracy and loss to update graphs */
            List<float> outputs = new List<float>();
            List<float> correct = new List<float>();
            float numCorrect = 0.0f;
            for (int i = 0; i < manager.additionDataset.Size; i++)
            {
                var datapoint = manager.additionDataset.dataset[i];
                DoInference(datapoint.inputs);
                outputs.Add(outputNeurons[0].Output);
                correct.Add(datapoint.output);

                if (Mathf.Abs(outputNeurons[0].Output - datapoint.output) < 0.1f)
                {
                    numCorrect += 1.0f;
                }
            }
            float loss = Error.MSE(outputs, correct);
            float acc = (numCorrect / manager.additionDataset.Size) * 100.0f;
            int epoch = GraphManager.Instance.Loss.Count > 0 ? GraphManager.Instance.Loss[GraphManager.Instance.Loss.Count - 1].epoch + 1 : 0;
            GraphManager.Instance.UpdateLists(epoch, loss, acc, updateGUI);

            return (loss, acc);
        }
    }
}

