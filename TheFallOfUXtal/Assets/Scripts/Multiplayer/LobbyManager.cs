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

            await Task.Delay(3000);
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
            // Wait for the backend to sync
            await Task.Delay(1000);
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
    private bool isJoining = false;
    private async void UpdateLobbyInfo()
    {
        Debug.Log("UpdateLobbyInfo() started");
        while (Application.isPlaying)
        {
            await Task.Delay(5000);
            if (string.IsNullOrEmpty(joinedLobbyId)) return;

            Lobby lobby;
            try
            {
                lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyId);
                Debug.Log("Fetched lobby: " + lobby.Name);
            }
            catch
            {
                Debug.LogWarning("Failed to get lobby.");
                continue;
            }

            // ✅ Ensure only one join attempt
            if (!isJoined && !isJoining && lobby.Data["JoinCode"].Value != string.Empty)
            {
                isJoining = true;
                try
                {
                    await relayManager.StartClientWithRelay(lobby.Data["JoinCode"].Value);
                    isJoined = true;
                    if (joinedLobbyParent != null)
                    {

                        joinedLobbyParent.SetActive(false);
                        NetworkManager.Singleton.SceneManager.LoadScene("Multi", LoadSceneMode.Single);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Join failed: " + e.Message);
                    isJoined = false;
                }
                return;
            }

            joinedLobbyNameText.text = lobby.Name;

            foreach (Transform t in playerListParent)
            {
                Destroy(t.gameObject);
            }

            foreach (Player player in lobby.Players)
            {
                Transform newPlayerItem = Instantiate(playerItemPrefab, playerListParent);

                string playerDisplayName = player.Data.ContainsKey("Name") ? player.Data["Name"].Value : "Unnamed";
                string role = lobby.HostId == player.Id ? "Owner" : "User";

                newPlayerItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerDisplayName;
                newPlayerItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = role;

                Debug.Log($"Added player: {playerDisplayName} ({role})");
            }
        }
    }

    public async void LobbyStart()
{
    if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
    {
        Debug.LogWarning("Already connected to a relay!");
        return;
    }

    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyId);
    string joinCode = await relayManager.StartHostWithRelay(lobby.MaxPlayers);
    Debug.Log("JoinCode on StartHostWithRelay: " + joinCode);

    isJoined = true;

    try
    {
        await LobbyService.Instance.UpdateLobbyAsync(joinedLobbyId, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                { "JoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
            }
        });

        Debug.Log("JoinCode successfully updated in lobby.");
    }
    catch (LobbyServiceException e)
    {
        Debug.LogError("Failed to update lobby with join code: " + e);
        return;
    }

    // ✅ Delay a moment to let clients see the update
    await Task.Delay(1500);

    lobbyListParent.SetActive(false);
    joinedLobbyParent.SetActive(false);
    NetworkManager.Singleton.SceneManager.LoadScene("Multi", LoadSceneMode.Single);
}


}
