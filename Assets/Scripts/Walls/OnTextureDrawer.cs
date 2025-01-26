using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTextureDrawer : MonoBehaviour
{
    [SerializeField] private int totalXPixels = 1024;
    [SerializeField] private int totalYPixels = 1024;
    [SerializeField] private int brushSize = 1;
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private Texture2D generatedTexture;

    [SerializeField] private Transform topLeftCorner;
    [SerializeField] private Transform bottomRightCorner;
    [SerializeField] private Transform point;

    private int xPixel, yPixel;
    private float xMult, yMult;

    private Color clearColor = new Color(1, 0, 0, 0);

    Color[] colorMap;

    // Start is called before the first frame update
    private void Start()
    {
        colorMap = new Color[totalXPixels * totalYPixels];
        generatedTexture = new Texture2D(totalYPixels, totalXPixels, TextureFormat.RGBA32, false);
        generatedTexture.filterMode = FilterMode.Trilinear;
        renderer.material.SetTexture("_BaseMap", generatedTexture);

        ResetColor();

        xMult = totalXPixels / (bottomRightCorner.localPosition.x - topLeftCorner.localPosition.x);
        yMult = totalYPixels / (bottomRightCorner.localPosition.z - topLeftCorner.localPosition.z);
    }

    public void DrawPoint(Vector3 drawPoint, Transform targetTransform, Texture2D texture)
    {
        point.position = drawPoint;
        xPixel = (int)((point.localPosition.x - topLeftCorner.localPosition.x) * xMult);
        yPixel = (int)((point.localPosition.z - topLeftCorner.localPosition.z) * yMult);

        var inverse = targetTransform.InverseTransformPoint(xPixel, yPixel, 0);

        xPixel = (int)inverse.x;
        yPixel = (int)inverse.z;
        ChangePixelsAroundPoint(texture);
    }

    private void ChangePixelsAroundPoint(Texture2D texture)
    {
        DrawBrush(xPixel, yPixel, texture);
        SetTexture();
    }

    private void SetTexture()
    {
        generatedTexture.SetPixels(colorMap);
        generatedTexture.Apply();
    }

    private void ResetColor()
    {
        for (int i = 0; i < colorMap.Length; i++)
        {
            colorMap[i] = clearColor;
        }

        SetTexture();
    }

    private void DrawBrush(int xPix, int yPix, Texture2D projTexture)
    {
        Color[] projPixels = projTexture.GetPixels();

        var i = xPix - brushSize + 1;
        var j = yPix - brushSize + 1;
        var maxi = xPix + brushSize - 1;
        var maxj = yPix + brushSize - 1;

        if (i < 0)
            i = 0;

        if (j < 0)
            j = 0;

        if (maxi >= totalXPixels)
            maxi = totalXPixels - 1;

        if (maxj >= totalYPixels)
            maxj = totalYPixels - 1;

        for (int x = i; x <= maxi; x++)
        {
            for (int y = j; y <= maxj; y++)
            {
                if ((x - xPix) * (x - xPix) + (y - yPix) * (y - yPix) <= brushSize * brushSize)
                    colorMap[x * totalYPixels + y] = projPixels[x * totalYPixels + y];
            }
        }
    }
}
