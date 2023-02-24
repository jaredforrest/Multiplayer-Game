using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MainMenu : NetworkBehaviour
{
    public GameObject primaryMenu;
    public GameObject selectMap;
    private string gameType;
    public GameObject Spawner;
    public GameObject HealtBoostSpawner;

    public Button SingleplayerBtn;
    public Button MultiplayerBtn;

    public Button Map1Btn;
    public Button Map2Btn;

    private void Awake() {
        SingleplayerBtn.onClick.AddListener(() => {
            primaryMenu.SetActive(false);
            selectMap.SetActive(true);
            gameType = "SinglePlayer";
        });

        MultiplayerBtn.onClick.AddListener(() => {
            SceneManager.LoadScene("Lobby");
            gameType = "MultiPlayer";
        });

        Map1Btn.onClick.AddListener(() => {
            SceneManager.LoadScene("Singleplayer");
            Destroy(NetworkManager);
            NetworkManager.Shutdown();
        });

    }

    private void StartSingleplayer() 
    {
         float[] xCord = new float[5] {-27.489f, 5.5f, 27.4f, 27.4f, -28.9f};
         float[] yCord = new float[5] {-10.87885f, -12f, 10.5f, -11.4f, 12.3f};
         float delay = 3f;
         for (int i = 0; i < xCord.Length; i++)
         {
             GameObject spawner = Instantiate(Spawner);
             spawner.transform.position = new Vector2(xCord[i], yCord[i]);
             spawner.GetComponent<Bot1Spawner>().initialDelay = delay * i;
         }
         Instantiate(HealtBoostSpawner);
    }
}

