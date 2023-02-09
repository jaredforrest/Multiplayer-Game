using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class NetworkManagerUIScript : MonoBehaviour
{
    [SerializeField] private Button ServerBtn;
    [SerializeField] private Button HostBtn;
    [SerializeField] private Button ClientBtn;
    [SerializeField] private GameObject Spawner;
    [SerializeField] private GameObject HealtBoostSpawner;
    public GameObject multiCanvas;

    private void Awake() {
        ServerBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            Destroy(multiCanvas);
        });
        HostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Destroy(multiCanvas);
            
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
            
        });
        ClientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Destroy(multiCanvas);
        });
        
    }
}
