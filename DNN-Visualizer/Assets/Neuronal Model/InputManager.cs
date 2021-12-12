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

        bool IsDragging = false;

        Neuron connectionSource = null;
        LineRenderer connectionLine = null;
        #endregion

        #region PREFABS
        [Header("Prefabs")]
        public Neuron pf_Neuron;
        #endregion

        #region MATERIALS
        [Header("Materials")]
        public Material ConnectionMaterial;
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

            /* If we are over a UI component, ignore any mouse actions */
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

            /* Handle the mouse actions */
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
                        if (hitinfo.collider != null)
                        {
                            Neuron clicked = hitinfo.collider.gameObject.GetComponent<Neuron>();
                            if(clicked != null)
                            {
                                Model.neuronList.Remove(clicked);
                                Model.inputNeurons.Remove(clicked);
                                Destroy(clicked.gameObject);
                            }
                        }
                        break;
                    case INPUT_STATE.CONNECT_NEURON:
                        if (hitinfo.collider != null)
                        {
                            Neuron clicked = hitinfo.collider.gameObject.GetComponent<Neuron>();
                            if (clicked != null)
                            {
                                /* Start the drag */
                                IsDragging = true;
                                connectionSource = clicked;

                                /* Generate a connection line */
                                connectionLine = new GameObject("line connector").AddComponent<LineRenderer>();
                                connectionLine.transform.SetParent(connectionSource.transform);
                                connectionLine.positionCount = 2;
                                connectionLine.widthMultiplier = 0.1f;
                                connectionLine.material = ConnectionMaterial;
                                connectionLine.SetPositions(new Vector3[] { connectionSource.transform.position, connectionSource.transform.position });
                            }
                        }
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
                switch (input_state)
                {
                    case INPUT_STATE.CONNECT_NEURON:
                        if (IsDragging)
                        {
                            connectionLine.SetPosition(1, mousePosition);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                switch (input_state)
                {
                    case INPUT_STATE.CONNECT_NEURON:
                        bool success = false;
                        if (hitinfo.collider != null)
                        {
                            Neuron clicked = hitinfo.collider.gameObject.GetComponent<Neuron>();
                            if (clicked != null && !clicked.Incoming.Contains(connectionSource) && clicked != connectionSource)
                            {
                                /* Connect the two neurons if they haven't already been connected */
                                success = true;
                                connectionSource.Outgoing.Add(clicked);
                                clicked.Incoming.Add(connectionSource);
                                clicked.Weights.Add(Random.Range(-1f, 1f));
                                connectionLine.SetPosition(1, clicked.transform.position);
                            }
                        }

                        /* Reset the drag */
                        IsDragging = false;
                        connectionSource = null;
                        if (!success) Destroy(connectionLine.gameObject);
                        connectionLine = null;
                        break;
                    default:
                        break;
                }
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

