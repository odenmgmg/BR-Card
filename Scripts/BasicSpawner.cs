using System.Collections.Generic;
using System;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using static UnityEngine.Rendering.CoreUtils;

public class BasicSpawner : SimulationBehaviour, INetworkRunnerCallbacks
{
    //[SerializeField]
    //private NetworkRunner networkRunnerPrefab;


    private NetworkRunner runner;
    [SerializeField]
    private LocalData localData;
    [SerializeField]
    private BattleManager battleManager;
    [SerializeField]
    private NetworkObject playerData;

    //private List<SessionInfo> sessions = new List<SessionInfo>();

    private void Start()
    {
        if (runner == null)
        {
            //    if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            //    {
            StartGame(GameMode.Shared);

            //    }
            //    if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            //    {
            //        localData = localDataAlt;
            //        StartGame(GameMode.Shared);
            //    }
        }
    }
    //private void Awake()
    //{
    //    _runner = GetComponent<NetworkRunner>();
    //    Connect();
    //}

    //private async void Connect()
    //{
    //    runner = gameObject.AddComponent<NetworkRunner>();
    //    runner.ProvideInput = true;
    //    runner.AddCallbacks(this);
    //    //var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
    //    //var sceneInfo = new NetworkSceneInfo();
    //    SessionInfo session = sessions.Find(x => x.Name == $"{localData.ruleID}_{localData.roomID}");
    //    if (session != null)
    //    {
    //        if (session.PlayerCount == 2) Debug.Log("部屋を検出");
    //        else
    //        {
    //            Debug.Log("部屋がありません");
    //            return;
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("部屋がありません");
    //        return;
    //    }
    //    var result = await runner.JoinSessionLobby(SessionLobby.Shared, $"{localData.ruleID}_{localData.roomID}");

    //    if (result.Ok)
    //    {
    //        battleManager.deckManager.CloseDeckMenu();
    //        // all good
    //        Debug.Log("JoinSessionLobby succeeded");
    //    }
    //    else
    //    {
    //        Debug.LogError($"Failed to Start: {result.ShutdownReason}");
    //    }
    //}
    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        //_runner = Instantiate(networkRunnerPrefab);
        runner = gameObject.AddComponent<NetworkRunner>();
        //_runner.AddCallbacks(this);
        runner.ProvideInput = true;
        //battleManager._runner = _runner;
        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        Debug.Log($"SessionName:{localData.ruleID}_{localData.roomID}");
        // Start or join (depends on gamemode) a session with a specific name
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = $"{localData.ruleID}_{localData.roomID}",
            PlayerCount = 20,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        ////観戦希望の2人未満の場合
        //if (!localData.isMatching & player == runner.LocalPlayer & player.PlayerId <= 2)
        //{
        //    runner.Shutdown();
        //    SceneManager.LoadScene("Title");
        //}
        //2人用の場合
        if (player == runner.LocalPlayer & player.PlayerId <= 2)
        {
            PlayerData tmp = runner.Spawn(playerData, new Vector3(0, 1, 0), Quaternion.identity).GetComponent<PlayerData>();
            tmp.Init(player.PlayerId, localData, battleManager);

        }
        //else
        //{
        //        runner.Shutdown();
        //        SceneManager.LoadScene("Title");


        //}
        //if (player == runner.LocalPlayer & player.PlayerId >= 3 ){
        //    if(battleManager.sessionPhase != SessionPhase.Battle)
        //    {
        //        battleManager.deckManager.CloseDeckMenu();
        //    }
        //    else
        //    {
        //    }

        //}
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!player.IsMasterClient & player.PlayerId <= 2)
        {
            Destroy(runner.gameObject);
            runner = null;
            Debug.Log("!");
            battleManager.BattleEnd();

        }
    }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Destroy(runner.gameObject);
        runner = null;
        Debug.Log("!");
        battleManager.BattleEnd();
    }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Destroy(runner.gameObject);
        Debug.Log("!");
        battleManager.BattleEnd();

    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}