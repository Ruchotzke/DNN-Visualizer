using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace neuronal
{
    public class ActionDisplayManager : MonoBehaviour
    {
        #region SINGLETON
        public static ActionDisplayManager Instance
        {
            get
            {
                if(_singleton == null) _singleton = GameObject.FindObjectOfType<ActionDisplayManager>();
                return _singleton;
            }
        }
        private static ActionDisplayManager _singleton;
        #endregion

        [SerializeField] InputManager inputManager;
        [SerializeField] NeuronModel model;

        [Header("Coloring")]
        public Color InferenceOutputColor = Color.yellow;
        public Color InferenceArrowColor = Color.red;
        public Color ErrorColor = Color.magenta;
        public Color ErrorArrowColor = Color.cyan;

        [Header("Timing and Display")]
        public float LayerTime;
        public float ArrowLen;
        public float ArrowOffsetPercentage;

        [Header("Prefabs")]
        public Arrow pf_Arrow;

        /// <summary>
        /// Display the actions with colorings for an inference.
        /// </summary>
        /// <param name="actions"></param>
        public void DisplayInference(List<ModelAction> inActions)
        {
            /* First split the list into timestamps */
            foreach(var action in inActions)
            {
                var light = LightNeuron(action.Affected, LayerTime * action.Timestamp, LayerTime, InferenceOutputColor);
                StartCoroutine(light);
                foreach(var output in action.Affected.Outgoing)
                {
                    /* Generate an arrow to each output */
                    Vector2 start = action.Affected.transform.position;
                    Vector2 end = output.transform.position;
                    float distance = Vector2.Distance(start, end);
                    float speed = (distance + ArrowLen) / ((1f - ArrowOffsetPercentage) * LayerTime);
                    var arrow = DisplayArrow(action.Affected.transform.position, output.transform.position, LayerTime * action.Timestamp + ArrowOffsetPercentage * LayerTime, speed, ArrowLen);
                    StartCoroutine(arrow);
                }
            }
        }

        public void DisplayBackpropagation(List<ModelAction> inActions)
        {
            /* First split the list into timestamps */
            foreach (var action in inActions)
            {
                if(action.ActionType == ModelActionType.CALCULATED_ERROR)
                {
                    var light = LightNeuron(action.Affected, LayerTime * action.Timestamp, LayerTime, ErrorColor);
                    StartCoroutine(light);
                    foreach (var incoming in action.Affected.Incoming)
                    {
                        /* Generate an arrow to each output */
                        Vector2 start = action.Affected.transform.position;
                        Vector2 end = incoming.transform.position;
                        float distance = Vector2.Distance(start, end);
                        float speed = (distance + ArrowLen) / ((1f - ArrowOffsetPercentage) * LayerTime);
                        var arrow = DisplayArrow(action.Affected.transform.position, incoming.transform.position, LayerTime * action.Timestamp + ArrowOffsetPercentage * LayerTime, speed, ArrowLen);
                        StartCoroutine(arrow);
                    }
                }
                else
                {

                }
                
            }
        }

        IEnumerator LightNeuron(Neuron n, float delay, float time, Color color)
        {
            yield return new WaitForSeconds(delay);
            Color originalColor = n.Edge.color;
            n.Edge.color = color;
            yield return new WaitForSeconds(time);
            n.Edge.color = originalColor;
        }

        /// <summary>
        /// Display an arrow travelling from one location to another.
        /// </summary>
        /// <param name="source">Source point</param>
        /// <param name="dest">End point</param>
        /// <param name="delay">Delay before initializing</param>
        /// <param name="time">Time for travel</param>
        /// <param name="maxLength">Maximum arrow length</param>
        /// <returns></returns>
        IEnumerator DisplayArrow(Vector2 source, Vector2 dest, float delay, float speed, float maxLength)
        {
            yield return new WaitForSeconds(delay);

            /* Generate a new arrow with length small */
            Arrow arrow = Instantiate(pf_Arrow, transform);
            arrow.SetColor(InferenceArrowColor);
            arrow.name = "Arrow";
            arrow.SetLength(0.01f);
            Vector2 direction = (dest - source).normalized;
            Vector2 currHeadPos = source;
            Vector2 currTailPos = source;
            Vector2 delta = speed * direction;

            /* Each frame move the arrow slightly closer to the destination */
            yield return new WaitForEndOfFrame();

            /* Phase 1: Extending from source */
            Vector2 nextHeadPoint = currHeadPos + delta * Time.deltaTime;
            while(Vector2.Distance(nextHeadPoint, source) < maxLength)
            {
                /* If the point lies beyond the end point, clamp it and move to the next stage */
                if (PointLiesBetweenPoints(currHeadPos, nextHeadPoint, dest))
                {
                    arrow.SetEndpoints(dest, source);
                    currHeadPos = dest;
                    break;
                }

                /* move the head towards the destination */
                currHeadPos = nextHeadPoint;
                arrow.SetEndpoints(currHeadPos, source);
                nextHeadPoint = currHeadPos + delta * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            /* Phase 2: Both ends moving */
            Vector2 nextTailPoint;
            while(currHeadPos != dest)
            {
                /* We need to move both the head and tail towards the destination */
                nextHeadPoint = currHeadPos + delta * Time.deltaTime;
                nextTailPoint = currTailPos + delta * Time.deltaTime;

                /* If we are going to overshoot the head, the loop ends */
                if(PointLiesBetweenPoints(currHeadPos, nextHeadPoint, dest))
                {
                    currHeadPos = dest;
                    currTailPos = nextTailPoint;
                    arrow.SetEndpoints(nextTailPoint, currHeadPos);
                    break;
                }

                /* Move the head and tail */
                currHeadPos = nextHeadPoint;
                currTailPos = nextTailPoint;
                arrow.SetEndpoints(currHeadPos, currTailPos);

                /* Wait for the next frame */
                yield return new WaitForEndOfFrame();
            }

            /* Phase 3: Head arrived, end travelling */
            while(currTailPos != dest)
            {
                /* Move the tail towards the destination */
                nextTailPoint = currTailPos + delta * Time.deltaTime;

                /* If the tail is going to overshoot, we are done */
                if(PointLiesBetweenPoints(currTailPos, nextTailPoint, dest))
                {
                    break;
                }

                /* move the tail */
                currTailPos = nextTailPoint;
                arrow.SetEndpoints(dest, currTailPos);

                /* Wait for the next frame */
                yield return new WaitForEndOfFrame();
            }

            /* Cleanup */
            Destroy(arrow.gameObject);
        }

        /// <summary>
        /// Does the point check lie between a and b (if they are all linear)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        private bool PointLiesBetweenPoints(Vector2 a, Vector2 b, Vector2 check)
        {
            return ((a.x <= check.x && b.x >= check.x) || (a.x >= check.x && b.x <= check.x)) && ((a.y <= check.y && b.y >= check.y) || (a.y >= check.y && b.y <= check.y));
        }
    }
}

