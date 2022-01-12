using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Game.Scripts.Common.Animation;
using UnityEngine;

namespace Assets.Game.Scripts.Common.UI
{
    [RequireComponent(typeof(LayoutAnchor))]
    public class Panel : MonoBehaviour
    {
        [Serializable] public class Position
        {
            public string name;
            public TextAnchor anchor;
            public TextAnchor parentAnchor;
            public Vector2 offset;

            public Position(string name)
            {
                this.name = name;
            }

            public Position(string name, TextAnchor anchor, TextAnchor parentAnchor) : this(name)
            {
                this.anchor = anchor;
                this.parentAnchor = parentAnchor;
            }

            public Position(string name, TextAnchor anchor, TextAnchor parentAnchor, Vector2 offset) 
                : this(name, anchor, parentAnchor)
            {
                this.offset = offset;
            }
        }
        
        [SerializeField] private List<Position> positionList;
        private Dictionary<string, Position> _positionMap;
        private LayoutAnchor _anchor;
        public Position CurrentPosition { get; private set; }
        public Tweener Transition { get; private set; }
        public bool InTransition => Transition != null;
        public Position this[string name] => _positionMap.ContainsKey(name) ? _positionMap[name] : null;

        private void Awake()
        {
            _anchor = GetComponent<LayoutAnchor>();
            _positionMap = new Dictionary<string, Position>(positionList.Count);
            for (int i = positionList.Count - 1; i >= 0; --i)
            {
                AddPosition(positionList[i]);
            }
        }

        private void Start()
        {
            if (CurrentPosition == null && positionList.Count > 0)
            {
                SetPosition(positionList[0], false);
            }
        }

        public void AddPosition(Position p)
        {
            _positionMap[p.name] = p;
        }

        public void RemovePosition(Position p)
        {
            if (_positionMap.ContainsKey(p.name))
            {
                _positionMap.Remove(p.name);
            }
        }

        public Tweener SetPosition(string positionName, bool animated)
        {
            return SetPosition(this[positionName], animated);
        }

        public Tweener SetPosition(Position p, bool animated)
        {
            CurrentPosition = p;
            if (CurrentPosition == null)
                return null;
            
            if (InTransition)
            {
                Transition.easingControl.Stop();
            }

            if (animated)
            {
                Transition = _anchor.MoveToAnchorPosition(p.anchor, p.parentAnchor, p.offset);
                return Transition;
            }
            else
            {
                _anchor.SnapToAnchorPosition(p.anchor, p.parentAnchor, p.offset);
                return null;
            }
        }
    }
}
