using UnityEngine;

namespace Assets.Game.Scripts.Common.State_Machine
{
    public abstract class State : MonoBehaviour
    {
        public virtual void Enter()
        {
            AddListeners();
        }

        public virtual void Exit()
        {
            RemoveListeners();
        }

        protected virtual void OnDestroy()
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
