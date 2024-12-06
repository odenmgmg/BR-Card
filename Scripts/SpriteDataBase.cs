using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpriteDataBase : ScriptableObject
{
    public List<SpriteData> data;
}

[System.Serializable]
public class SpriteData
{
    public string fileName;
    public int ID = 0;
    [SerializeField]
    public Sprite sprite;
}

