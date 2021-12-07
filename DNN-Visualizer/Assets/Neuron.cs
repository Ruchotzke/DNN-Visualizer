using DNNElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class Neuron : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private List<UILineRenderer> outputConnections = new List<UILineRenderer>();
    private List<Color> originalColors = new List<Color>();

    public Layer layer;
    public int index;

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach(var line in outputConnections)
        {
            line.color = Color.red;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        for(int i = 0; i < outputConnections.Count; i++)
        {
            outputConnections[i].color = originalColors[i];
        }
    }

    public void AddLine(UILineRenderer line)
    {
        outputConnections.Add(line);
        originalColors.Add(line.color);
    }

    public void UpdateColors(float activation, float[] weights)
    {
        for (int i = 0; i < outputConnections.Count; i++)
        {
            outputConnections[i].color = Color.Lerp(Color.black, Color.white, activation * weights[i] / 5f);
        }
    }

    public void ResetColors()
    {
        for(int i = 0; i < outputConnections.Count; i++)
        {
            outputConnections[i].color = originalColors[i];
        }
    }
}
