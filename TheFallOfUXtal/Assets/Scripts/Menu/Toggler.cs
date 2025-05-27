// ToggleOnLoad.cs
using UnityEngine;

[DisallowMultipleComponent]
public class ToggleOnLoad : MonoBehaviour
{
    [Tooltip("Time in seconds to wait after scene load before toggling.")]
    public float delay = 2f;
    public GameObject music;

    void Start()
    {
        // Start the toggle coroutine
        StartCoroutine(ToggleAfterDelay());
    }

    private System.Collections.IEnumerator ToggleAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Toggle this GameObject's active state

        gameObject.SetActive(false);
        music.SetActive(true);
    }
}
