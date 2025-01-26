using DG.Tweening;
using EzySlice;
using HelpersFunctions;
using System;
using UnityEngine;
using Zenject;

public class EditableWall : MonoBehaviour
{
    [SerializeField] private WallConfig _config;
    private Color blinkColor = new Color(209/255f, 209/255f, 209 / 255f,1);
    private float blinkDuration = 1f;
    private Material wallMaterial;
    public Action onSelected { get; set; }
    public Action onDeselected { get; set; }
    public WallConfig config => _config;

    private void Start()
    {
        wallMaterial = GetComponent<MeshRenderer>().material;
    }

    public void Configure(WallConfig config)
    {
        this._config = config;
    }


    public void SetTexture(Texture2D texture)
    {
        wallMaterial.SetTexture("_BaseMap", texture);
    }

    public void Select()
    {
        wallMaterial.DOColor(blinkColor, blinkDuration)
         .SetLoops(-1, LoopType.Yoyo);
        gameObject.layer = Layers.Selectable;
        onSelected?.Invoke();
    }

    public void Deselect()
    {
        wallMaterial.DOKill();
        wallMaterial.color = Color.white;
        gameObject.layer = Layers.Default;
        onDeselected?.Invoke();
    }
}

[Serializable]
public struct WallConfig
{
    public Vector3 normal;//to do generate by code
}
