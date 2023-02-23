using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float TimeLeft;
    public bool TimerOn = false;

    public TextMeshProUGUI TimerText;

    // Start is called before the first frame update
    void Start()
    {
        TimerOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerOn){
            if (TimeLeft>0){
                TimeLeft -= Time.deltaTime;
            }
            else{
                Debug.Log("Timer Finished");
                TimeLeft = 0;
                TimerOn = false;
            }
        }
    }
    void updateTimer(float currentTime){
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime/60);
        float seconds = Mathf.FloorToInt(currentTime%60);

        TimerText.text = string.Format("{0:00} : {1:00} ", minutes, seconds);

    }
}
