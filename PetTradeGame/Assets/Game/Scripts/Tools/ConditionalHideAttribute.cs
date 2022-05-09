using System;
using UnityEngine;

//Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//Modified by: Danny Chan

namespace Game.Scripts.Tools
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        public readonly string ConditionalSourceField = "";
        public string ConditionalSourceField2 = "";
        public readonly string[] ConditionalSourceFields = { };
        public readonly bool[] ConditionalSourceFieldInverseBools = { };
        public readonly bool HideInInspector = false;
        public readonly bool Inverse = false;
        public bool UseOrLogic = false;

        public bool InverseCondition1 = false;
        public bool InverseCondition2 = false;


        // Use this for initialization

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditionalSourceField"></param>
        public ConditionalHideAttribute(string conditionalSourceField)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = false;
            Inverse = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditionalSourceField"></param>
        /// <param name="hideInInspector"></param>
        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = hideInInspector;
            Inverse = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditionalSourceField"></param>
        /// <param name="hideInInspector"></param>
        /// <param name="inverse"></param>
        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector, bool inverse)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = hideInInspector;
            Inverse = inverse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hideInInspector"></param>
        public ConditionalHideAttribute(bool hideInInspector = false)
        {
            ConditionalSourceField = "";
            HideInInspector = hideInInspector;
            Inverse = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditionalSourceFields"></param>
        /// <param name="conditionalSourceFieldInverseBools"></param>
        /// <param name="hideInInspector"></param>
        /// <param name="inverse"></param>
        public ConditionalHideAttribute(string[] conditionalSourceFields, bool[] conditionalSourceFieldInverseBools, bool hideInInspector, bool inverse)
        {
            ConditionalSourceFields = conditionalSourceFields;
            ConditionalSourceFieldInverseBools = conditionalSourceFieldInverseBools;
            HideInInspector = hideInInspector;
            Inverse = inverse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditionalSourceFields"></param>
        /// <param name="hideInInspector"></param>
        /// <param name="inverse"></param>
        public ConditionalHideAttribute(string[] conditionalSourceFields, bool hideInInspector, bool inverse)
        {
            ConditionalSourceFields = conditionalSourceFields;
            HideInInspector = hideInInspector;
            Inverse = inverse;
        }
    }
}
