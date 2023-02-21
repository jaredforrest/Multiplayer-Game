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

    // Start is called before the first frame update
    void Start()
    {
            switch(gameMap) {
                case "basic": Instantiate(basicMap); 
                break;
            }
            SpawnPlayerServerRpc();

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
        GameObject _selectedPlayer = Instantiate(selectedPlayer);
        _selectedPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }
}
