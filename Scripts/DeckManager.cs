using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    [SerializeField]
    private CardDataBase cardDataBase;
    [SerializeField]
    private GameObject deckArea, poolArea, deckWindow, matchingWindow;
    [SerializeField] private LocalData localData;
    [SerializeField]
    private Image finishButton, enemyImage;
    [SerializeField]
    private TextMeshProUGUI textCount, textMatchID, textMatchRule, textPlayerReady, textEnemyReady, textEnemyName;
    [SerializeField]
    private Color trueColor, falseColor, readyColor;

    private BattleManager battleManager;
    private LocalManager localManager;
    private List<CardObject> cardList = new List<CardObject>();
    private PlayerData enemyData;


    private int localID;
    private bool ready = false, initFlag = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (localData != null)
        {
            textMatchID.text = localData.roomID;
            textMatchRule.text = localData.ruleID;
        }
        textCount.text = $"デッキ枚数：{deckArea.transform.childCount}/7";
        if (battleManager != null)
        {
            if (deckArea.transform.childCount == 7)
            {
                if (!ready)
                {
                    finishButton.color = trueColor;
                    textPlayerReady.text = $"準備中({deckArea.transform.childCount}/7)";
                }
                if (ready)
                {
                    finishButton.color = readyColor;
                    textPlayerReady.text = "準備完了";

                }
            }
            else
            {
                textPlayerReady.text = $"準備中";

                if (ready)
                {
                    ready = false;
                    if (localID == 1) battleManager.playerData1.RPC_DeckReset();
                    if (localID == 2) battleManager.playerData2.RPC_DeckReset();
                }
                finishButton.color = falseColor;
            }
            if (!initFlag)
            {
                enemyData = null;
                if (localID == 1) enemyData = battleManager.playerData2;
                if (localID == 2) enemyData = battleManager.playerData1;
                textEnemyName.text = enemyData.userName.ToString();
                enemyImage.sprite = localManager.avatarSprites.data.Find(x => x.ID == enemyData.avatarId).sprite;
                initFlag = true;
            }
            if (enemyData.ready == 1)
            {
                textEnemyReady.text = "準備完了";
            }
            if (enemyData.ready == 0)
            {
                textEnemyReady.text = $"準備中...";
            }


        }
    }

    public void OpenDeckMenu(BattleManager battleManager, int localID)
    {
        deckWindow.SetActive(true);
        matchingWindow.SetActive(false);

        this.localID = localID;
        this.battleManager = battleManager;
        this.localManager = battleManager.localManager;

        PlayerData playerData = null;
        if (battleManager.playerData1.playerId == localID) { playerData = battleManager.playerData1; }
        if (battleManager.playerData2.playerId == localID) { playerData = battleManager.playerData2; }

        foreach (int i in playerData.tmpDeck)
        {
            CardCreate(localID, localID, i);

        }

    }

    /// <summary>
    /// local
    /// </summary>
    public void OpenDeckMenu(LocalManager localManager)
    {
        this.localManager = localManager;
        deckWindow.SetActive(true);
        foreach (Transform child in poolArea.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in deckArea.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (CardData i in cardDataBase.data)
        {
            CardCreate(0, 0, i.cardID);

        }

    }

    public void CloseDeckMenu()
    {
        deckWindow.SetActive(false);
        matchingWindow.SetActive(false);

    }


    /// <summary>
    /// カードオブジェクトの生成
    /// </summary>
    /// <param name="localID">誰の画面か</param>
    /// <param name="userID">所有者</param>
    /// <param name="cardArea">どこに</param>
    /// <param name="cardID">カードID</param>
    public void CardCreate(int localID, int userID, int cardID)
    {
        CardObject tmp = Instantiate(localManager.cardPrefab).GetComponent<CardObject>();

        tmp.Init(localManager, battleManager, cardDataBase.data.Find(x => x.cardID == cardID), cardList.Count, localID, userID, CardArea.Pool);
        tmp.transform.SetParent(poolArea.transform, false);
        tmp.transform.localPosition = new Vector3(0, 0, 50);
        tmp.transform.localScale = Vector3.one * 1.2f;
        localManager.SortCommmonHand(poolArea.transform);
        cardList.Add(tmp);

    }

    public void CardPick(int objectID, int localID, int userID, CardArea cardArea, int posX, CardState cardState)
    {
        if (localID != userID) { return; }
        if (deckArea.transform.childCount >= 7 & cardArea == CardArea.Deck) return;


        Debug.Log($"player{userID}の[objectID:{objectID}]を[{cardArea}]に移動");
        //battleManager.TargetPlayerData(userID).deck.IndexOf(objectID);

        //PlayerView targetView = SelectViewSide(localID, userID);
        CardObject tmp = cardList.Find(x => x.objectID == objectID & x.localID == localID & x.playerID == userID);

        if (cardArea == tmp.cardArea) return;
        if (cardArea != CardArea.Deck && cardArea != CardArea.Pool) return;

        tmp.cardArea = cardArea;

        Transform parent = null;
        float ratio = 1.2f;

        bool sortFlg = false;

        switch (cardArea)
        {
            case CardArea.Pool:
                parent = poolArea.transform;
                tmp.cardState = CardState.Open;
                //ratio = 1.0f;
                sortFlg = true;
                break;
            case CardArea.Deck:
                parent = deckArea.transform;
                tmp.cardState = CardState.Open;
                //ratio = 1.0f;
                sortFlg = true;
                break;

        }
        Debug.Log($"[{parent.name}]に移動");
        tmp.animator.SetTrigger("move");

        localManager.audioSE.PlayOneShot(localManager.audioData.audioSEClips[1]);

        tmp.transform.SetParent(parent, false);
        tmp.transform.localPosition = new Vector3(0, 0, 50);
        tmp.transform.localScale = Vector3.one * ratio;

        if (sortFlg) localManager.SortCommmonHand(parent);

        tmp.UpdateView();
    }

    public void Finish()
    {

        if (deckArea.transform.childCount != 7)
        {
            return;
        }
        if (ready)
        {
            ready = false;
            if (localID == 1) battleManager.playerData1.RPC_DeckReset();
            if (localID == 2) battleManager.playerData2.RPC_DeckReset();
            return;
        }

        List<string> strs = new List<string>();
        foreach (Transform child in deckArea.transform)
        {
            strs.Add(child.GetComponent<CardObject>().cardID.ToString());
        }

        string tmp = strs.ToSeparatedString(",");
        if (localID == 1) battleManager.playerData1.RPC_DeckSet(tmp);
        if (localID == 2) battleManager.playerData2.RPC_DeckSet(tmp);
        ready = true;
    }

}
