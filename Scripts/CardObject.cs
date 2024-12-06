
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private TextMeshProUGUI textName, textCost, textEffect, textPopName, textPopFunc, textCustomCost,
        textAtk1,
        textAtk2,
        textDef
        ;
    [SerializeField]
    private Image imageCardTypeBG, cardImage, typeImage;
    [SerializeField]
    private GameObject
        closeImage,
        maskImage,
        limitedImage,
        selectImage,
        namePopup,
        funcPopup,
        effectRoot,
        textPrefab,
        blockPrefab,
        miniblockPrefab,
        customCost,
        atk1Obj,
        atk2Obj,
        defObj,
        atk1_disable,
        atk2_disable,
        coinAreaPrefab;
    [SerializeField]
    private TMP_ColorGradient normalGrad, limitedGrad;
    /// カードオブジェクト固有のID objectIDとplayerIDで一意になる
    /// </summary>
    public int objectID;
    public int localID;
    public int playerID;
    private LocalManager localManager;
    private BattleManager battleManager;
    public int cardID;
    public CardArea cardArea;
    public CardState cardState;
    public CardData cardData;
    //public int health;
    //public int power;

    private Vector3 prePosition;

    [SerializeField]
    public Animator animator;

    public int specialValue = 0;

    private bool ready = false, initFlag = false;

    public void Init(LocalManager localManager, BattleManager battleManager, CardData cardData, int objectID, int localID, int playerID, CardArea cardArea)
    {
        this.objectID = objectID;
        this.localID = localID;
        this.playerID = playerID;
        this.localManager = localManager;
        this.battleManager = battleManager;
        this.cardData = cardData;
        DetailCardType detailCardType = localManager.staticData.detailCardTypes.Find(x => x.cardType == cardData.cardType);
        imageCardTypeBG.color = detailCardType.color;
        FunctionInfo functionInfo = localManager.staticData.functionInfos.Find(x => x.cardType == cardData.specialID);
        textPopFunc.text = functionInfo.text;
        typeImage.sprite = detailCardType.image;
        cardImage.sprite = cardData.cardImage;
        cardID = cardData.cardID;
        //health = cardData.health;
        //power = cardData.power;
        this.cardArea = cardArea;
        cardState = CardState.UserOnlyOpen;
        if (cardArea == CardArea.Pool) cardState = CardState.Open;
        if (cardArea == CardArea.CommonHand) cardState = CardState.Open;

        UpdateView();

        //RPC_Fetch();
        if (battleManager != null) battleManager.RPC_Log($"player{playerID}の[objectID:{objectID}/{cardData.cardName}]を生成");

        ready = true;
    }

    public void Update()
    {
        if (ready) UpdateView();
    }

    public void UpdateView()
    {
        switch (cardState)
        {
            case CardState.None:
                closeImage.SetActive(false);
                maskImage.SetActive(false);

                break;
            case CardState.Open:
                closeImage.SetActive(false);
                maskImage.SetActive(false);
                break;
            case CardState.UserOnlyOpen:
                closeImage.SetActive(localID != playerID); //一致していない場合、マスク非表示になる
                maskImage.SetActive(cardArea != CardArea.Hand & cardData.cardType == CardType.Tactics);
                break;
            case CardState.Close:
                closeImage.SetActive(true);
                maskImage.SetActive(false);
                break;
        }
        if (!initFlag)
        {
            imageCardTypeBG.color = localManager.staticData.detailCardTypes.Find(x => x.cardType == cardData.cardType).color;
            cardImage.sprite = cardData.cardImage;
            limitedImage.SetActive(cardData.limited == "L");
            textName.text = cardData.cardName;
            if (cardData.limited != "L") textName.colorGradientPreset = normalGrad;
            if (cardData.limited == "L") textName.colorGradientPreset = limitedGrad;
            atk1Obj.SetActive(cardData.unit_atk1 != "");
            if (cardData.unit_atk1 != "")
            {
                textAtk1.text = cardData.unit_atk1;
            }
            atk2Obj.SetActive(cardData.unit_atk2 != "");
            if (cardData.unit_atk2 != "")
            {
                textAtk2.text = cardData.unit_atk2;
            }
            defObj.SetActive(cardData.unit_def != "");
            if (cardData.unit_def != "")
            {
                textDef.text = cardData.unit_def;
            }
            initFlag = true;

        }
        if (cardArea == CardArea.Hand & localID != playerID)
        {
            customCost.SetActive(false);

        }
        else
        {

            customCost.SetActive(specialValue != 0);
            if (cardData.specialID == Special.CommonDef)
            {
                if (specialValue != 0)
                {
                    textCustomCost.text = specialValue.ToString();
                }

            }
            else if (cardData.specialID == Special.AddCost)
            {
                if (specialValue > 0)
                {
                    textCustomCost.text = $"{cardData.cost}+{specialValue.ToString()}";
                }
                if (specialValue < 0)
                {
                    textCustomCost.text = $"{cardData.cost}{specialValue.ToString()}";
                }

            }
        }
        if (battleManager != null)
        {
            if (cardData.unit_atk2 != "")
            {
                atk1_disable.SetActive(battleManager.isMoon);
                atk2_disable.SetActive(!battleManager.isMoon);
            }
            else
            {

            }

        }

        textCost.text = cardData.cost;

        //textEffect.text = cardData.effectText;
        SetText(cardData.effectText);
        selectImage.SetActive(this.Equals(localManager.activeCard));
    }

    public void SetText(string text)
    {
        if (playerID == -1) effectRoot.transform.localScale = Vector3.one * 1.3f;
        foreach (Transform n in effectRoot.transform)
        {
            if (n.GetComponent<CardText>() != null) Destroy(n.gameObject);
        }

        string[] lines = text.Split("<block>");
        //初期化
        Transform parent = null;
        bool disableFlag = false;
        foreach (string line in lines)
        {
            CardText cardText;
            string str = line;
            Sprite sprite = null;
            if (line.StartsWith("<box>"))
            {
                string[] tmp = line.Split("<box>");
                cardText = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity).GetComponent<CardText>();
                string[] contents = tmp[1].Split("<div>");

                str = contents[1];
                SpriteData sd = localManager.iconSprites.data.Find(x => x.fileName == contents[0]);
                if (sd != null) sprite = sd.sprite;
                if (battleManager != null)
                {
                    if (battleManager.sessionPhase == SessionPhase.Battle & (contents[0] == "昼" | contents[0] == "夜"))
                    {
                        disableFlag = (battleManager.isMoon & contents[0] == "昼") | (!battleManager.isMoon & contents[0] == "夜");
                    }
                    else
                    {
                        disableFlag = false;
                    }

                }

                cardText.transform.SetParent(effectRoot.transform, false);
                //minibox用
                parent = cardText.transform;
            }
            //入れ子専用
            else if (line.StartsWith("<minibox>"))
            {
                string[] tmp = line.Split("<minibox>");
                cardText = Instantiate(miniblockPrefab, Vector3.zero, Quaternion.identity).GetComponent<CardText>();
                string[] contents = tmp[1].Split("<div>");

                str = contents[1];
                SpriteData sd = localManager.iconSprites.data.Find(x => x.fileName == contents[0]);
                if (sd != null) sprite = sd.sprite;
                cardText.transform.SetParent(parent, false);
            }
            //入れ子専用
            else if (line.StartsWith("<coin>"))
            {
                cardText = Instantiate(coinAreaPrefab, Vector3.zero, Quaternion.identity).GetComponent<CardText>();
                str = $"<coin>{cardData.coin}";
                cardText.transform.SetParent(parent, false);
            }
            else
            {
                disableFlag = false;
                cardText = Instantiate(textPrefab, Vector3.zero, Quaternion.identity).GetComponent<CardText>();
                cardText.transform.SetParent(effectRoot.transform, false);
            }

            cardText.transform.localScale = Vector3.one;
            cardText.transform.localPosition = Vector3.zero;
            cardText.SetContent(str, sprite, disableFlag);
        }
    }

    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    //public void RPC_Fetch(RpcInfo info = default)
    //{
    //    Power += 1;
    //}


    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //public void RPC_SetParent(RpcInfo info = default)
    //{
    //    PlayerRef player = info.Source;
    //    Debug.Log($"al-s id:{player.PlayerId}/user:{Runner.LocalPlayer.PlayerId}/card:{playerID}");

    //    Transform transform = null;
    //    if (playerID == player.PlayerId)
    //    {
    //        transform = battleManager.localManager.playerBottomView.handArea.transform;
    //    }
    //    if (playerID != player.PlayerId)
    //    {
    //        transform = battleManager.localManager.playerUpperView.handArea.transform;
    //    }
    //    this.transform.SetParent(transform, false);
    //    this.transform.localScale = Vector3.one;

    //}
    //[Rpc(RpcSources.All, RpcTargets.All)]
    //private void RPC_Log(string message, RpcInfo info = default)
    //{
    //    Debug.Log($"action:{info.Source.PlayerId} " + message);
    //    StateChanged();

    //}

    //public void StateChanged()
    //{
    //    textName.text = CardName.ToString();
    //    textHealth.text = Health.ToString();
    //}

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                if (battleManager != null) battleManager.RPC_Log($"player{playerID}の[objectID:{objectID}/{cardData.cardName}]を選択");
                localManager.CardSelect(localID, playerID, cardState, cardData, this);
                break;
            case PointerEventData.InputButton.Right:
                if (battleManager != null)
                {

                    battleManager.RPC_Log($"player{playerID}の[objectID:{objectID}/{cardData.cardName}]を右クリック");
                    if (cardArea == CardArea.Deck)
                    {
                        battleManager.RPC_PickCard(objectID, playerID, CardArea.Pool, 0);
                    }
                    else if (cardArea == CardArea.Pool)
                    {
                        battleManager.RPC_PickCard(objectID, playerID, CardArea.Deck, 0);
                    }
                    else
                    {
                        battleManager.RPC_ExecCard(objectID, playerID, cardArea, 0);

                    }

                }
                else
                {
                    if (cardArea == CardArea.Deck)
                    {
                        localManager.deckManager.CardPick(objectID, playerID, playerID, CardArea.Pool, 0, CardState.Open);
                    }
                    else if (cardArea == CardArea.Pool)
                    {
                        localManager.deckManager.CardPick(objectID, playerID, playerID, CardArea.Deck, 0, CardState.Open);
                    }


                }
                break;

        }
        //battleManager.RPC_PlayCard(objectID,playerID);


    }

    public void AnimEventExecCard()
    {
        if (cardState == CardState.UserOnlyOpen)
        {
            cardState = CardState.Open;
        }
        else if (cardState == CardState.Open)
        {
            cardState = CardState.UserOnlyOpen;
        }

    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (playerID == -1 | localID >= 3) return;

        prePosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (playerID == -1 | localID>=3) return;


        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 80));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (playerID == -1 | localID >= 3) return;


        transform.position = prePosition;
        var raycasts = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycasts);
        foreach (var hit in raycasts)
        {
            Debug.Log($"raycast:{hit.gameObject.name}");
            if (hit.gameObject.GetComponent<UnitArea>() != null)
            {
                UnitArea unitArea = hit.gameObject.GetComponent<UnitArea>();
                if (cardArea == CardArea.Deck | cardArea == CardArea.Pool)
                {
                    if (battleManager != null)
                    {

                        battleManager.RPC_Log($"player{playerID}の[objectID:{objectID}/{cardData.cardName}]を[{unitArea.cardArea}:{unitArea.potisionX}]に移動");
                        battleManager.RPC_PickCard(objectID, playerID, unitArea.cardArea, unitArea.potisionX);
                    }
                    else
                    {
                        localManager.deckManager.CardPick(objectID, playerID, playerID, cardArea, 0, CardState.Open);
                    }

                }
                else
                {
                    if (battleManager != null)
                    {

                        battleManager.RPC_Log($"player{playerID}の[objectID:{objectID}/{cardData.cardName}]を[{unitArea.cardArea}:{unitArea.potisionX}]にプレイ");
                        battleManager.RPC_PlayCard(objectID, playerID, unitArea.cardArea, cardState, unitArea.potisionX, unitArea.playerView.playerId);
                    }
                    else
                    {

                    }

                }

            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playerID == -1) return;

        if (playerID != localID & cardState != CardState.Open) return;
        string prefix = "";
        if (cardData.limited != "L") textPopName.colorGradientPreset = normalGrad;
        if (cardData.limited == "L")
        {
            textPopName.colorGradientPreset = limitedGrad;
            prefix = "[L]";
        }

        textPopName.text = prefix + cardData.cardName;
        namePopup.SetActive(true);
        funcPopup.SetActive(cardData.specialID != Special.None);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (playerID == -1) return;

        if (playerID != localID & cardState != CardState.Open) return;
        namePopup.SetActive(false);
        funcPopup.SetActive(false);
    }
}
