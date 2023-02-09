using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private Button PauseGameBtn;

    private void Awake() {
        PauseGameBtn.onClick.AddListener(() => {
            if(Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        });
    }
}
