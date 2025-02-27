using UnityEngine;

public class PlayerColorChange : MonoBehaviour
{
    private Renderer playerRenderer;
    private Color originalColor;
    public Color collisionColor = Color.red;

    private void Start()
    {
        playerRenderer = GetComponent<Renderer>();

        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
        else
        {
            Debug.LogError("Renderer component missing from the player!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = collisionColor;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
        }
    }
}