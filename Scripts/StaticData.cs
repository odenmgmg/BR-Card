using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StaticData : ScriptableObject
{
    public List<DetailCardType> detailCardTypes;
    public List<FunctionInfo> functionInfos;
    public List<UIColor> uIColors;
}

[System.Serializable]
public class DetailCardType
{
    public CardType cardType;
    public Color32 color;
    public Sprite image;
}
[System.Serializable]
public class FunctionInfo
{
    public Special cardType;
    public string text;
    public Sprite image;

}
[System.Serializable]
public class UIColor
{
    public string text;
    public Color32 color;

}