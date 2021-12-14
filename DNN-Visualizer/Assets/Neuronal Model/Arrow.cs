using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    const float ARROW_SCALE = 300.0f / 80.0f;

    [Header("Components")]
    public SpriteRenderer Head;
    public SpriteRenderer Body;

    [Header("Parameters")]
    public float ArrowLength = 1.0f;

    public void SetLength(float length)
    {
        ArrowLength = length;
        Body.transform.localScale = new Vector3(ARROW_SCALE * ArrowLength, Body.transform.localScale.y, Body.transform.localScale.z);
    }

    public void SetEndpoints(Vector2 head, Vector2 tail)
    {
        ArrowLength = Vector2.Distance(head, tail);
        transform.position = head;
        Body.transform.localScale = new Vector3(ARROW_SCALE * ArrowLength, Body.transform.localScale.y, Body.transform.localScale.z);

        float angle = Mathf.Atan2(head.y - tail.y, head.x - tail.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void SetColor(Color color)
    {
        Head.color = color;
        Body.color = color;
    }
}
