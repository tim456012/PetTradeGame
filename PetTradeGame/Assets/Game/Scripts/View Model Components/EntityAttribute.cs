using System;
using Game.Scripts.Enum;
using Game.Scripts.EventArguments;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.View_Model_Components
{
    public class EntityAttribute : MonoBehaviour
    {
        public bool isDocument;
        public bool isFunctionalObject;
        public bool isDraggable;
        
        [ConditionalHide("isFunctionalObject", true)]
        public ObjectType objectType = ObjectType.None;
        
        [ConditionalHide("isDocument", true)]
        public DocumentType paperType = DocumentType.None;
        
        public static event EventHandler<InfoEventArgs<GameObject>> FunctionalObjCollisionEvent;
        
        //If object enter others' collider, call ObjectController to process
        private void OnTriggerStay2D(Collider2D col)
        {
            var collided = col.GetComponent<EntityAttribute>();
            if(!collided || collided.objectType is ObjectType.None || objectType is ObjectType.License)
                return;

            FunctionalObjCollisionEvent?.Invoke(gameObject, new InfoEventArgs<GameObject>(collided.gameObject));
        }
    }
}