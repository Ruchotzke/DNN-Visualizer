using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ReceptiveFields
{
    public class Featuremap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region PROPERTIES
        RectTransform rect;
        [HideInInspector] public Model model;
        #endregion

        #region ELEMENT CALCULATION
        Vector2 Size;
        Vector2Int TextureSize;
        Texture2D texture;

        public Vector2Int NumElements;
        Vector2Int ElementSize;
        #endregion

        #region HOVERING
        bool isOverImage = false;
        Vector2Int tile;
        #endregion

        private void Awake()
        {
            /* Generate a texture */
            rect = GetComponent<RectTransform>();
            Size = rect.rect.size;
            TextureSize = Vector2Int.RoundToInt(Size);

            /* Figure out the cell sizes, and adjust the texture size to fit */
            var temp = (TextureSize - Vector2Int.one);
            ElementSize = new Vector2Int(temp.x / NumElements.x, temp.y / NumElements.y) - Vector2Int.one;
            TextureSize = Vector2Int.one + NumElements * (ElementSize + Vector2Int.one);

            texture = new Texture2D(TextureSize.x, TextureSize.y);
            GetComponent<RawImage>().texture = texture;
            texture.filterMode = FilterMode.Point;
            GenerateGrid();
        }

        void GenerateGrid()
        {
            for (int y = 0; y < TextureSize.y; y++)
            {
                for (int x = 0; x < TextureSize.x; x++)
                {
                    texture.SetPixel(x, y, (x % (ElementSize.x + 1) == 0 || y % (ElementSize.y + 1) == 0) ? Color.black : Color.white);
                }
            }
            texture.Apply();
        }

        Vector2Int? prevCell = null;

        private void Update()
        {
            /* if the mouse is over this image, we can calculate a box */
            if (isOverImage)
            {
                /* Get the local mouse position over the texture */
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, null, out Vector2 outValue);
                Vector2 normalized = outValue / Size;

                /* Calculate the box to convert */
                Vector2Int element = Vector2Int.FloorToInt(TextureSize * normalized / ElementSize);
                if (element.x >= 0 && element.x < NumElements.x && element.y >= 0 && element.y < NumElements.y)
                {
                    model.SetFeaturemapElement(this, element);
                }
            }
        }

        public void SetElementColor(Color color, int tx, int ty)
        {
            /* Find the lower left corner */
            Vector2Int corner = new Vector2Int(1 + (ElementSize.x + 1) * tx, 1 + (ElementSize.y + 1) * ty);
            for (int x = 0; x < ElementSize.x; x++)
            {
                for (int y = 0; y < ElementSize.y; y++)
                {
                    texture.SetPixel(corner.x + x, corner.y + y, color);
                }
            }
            texture.Apply();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isOverImage = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isOverImage = false;
            model.SetFeaturemapElement(null, Vector2Int.zero);
        }
    }
}