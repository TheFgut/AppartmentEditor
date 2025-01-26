using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturesPalette : MonoBehaviour
{
    [SerializeField] private List<Texture2D> allTextures;
    private int randomTextureNum;
    public Texture2D[] GetAllAwailableTextures() => allTextures.ToArray();

    public Texture2D GetTextureByName(string textureName)
    {
        Texture2D currentTexture = allTextures.Find(x => x.name == textureName);
        return currentTexture;
    }

    public Texture2D GetRandomTexture()
    {
        int prewRandTextureNum = randomTextureNum;
        if (allTextures.Count == 1)
        {
            prewRandTextureNum = 0;
            return allTextures[0];
        }

        do
        {
            randomTextureNum = UnityEngine.Random.Range(0, allTextures.Count);

        } while (randomTextureNum == prewRandTextureNum);
        return allTextures[randomTextureNum];
    }
}