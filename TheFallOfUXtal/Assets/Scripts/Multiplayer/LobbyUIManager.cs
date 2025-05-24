using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager Instance { get; private set; }

    [Header("Lobby Panels")]
    [SerializeField] public GameObject profileSetupParent;
    [SerializeField] public GameObject lobbyListParent;
    [SerializeField] public GameObject lobbyCreationParent;
    [SerializeField] public GameObject joinedLobbyParent;

    [Header("UI References")]
    [SerializeField] public TMP_InputField profileNameField;
    [SerializeField] public TMP_InputField createLobbyNameField;
    [SerializeField] public Transform lobbyContentParent;
    [SerializeField] public Transform lobbyItemPrefab;
    [SerializeField] public Transform playerItemPrefab;
    [SerializeField] public Transform playerListParent;
    [SerializeField] public TextMeshProUGUI joinedLobbyNameText;

    public void Awake()
    {
        Instance = this;
    }
    public void InitUI()
    {
        profileSetupParent.SetActive(true);
        lobbyListParent.SetActive(false);
        joinedLobbyParent.SetActive(false);
        lobbyCreationParent.SetActive(false);
    }

    public string GetPlayerName() => profileNameField.text;
    public string GetLobbyName() => createLobbyNameField.text;

    public void ShowLobbyList()
    {
        lobbyListParent.SetActive(true);
        lobbyCreationParent.SetActive(false);
        profileSetupParent.SetActive(false);
    }

    public void ShowJoinedLobby(bool show)
    {
        joinedLobbyParent.SetActive(show);
        lobbyListParent.SetActive(!show);
        lobbyCreationParent.SetActive(!show);
    }

    public void ShowLobbyCreation()
    {
        lobbyListParent.SetActive(false);
        lobbyCreationParent.SetActive(true);
    }
    public void SetLobbyName(string name)
    {
        if (joinedLobbyNameText != null)
            joinedLobbyNameText.text = name;
    }

    public void UpdatePlayerList(List<(string name, string role)> players)
    {
        foreach (Transform child in playerListParent)
            Destroy(child.gameObject);

        foreach (var player in players)
        {
            Transform newPlayerItem = Instantiate(playerItemPrefab, playerListParent);
            newPlayerItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.name;
            newPlayerItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.role;
        }
    }

    public void ClearAllLobbyUI()
    {
        joinedLobbyParent.SetActive(false);
    }
}

