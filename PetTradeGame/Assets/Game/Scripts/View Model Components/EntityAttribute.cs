using System;
using Game.Scripts.Controller;
using UnityEngine;
using Game.Scripts.Tools;
using Game.Scripts.Enum;

namespace Game.Scripts.View_Model_Components
{
    public class EntityAttribute : MonoBehaviour
    {
        [Header("Features")]
        public bool isDocument;
        public bool isFunctionalObject = false;
        public bool isDraggable = false;

        [ConditionalHide("isFunctionalObject", true)]
        public ObjectType objectType = ObjectType.None;
        
        [ConditionalHide("isDocument", true)]
        public PaperType paperType = PaperType.None;
        
        //If object enter others' collider, call ObjectController to process
        private void OnTriggerEnter2D(Collider2D col)
        {
            //if(isFunctionalObject)
                //ObjectController.ProcessCollision(objectType, col);
        }
    }
}
