using UnityEngine;
using TMPro;

public enum WeaponType
{
    Melee,
    Spear
}

public class PlayerStats : MonoBehaviour
{
    [Header("Health Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Energy Stats")]
    public int maxEnergy = 50;
    public int currentEnergy;

    [Header("Shield Stats")]
    public int maxShield = 30;
    public int currentShield;

    [Header("Weapons Owned")]
    public bool hasMelee = true;
    public bool hasSpear = true;  

    [Header("Current Weapon")]
    public WeaponType currentWeapon = WeaponType.Melee;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI shieldText;

    void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        currentShield = maxShield;

        UpdateAllStatsText();
    }

    public void TakeDamage(int damage)
    {
        if (currentShield > 0)
        {
            int shieldDamage = Mathf.Min(damage, currentShield);
            currentShield -= shieldDamage;
            damage -= shieldDamage;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player took " + damage + " damage. HP left: " + currentHealth);

        UpdateAllStatsText();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player healed. HP: " + currentHealth);

        UpdateAllStatsText();
    }

    public void UseEnergy(int amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        Debug.Log("Player used " + amount + " energy. Energy left: " + currentEnergy);

        UpdateAllStatsText();
    }

    public void RechargeEnergy(int amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        Debug.Log("Player recharged " + amount + " energy. Energy: " + currentEnergy);

        UpdateAllStatsText();
    }

    void UpdateAllStatsText()
    {
        if (hpText != null)
            hpText.text = currentHealth + " / " + maxHealth;

        if (energyText != null)
            energyText.text = currentEnergy + " / " + maxEnergy;

        if (shieldText != null)
            shieldText.text = currentShield + " / " + maxShield;
    }
}
