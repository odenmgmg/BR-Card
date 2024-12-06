using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class LocalData : ScriptableObject
{
    public string userName = "user";
    public string roomID = "roomID";
    public string ruleID = "ruleID";
    public bool isMatching = false;
    public int avatarID;
    public float bgm, se;
    public int health = 15;
    public int mana = 0;
    public int attack = 0;
    public int deffence = 0;
    [SerializeField]
    public List<int> deck;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
