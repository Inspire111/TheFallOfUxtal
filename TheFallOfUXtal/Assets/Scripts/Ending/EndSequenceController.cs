using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndSequenceController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;  // Canvas Group on the Canvas
    [SerializeField] private GameObject fadePanel;          // Black bg
    [SerializeField] private GameObject completePanel;     // The “Tutorial Completed” panel

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;      // Time to fade to black
    [SerializeField] private float displayDuration = 5f;   // How long to show the completion panel

    // Call this from your boss’s death logic
    public void TriggerEndSequence()
    {
        StartCoroutine(EndSequenceRoutine());
    }

    private IEnumerator EndSequenceRoutine()
    {
        fadePanel.SetActive(true);

        // 2) Show completion panel
        completePanel.SetActive(true);

        // 3) Wait
        yield return new WaitForSeconds(displayDuration);

        // 4) Load Menu scene
        SceneManager.LoadScene("Menu");
    }
}
