using UnityEngine;

namespace DNN
{
    public class Neuron
    {
        /* Parameters */
        public float[] Inputs;
        public float[] Weights;
        public float bias = 0.0f;
        public ActivationFunction activationFunction;
        
        /// <summary>
        /// Construct a new neuron.
        /// </summary>
        /// <param name="func"></param>
        public Neuron(ref float[] Inputs, ActivationFunction func)
        {
            this.Inputs = Inputs;
            Weights = new float[Inputs.Length];
            this.activationFunction = func;

            /* Randomly assign normalized weights */
            for(int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = Random.value * 2.0f - 1.0f;
            }
        }

        /// <summary>
        /// The output of this neuron with respect to current inputs.
        /// </summary>
        public float Output
        {
            get
            {
                float output = bias;
                for (int i = 0; i < Inputs.Length; i++)
                {
                    output += Inputs[i] * Weights[i];
                }
                output = activationFunction.Evaluate(output);

                return output;
            }
        }

        public void UpdateParameters(float error, float learningRate)
        {
            for(int i = 0;i < Weights.Length; i++)
            {
                Weights[i] -= error * Inputs[i] * learningRate;
            }
            bias -= error * learningRate;
        }
    }
}
