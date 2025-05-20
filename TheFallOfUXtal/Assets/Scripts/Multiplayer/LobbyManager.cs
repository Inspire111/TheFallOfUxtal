    using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance {get;private set;}

    [SerializeField] private RelayManager relayManager;

    [Header("Lobby creation")]
    [SerializeField] private TMP_InputField createLobbyNameField;
    [SerializeField] private GameObject lobbyCreationParent;


    [Space(10)]
    [Header("Lobby list")]
    [SerializeField] private GameObject lobbyListParent;
    [SerializeField] private Transform lobbyContentParent;
    [SerializeField] private Transform lobbyItemPrefab;

    [Space(10)]
    [Header("Profile Setup")]
    [SerializeField] private GameObject profileSetupParent;
    [SerializeField] private TMP_InputField profileNameField;

    [Space(10)]
    [Header("Joined lobby")]
    [SerializeField] private GameObject joinedLobbyParent;
    [SerializeField] private Transform playerItemPrefab;
    [SerializeField] private Transform playerListParent;
    [SerializeField] private TextMeshProUGUI joinedLobbyNameText;
    [SerializeField] private GameObject joinedLobbyStartButton;
    
    private string playerName;
    private Player playerData;
    public string joinedLobbyId;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        Instance = this;

        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.ClearSessionToken();
        await AuthenticationService.Instance.SignInAnonymouslyAsync(new SignInOptions { CreateAccount = true });
        Debug.Log($"Signed in with PlayerID: {AuthenticationService.Instance.PlayerId}");

        profileSetupParent.SetActive(true);
        lobbyListParent.SetActive(false);
        joinedLobbyParent.SetActive(false);
        lobbyCreationParent.SetActive(false);
    }

    public void CreateProfile()
    {
        playerName = profileNameField.text;
        profileSetupParent.SetActive(false);
        lobbyListParent.SetActive(true);
        ShowLobbies();

        PlayerDataObject playerDataObjectName = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName);
        

        playerData = new Player(id: AuthenticationService.Instance.PlayerId, data:
        new Dictionary<string, PlayerDataObject> { { "Name", playerDataObjectName } });

        Debug.Log("Profile Created with Name :" + playerName);
    } 
    private async void ShowLobbies()
    {
        while(Application.isPlaying && lobbyListParent.activeInHierarchy)
        {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            foreach(Transform t in lobbyContentParent)
            {
                Destroy(t.gameObject);
            }

            foreach(Lobby lobby in queryResponse.Results)
            {
                Transform newLobbyItem = Instantiate(lobbyItemPrefab,lobbyContentParent);
                newLobbyItem.GetComponent<JoinLobbyButton>().lobbyId = lobby.Id;
                newLobbyItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
                newLobbyItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = lobby.Players.Count.ToString();
            }

            await Task.Delay(1000);
        }
    }

    public void ExitLobbyCreationButton()
    {
        lobbyCreationParent.SetActive(false);
        lobbyListParent.SetActive(true);
        ShowLobbies();
    }
    
    public void CreateNewLobbyButton()
    {
        lobbyCreationParent.SetActive(true);
        lobbyListParent.SetActive(false);
    }

    public async void CreateLobby()
    {
        Lobby createdLobby = null;

        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;
        options.Player = playerData;

        DataObject DataObjectJoinCode = new DataObject(DataObject.VisibilityOptions.Public, string.Empty);
        options.Data = new Dictionary<string, DataObject> {{"JoinCode", DataObjectJoinCode}};
        try
        {
            createdLobby = await LobbyService.Instance.CreateLobbyAsync(createLobbyNameField.text,2,options);
            lobbyCreationParent.SetActive(false);
            joinedLobbyParent.SetActive(true);
            joinedLobbyId = createdLobby.Id;
            UpdateLobbyInfo();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        LobbyHeartBeat(createdLobby);
    }

    private async void LobbyHeartBeat(Lobby lobby)
    {
        await Task.Delay(1000);
        while(true)
        {
            if(lobby == null)
                return;
            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);

            await Task.Delay(15 * 1000);
        }
    }

    public async void JoinLobby(string lobbyID)
    {
        try
        {
            await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID, new JoinLobbyByIdOptions {Player = playerData});

            joinedLobbyId = lobbyID;
            lobbyListParent.SetActive(false);
            joinedLobbyParent.SetActive(true);
            UpdateLobbyInfo();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private bool isJoined = false;
    private async void UpdateLobbyInfo()
    {
        while (Application.isPlaying)
        {
            if (string.IsNullOrEmpty(joinedLobbyId))
            {
                return;
            }

            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyId);

            if(!isJoined && lobby.Data["JoinCode"].Value != string.Empty)
            {
                await relayManager.StartClientWithRelay(lobby.Data["JoinCode"].Value);
                isJoined = true;
                if (joinedLobbyParent != null)
                {
                    joinedLobbyParent.SetActive(false);
                    NetworkManager.Singleton.SceneManager.LoadScene("Multi",LoadSceneMode.Single);
                }
                else
                {
                    Debug.LogError("joinedLobbyParent is null in UpdateLobbyInfo()");
                }
                return;
            }
            if (joinedLobbyStartButton != null)
            {
                if(AuthenticationService.Instance.PlayerId == lobby.HostId)
                {
                    joinedLobbyStartButton.SetActive(true);
                }
                else
                {
                    joinedLobbyStartButton.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("joinedLobbyStartButton is null in UpdateLobbyInfo()");
            }

            

            joinedLobbyNameText.text = lobby.Name;

            foreach (Transform t in playerListParent)
            {
                Destroy(t.gameObject);
            }

            foreach (Player player in lobby.Players)
            {
                Transform newPlayerItem = Instantiate(playerItemPrefab, playerListParent);
                newPlayerItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.Data["Name"].Value;
                newPlayerItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = (lobby.HostId == player.Id) ? "Owner" : "User";
            }

            await Task.Delay(1000);
        }
    }

    public async void LobbyStart()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("Already connected to a relay!");
            return; // Prevent starting another instance
        }

        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyId);
        string JoinCode = await relayManager.StartHostWithRelay(lobby.MaxPlayers);
        Debug.Log("JoinCode on StartHostWithRelay : "+ JoinCode);
        isJoined = true;
        await LobbyService.Instance.UpdateLobbyAsync(joinedLobbyId, new UpdateLobbyOptions
        { Data = new Dictionary<string, DataObject> { { "JoinCode", new DataObject(DataObject.VisibilityOptions.Public, JoinCode) } } });

        lobbyListParent.SetActive(false);
        joinedLobbyParent.SetActive(false);
        NetworkManager.Singleton.SceneManager.LoadScene("World",LoadSceneMode.Single);
    }

}
