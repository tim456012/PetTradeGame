using System;
using Game.Scripts.Enum;
using UnityEngine;

namespace Game.Scripts.Controller
{
    /// <summary>
    /// This sub-controller will responsible for all gameObject interaction behavior.
    /// </summary>
    public class InteractionController : MonoBehaviour
    {
        public bool DetectCollision(ObjectType objectType, ObjectType collided)
        {
            //Collider2D self = obj.GetComponent<Collider2D>();

            /*foreach (Poolable other in instances)
            {
                if (other.gameObject == obj)
                    continue;

                Collider2D target = other.GetComponent<Collider2D>();

                if (!target.bounds.Intersects(self.bounds)) 
                    continue;

                collided = target.gameObject;
                return true;
            }*/
            return false;
        }
    }
}
