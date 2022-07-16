using System;
using System.Collections.Generic;
using Game.Scripts.Common.Animation;
using UnityEngine;

namespace Game.Scripts.Common.UI
{
    /// <summary>
    ///     The component to define target positions and work with LayoutAnchor to snap or move the UI GameObjet.
    /// </summary>
    [RequireComponent(typeof(LayoutAnchor))]
    public class Panel : MonoBehaviour
    {

        [SerializeField] private List<Position> positionList;
        private LayoutAnchor _anchor;
        private Dictionary<string, Position> _positionMap;
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
            //If no Position has been init when reaching the Start method, assign the first Position as the default.
            if (CurrentPosition == null && positionList.Count > 0)
            {
                SetPosition(positionList[0], false);
            }
        }

        /// <summary>
        ///     Add a new Position dynamically in the scripts.
        /// </summary>
        /// <param name="p">The Position.</param>
        public void AddPosition(Position p)
        {
            _positionMap[p.name] = p;
        }

        /// <summary>
        ///     Remove the Position dynamically in the scripts.
        /// </summary>
        /// <param name="p">The Position.</param>
        public void RemovePosition(Position p)
        {
            if (_positionMap.ContainsKey(p.name))
            {
                _positionMap.Remove(p.name);
            }
        }

        /// <summary>
        ///     Move the Panel to specified positions.
        /// </summary>
        /// <param name="positionName">The name of specified position.</param>
        /// <param name="animated">Does it have animation?</param>
        /// <returns>Panel.Transition / Null</returns>
        public Tweener SetPosition(string positionName, bool animated)
        {
            return SetPosition(this[positionName], animated);
        }

        /// <summary>
        ///     Move the Panel to specified positions.
        /// </summary>
        /// <param name="p">The specified position.</param>
        /// <param name="animated">Does it have animation?</param>
        /// <returns>Panel.Transition / Null</returns>
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
            _anchor.SnapToAnchorPosition(p.anchor, p.parentAnchor, p.offset);
            return null;
        }
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
    }
}