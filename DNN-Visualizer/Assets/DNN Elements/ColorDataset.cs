using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DNNElements
{
    public class ColorDataset
    {
        /* Colors into classifications for this toy neural network */
        /* Color : Classification */

        /* Possible Classifications stored in a 1-hot array */
        /**
         * RED
         * YELLOW
         * BLUE
         * GREEN
         * ORANGE
         * PURPLE
         * BLACK
         * WHITE
        **/

        public Dictionary<Color, (ColorChoices color, float[] oneHot)> values = new Dictionary<Color, (ColorChoices color, float[] oneHot)>();

        public void AddDataset(string[] lines)
        {
            foreach(string line in lines)
            {
                if (line.StartsWith("VERSION"))
                {
                    continue;
                }

                string[] split = line.Split(' ');
                try
                {
                    float r = float.Parse(split[0]);
                    float g = float.Parse(split[1]);
                    float b = float.Parse(split[2]);
                    int color = int.Parse(split[3]);
                    ColorChoices choice = (ColorChoices)color;

                    values.Add(new Color(r, g, b), (choice, choice.ToOneHot()));
                }
                catch (System.Exception)
                {
                    Debug.LogError("Line not correctly formatted: " + line);
                    continue;
                }
                
            }
        }

        public void AddDataset(string directory)
        {
            foreach(var name in System.IO.Directory.EnumerateFiles(directory))
            {
                string[] file = System.IO.File.ReadAllLines(name);
                AddDataset(file);
            }
        }
    }

    public static class ColorChoiceExtensions
    {
        public static float[] ToOneHot(this ColorChoices choice)
        {
            switch (choice)
            {
                case ColorChoices.RED:
                    return new float[8] { 1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
                case ColorChoices.YELLOW:
                    return new float[8] { 0f, 1f, 0f, 0f, 0f, 0f, 0f, 0f };
                case ColorChoices.BLUE:
                    return new float[8] { 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0f };
                case ColorChoices.GREEN:
                    return new float[8] { 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f };
                case ColorChoices.ORANGE:
                    return new float[8] { 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f };
                case ColorChoices.PURPLE:
                    return new float[8] { 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f };
                case ColorChoices.BLACK:
                    return new float[8] { 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0f };
                case ColorChoices.WHITE:
                    return new float[8] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f };
                default:
                    return new float[8] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
            }
        }
    }

    public enum ColorChoices
    {
        RED,
        YELLOW,
        BLUE,
        GREEN,
        ORANGE,
        PURPLE,
        BLACK,
        WHITE
    }
}
