using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityObject", menuName = "EntityObject")]
public class EntityObject : ScriptableObject
{
    public string GameObjectName;
    public string Description;

    public bool Draggable = false;
}
