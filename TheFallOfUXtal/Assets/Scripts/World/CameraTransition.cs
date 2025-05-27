using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraTransition : MonoBehaviour
{
    [Tooltip("Starting position for the camera on scene load.")]
    public Transform cameraStart;

    [Tooltip("Ending position the camera will move to.")]
    public Transform cameraEnd;

    [Tooltip("Duration of the transition in seconds.")]
    public float transitionDuration = 2f;

    [Tooltip("Delay at the start before beginning the move.")]
    public float startDelay = 3f;

    void Start()
    {
        if (cameraStart == null || cameraEnd == null)
        {
            Debug.LogError("CameraTransition: Assign both cameraStart and cameraEnd Transforms in the Inspector.");
            enabled = false;
            return;
        }

        // Snap immediately to the start
        transform.position = cameraStart.position;

        // Begin the delayed, eased transition
        StartCoroutine(DelayedTransition());
    }

    private IEnumerator DelayedTransition()
    {
        // 1) Wait at the start position
        yield return new WaitForSeconds(startDelay);

        // 2) Then do the smooth eased move
        yield return StartCoroutine(TransitionCamera());
    }

    private IEnumerator TransitionCamera()
    {
        float elapsed = 0f;
        Vector3 fromPos = cameraStart.position;
        Vector3 toPos = cameraEnd.position;

        while (elapsed < transitionDuration)
        {
            float tRaw = elapsed / transitionDuration;

            // Ease-in-out via SmoothStep
            float tEased = Mathf.SmoothStep(0f, 1f, tRaw);

            transform.position = Vector3.Lerp(fromPos, toPos, tEased);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure exact final position
        transform.position = toPos;
    }
}
