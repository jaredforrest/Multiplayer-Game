using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot1Spawner : MonoBehaviour
{
    public GameObject target;

    public GameObject bot1Prefab;
    public GameObject healthBarCanvasPrefab;
    
    float timer = 0f;
    float waitingTime = 5f;

    // Start is called before the first frame update
    void Start()
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
        GameObject _bot1 = Instantiate(bot1Prefab, transform.position, Quaternion.identity);
        GameObject healthBarCanvas = Instantiate(healthBarCanvasPrefab);

        Bot1 bot1 = _bot1.GetComponent<Bot1>();
        bot1.healthBarCanvas = healthBarCanvas;
        bot1.target = target;

        StayAboveTarget sat = healthBarCanvas.GetComponent<StayAboveTarget>();
        sat.targetToFollow = _bot1.transform;

        //HealthBar healthBar = healthBarCanvas.transform.GetChild(0).GetComponent<HealthBar>();
    }
}