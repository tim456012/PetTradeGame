using UnityEngine;

namespace Game.Scripts.Common.State_Machine
{
    public abstract class State : MonoBehaviour
    {

        protected virtual void OnDestroy()
        {
            RemoveListeners();
        }
        public virtual void Enter()
        {
            AddListeners();
        }

        public virtual void Exit()
        {
            RemoveListeners();
        }

        protected virtual void AddListeners()
        {

        }

        protected virtual void RemoveListeners()
        {

        }
    }
}