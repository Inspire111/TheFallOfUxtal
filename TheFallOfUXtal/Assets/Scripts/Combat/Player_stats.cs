using UnityEngine;
using TMPro; // Add this if using TextMeshPro

public class PlayerStats : MonoBehaviour
{
    // Health stats
    [Header("Health Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    // Energy stats
    [Header("Energy Stats")]
    public int maxEnergy = 50;
    public int currentEnergy;

    // Shield stats
    [Header("Shield Stats")]
    public int maxShield = 30;
    public int currentShield;

    // UI References
    public TextMeshProUGUI hpText; // Assign this in Inspector
    public TextMeshProUGUI energyText; // Assign this in Inspector
    public TextMeshProUGUI shieldText; // Assign this in Inspector

    void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        currentShield = maxShield;

        UpdateAllStatsText(); // Update all UI texts at the start
    }

    // Function to take damage
    public void TakeDamage(int damage)
    {
        // If the player has a shield, absorb damage with shield first
        if (currentShield > 0)
        {
            int shieldDamage = Mathf.Min(damage, currentShield);
            currentShield -= shieldDamage;
            damage -= shieldDamage;
        }

        // Apply remaining damage to health
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player took " + damage + " damage. HP left: " + currentHealth);

        UpdateAllStatsText();
    }

    // Function to heal
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player healed. HP: " + currentHealth);

        UpdateAllStatsText();
    }

    // Function to use energy
    public void UseEnergy(int amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        Debug.Log("Player used " + amount + " energy. Energy left: " + currentEnergy);

        UpdateAllStatsText();
    }

    // Function to recharge energy
    public void RechargeEnergy(int amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        Debug.Log("Player recharged " + amount + " energy. Energy: " + currentEnergy);

        UpdateAllStatsText();
    }

    // Function to update all stats text
    void UpdateAllStatsText()
    {
        if (hpText != null)
            hpText.text =  currentHealth + " / " + maxHealth;

        if (energyText != null)
            energyText.text = currentEnergy + " / " + maxEnergy;

        if (shieldText != null)
            shieldText.text = currentShield + " / " + maxShield;
    }
}
