using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace neuronal
{
    public class ModelAction
    {
        /// <summary>
        /// What type of action was taken.
        /// </summary>
        public ModelActionType ActionType;

        /// <summary>
        /// What neuron was affected by this action.
        /// </summary>
        public Neuron Affected;

        /// <summary>
        /// What (if any) was the source of this neurons action.
        /// </summary>
        public Neuron Source;

        /// <summary>
        /// The pseudo chronological time this action was taken relative
        /// to other actions.
        /// </summary>
        public int Timestamp;

        public ModelAction(ModelActionType actionType, Neuron affected, Neuron source, int timestamp)
        {
            ActionType = actionType;
            Affected = affected;
            Source = source;
            Timestamp = timestamp;
        }

        public override string ToString()
        {
            return "[" + Timestamp + "] " + ActionType.ToString() + " " + Affected.name;
        }
    }

    public enum ModelActionType
    {
        CALCULATED_OUTPUT,
        CALCULATED_ERROR,
        UPDATED_PARAMS,

    }
}

