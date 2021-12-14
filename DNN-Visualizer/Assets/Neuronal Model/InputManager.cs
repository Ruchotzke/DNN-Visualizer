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
        public Label pf_Label;
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

        #region INPUTS AND OUTPUTS
        [Header("Inputs and Outputs")]
        public Color HiddenColor;
        public Color InputColor;
        public Color OutputColor;

        public Color WeightLabelColor;
        public Color ActivationLabelColor;
        public Color InputLabelColor;
        public Color OutputLabelColor;
        #endregion

        Neuron firstAddition = null;
        AdditionDataset additionDataset = null;

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
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

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
                            neuron.Name.text = neuron.name.Substring(neuron.name.IndexOf(' ') + 1);
                            neuron.transform.position = mousePosition;

                            /* Make sure the neuron has the same "step" as the other neurons */
                            if(firstAddition != null)
                            {
                                neuron.InferenceStepCount = firstAddition.InferenceStepCount;
                            }
                            else
                            {
                                firstAddition = neuron;
                            }
                        }
                        break;
                    case INPUT_STATE.REMOVE_NEURON:
                        if (hitinfo.collider != null)
                        {
                            Neuron clicked = hitinfo.collider.gameObject.GetComponent<Neuron>();
                            if(clicked != null)
                            {
                                /* Remove the corresponding outputs from its ancestors */
                                foreach(var neuron in clicked.Incoming)
                                {
                                    neuron.Outgoing.Remove(clicked);
                                    Transform lr = neuron.transform.Find("LINE: " + clicked.name);
                                    if(lr != null) Destroy(lr.gameObject);
                                }

                                /* Remove this neuron and it's weight from descendents */
                                foreach(var neuron in clicked.Outgoing)
                                {
                                    int index = neuron.Incoming.IndexOf(clicked);
                                    neuron.Incoming.RemoveAt(index);
                                    neuron.Weights.RemoveAt(index);
                                }

                                /* Finally, remove the neuron */
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
                                /* Clean up any loose drags */
                                if(connectionLine != null) Destroy(connectionLine.gameObject);

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
                        if (hitinfo.collider != null)
                        {
                            Neuron clicked = hitinfo.collider.gameObject.GetComponent<Neuron>();
                            if (clicked != null)
                            {
                                Label label = clicked.activationLabel;
                                if (label == null)
                                {
                                    label = Instantiate(pf_Label, clicked.transform);
                                    label.name = "Label";
                                    label.gameObject.SetActive(false);
                                    clicked.activationLabel = label;
                                }

                                /* Alter the state of this neuron. */
                                if (clicked.IsInputNeuron)
                                {
                                    label.transform.position = new Vector3(clicked.transform.position.x + clicked.transform.localScale.x, clicked.transform.position.y, clicked.transform.position.z);
                                    label.gameObject.SetActive(true);
                                    clicked.IsInputNeuron = false;
                                    clicked.IsOutputNeuron = true;
                                    clicked.Center.color = OutputColor;
                                    Model.inputNeurons.Remove(clicked);
                                    Model.outputNeurons.Add(clicked);

                                    /* sort the neurons */
                                    Model.outputNeurons.Sort((a, b) =>
                                    {
                                        return (a.transform.position.y >= b.transform.position.y) ? -1 : 1;
                                    });

                                    /* Output neurons have no activation here */
                                    clicked.Activation = DNN_V2.ActivationFunction.LINEAR;
                                }
                                else if (clicked.IsOutputNeuron)
                                {
                                    label.gameObject.SetActive(false);
                                    clicked.IsOutputNeuron = false;
                                    clicked.IsInputNeuron = false;
                                    clicked.Center.color = HiddenColor;
                                    Model.outputNeurons.Remove(clicked);

                                    /* hidden/input neurons have sigmoid activation */
                                    clicked.Activation = DNN_V2.ActivationFunction.SIGMOID;
                                }
                                else
                                {
                                    label.transform.position = new Vector3(clicked.transform.position.x - clicked.transform.localScale.x, clicked.transform.position.y, clicked.transform.position.z);
                                    label.gameObject.SetActive(true);
                                    clicked.IsInputNeuron = true;
                                    clicked.IsOutputNeuron = false;
                                    clicked.Center.color = InputColor;
                                    Model.inputNeurons.Add(clicked);

                                    /* sort the neurons */
                                    Model.inputNeurons.Sort((a, b) =>
                                    {
                                        return (a.transform.position.y >= b.transform.position.y) ? -1 : 1;
                                    });
                                }
                            }
                        }
                        break;
                    case INPUT_STATE.MOVE_NEURON:
                        if (hitinfo.collider != null)
                        {
                            Neuron clicked = hitinfo.collider.gameObject.GetComponent<Neuron>();
                            if (clicked != null)
                            {
                                /* Start the drag */
                                IsDragging = true;
                                connectionSource = clicked;
                            }
                        }
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
                    case INPUT_STATE.MOVE_NEURON:
                        if (IsDragging)
                        {
                            connectionSource.transform.position = mousePosition;
                            foreach (var neuron in connectionSource.Incoming)
                            {
                                neuron.transform.Find("LINE: " + connectionSource.name).GetComponent<LineRenderer>().SetPosition(1, connectionSource.transform.position);
                            }
                            foreach (var lr in connectionSource.transform.GetComponentsInChildren<LineRenderer>())
                            {
                                lr.SetPosition(0, connectionSource.transform.position);
                            }
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
                        if (IsDragging)
                        {
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
                                    connectionLine.name = "LINE: " + clicked.name;

                                    /* If we were successful, add a weight label */
                                    var label = Instantiate(pf_Label, connectionSource.transform);
                                    label.name = "Weight Label: " + clicked.name;
                                    label.transform.position = Vector3.Lerp(connectionSource.transform.position, clicked.transform.position, 0.3f);
                                    label.SetText(clicked.Weights[clicked.Incoming.IndexOf(connectionSource)], WeightLabelColor);
                                }
                            }

                            
                        }

                        /* Reset the drag */
                        IsDragging = false;
                        if (!success && connectionLine != null)
                            Destroy(connectionLine.gameObject);
                        connectionSource = null;
                        connectionLine = null;

                        break;
                    case INPUT_STATE.MOVE_NEURON:
                        if (IsDragging)
                        {
                            connectionSource.transform.position = mousePosition;
                            foreach (var neuron in connectionSource.Incoming)
                            {
                                neuron.transform.Find("LINE: " + connectionSource.name).GetComponent<LineRenderer>().SetPosition(1, connectionSource.transform.position);
                            }
                            foreach (var lr in connectionSource.transform.GetComponentsInChildren<LineRenderer>())
                            {
                                lr.SetPosition(0, connectionSource.transform.position);
                            }
                        }

                        /* Reset the drag */
                        IsDragging = false;
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
            if (nextState == (int)input_state)
            {
                input_state = INPUT_STATE.NONE;
            }
            else
            {
                input_state = (INPUT_STATE)nextState;
            }
        }

        public void OnInferencePress()
        {
            /* First make sure the editing is off */
            input_state = INPUT_STATE.NONE;

            /* Also make sure we have a dataset */
            if (additionDataset == null) additionDataset = new AdditionDataset(new Vector2(-1.0f, 1.0f), 100);

            /* Select and input and pass it into the input nodes */
            var datapoint = additionDataset.dataset[Random.Range(0, additionDataset.dataset.Count)];
            Model.lastInput = datapoint.inputs;
            Model.lastOutput = new float[] { datapoint.output };

            /* Perform an inference */
            Model.DoInference(datapoint.inputs);
        }

        public void OnBackpropagationPress()
        {
            /* First make sure the editing is off */
            input_state = INPUT_STATE.NONE;

            /* Tell the system to perform a single backprop */
            Model.DoBackpropagation(Model.lastOutput);
        }

        public void OnTrainEpochs(int numEpochs)
        {
            for(int epoch = 0; epoch < numEpochs; epoch++)
            {
                /* For each epoch do one inference and backprop per dataset */
                for(int sample = 0; sample < 100; sample++)
                {
                    /* Inference */
                    var datapoint = additionDataset.dataset[sample];
                    Model.lastInput = datapoint.inputs;
                    Model.lastOutput = new float[] { datapoint.output };
                    Model.DoInference(datapoint.inputs);

                    /* Backprop */
                    Model.DoBackpropagation(Model.lastOutput);
                }

                /* After an epoch, calculate loss */
                List<float> outputs = new List<float>();
                List<float> correct = new List<float>();
                for(int i = 0; i < 100; i++)
                {
                    var datapoint = additionDataset.dataset[i];
                    Model.DoInference(datapoint.inputs);
                    outputs.Add(Model.outputNeurons[0].Output);
                    correct.Add(datapoint.output);
                }
                float loss = Error.MSE(outputs, correct);
                Debug.Log("Epoch: " + epoch + " Loss: " + loss);
            }
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
        MARK_NEURON,
        MOVE_NEURON
    }

    public enum MARK_STATE
    {
        INPUT,
        HIDDEN,
        OUPTUT
    }
}

