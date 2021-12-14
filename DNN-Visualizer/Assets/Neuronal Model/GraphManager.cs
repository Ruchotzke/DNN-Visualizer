using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI LossData;
    public TextMeshProUGUI AccData;

    [Header("Panels")]
    public Transform LossPanel;
    public Transform AccPanel;

    [Header("Graphs")]
    public LineRenderer LossLineRenderer;
    public LineRenderer AccLineRenderer;

    #region SINGLETON
    public static GraphManager Instance
    {
        get
        {
            if(_singleton == null) _singleton = GameObject.FindObjectOfType<GraphManager>();
            return _singleton;
        }
    }
    private static GraphManager _singleton;
    #endregion

    public List<(int epoch, float loss)> Loss = new List<(int epoch, float loss)> ();
    public List<(int epoch, float acc)> Accuracy = new List<(int epoch, float acc)> ();

    public void UpdateLists(int epoch, float loss, float acc, bool updateGraphics)
    {
        /* Update and sort the lists */
        Loss.Add((epoch, loss));
        Accuracy.Add((epoch, acc));

        if (updateGraphics)
        {
            /* Update UI */
            LossData.text = "Epoch: " + epoch + " Loss: " + loss.ToString("f4");
            AccData.text = "Epoch: " + epoch + " Acc: " + acc.ToString("f1") + "%";

            /* Generate graphs */
            if(Loss.Count > 1) GenerateGraphs();
        }
        
    }

    public void GenerateGraphs()
    {
        /* First get the range of the datasets */
        float maxLoss = Loss[0].loss;
        float maxAcc = 100.0f;
        for(int i = 1; i < Loss.Count; i++)
        {
            if(Loss[i].loss > maxLoss) maxLoss = Loss[i].loss;
        }

        /* Plot the loss */
        LossLineRenderer.positionCount = Loss.Count;
        Vector3[] positions = new Vector3[Loss.Count];
        for(int i = 0; i < Loss.Count; i++)
        {
            positions[i] = new Vector3(Mathf.Lerp(10f, 480f, i / (float)(Loss.Count - 1)), Mathf.Lerp(0f, 180f, Loss[i].loss / maxLoss), -5f);
        }
        LossLineRenderer.SetPositions(positions);

        /* Plot the accuracy */
        AccLineRenderer.positionCount = Loss.Count;
        positions = new Vector3[Accuracy.Count];
        for (int i = 0; i < Loss.Count; i++)
        {
            positions[i] = new Vector3(Mathf.Lerp(10f, 480f, i / (float)(Accuracy.Count - 1)), Mathf.Lerp(0f, 180f, Accuracy[i].acc / maxAcc), -5f);
        }
        AccLineRenderer.SetPositions(positions);
    }
}
