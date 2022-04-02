using System;
using System.Collections;
using Game.Scripts.Controller;
using UnityEngine;
using Game.Scripts.Tools;
using Game.Scripts.Enum;
using Game.Scripts.EventArguments;

namespace Game.Scripts.View_Model_Components
{
    public class EntityAttribute : MonoBehaviour
    {
        public static event EventHandler<InfoEventArgs<GameObject>> FunctionalObjCollisionEvent;
        
        [Header("Features")]
        public bool isDocument;
        public bool isFunctionalObject = false;
        public bool isDraggable = false;

        [ConditionalHide("isFunctionalObject", true)]
        public ObjectType objectType = ObjectType.None;
        
        [ConditionalHide("isDocument", true)]
        public PaperType paperType = PaperType.None;
        
        //If object enter others' collider, call ObjectController to process
        private void OnTriggerStay2D(Collider2D col)
        {
            if (!isFunctionalObject)
                return;
            
            FunctionalObjCollisionEvent?.Invoke(gameObject, new InfoEventArgs<GameObject>(col.gameObject));
        }
    }
}
