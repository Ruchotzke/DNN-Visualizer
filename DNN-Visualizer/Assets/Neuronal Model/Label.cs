using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace neuronal
{
    public class Label : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI Text;

        public void SetText(float value, Color color)
        {
            Text.text = value.ToString("f2");
            Text.color = color;
        }

        public void SetText(float value)
        {
            Text.text = value.ToString("f2");
        }
    }
}

