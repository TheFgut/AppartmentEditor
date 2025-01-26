using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AssignRandomColorToSelectionBut : MonoBehaviour
{
    [SerializeField] private Button but;

    private WallSelectorController wallSelector;
    private TexturesPalette palette;
    private EditableWall selectedWall;
    [Inject]
    protected void Construct(WallSelectorController wallSelector, TexturesPalette palette)
    {
        this.wallSelector = wallSelector;
        this.palette = palette;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        wallSelector.onSelectionChanges += WallSelectionChaged;
        but.onClick.AddListener(AssignRandomTexture);
    }

    private void WallSelectionChaged(EditableWall wall)
    {
        selectedWall = wall;
        if(selectedWall == null) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }

    public void AssignRandomTexture()
    {
        selectedWall.SetTexture(palette.GetRandomTexture());
    }
}
