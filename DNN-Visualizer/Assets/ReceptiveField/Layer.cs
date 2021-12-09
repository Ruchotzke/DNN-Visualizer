using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReceptiveFields
{
    public class Layer
    {
        public Vector2Int Size;
        public Vector2Int Kernel;

        /// <summary>
        /// Create a new layer.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="kernel"></param>
        public Layer(Vector2Int size, Vector2Int kernel)
        {
            this.Size = size;
            this.Kernel = kernel;
        }

        /// <summary>
        /// Get the size of the resultant feature map.
        /// </summary>
        /// <returns></returns>
        public Vector2Int GetOutputSize()
        {
            return Size - Kernel + Vector2Int.one;
        }

        public override string ToString()
        {
            return "Layer: Size=" + Size + " Kernel=" + Kernel;
        }
    }
}

