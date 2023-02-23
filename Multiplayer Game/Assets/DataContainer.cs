using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;



public class DataContainer : MonoBehaviour
{
    public Lobby lobby;

    void Awake() {
        DontDestroyOnLoad(this.gameObject);    
    }
}
