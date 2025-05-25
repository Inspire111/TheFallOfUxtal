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
    }

    void OnEnable()
    {
        if (inputActions == null)
        {
            movement = GetComponent<Player_mvt>();
            inputActions = movement.GetInputActions();
        }
    }

    void Update()
    {
        if (stats.currentWeapon != WeaponType.Potions) return;

        if (inputActions.Player.Attack.WasPressedThisFrame())
        {
            TryUsePotion();
        }
    }

    void TryUsePotion()
    {
        if (stats.usingHealPotion)
        {
            if (stats.HealPotions > 0)
            {
                stats.Heal(20);
                stats.HealPotions--;
                Debug.Log("Used Heal Potion.");
            }
            else
            {
                Debug.Log("No Heal Potions left!");
            }
        }
        else
        {
            if (stats.ShieldPotions > 0)
            {
                stats.GainShield(25);
                stats.ShieldPotions--;
                Debug.Log("Used Shield Potion.");
            }
            else
            {
                Debug.Log("No Shield Potions left!");
            }
        }

        stats.UpdateAllStatsText();
    }
}
