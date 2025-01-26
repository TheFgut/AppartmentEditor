using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSelectorController : MonoBehaviour, IActivatable
{
    private EditableWall selection;
    private bool activated;

    public EditableWall selected => selection;
    public Action<EditableWall> onSelectionChanges {  get; set; }
    #region activation

    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }

    #endregion

    public void TryInteract(EditableWall iteractingObj)
    {
        if (!activated) return;
        if(selection == iteractingObj)
        {
            iteractingObj.Deselect();
            selection = null;
            onSelectionChanges?.Invoke(null);
            return;
        }
        if(selection != null) selection.Deselect();
        selection = iteractingObj;
        iteractingObj.Select();
        onSelectionChanges?.Invoke(iteractingObj);
    }

}
