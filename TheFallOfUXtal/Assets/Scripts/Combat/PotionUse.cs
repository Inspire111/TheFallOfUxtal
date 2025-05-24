using UnityEngine;

public class PotionUser : MonoBehaviour
{
    private PlayerStats stats;
    private Player_mvt movement;
    private InputSystem_Actions inputActions;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        movement = GetComponent<Player_mvt>();
        inputActions = movement.GetInputActions();

        inputActions.Enable();
    }


    void Update()
    {
        if (stats.currentWeapon != WeaponType.Potions)
            return;

        if (inputActions.Player.Attack.WasPressedThisFrame())
        {
            Debug.Log("Potion use attempt");

            if (stats.usingHealPotion && stats.HealPotions > 0)
            {
                Debug.Log("Using heal potion");
                stats.Heal(20);
                stats.HealPotions--;
            }
            else if (!stats.usingHealPotion && stats.ShieldPotions > 0)
            {
                Debug.Log("Using shield potion");
                stats.GainShield(25);
                stats.ShieldPotions--;
            }

            stats.UpdateAllStatsText();
        }
    }
}
