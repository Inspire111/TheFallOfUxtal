using UnityEngine;
using UnityEngine.LowLevel;

public class ShopTile : MonoBehaviour
{
    private bool playerInRange = false;
    private PlayerShop playerShop;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerShop = other.GetComponent<PlayerShop>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && playerShop != null && !playerShop.IsShopOpen() &&
            playerShop.GetInputActions().Player.Interact.WasPressedThisFrame())
        {
            playerShop.OpenShop();
        }
    }
}
