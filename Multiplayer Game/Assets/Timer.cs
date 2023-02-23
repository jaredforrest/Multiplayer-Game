using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
public class Timer : NetworkBehaviour
{

    private NetworkVariable<float> TimeLeft = new NetworkVariable<float>(100);
    public float TotalTime;
    public bool TimerOn = false;

    public TextMeshProUGUI TimerText;

    // Start is called before the first frame update
    void Start()
    {
        if(IsHost){
            TimeLeft.Value = TotalTime;
            TimerOn = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsHost){
            if (TimerOn){
                if (TimeLeft.Value>0){
                    TimeLeft.Value -= Time.deltaTime;
                }
                else{
                    Debug.Log("Timer Finished");
                    TimeLeft.Value = 0;
                    TimerOn = false;
                }
            }
        }
        updateTimer(TimeLeft.Value);
        
    }

    void updateTimer(float currentTime){
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime/60);
        float seconds = Mathf.FloorToInt(currentTime%60);

        TimerText.text = string.Format("{0:00} : {1:00} ", minutes, seconds);

    }
}
