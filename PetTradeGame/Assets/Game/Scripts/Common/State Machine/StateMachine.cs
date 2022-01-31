using UnityEngine;
using System.Collections;

namespace Assets.Game.Scripts.Common.State_Machine
{
    public class StateMachine : MonoBehaviour
    {
        public virtual State CurrentState
        {
            get => _currentState;
            set => Transition(value);
        }
        protected State _currentState;
        protected bool _transition;

        /// <summary>
        /// Get the specific state on the controller. Add as new component if it doesn't exists.  
        /// </summary>
        /// <typeparam name="T">The State class name</typeparam>
        /// <returns>T</returns>
        public virtual T GetState<T>() where T : State
        {
            var target = GetComponent<T>();
            if (target == null)
            {
                target = gameObject.AddComponent<T>();
            }
            return target;
        }

        /// <summary>
        /// Change the current state to target state.
        /// </summary>
        /// <typeparam name="T">The State class name</typeparam>
        public virtual void ChangeState<T>() where T : State
        {
            CurrentState = GetState<T>();
        }

        protected virtual void Transition(State value)
        {
            if (_currentState == value || _transition)
                return;

            _transition = true;
            
            if (_currentState != null)
            {
                _currentState.Exit();
            }

            _currentState = value;

            if (_currentState != null)
            {
                _currentState.Enter();
            }

            _transition = false;
        }
    }
}
