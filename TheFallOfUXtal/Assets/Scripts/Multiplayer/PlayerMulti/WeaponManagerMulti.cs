using UnityEngine;

public class WeaponManagerMulti : MonoBehaviour
{
    private PlayerStatsMulti stats;
    private DirectionalMeleeAttackMulti meleeAttack;
    private SpearAttackMulti spearAttackMulti;
    private Player_mvtMulti playerMovement;
    private InputSystem_Actions inputActions;

    void Start()
    {
        stats = GetComponent<PlayerStatsMulti>();
        meleeAttack = GetComponent<DirectionalMeleeAttackMulti>();
        spearAttackMulti = GetComponent<SpearAttackMulti>();
        playerMovement = GetComponent<Player_mvtMulti>();
        inputActions = playerMovement.GetInputActions();

        UpdateWeaponScripts();
    }

    void Update()
    {
        if (inputActions.Player.MeleeSelect.WasPressedThisFrame())
        {
            stats.currentWeapon = WeaponTypeMulti.Melee;
            Debug.Log("Switched to Melee");
            UpdateWeaponScripts();
        }

        if (inputActions.Player.BowSelect.WasPressedThisFrame())
        {
            if (stats.hasSpear)
            {
                stats.currentWeapon = WeaponTypeMulti.Spear;
                Debug.Log("Switched to Spear");
                UpdateWeaponScripts();
            }
            else
            {
                Debug.Log("You don't own a spear!");
            }
        }
    }
    
    void UpdateWeaponScripts()
    {
        meleeAttack.enabled = stats.currentWeapon == WeaponTypeMulti.Melee;
        spearAttackMulti.enabled = stats.currentWeapon == WeaponTypeMulti.Spear;
    }
    
}


