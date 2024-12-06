
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    private BattleManager battleManager;

    [Networked] public int playerId { get; set; }
    [Networked] public int Health { get; set; }
    [Networked] public int Mana { get; set; }
    [Networked] public int Attack { get; set; }
    [Networked] public int Deffence { get; set; }
    [Networked] public int avatarId { get; set; }
    [Networked] public NetworkString<_32> userName { get; set; }
    [Networked][Capacity(30)] public NetworkLinkedList<int> deck { get; } = MakeInitializer(new int[] { });
    [Networked][Capacity(30)] public NetworkLinkedList<int> tmpDeck { get; } = MakeInitializer(new int[] { });

    [Networked] public int ready { get; set; } = 0;

    [SerializeField]
    private bool initFlag = false;
    [SerializeField]
    private int cnt = 0;

    public int selectedObjectID;
    public Sprite avatarImage;

    public void Init(int id, LocalData localData, BattleManager battleManager)
    {
        this.battleManager = battleManager;
        this.name += $"_{id}";
        playerId = id;
        Health = localData.health;
        Mana = localData.mana;
        Attack = localData.attack;
        Deffence = localData.deffence;
        userName = localData.userName;
        avatarId = localData.avatarID;
        foreach (int i in localData.deck)
        {
            deck.Add(i);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!initFlag & Runner.ActivePlayers.Count() <= 2)
        {
            if (cnt++ <= 150) return;
            Debug.Log($"login:[id:{playerId}][]");
            battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
            if (battleManager != null)
            {
                if (playerId == 1) { this.battleManager.playerData1 = this; }
                if (playerId == 2) { battleManager.SyncPlayerData(this); }
                initFlag = true;
                cnt = 0;
            }

        }
    }

    public void RPC_ChangePlayerData(int targetID, PlayerStatus playerStatus, int value, RpcInfo info = default)
    {
        RPC_ChangePlayerData(targetID, playerStatus, value, false);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ChangePlayerData(int targetID, PlayerStatus playerStatus, int value, bool constFlag, RpcInfo info = default)
    {
        PlayerData playerData = battleManager.TargetPlayerData(targetID);
        battleManager.RPC_Log($"Phase:{playerData.playerId}Ë{playerStatus}:{value}");

        if (value != 0)
        {
            if (constFlag)
            {
                if (playerStatus == PlayerStatus.Health) playerData.Health = value;
                if (playerStatus == PlayerStatus.Mana) playerData.Mana = value;
                if (playerStatus == PlayerStatus.Attack) playerData.Attack = value;
                if (playerStatus == PlayerStatus.Deffence) playerData.Deffence = value;
            }
            else
            {
                if (playerStatus == PlayerStatus.Health) playerData.Health += value;
                if (playerStatus == PlayerStatus.Mana) playerData.Mana += value;
                if (playerStatus == PlayerStatus.Attack) playerData.Attack += value;
                if (playerStatus == PlayerStatus.Deffence) playerData.Deffence += value;
            }
        }
        if (playerData.Mana > 9)
        {
            playerData.Mana = 9;
            playerData.Health += -1;

        }
        if (playerData.Health > 15)
        {
            playerData.Health = 15;

        }
        battleManager.RPC_UpdateView();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_DeckSet(string deck)
    {
        this.deck.Clear();
        var tmp = deck.Split(',');
        foreach (var item in tmp)
        {
            this.deck.Add(int.Parse(item));
        }
        ready = 1;
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_DeckReset()
    {
        this.deck.Clear();
        ready = 0;
    }

}
