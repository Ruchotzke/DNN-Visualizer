using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReceptiveFields
{
    public class Model : MonoBehaviour
    {
        List<Layer> layers = new List<Layer>();
        List<Featuremap> featuremaps = new List<Featuremap>();

        List<(Featuremap fmap, Vector2Int element)> SetElements = new List<(Featuremap fmap, Vector2Int element)>();
        (Featuremap fmap, Vector2Int element)? prevMouse = null;

        [Header("Model")]
        public Vector2Int StartElements;
        public Vector2Int[] Kernels;

        [Header("Prefabs")]
        public Featuremap pf_Featuremap;

        [Header("Containers")]
        public Transform FeaturemapContainer;

        private void Awake()
        {
            /* Generate a complete model from the inputs */
            layers.Add(new Layer(StartElements, Kernels[0]));
            for(int i = 1; i < Kernels.Length; i++)
            {
                layers.Add(new Layer(layers[layers.Count - 1].GetOutputSize(), Kernels[i]));
            }

            GenerateLayout();
        }

        void GenerateLayout()
        {
            /* Generate a new featuremap for each layer */
            foreach (var layer in layers)
            {
                Featuremap fmap = Instantiate(pf_Featuremap, FeaturemapContainer);
                fmap.model = this;
                fmap.NumElements = layer.Size;
                fmap.GetComponent<RectTransform>().sizeDelta = new Vector2(20 * layer.Size.x, 20 * layer.Size.y);
                featuremaps.Add(fmap);
            }
        }

        public void SetFeaturemapElement(Featuremap fmap, Vector2Int element)
        {
            if(fmap == null)
            {
                /* This signals we need to clear the featuremaps */
                foreach(var set in SetElements)
                {
                    set.fmap.SetElementColor(Color.white, set.element.x, set.element.y);
                }
                if(prevMouse != null)
                {
                    prevMouse.Value.fmap.SetElementColor(Color.white, prevMouse.Value.element.x, prevMouse.Value.element.y);
                    prevMouse = null;
                }
            }
            else
            {
                /* If the previous mouse input is still active, compare it */
                if(prevMouse != null)
                {
                    /* No change = No updates */
                    if (prevMouse.Value.element == element) return;

                    /* Clear out old data and add new data */
                    foreach (var set in SetElements)
                    {
                        set.fmap.SetElementColor(Color.white, set.element.x, set.element.y);
                    }
                    prevMouse.Value.fmap.SetElementColor(Color.white, prevMouse.Value.element.x, prevMouse.Value.element.y);
                }

                /* Working backwards from this layer, show the building receptive field */
                int layerIndex = featuremaps.FindIndex(x => x == fmap);
                fmap.SetElementColor(Color.red, element.x, element.y);
                prevMouse = (fmap, element);

                for (int i = layerIndex - 1; i >= 0; i--)
                {
                    for (int y = -layers[i].Kernel.y / 2; y <= layers[i].Kernel.y / 2; y++)
                    {
                        for (int x = -layers[i].Kernel.x / 2; x <= layers[i].Kernel.x / 2; x++)
                        {
                            Vector2Int loc = new Vector2Int(x + element.x, y + element.y);
                            if (loc.x < 0 || loc.y < 0 || loc.x >= layers[i].Size.x || loc.y >= layers[i].Size.y) continue;
                            featuremaps[i].SetElementColor(Color.red, loc.x, loc.y);
                            SetElements.Add((featuremaps[i], loc));
                        }
                    }
                }
                
            }
        }
    }
}
