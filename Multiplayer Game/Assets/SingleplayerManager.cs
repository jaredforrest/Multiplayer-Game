using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SingleplayerManager : NetworkBehaviour
{
    public string playerType = "basic";
    public GameObject basicPlayer;
    private GameObject selectedPlayer;

    public string mapName = "test_map_name";
    public GameObject basicMap;

    public GameObject HealtBoostSpawner;
    public GameObject BotSpawner;

    public FieldOfView fieldOfView;

    void Start()
    {
        NetworkManager.Singleton.StartHost();

        switch(mapName) {
            case "test_map_name":
                Instantiate(basicMap); 
                break;
        }

        Instantiate(HealtBoostSpawner);
        Instantiate(BotSpawner);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
