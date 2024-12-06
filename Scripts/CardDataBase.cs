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
    //4�J�[�h��
    public string cardName = "test";
    //0�O���[�v
    public string groupID;
    //1ID
    public int cardID = 0;
    public Sprite cardImage;
    //2�J�[�h���
    //3�J�[�h���ID
    public CardType cardType = CardType.None;
    //5���~�e�b�h
    public string limited = "-";
    //6�R�X�g
    public string cost = "X";
    //7�z��_�U����1
    public string unit_atk1 = "X";
    //8�z��_�U����2
    public string unit_atk2 = "X";
    //9�z��_�h���
    public string unit_def = "X";
    //10����
    public string coin = "3";
    //11�������
    //12�������ID
    public Special specialID = Special.None;

    //13���ʗ�
    public string specialValue = "X";

    //14���ʃe�L�X�g
    //15���ʃe�L�X�g�i�^�O�t���j
    public string effectText = "effect";


}
