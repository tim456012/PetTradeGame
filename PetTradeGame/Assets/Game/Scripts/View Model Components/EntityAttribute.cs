using System;
using Game.Scripts.Enum;
using Game.Scripts.EventArguments;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.View_Model_Components
{
    public class EntityAttribute : MonoBehaviour
    {

        [Header("Features")]
        public bool isDocument;
        public bool isFunctionalObject;
        public bool isDraggable;

        [ConditionalHide("isFunctionalObject", true)]
        public ObjectType objectType = ObjectType.None;

        [ConditionalHide("isDocument", true)]
        public DocumentType paperType = DocumentType.None;

        //If object enter others' collider, call ObjectController to process
        private void OnTriggerStay2D(Collider2D col)
        {
            if (!isFunctionalObject)
                return;

            FunctionalObjCollisionEvent?.Invoke(gameObject, new InfoEventArgs<GameObject>(col.gameObject));
        }
        public static event EventHandler<InfoEventArgs<GameObject>> FunctionalObjCollisionEvent;
    }
}