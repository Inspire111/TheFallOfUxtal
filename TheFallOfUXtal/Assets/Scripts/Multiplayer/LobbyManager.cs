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


public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [SerializeField] private RelayManager relayManager;

    private Player playerData;
    private string joinedLobbyId;
    private bool isJoined = false;
    private bool isJoining = false;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log($"Signed in: {AuthenticationService.Instance.PlayerId}");

        LobbyUIManager.Instance.InitUI();
    }

    public void CreateProfile()
    {
        string playerName = LobbyUIManager.Instance.GetPlayerName();

        playerData = new Player(
            id: AuthenticationService.Instance.PlayerId,
            data: new Dictionary<string, PlayerDataObject>
            {
                { "Name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
            });

        LobbyUIManager.Instance.ShowLobbyList();
        ShowLobbies();
    }

    private async void ShowLobbies()
    {
        while (SceneManager.GetActiveScene().name == "Lobby")
        {
            var lobbies = await LobbyService.Instance.QueryLobbiesAsync();

            foreach (Transform t in LobbyUIManager.Instance.lobbyContentParent)
                Destroy(t.gameObject);

            foreach (Lobby lobby in lobbies.Results)
            {
                var item = Instantiate(LobbyUIManager.Instance.lobbyItemPrefab, LobbyUIManager.Instance.lobbyContentParent);
                item.GetComponent<JoinLobbyButton>().lobbyId = lobby.Id;
                item.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
                item.GetChild(1).GetComponent<TextMeshProUGUI>().text = lobby.Players.Count.ToString();
            }

            await Task.Delay(3000);
        }
    }

    public async void CreateLobby()
    {
        string name = LobbyUIManager.Instance.GetLobbyName();

        var options = new CreateLobbyOptions
        {
            IsPrivate = false,
            Player = playerData,
            Data = new Dictionary<string, DataObject>
            {
                { "JoinCode", new DataObject(DataObject.VisibilityOptions.Public, string.Empty) }
            }
        };

        Lobby createdLobby = await LobbyService.Instance.CreateLobbyAsync(name, 2, options);
        joinedLobbyId = createdLobby.Id;

        LobbyUIManager.Instance.ShowJoinedLobby(true);
        _ = LobbyHeartBeat(createdLobby);
        _ = UpdateLobbyInfo();
    }

    public async void JoinLobby(string lobbyId)
    {
        await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, new JoinLobbyByIdOptions { Player = playerData });
        joinedLobbyId = lobbyId;

        LobbyUIManager.Instance.ShowJoinedLobby(true);
        _ = UpdateLobbyInfo();
    }

    private async Task LobbyHeartBeat(Lobby lobby)
    {
        while (SceneManager.GetActiveScene().name == "Lobby")
        {
            await Task.Delay(15000);
            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
        }
    }

    private async Task UpdateLobbyInfo()
    {
        while (SceneManager.GetActiveScene().name == "Lobby")
        {
            await Task.Delay(5000);

            if (string.IsNullOrEmpty(joinedLobbyId)) return;

            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyId);
            LobbyUIManager.Instance.SetLobbyName(lobby.Name);

            // Check for relay join condition
            if (!isJoined && !isJoining && lobby.Data["JoinCode"].Value != string.Empty && lobby.HostId != AuthenticationService.Instance.PlayerId)
            {
                isJoining = true;

                try
                {
                    await relayManager.StartClientWithRelay(lobby.Data["JoinCode"].Value);
                    isJoined = true;

                    LobbyUIManager.Instance.ClearAllLobbyUI();
                    await Task.Delay(2000);
                    NetworkManager.Singleton.SceneManager.LoadScene("Multi", LoadSceneMode.Single);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Relay join failed: " + ex.Message);
                    isJoined = false;
                }

                return;
            }

            // Update player list
            var players = new List<(string, string)>();
            foreach (Player player in lobby.Players)
            {
                string name = player.Data.ContainsKey("Name") ? player.Data["Name"].Value : "Anonyme";
                string role = lobby.HostId == player.Id ? "Hote" : "Client";
                players.Add((name, role));
            }

            LobbyUIManager.Instance.UpdatePlayerList(players);
        }
    }

    public async void LobbyStart()
    {
        LobbyUIManager.Instance.ClearAllLobbyUI();
        Debug.Log("Clearing UI");
        await Task.Delay(2000);
        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyId);
        string joinCode = await relayManager.StartHostWithRelay(lobby.MaxPlayers);

        await LobbyService.Instance.UpdateLobbyAsync(joinedLobbyId, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                { "JoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
            }
        });

        await Task.Delay(1500);

        NetworkManager.Singleton.SceneManager.LoadScene("Multi", LoadSceneMode.Single);
    }

    
}
