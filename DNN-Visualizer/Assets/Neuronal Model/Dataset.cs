using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace neuronal
{
    /// <summary>
    /// A dataset where the inputs are 2 scalar values, and the output is their sum.
    /// </summary>
    public class AdditionDataset
    {
        public List<(float[] inputs, float output)> dataset = new List<(float[] inputs, float output)> ();

        public AdditionDataset(Vector2 inputRange, int values)
        {
            for(int i = 0; i < values; i++)
            {
                float a = Random.Range(inputRange.x, inputRange.y);
                float b = Random.Range(inputRange.x, inputRange.y);
                dataset.Add((new float[] { a, b }, a + b));
            }
        }
    }
}

