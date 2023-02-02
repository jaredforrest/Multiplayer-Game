using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthBoostSpawner : NetworkBehaviour
{
    public GameObject HealthBoost;

    float timer = 0f;
    float waitingTime = 3f;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > waitingTime){
            Spawn();
            timer = 0;
        }
    }

    void Spawn()
    {
        GameObject _bot1 = Instantiate(HealthBoost, RandomPoint(30.0f), Quaternion.identity);
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
