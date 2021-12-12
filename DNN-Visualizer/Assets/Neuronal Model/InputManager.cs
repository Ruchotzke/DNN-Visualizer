using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace neuronal
{
    public class InputManager : MonoBehaviour
    {
        #region INPUT STATES
        [Header("Input State")]
        public INPUT_STATE input_state;
        public MARK_STATE mark_state;
        #endregion

        #region PREFABS
        [Header("Prefabs")]
        public Neuron pf_Neuron;
        #endregion

        #region CONTAINERS
        [Header("Containers")]
        public Transform NeuronContainer;
        #endregion

        #region MODEL
        [Header("Model")]
        public NeuronModel Model;
        #endregion

        private void Update()
        {
            HandleClicks();
        }

        #region MOUSE EVENTS
        void HandleClicks()
        {
            /* General useful information */
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitinfo = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (Input.GetMouseButtonDown(0))
            {
                switch (input_state)
                {
                    case INPUT_STATE.NONE:
                        break;
                    case INPUT_STATE.ADD_NEURON:
                        if(hitinfo.collider == null)
                        {
                            var neuron = Instantiate(pf_Neuron, NeuronContainer);
                            neuron.name = "Neuron " + Model.neuronList.Count;
                            Model.neuronList.Add(neuron);
                            neuron.transform.position = mousePosition;
                        }
                        break;
                    case INPUT_STATE.REMOVE_NEURON:
                        break;
                    case INPUT_STATE.CONNECT_NEURON:
                        break;
                    case INPUT_STATE.REMOVE_CONNECTIONS:
                        break;
                    case INPUT_STATE.MARK_NEURON:
                        break;
                    default:
                        break;
                }
            }

            if (Input.GetMouseButton(0))
            {

            }

            if (Input.GetMouseButtonUp(0))
            {

            }
        }
        #endregion

        #region BUTTON EVENTS
        public void OnInputModeChange(int nextState)
        {
            input_state = (INPUT_STATE)nextState;
        }
        #endregion
    }

    public enum INPUT_STATE
    {
        NONE,
        ADD_NEURON,
        REMOVE_NEURON,
        CONNECT_NEURON,
        REMOVE_CONNECTIONS,
        MARK_NEURON
    }

    public enum MARK_STATE
    {
        INPUT,
        HIDDEN,
        OUPTUT
    }
}

