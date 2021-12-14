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

        [Header("Timing")]
        public float Speed;

        /// <summary>
        /// Display the actions with colorings for an inference.
        /// </summary>
        /// <param name="actions"></param>
        public void DisplayInference(List<ModelAction> inActions)
        {
            /* First split the list into timestamps */
            /* Then invoke the color change over time */
            foreach(var action in inActions)
            {
                var Coroutine = LightNeuron(action.Affected, action.Timestamp * Speed, Speed);
                StartCoroutine(Coroutine);
            }
        }

        IEnumerator LightNeuron(Neuron n, float delay, float time)
        {
            yield return new WaitForSeconds(delay);
            Color originalColor = n.Edge.color;
            n.Edge.color = InferenceOutputColor;
            yield return new WaitForSeconds(time);
            n.Edge.color = originalColor;
        }
    }
}

