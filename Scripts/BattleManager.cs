using Fusion;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class BattleManager : NetworkBehaviour
{
    //private NetworkObject CardPrefab;
    //[SerializeField]
    //public NetworkRunner _runner;

    public LocalData localData;
    public LocalManager localManager;
    public DeckManager deckManager;
    //[SerializeField]
    //private CardDataBase cardDataBase;

    //public List<int> playerDeck = new List<int>(), opponentDeck = new List<int>();
    [Networked, OnChangedRender(nameof(PlayerChanged))] public PlayerData playerData1 { get; set; }
    [Networked, OnChangedRender(nameof(PlayerChanged))] public PlayerData playerData2 { get; set; }

    [Networked, OnChangedRender(nameof(PhaseChanged))] public int TurnCount { get; set; } = -1;
    [Networked, OnChangedRender(nameof(PhaseChanged))] public int TurnPlayer { get; set; } = -1;
    [Networked, OnChangedRender(nameof(PhaseChanged))] public SessionPhase sessionPhase { get; set; } = SessionPhase.None;
    [Networked, OnChangedRender(nameof(PhaseChanged))] public BattlePhase battlePhase { get; set; } = BattlePhase.None;

    [Networked][Capacity(30)] public NetworkLinkedList<NetworkObject> hands { get; } = MakeInitializer(new NetworkObject[] { });

    [Networked] public int player1Cnt { get; set; } = 0;
    [Networked] public int player2Cnt { get; set; } = 0;

    [Networked] public bool isMoon { get; set; } = false;
    [Networked] public bool isTimeLock { get; set; } = false;

    public override void Spawned()
    {
        base.Spawned();
        localManager.audioBGM.clip = localManager.audioData.audioBGMClips[0];
        //if (Runner.LocalPlayer.PlayerId != 1)
        //{
        //    PlayerView tmp = localView.playerBottomView;
        //    localView.playerBottomView = localView.playerUpperView;
        //    localView.playerUpperView = tmp;
        //}
    }

    void PhaseChanged()
    {
        localManager.turnView.UpdateView(TurnCount, TurnPlayer, battlePhase, Runner.LocalPlayer.PlayerId);
    }
    void PlayerChanged()
    {
        if (sessionPhase == SessionPhase.Battle & playerData1.Mana + playerData2.Mana >= 12 & !isTimeLock)
        {
            isMoon = true;
        }
        localManager.timeIcon.Change(isMoon);
        //ローカルプレイヤーが下に来るように描画
        if (Runner.LocalPlayer.PlayerId == playerData1.playerId)
        {
            localManager.playerBottomView.UpdateView(playerData1, localManager);
            localManager.playerUpperView.UpdateView(playerData2, localManager);
        }
        if (Runner.LocalPlayer.PlayerId == playerData2.playerId)
        {
            localManager.playerBottomView.UpdateView(playerData2, localManager);
            localManager.playerUpperView.UpdateView(playerData1, localManager);
        }
        if (Runner.LocalPlayer.PlayerId >= 3)
        {
            localManager.playerBottomView.UpdateView(playerData1, localManager);
            localManager.playerUpperView.UpdateView(playerData2, localManager);
        }

    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (TurnPlayer == -1 & playerData1 != null & playerData2 != null & HasStateAuthority)
        {
            if (sessionPhase == SessionPhase.None)
            {
                sessionPhase = SessionPhase.Ready;
                //デッキ作成フェーズに移行
                CreateCardPool(localData.ruleID);

            }
            if (sessionPhase == SessionPhase.Ready & playerData1.ready == 1 & playerData2.ready == 1)
            {
                sessionPhase = SessionPhase.Battle;
                playerData1.ready = 0;
                playerData2.ready = 0;
                RPC_CloseDeckMenu();
            }
            if (sessionPhase == SessionPhase.Battle)
            {
                BattleStart();
            }
        }
    }

    public void CreateCardPool(string groupID)
    {
        var cardIDList = localManager.cardDataBase.data.Where(x => x.groupID.Equals(groupID)).ToList();

        var poolList = new List<int>();
        var shuffled = new List<int>();
        var tmpPool1 = new List<int>();
        var tmpPool2 = new List<int>();

        foreach (var card in cardIDList)
        {
            poolList.Add(card.cardID);
            poolList.Add(card.cardID);
        }
        RPC_Log($"CardPool:{poolList.ToSeparatedString(",")}");

        shuffled = poolList.OrderBy(i => Guid.NewGuid()).ToList();
        tmpPool1 = shuffled.Take(15).ToList();
        tmpPool2 = shuffled.Skip(15).Take(15).ToList();
        RPC_Log($"CardPool_shuffled:{shuffled.ToSeparatedString(",")}");
        RPC_Log($"CardPool_tmpPool1:{tmpPool1.ToSeparatedString(",")}");
        RPC_Log($"CardPool_tmpPool2:{tmpPool2.ToSeparatedString(",")}");

        playerData1.tmpDeck.Clear();
        playerData2.tmpDeck.Clear();

        RPC_OpenDeckMenu(tmpPool1.ToSeparatedString(","), tmpPool2.ToSeparatedString(","));

    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OpenDeckMenu(string str1, string str2, RpcInfo info = default)
    {
        var tmpPool1 = str1.Split(',');
        var tmpPool2 = str2.Split(',');

        foreach (var i in tmpPool1)
        {
            playerData1.tmpDeck.Add(int.Parse(i));
        }
        foreach (var i in tmpPool2)
        {
            playerData2.tmpDeck.Add(int.Parse(i));
        }
        deckManager.OpenDeckMenu(this, Runner.LocalPlayer.PlayerId);

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_CloseDeckMenu(RpcInfo info = default)
    {
        deckManager.CloseDeckMenu();

    }

    public PlayerData TargetPlayerData(int playerID)
    {
        if (playerID == 1) { return playerData1; }
        if (playerID == 2) { return playerData2; }
        return null;

    }
    //public PlayerView TargetPlayerView(int playerID,int targetID)
    //{
    //    //プレイヤー１、自身のビュー
    //    if (playerID == playerData1.playerId & playerID == targetID) { return localManager.playerBottomView; }
    //    //プレイヤー１、相手のビュー
    //    if (playerID == playerData1.playerId & playerID != targetID) { return localManager.playerUpperView; }
    //    //プレイヤー２、自身のビュー
    //    if (playerID == playerData2.playerId & playerID == targetID) { return localManager.playerBottomView; }
    //    //プレイヤー２、相手のビュー
    //    if (playerID == playerData2.playerId & playerID != targetID) { return localManager.playerUpperView; }
    //    return null;

    //}

    private void PhaseChange(BattlePhase battlePhase)
    {
        RPC_Log($"Phase:{this.battlePhase}⇒{battlePhase}");
        this.battlePhase = battlePhase;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Log(string message, RpcInfo info = default) { Debug.Log($"action:{info.Source.PlayerId} " + message); }


    public void BattleStart()
    {
        if (!HasStateAuthority) return;


        PhaseChange(BattlePhase.BattleStart);
        isMoon = false;
        TurnCount = 1;
        TurnPlayer = new int[] { 1, 2 }.OrderBy(i => Guid.NewGuid()).First();
        //Random.Range(1, 3);
        //foreach (int i in playerData1.deck)
        //{
        //    NetworkObject tmp = Runner.Spawn(CardPrefab);
        //    tmp.GetComponent<CardObject>().Init(this, cardDataBase.data.Find(x => x.CardID == i), 1);
        //    tmp.GetComponent<CardObject>().Power = 1;
        //    hands.Add(tmp);
        //    //tmp.transform.SetParent(localView.playerBottomView.HandArea.transform, false);
        //    //tmp.transform.localScale = Vector3.one;
        //}
        //foreach (int i in playerData2.deck)
        //{
        //    NetworkObject tmp = Runner.Spawn(CardPrefab);
        //    tmp.GetComponent<CardObject>().Init(this, cardDataBase.data.Find(x => x.CardID == i), 2);
        //    tmp.GetComponent<CardObject>().Power = 0;
        //    hands.Add(tmp);
        //    //tmp.transform.SetParent(localView.playerUpperView.HandArea.transform, false);
        //    //tmp.transform.localScale = Vector3.one;
        //}
        RPC_BattleStart();

        TurnStart();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_BattleStart(RpcInfo info = default)
    {
        Debug.Log($"プレイヤー{TurnPlayer}の先行");
        //処理
        localManager.audioBGM.clip = localManager.audioData.audioBGMClips[1];
        localManager.audioBGM.Play();
        var commonCards = localManager.cardDataBase.data.Where(x => x.groupID.Equals("CMN")).OrderBy(x => x.cardID).ToList();

        foreach (CardData cd in commonCards)
        {
            if (cd.cardID == -3)
            {
                if (TurnPlayer == 2)
                {
                    localManager.CardCreate(Runner.LocalPlayer.PlayerId, 1, CardArea.CommonHand, cd.cardID);
                    playerData1.Attack = 1;
                }
                else
                {
                    localManager.CardCreate(Runner.LocalPlayer.PlayerId, 2, CardArea.CommonHand, cd.cardID);
                    playerData2.Attack = 1;
                }
                PlayerChanged();
            }
            else
            {
                localManager.CardCreate(Runner.LocalPlayer.PlayerId, 1, CardArea.CommonHand, cd.cardID);
                localManager.CardCreate(Runner.LocalPlayer.PlayerId, 2, CardArea.CommonHand, cd.cardID);
            }
        }

        foreach (int i in playerData1.deck)
        {
            localManager.CardCreate(Runner.LocalPlayer.PlayerId, 1, CardArea.Hand, i);
        }
        foreach (int i in playerData2.deck)
        {
            localManager.CardCreate(Runner.LocalPlayer.PlayerId, 2, CardArea.Hand, i);
        }





        //ローカルプレイヤーが下に来るように描画
        //foreach (NetworkObject tmp in hands)
        //{
        //    int id =tmp.GetComponent<CardObject>().playerID;
        //    if (Runner.LocalPlayer.PlayerId == id)
        //    {
        //        tmp.transform.SetParent(localManager.playerBottomView.handArea.transform, false);
        //    }
        //    if (Runner.LocalPlayer.PlayerId != id)
        //    {
        //        tmp.transform.SetParent(localManager.playerUpperView.handArea.transform, false);
        //    }
        //    tmp.transform.localScale = Vector3.one;

        //}

    }

    public void TurnStart()
    {
        if (!HasStateAuthority) return;
        PhaseChange(BattlePhase.TurnStart);
        //処理
        if (TurnPlayer == 1) playerData1.RPC_ChangePlayerData(TurnPlayer, PlayerStatus.Mana, 1);
        if (TurnPlayer == 2) playerData2.RPC_ChangePlayerData(TurnPlayer, PlayerStatus.Mana, 1);



        PhaseChange(BattlePhase.Action);

    }


    public void TurnEnd()
    {
        if (TurnPlayer != Runner.LocalPlayer.PlayerId) return;
        RPC_TurnEnd();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TurnEnd(RpcInfo info = default)
    {

        Debug.Log($"id:{info.Source.PlayerId}/TurnPlayer:{TurnPlayer}");

        List<CardObject> list = new List<CardObject>();

        //処理
        foreach (Transform child in localManager.playerBottomView.unitAreaFront.transform)
        {
            CardObject obj = child.GetComponent<CardObject>();
            list.Add(obj);

        }
        if (TurnPlayer != localManager.playerBottomView.playerId)
        {
            foreach (Transform child in localManager.playerBottomView.unitAreaBack.transform)
            {
                CardObject obj = child.GetComponent<CardObject>();
                if (obj.cardData.cardType == CardType.Tactics)
                {
                    list.Add(obj);
                }
            }
        }
        else if (TurnPlayer != localManager.playerUpperView.playerId)
        {
            foreach (Transform child in localManager.playerUpperView.unitAreaBack.transform)
            {
                CardObject obj = child.GetComponent<CardObject>();
                if (obj.cardData.cardType == CardType.Tactics)
                {
                    list.Add(obj);
                }
            }

        }
        foreach (CardObject obj in list)
        {

            RPC_TurnEndCard(obj.objectID, obj.playerID, obj.cardData.cardType, obj.cardData.limited, obj.cardState, obj.cardData.specialID);

        }
        if (info.Source.PlayerId == TurnPlayer)
        {
            PhaseChange(BattlePhase.TurnEnd);


            TurnCount++;
            isTimeLock = false;
            if (playerData1.Mana + playerData2.Mana >= 12)
            {
                isMoon = true;
            }
            else
            {
                isMoon = false;
            }

            localManager.timeIcon.Change(isMoon);

            if (TurnPlayer == 1) TurnPlayer = 2;
            else if (TurnPlayer == 2) TurnPlayer = 1;
            Debug.Log($"TurnPlayer:{info.Source.PlayerId}⇒{TurnPlayer}");
            TurnStart();
        }
        else
        {

        }
    }

    public void BattleEnd()
    {
        Runner.Shutdown();
        SceneManager.LoadScene("Title");

    }

    public void SyncPlayerData(PlayerData playerData)
    {
        RPC_SyncPlayerData(playerData);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SyncPlayerData(PlayerData playerData, RpcInfo info = default)
    {
        Debug.Log($"id:{Runner.LocalPlayer.PlayerId}");
        playerData2 = playerData;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayCard(int objectID, int playerID, CardArea cardArea, CardState cardState, int posX, int areaID, RpcInfo info = default)
    {
        Debug.Log($"action:{info.Source.PlayerId} RPC_ClickCard");
        if (info.Source.PlayerId == playerID | cardState == CardState.Open)
        {
            Debug.Log($"player{playerID}の[objectID:{objectID}]を使用");
            localManager.CardMove(objectID, Runner.LocalPlayer.PlayerId, playerID, cardArea, posX, CardState.Open, areaID);

        }
        else
        {
            Debug.Log($"player{playerID}の[objectID:{objectID}]の所有者でないため、使用不可");

        }
        //cardObject.Power += 1;

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TurnEndCard(int objectID, int playerID, CardType cardType, string limited, CardState cardState, Special special, RpcInfo info = default)
    {
        Debug.Log($"turnend:{info.Source.PlayerId} RPC_TurnEndCard");
        if (limited == "L" & cardState == CardState.Open)
        {
            if (special == Special.Seal)
            {
                Debug.Log($"player{playerID}の[objectID:{objectID}]を維持[封印]");

                return;
            }
            Debug.Log($"player{playerID}の[objectID:{objectID}]を破棄");
            localManager.CardMove(objectID, Runner.LocalPlayer.PlayerId, playerID, CardArea.Trush, 0, CardState.Open, playerID);

        }
        else if (cardType != CardType.None)
        {
            Debug.Log($"player{playerID}の[objectID:{objectID}]を回収");
            localManager.CardMove(objectID, Runner.LocalPlayer.PlayerId, playerID, CardArea.Hand, 0, CardState.UserOnlyOpen, playerID);

        }
        else if (cardType == CardType.None)
        {
            Debug.Log($"player{playerID}の[objectID:{objectID}]を回収");
            localManager.CardMove(objectID, Runner.LocalPlayer.PlayerId, playerID, CardArea.CommonHand, 0, CardState.Open, playerID);
        }
        //else
        //{
        //    Debug.Log($"player{playerID}の[objectID:{objectID}]の所有者でないため、使用不可");

        //}
        //cardObject.Power += 1;

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PickCard(int objectID, int playerID, CardArea cardArea, int posX, RpcInfo info = default)
    {
        Debug.Log($"action:{info.Source.PlayerId} RPC_PickCard");
        if (info.Source.PlayerId == playerID)
        {
            Debug.Log($"player{playerID}の[objectID:{objectID}]を移動");
            deckManager.CardPick(objectID, Runner.LocalPlayer.PlayerId, playerID, cardArea, 0, CardState.Open);
        }
        else
        {
            Debug.Log($"player{playerID}の[objectID:{objectID}]の所有者でないため、使用不可");

        }
        //cardObject.Power += 1;

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ExecCard(int objectID, int playerID, CardArea cardArea, int posX, RpcInfo info = default)
    {
        Debug.Log($"action:{info.Source.PlayerId} RPC_FripCard");
        if (info.Source.PlayerId == playerID)
        {
            Debug.Log($"player{playerID}の[objectID:{objectID}]を使用");
            localManager.CardExec(objectID, Runner.LocalPlayer.PlayerId, playerID, cardArea);

        }
        else
        {
            Debug.Log($"player{playerID}の[objectID:{objectID}]の所有者でないため、使用不可");

        }
        //cardObject.Power += 1;

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdateView()
    {
        PlayerChanged();
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ChangeTime(bool lockFlag, bool overrideFlag)
    {
        Debug.Log($"RPC_昼夜を反転");
        isMoon = !isMoon;
        isTimeLock = lockFlag;
        RPC_UpdateView();
    }


}

