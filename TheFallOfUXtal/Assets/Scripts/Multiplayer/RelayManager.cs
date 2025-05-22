using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

public class RelayManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI joinCodeText;

    private bool isStartingRelay = false; // ✅ Prevent spam
    private float relayCooldownTime = 5f; // Optional delay buffer
    private float lastRelayStartTime = -10f;

    public async void StartRelay()
    {
        if (isStartingRelay || Time.time - lastRelayStartTime < relayCooldownTime)
        {
            Debug.LogWarning("Relay start ignored: either already starting or called too soon.");
            return;
        }

        isStartingRelay = true;
        lastRelayStartTime = Time.time;

        try
        {
            string joinCode = await StartHostWithRelay();
            joinCodeText.text = joinCode;
            Debug.Log("StartRelay - " + joinCode);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Relay setup failed: " + e.Message);
        }
        finally
        {
            isStartingRelay = false;
        }
    }

    public async void JoinRelay()
    {
        if (string.IsNullOrWhiteSpace(joinCodeText.text))
        {
            Debug.LogWarning("JoinRelay failed: no join code provided.");
            return;
        }

        try
        {
            await StartClientWithRelay(joinCodeText.text);
        }
        catch (System.Exception e)
        {
            Debug.LogError("JoinRelay failed: " + e.Message);
        }
    }

    public async Task<string> StartHostWithRelay(int maxConnections = 3)
    {
        Allocation allocation;

        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Creating allocation failed: " + e.Message);
            throw;
        }

        var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log("StartHostWithRelay with code: " + joinCode);

        bool started = NetworkManager.Singleton.StartHost();
        return started ? joinCode : null;
    }

    public async Task<bool> StartClientWithRelay(string joinCode)
    {
        Debug.Log("StartClientWithRelay with code: " + joinCode);

        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        return NetworkManager.Singleton.StartClient();
    }
}
