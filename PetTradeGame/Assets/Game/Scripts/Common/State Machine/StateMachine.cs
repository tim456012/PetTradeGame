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
        protected bool _Transition;

        public virtual T GetState<T>() where T : State
        {
            var target = GetComponent<T>();
            if (target == null)
            {
                target = gameObject.AddComponent<T>();
            }
            return target;
        }

        public virtual void ChangeState<T>() where T : State
        {
            CurrentState = GetState<T>();
        }

        protected virtual void Transition(State value)
        {
            if (_currentState == value || _Transition)
                return;

            _Transition = true;
            
            if (_currentState != null)
            {
                _currentState.Exit();
            }

            _currentState = value;

            if (_currentState != null)
            {
                _currentState.Enter();
            }

            _Transition = false;
        }
    }
}
