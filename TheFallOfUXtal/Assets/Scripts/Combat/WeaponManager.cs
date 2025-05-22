using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private PlayerStats stats;
    private DirectionalMeleeAttack meleeAttack;
    private SpearAttack spearAttack;
    private Player_mvt playerMovement;
    private InputSystem_Actions inputActions;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        meleeAttack = GetComponent<DirectionalMeleeAttack>();
        spearAttack = GetComponent<SpearAttack>();
        playerMovement = GetComponent<Player_mvt>();
        inputActions = playerMovement.GetInputActions();

        UpdateWeaponScripts();
    }

    void Update()
    {
        if (inputActions.Player.Previous.WasPressedThisFrame())
        {
            stats.currentWeapon = WeaponType.Melee;
            Debug.Log("Switched to Melee");
            UpdateWeaponScripts();
        }

        if (inputActions.Player.Next.WasPressedThisFrame())
        {
            if (stats.hasSpear)
            {
                stats.currentWeapon = WeaponType.Spear;
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
        meleeAttack.enabled = (stats.currentWeapon == WeaponType.Melee);
        spearAttack.enabled = (stats.currentWeapon == WeaponType.Spear);
    }
}


