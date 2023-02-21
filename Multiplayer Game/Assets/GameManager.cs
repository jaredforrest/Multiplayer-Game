using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

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
        switch(gameMap) {
            case "basic": Instantiate(basicMap); 
            break;
        }
        SpawnPlayerServerRpc();
        //myPlayer.GetComponent<PlayerController>().fieldOfView = fieldOfView;

        if(IsHost){
            Instantiate(HealtBoostSpawner);
            Instantiate(BotSpawner);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
