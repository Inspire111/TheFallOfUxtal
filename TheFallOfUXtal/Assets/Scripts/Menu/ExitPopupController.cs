using UnityEngine;

public class ExitPopupController : MonoBehaviour
{
    public GameObject exitPopup; // Reference to the Exit Popup Panel

    // Show the popup
    public void ShowExitPopup()
    {
        exitPopup.SetActive(true);
    }

    // Hide the popup
    public void HideExitPopup()
    {
        exitPopup.SetActive(false);
    }

    // Quit the game
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#endif
    }
}
