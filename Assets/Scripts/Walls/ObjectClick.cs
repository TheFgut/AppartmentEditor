using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EditableWall))]
public class ObjectClick : MonoBehaviour
{    
    private EditableWall editableWall;
    private WallSelectorController wallSelector;


    [Inject]
    private void Construct(WallSelectorController wallSelector)
    {
        this.wallSelector = wallSelector;
    }

    private void Awake()
    {
        editableWall = GetComponent<EditableWall>();
    }

    public void OnMouseDown()
    {
        wallSelector.TryInteract(editableWall);
    }
}
