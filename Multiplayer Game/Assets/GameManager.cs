using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;


public class GameManager : NetworkBehaviour
{
    public string gameMap = "basic";
    public GameObject basicMap;

    public string playerType = "basic";
    public GameObject basicPlayer;
    private GameObject selectedPlayer;
    private GameObject myPlayer;

    public GameObject HealtBoostSpawner;
    public GameObject BotSpawner;

    public FieldOfView fieldOfView;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayerServerRpc();

        if(IsHost){
            Instantiate(HealtBoostSpawner);
            Instantiate(BotSpawner);

            Lobby lobby = GameObject.Find("DataForMulti").GetComponent<DataContainer>().lobby;
            
            Debug.Log(lobby.Name);
            Debug.Log(lobby.Data["GameMap"].Value);

            switch(lobby.Name) {
                case "test_map_name":
                    map = Instantiate(basicMap); 
                    break;
            }
            map.GetComponent<NetworkObject>().Spawn();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        switch(playerType){
            case "basic": selectedPlayer = basicPlayer;
            break;

        }
        myPlayer = Instantiate(selectedPlayer);
        myPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }
}
