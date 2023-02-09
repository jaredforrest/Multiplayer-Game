using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;


public class ScoreManager : NetworkBehaviour
{
    private const string KILLS = " KILLS";
    public static ScoreManager Instance { get; private set; }
    private int score = 0;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = score.ToString() + KILLS;
    }
    public void AddPoint(ulong shooterClientId)
    {
        // If is not the Server/Host then we should early return here!
        if (!IsServer) return;


        // NOTE! In case you know a list of ClientId's ahead of time, that does not need change,
        // Then please consider caching this (as a member variable), to avoid Allocating Memory every time you run this function
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{shooterClientId}
            }
        };

        AddPointClientRpc(clientRpcParams);
    }

    [ClientRpc]
    void AddPointClientRpc(ClientRpcParams clientRpcParams = default)
    {
        score += 1;
        scoreText.text = score + KILLS;
    }


}
