using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DNNElements;
using TMPro;

public class DataController : MonoBehaviour
{
    #region PREFABS
    [Header("Prefabs")]
    public Button pf_ChoiceButton;
    #endregion

    #region SERIALIZED
    [Header("Containers")]
    public Transform ButtonContainer;
    public RawImage ColorImage;
    #endregion

    #region PROPERTIES
    Dictionary<Color, ColorChoices> data = new Dictionary<Color, ColorChoices>();
    int colorCount = 1; //start at 1 to ensure the first color isn't drab
    #endregion

    private void Awake()
    {
        /* First generate buttons */
        foreach(ColorChoices color in System.Enum.GetValues(typeof(ColorChoices)))
        {
            Button button = Instantiate(pf_ChoiceButton, ButtonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = color.ToString();
            button.onClick.AddListener(delegate
            {
                AddData(ColorImage.color, color);
            });
        }

        /* Set the first color */
        GenerateColor();
    }

    public void AddData(Color color, ColorChoices choice)
    {
        data.Add(color, choice);
        GenerateColor();
    }

    void GenerateColor()
    {
        /* Normally generate colors in a high saturation/value range */
        /* Sometimes generate dark/light colors */
        if(colorCount % 4 == 0)
        {
            if(Random.value < 0.33f)
            {
                //black color
                ColorImage.color = Random.ColorHSV(0, 1, 1, 1, 0, .4f);
            }
            else if(Random.value < 0.66f)
            {
                //white color
                ColorImage.color = Random.ColorHSV(0, 1, 0, .4f, 1, 1);
            }
            else
            {
                //grayscale color (for finding range)
                ColorImage.color = Random.ColorHSV(0, 1, 0, 0, 0, 1);
            }
            
        }
        else
        {
            ColorImage.color = Random.ColorHSV(0, 1, .7f, 1, .7f, 1);
        }

        colorCount += 1;
    }

    public void Exit()
    {
        /* Save a text file wherever this is executing */
        string[] lines = new string[data.Count + 1];
        lines[0] = "VERSION " + Application.version;
        int i = 1;
        foreach(var entry in data)
        {
            lines[i] = entry.Key.r + " " + entry.Key.g + " " + entry.Key.b + " " + (int)entry.Value;
            i++;
        }
        System.IO.File.WriteAllLines("Color-Output-" + Random.Range(0, 500) + ".txt", lines);

        /* Exit */
        Application.Quit();
    }
}
