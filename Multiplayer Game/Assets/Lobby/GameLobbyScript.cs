using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class GameLobbyScript : MonoBehaviour
{

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private float RefreshLobbyTimer;
    private string playerName;

    public GameObject NameScreen;
    public GameObject CreateLobby;
    public GameObject LobbyMain;
    public GameObject JoinByCode;
    public GameObject InALobby;

    public Button NameScreen_nextBtn;
    public GameObject NameScreen_nameInput;
    public Button NameScreen_BackBtn;

    public Button LobbyMainBackButton;
    public Button LobbyMainJoinByCodeBtn;
    public Button LobbyMainCreateLobbyBtn;

    public Button JoinByCodeBackBtn;
    public Button JoinByCodeJoinBtn;
    public GameObject JoinByCodeInput;

    public Button CreateLobbyBackBtn;
    public Button CreateLobbyCreateBtn;
    public GameObject CreateLobbyNameInput;
    public Slider MaxPlayerSlider;
    public GameObject privateLobbyToggle;

    public TextMeshProUGUI IAL_LobbyName;
    public TextMeshProUGUI IAL_MapName;
    public TextMeshProUGUI IAL_MaxPlayers;
    public TextMeshProUGUI IAL_LobbyCode;
    public Button IAL_BackBtn;
    public Button IAL_StartBtn;

    public Transform NameContentContainer;
    public GameObject PlayerContentPrefab;

    public Transform LobbiesContentContainer;
    public GameObject LobbiesContentPrefab;



    private async void Start(){
        //Signs user in anonymously to get an id
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        CreateLobby.SetActive(false);
        LobbyMain.SetActive(false);
        JoinByCode.SetActive(false);
        InALobby.SetActive(false);

//Name Screen
        NameScreen_BackBtn.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
            AuthenticationService.Instance.SignOut();
        });
        NameScreen_nextBtn.onClick.AddListener(() => {
            playerName = NameScreen_nameInput.GetComponent<TMP_InputField>().text;
            Debug.Log("Name is: " + playerName);
            NameScreen.SetActive(false);
            LobbyMain.SetActive(true);
        });
//Main Screen
        LobbyMainBackButton.onClick.AddListener(() => {
            NameScreen.SetActive(true);
            LobbyMain.SetActive(false);
        });

        LobbyMainJoinByCodeBtn.onClick.AddListener(() => {
            LobbyMain.SetActive(false);
            JoinByCode.SetActive(true);
        });

        LobbyMainCreateLobbyBtn.onClick.AddListener(() => {
            LobbyMain.SetActive(false);
            CreateLobby.SetActive(true);
        });

//Join By Code Screen
        JoinByCodeBackBtn.onClick.AddListener(() => {
            JoinByCode.SetActive(false);
            LobbyMain.SetActive(true);
        });

        JoinByCodeJoinBtn.onClick.AddListener(() => {
            string code = JoinByCodeInput.GetComponent<TMP_InputField>().text;
            joinLobbyByCode(code);
            //will need some logic for invalid codes
            JoinByCode.SetActive(false);
            InALobby.SetActive(true);
        });  

//Create Lobby Screen
        CreateLobbyBackBtn.onClick.AddListener(() => {
            CreateLobby.SetActive(false);
            LobbyMain.SetActive(true);
        });
        CreateLobbyCreateBtn.onClick.AddListener(() => {
            string lobbyName = CreateLobbyNameInput.GetComponent<TMP_InputField>().text;
            int maxPlayers = (int)MaxPlayerSlider.value;
            bool toggleStatus = false;
            if (privateLobbyToggle.GetComponent<Toggle>().isOn == true){
                toggleStatus = true;
            }
            
            //Change at later point
            string mapName = "test_map_name";

            createLobby(lobbyName, maxPlayers, toggleStatus, mapName);
            CreateLobby.SetActive(false);
            InALobby.SetActive(true);
        });

