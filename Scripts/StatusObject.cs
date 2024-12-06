using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatusObject : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private BattleManager battleManager;
    [SerializeField]
    public PlayerStatus playerStatus;

    public int playerID;

    [SerializeField]
    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(int id)
    {
        playerID = id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        Debug.Log($"{playerStatus}:{eventData.button}");
            PlayerView playerView = battleManager.localManager.SelectViewSide(battleManager.localManager.playerBottomView.playerId, playerID);
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                if (playerID == 1) battleManager.playerData1.RPC_ChangePlayerData(playerView.playerId, playerStatus, 1);
                if (playerID == 2) battleManager.playerData2.RPC_ChangePlayerData(playerView.playerId, playerStatus, 1);
                break;
            case PointerEventData.InputButton.Right:
                if (playerID == 1) battleManager.playerData1.RPC_ChangePlayerData(playerView.playerId, playerStatus, -1);
                if (playerID == 2) battleManager.playerData2.RPC_ChangePlayerData(playerView.playerId, playerStatus, -1);
                break;

        }
        //battleManager.RPC_PlayCard(objectID,playerID);


    }

}
