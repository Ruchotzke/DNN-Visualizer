using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DNN
{
    public class Dataset
    {
        #region SINGLETON
        public static Dataset Instance
        {
            get
            {
                if(_singleton == null) _singleton = new Dataset();
                return _singleton;
            }
        }
        private static Dataset _singleton;
        #endregion
    
        public List<(float[] input, float output)> Values = new List<(float[] input, float output)>();

        private Dataset()
        {
            for(byte i = 0; i < byte.MaxValue; i++)
            {
                /* Generate an Input */
                float[] input = new float[8] {
                    (i & (1 << 0)) != 0 ? 1.0f : 0.0f,
                    (i & (1 << 1)) != 0 ? 1.0f : 0.0f,
                    (i & (1 << 2)) != 0 ? 1.0f : 0.0f,
                    (i & (1 << 3)) != 0 ? 1.0f : 0.0f,
                    (i & (1 << 4)) != 0 ? 1.0f : 0.0f,
                    (i & (1 << 5)) != 0 ? 1.0f : 0.0f,
                    (i & (1 << 6)) != 0 ? 1.0f : 0.0f,
                    (i & (1 << 7)) != 0 ? 1.0f : 0.0f,
                };

                /* Generate an Output */
                float output = (i % 3 == 0) ? 1.0f : 0.0f;

                /* Save the data */
                Values.Add((input, output));
            }
        }

        public (List<(float[] input, float output)> training, List<(float[] input, float output)> validation) DivideSet()
        {
            List<(float[] input, float output)> training = new List<(float[] input, float output)>();
            List<(float[] input, float output)> validation = new List<(float[] input, float output)>();

            byte numValidation = (byte)(byte.MaxValue * 0.2f);

            training.AddRange(Values);
            Shuffle(training);

            for(int i = 0; i < numValidation; i++)
            {
                validation.Add(training[training.Count - 1]);
                training.RemoveAt(training.Count - 1);
            }

            return (training, validation);
        }

        private void Shuffle<T>(List<T> list)
        {
            for(int index = list.Count - 1; index >= 0; index--)
            {
                int swapIndex = Random.Range(0, index + 1);
                T temp = list[swapIndex];
                list[swapIndex] = list[index];
                list[index] = temp;
            }
        }
    }
}
