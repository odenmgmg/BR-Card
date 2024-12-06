using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalManager : MonoBehaviour
{
    [SerializeField]
    public TurnView turnView;
    [SerializeField]
    public TimeIcon timeIcon;
    [SerializeField]
    public PlayerView playerBottomView, playerUpperView;
    [SerializeField]
    public GameObject cardPrefab;
    [SerializeField]
    public CardDataBase cardDataBase;
    [SerializeField]
    public SpriteDataBase avatarSprites;
    [SerializeField]
    public SpriteDataBase iconSprites;
    [SerializeField]
    public StaticData staticData;
    [SerializeField]
    private BattleManager battleManager;
    [SerializeField]
    public DeckManager deckManager;
    [SerializeField]
    private Info info;
    [SerializeField]
    public CSVDataIO csvData;
    [SerializeField]
    public AudioSource audioBGM, audioSE;
    [SerializeField]
    public AudioData audioData;

    private List<CardObject> cardList = new List<CardObject>();

    public CardObject activeCard;

    public void Update()
    {
    }

    public int GetTurnPlayer()
    {
        return battleManager.TurnPlayer;
    }

    /// <summary>
    /// 表示するサイドの判定
    /// </summary>
    /// <param name="localID">誰の画面か</param>
    /// <param name="userID">入力者のID</param>
    public PlayerView SelectViewSide(int localID, int userID)
    {
        if (localID >= 3)
        {
            if (userID == 1) return playerBottomView;
            return playerUpperView;

        }
        if (localID == userID) return playerBottomView;
        return playerUpperView;
    }

    /// <summary>
    /// 表示するサイドの判定
    /// </summary>
    /// <param name="localID">誰の画面か</param>
    /// <param name="userID">入力者のID</param>
    /// <param name="areaID">表示エリアのID</param>
    private PlayerView SelectViewSide(int localID, int userID, int areaID)
    {

        //if (localID == userID)
        {
            if (areaID == playerBottomView.playerId) return playerBottomView;
            return playerUpperView;
        }

    }

    /// <summary>
    /// カードオブジェクトの生成
    /// </summary>
    /// <param name="localID">誰の画面か</param>
    /// <param name="userID">所有者</param>
    /// <param name="cardArea">どこに</param>
    /// <param name="cardID">カードID</param>
    public void CardCreate(int localID, int userID, CardArea cardArea, int cardID)
    {
        PlayerView targetView = SelectViewSide(localID, userID);
        CardObject tmp = Instantiate(cardPrefab).GetComponent<CardObject>();
        tmp.Init(this, battleManager, cardDataBase.data.Find(x => x.cardID == cardID), cardList.Count, localID, userID, cardArea);
        if (cardArea == CardArea.Hand) tmp.transform.SetParent(targetView.handArea.transform, false);
        if (cardArea == CardArea.CommonHand) tmp.transform.SetParent(targetView.commonHandArea.transform, false);
        tmp.transform.localPosition = new Vector3(0, 0, 50);
        tmp.transform.localScale = Vector3.one;
        SortCommmonHand(targetView.handArea.transform);
        cardList.Add(tmp);
    }

    public void CardSelect(int localID, int userID, CardState cardState, CardData cardData, CardObject cardObject)
    {
        audioSE.PlayOneShot(audioData.audioSEClips[2]);
        if (cardState == CardState.UserOnlyOpen & localID != userID) return;
        activeCard = cardObject;
        info.UpdateInfo(cardObject, this, battleManager);
    }

    public void CardMove(int objectID, int localID, int userID, CardArea cardArea, int posX, CardState cardState, int areaID)
    {

        //if (cardArea == CardArea.Hand & localID != userID) { return; }


        Debug.Log($"player{userID}の[objectID:{objectID}]を[{cardArea}:id{areaID}]に移動");
        //battleManager.TargetPlayerData(userID).deck.IndexOf(objectID);

        PlayerView targetView = SelectViewSide(localID, userID, areaID);
        CardObject tmp = cardList.Find(x => x.objectID == objectID & x.playerID == userID & x.localID == localID);

        if (cardArea == tmp.cardArea) return;
        if (cardArea == CardArea.Hand & tmp.cardData.cardType == CardType.None) return;
        if (cardArea == CardArea.CommonHand & tmp.cardData.cardType != CardType.None) return;

        tmp.cardArea = cardArea;

        if (tmp.cardArea != CardArea.Hand && tmp.cardData.cardType == CardType.Tactics) cardState = CardState.UserOnlyOpen;

        Transform parent = null;
        float ratio = 1.0f;

        bool sortFlg = false;
        audioSE.PlayOneShot(audioData.audioSEClips[0]);

        switch (cardArea)
        {
            case CardArea.Hand:
                parent = targetView.handArea.transform;
                tmp.cardState = CardState.UserOnlyOpen;
                ratio = 1.0f;
                sortFlg = true;
                break;
            case CardArea.FieldFront:
                parent = targetView.unitAreaFront.transform;
                ratio = 1.3f;
                tmp.cardState = cardState;
                if (localID == userID)
                {

                    //加熱
                    if (tmp.cardData.specialID == Special.CommonAtk)
                    {
                        if (tmp.playerID == 1) battleManager.playerData1.RPC_ChangePlayerData(tmp.playerID, PlayerStatus.Attack, 1);
                        if (tmp.playerID == 2) battleManager.playerData2.RPC_ChangePlayerData(tmp.playerID, PlayerStatus.Attack, 1);

                    }
                    //加護
                    else if (tmp.cardData.specialID == Special.CommonDef)
                    {
                        if (tmp.playerID == 1) battleManager.playerData1.RPC_ChangePlayerData(tmp.playerID, PlayerStatus.Deffence, tmp.specialValue, true);
                        if (tmp.playerID == 2) battleManager.playerData2.RPC_ChangePlayerData(tmp.playerID, PlayerStatus.Deffence, tmp.specialValue, true);

                    }
                }

                break;
            case CardArea.FieldBack:
                parent = targetView.unitAreaBack.transform;
                ratio = 1.0f;
                tmp.cardState = cardState;
                break;
            case CardArea.Trush:
                parent = targetView.trushArea.transform;
                tmp.cardState = CardState.Open;
                ratio = 1.0f;
                break;
            case CardArea.CommonHand:
                parent = targetView.commonHandArea.transform;
                tmp.cardState = CardState.Open;
                ratio = 1.0f;
                sortFlg = true;
                if (tmp.cardData.specialID == Special.CommonDef) tmp.specialValue = 0;
                break;

        }
        Debug.Log($"[{parent.name}]に移動");
        tmp.animator.SetTrigger("move");

        tmp.transform.SetParent(parent, false);
        tmp.transform.localPosition = new Vector3(0, 0, 50);
        tmp.transform.localScale = Vector3.one * ratio;

        if (sortFlg) SortCommmonHand(parent);

        tmp.UpdateView();
    }

    public void CardExec(int objectID, int localID, int userID, CardArea cardArea)
    {
        //if (cardArea == CardArea.Hand & localID != userID) { return; }


        Debug.Log($"player{userID}の[objectID:{objectID}]を反転");
        //battleManager.TargetPlayerData(userID).deck.IndexOf(objectID);

        PlayerView targetView = SelectViewSide(localID, userID);
        CardObject tmp = cardList.Find(x => x.objectID == objectID & x.playerID == userID & x.localID == localID);


        //布石の場合
        if (tmp.cardData.cardType == CardType.Tactics)
        {
            if (cardArea == CardArea.Hand & localID != userID)
            {

            }
            else
            {

                audioSE.PlayOneShot(audioData.audioSEClips[1]);
                tmp.animator.SetTrigger("frip");
                if (tmp.cardData.specialID == Special.TimeChange)
                {
                    battleManager.RPC_ChangeTime(true, true);
                }

            }
            //if (tmp.cardState == CardState.UserOnlyOpen)
            //{
            //    tmp.cardState = CardState.Open;
            //}
            //else if (tmp.cardState == CardState.Open)
            //{
            //    tmp.cardState = CardState.UserOnlyOpen;
            //}
        }
        //加護
        else if (tmp.cardData.specialID == Special.CommonDef)
        {
            if (cardArea == CardArea.Hand & localID != userID)
            {

            }
            else
            {

                tmp.animator.SetTrigger("move");
            }
            //if (localID == userID)
            {
                if (tmp.specialValue == 9) tmp.specialValue = 0;
                else { tmp.specialValue++; }
            }
        }
        else if (tmp.cardData.specialID == Special.AddCost)
        {
            if (cardArea == CardArea.Hand & localID != userID)
            {

            }
            else
            {

                tmp.animator.SetTrigger("move");
            }
            if (tmp.cardData.specialValue == "X")
            {
                //ベースと追加分が9を超えたら戻る
                //if (tmp.specialValue + int.Parse(tmp.cardData.cost) == 9) tmp.specialValue = 0;
                if (tmp.specialValue == 9) tmp.specialValue = 0;
                else { tmp.specialValue++; }
            }
            else if (tmp.cardData.specialValue == "-X")
            {
                //ベースより軽減分が下回るなら戻る
                if (tmp.specialValue + int.Parse(tmp.cardData.cost) <= 0) tmp.specialValue = 0;
                //if (tmp.specialValue == 9) tmp.specialValue = 0;
                else { tmp.specialValue--; }
            }
            else
            {
                if (tmp.specialValue == 0) tmp.specialValue = int.Parse(tmp.cardData.specialValue);
                else { tmp.specialValue = 0; }
            }
        }
        else
        {
            if (cardArea == CardArea.Hand & localID != userID)
            {

            }
            else
            {

                tmp.animator.SetTrigger("move");
            }

        }


        tmp.UpdateView();


    }

    public void SortCommmonHand(Transform parent)
    {
        List<CardObject> list = new List<CardObject>();

        foreach (Transform child in parent.transform)
        {
            list.Add(child.GetComponent<CardObject>());
        }

        list = list.OrderBy(X => X.cardID).ToList();

        int index = 0;

        foreach (CardObject child in list)
        {
            child.transform.SetSiblingIndex(index++);
        }

    }

    public void ChangeTime()
    {
        Debug.Log($"昼夜を反転");
        battleManager.RPC_ChangeTime(true, true);
    }
}
