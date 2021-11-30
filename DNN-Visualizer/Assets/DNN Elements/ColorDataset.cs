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

        public static Dictionary<Color, ColorChoices> values = new Dictionary<Color, ColorChoices>()
        {
            /* Unity Default Colors */
            {Color.red, ColorChoices.RED},
            {Color.yellow, ColorChoices.YELLOW},
            {Color.blue, ColorChoices.BLUE},
            {Color.green, ColorChoices.GREEN},
            {Color.white, ColorChoices.WHITE},
            {Color.black, ColorChoices.BLACK},

            /* Custom Colors */
            {new Color(1f, .71f, .2f), ColorChoices.ORANGE },
            {new Color(.63f, .03f, 1f), ColorChoices.PURPLE},
        };
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
