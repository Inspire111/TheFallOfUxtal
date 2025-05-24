using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private PlayerStats stats;
    private DirectionalMeleeAttack meleeAttack;
    private SpearAttack spearAttack;
    private BowAttack bowAttack;
    private Player_mvt playerMovement;
    private InputSystem_Actions inputActions;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        meleeAttack = GetComponent<DirectionalMeleeAttack>();
        spearAttack = GetComponent<SpearAttack>();
        bowAttack = GetComponent<BowAttack>();
        playerMovement = GetComponent<Player_mvt>();
        inputActions = playerMovement.GetInputActions();
        UpdateWeaponScripts();
    }

    void Update()
    {
        if (inputActions.Player.MeleeSelect.WasPressedThisFrame())
        {
            stats.currentWeapon = WeaponType.Melee;
            UpdateWeaponScripts();
        }

        if (inputActions.Player.SpearSelect.WasPressedThisFrame())
        {
            if (stats.hasSpear)
            {
                stats.currentWeapon = WeaponType.Spear;
                UpdateWeaponScripts();
            }
        }

        if (inputActions.Player.BowSelect.WasPressedThisFrame())
        {
            if (stats.hasBow)
            {
                stats.currentWeapon = WeaponType.Bow;
                UpdateWeaponScripts();
            }
        }

        if (inputActions.Player.PotionSelect.WasPressedThisFrame())
        {
            if (stats.currentWeapon != WeaponType.Potions)
            {
                stats.currentWeapon = WeaponType.Potions;
                stats.usingHealPotion = true;
            }
            else
            {
                stats.usingHealPotion = !stats.usingHealPotion;
            }

            UpdateWeaponScripts();
        }
    }
    
    void UpdateWeaponScripts()
    {
        meleeAttack.enabled = (stats.currentWeapon == WeaponType.Melee);
        spearAttack.enabled = (stats.currentWeapon == WeaponType.Spear);
        bowAttack.enabled = (stats.currentWeapon == WeaponType.Bow);
        stats.UpdateAllStatsText();
    }
    
}

