using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu]
public class CardDataBase : ScriptableObject
{
    public List<CardData> data;
}

[System.Serializable]
public class CardData
{
    //4カード名
    public string cardName = "test";
    //0グループ
    public string groupID;
    //1ID
    public int cardID = 0;
    public Sprite cardImage;
    //2カード種類
    //3カード種類ID
    public CardType cardType = CardType.None;
    //5リミテッド
    public string limited = "-";
    //6コスト
    public string cost = "X";
    //7配下_攻撃力1
    public string unit_atk1 = "X";
    //8配下_攻撃力2
    public string unit_atk2 = "X";
    //9配下_防御力
    public string unit_def = "X";
    //10金貨
    public string coin = "3";
    //11特殊効果
    //12特殊効果ID
    public Special specialID = Special.None;

    //13効果量
    public string specialValue = "X";

    //14効果テキスト
    //15効果テキスト（タグ付き）
    public string effectText = "effect";


}
