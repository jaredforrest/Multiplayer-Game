using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Bot1Spawner : NetworkBehaviour
{

    public GameObject Bot;

    float timer = 0f;
    float waitingTime = 15f;

    public float initialDelay;

    public int lobbySize = 7;

    private void Start()
    {
        timer = -initialDelay;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > waitingTime){
            int playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
            if(GameObject.FindGameObjectsWithTag("Bot").Length + playerCount < lobbySize)
            {
                Spawn();
            }
            timer = 0;
        }
    }

    void Spawn()
    {
        GameObject _bot1 = Instantiate(Bot, RandomPoint(30.0f), Quaternion.identity);
        _bot1.GetComponent<NetworkObject>().Spawn();
    }

    Vector2 RandomPoint(float range)
    {
        Vector3 randomPoint = Random.insideUnitSphere * range;
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 30.0f, UnityEngine.AI.NavMesh.AllAreas);
        return hit.position;
    }
}