//In A Lobby
        IAL_BackBtn.onClick.AddListener(() => {
            leaveLobby();
            InALobby.SetActive(false);
            LobbyMain.SetActive(true);
        });
        IAL_StartBtn.onClick.AddListener(() => {
            CreateRelay();
        });
    }


    //Signs user in anonymously to get an id
    private async void signInAndName()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update(){
        handleHeartbeat();
        HandleLobbyPollForUpdates();
        RefreshLobbyList();

        //update in a lobby screen
        if(joinedLobby != null){
            //Debug.Log(joinedLobby.Name + " " + joinedLobby.Data["GameMap"].Value +" "+joinedLobby.MaxPlayers.ToString()+" "+joinedLobby.LobbyCode);
            IAL_LobbyName.text = joinedLobby.Name;
            IAL_MapName.text = joinedLobby.Data["GameMap"].Value;
            IAL_MaxPlayers.text = joinedLobby.MaxPlayers.ToString();
            IAL_LobbyCode.text = joinedLobby.LobbyCode;

            if(joinedLobby.Data["JoinCode"].Value != ""){
                JoinRelay(joinedLobby.Data["JoinCode"].Value);
            }
        }
        
    }

    // Lobbies time out after 30 seconds of inactivity so heartbeat keeps them alive
    private async void handleHeartbeat(){
        if (hostLobby != null){
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f){
                Debug.Log("Heartbeat sent");
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyPollForUpdates(){
        if (joinedLobby != null){
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f){
                float lobbyUpdateTimerMax = 1.5f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;

                //refresh player list
                if (InALobby.active){
                    foreach(Transform child in NameContentContainer){
                        GameObject.Destroy(child.gameObject);
                    }
                    foreach(Player player in joinedLobby.Players){
                        var item = Instantiate(PlayerContentPrefab);
                        TextMeshProUGUI NameContainer = item.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
                        NameContainer.text = player.Data["PlayerName"].Value;
                        item.transform.SetParent(NameContentContainer);
                        item.transform.localScale = Vector2.one;
                    }
                }
            }
        }        
    }

    private void RefreshLobbyList(){
        RefreshLobbyTimer -= Time.deltaTime;
        if (RefreshLobbyTimer < 0f){
            Debug.Log("Time");
            float RefreshLobbyTimerMax = 2f;
            RefreshLobbyTimer = RefreshLobbyTimerMax;
            if (LobbyMain.active){
                    Debug.Log("Active");
                    listLobbies();
                }
        }
    }


    // create lobby, takes name and max players as args 
    private async void createLobby(string lobbyName, int maxPlayers, bool isprivatelobby, string mapName)
    {
        try{
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions{
                IsPrivate = isprivatelobby,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>{
                    { "GameMap" , new DataObject(DataObject.VisibilityOptions.Public, mapName, DataObject.IndexOptions.S1) },
                    { "JoinCode", new DataObject(DataObject.VisibilityOptions.Member, "") }
                }
            };


            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            hostLobby = lobby;
            joinedLobby = hostLobby;
            Debug.Log("Lobby Created! " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            printPlayers(hostLobby); 
        }catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    // outputs a list of lobbies
    private async void listLobbies(){
        try{
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions{
                //max lobbies to show
                Count = 25,
                //only show greater than 1 availble slots
                Filters = new List<QueryFilter>{
                    new QueryFilter (QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                //descending by creation time
                Order = new List<QueryOrder>{
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };


            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            

            foreach(Transform child in LobbiesContentContainer){
                GameObject.Destroy(child.gameObject);
            }


            foreach (Lobby lobby in queryResponse.Results){
                var item = Instantiate(LobbiesContentPrefab);
                TextMeshProUGUI NameContainer = item.transform.Find("NameText").gameObject.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI MapContainer = item.transform.Find("MapText").gameObject.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI CountContainer = item.transform.Find("PlayerCount").gameObject.GetComponent<TextMeshProUGUI>();
                NameContainer.text = lobby.Name;
                MapContainer.text = lobby.Data["GameMap"].Value;
                CountContainer.text = lobby.Players.Count+"/"+lobby.MaxPlayers;
                item.transform.SetParent(LobbiesContentContainer);
                item.transform.localScale = Vector2.one;

                item.transform.Find("Button").gameObject.GetComponent<Button>().onClick.AddListener(() => {
                    joinLobbyById(lobby.Id);
                    LobbyMain.SetActive(false);
                    InALobby.SetActive(true);
                });
            }


            // Debug.Log("Lobbies Found: "+queryResponse.Results.Count);
            // foreach (Lobby lobby in queryResponse.Results){
            // Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            // }
        }catch (LobbyServiceException e){
            Debug.Log(e);
        }   
            
    }

    //adds a player to a lobby by code, takes lobby code as arg
    private async void joinLobbyByCode(string lobbyCode){
        try{
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions {
                Player = GetPlayer()
            };

            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            joinedLobby = lobby;
            Debug.Log("Joined Lobby With Code: " + lobbyCode);
            printPlayers(joinedLobby);
        }catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void joinLobbyById(string lobbyId){
        try{
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions {
                Player = GetPlayer()
            };

            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);

            joinedLobby = lobby;
        }catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    //
    private async void quickJoinLobby(){
        try{
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private Player GetPlayer(){
        return new Player {
            Data = new Dictionary<string, PlayerDataObject> {
                {"PlayerName" , new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }

    private void printPlayers(){
        printPlayers(joinedLobby);
    }

    private void printPlayers(Lobby lobby){
        Debug.Log("Players In Lobby: " + lobby.Name);
        foreach(Player player in lobby.Players){
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    private async void updateLobbyMap(string mapName){
        try{
        hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions {
            Data = new Dictionary<string, DataObject>{
                { "GameMap", new DataObject(DataObject.VisibilityOptions.Public, mapName, DataObject.IndexOptions.S1)}
            }
        });
        joinedLobby = hostLobby;
        printPlayers(hostLobby);
        }catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void updatePlayerName(string newPlayerName){
        try{
        playerName = newPlayerName;
        await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions{
            Data = new Dictionary<string, PlayerDataObject>{
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
            }
        });    
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
        
    }

    private async void leaveLobby(){
        try{
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void CreateRelay(){
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(joinedLobby.Players.Count-1);
            
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            updateJoinCode(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Game",  LoadSceneMode.Single);
        }catch (RelayServiceException e){
            Debug.Log(e);
        }
    }

    private async void JoinRelay(string joinCode){
        try {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        } catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }
    
    private async void updateJoinCode(string joinCode){
        try{
        hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions {
            Data = new Dictionary<string, DataObject>{
                { "JoinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
            }
        });
        joinedLobby = hostLobby;
        }catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }
}
