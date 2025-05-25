using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum WeaponType
{
    Melee,
    Spear,
    Bow,
    Potions
}

public class PlayerStats : MonoBehaviour
{
    [Header("Health Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Shield Stats")]
    public int maxShield = 30;
    public int currentShield;

    [Header("Energy Stats")]
    public int maxEnergy = 30;
    public int currentEnergy;

    [Header("Gold Amount")]
    public int Gold = 0;

    [Header("Energy Regeneration")]
    public float energyRegenRate = 5f;

    [Header("Weapons Owned")]
    public bool hasMelee = true;
    public bool hasSpear = true;
    public bool hasBow = true;

    [Header("Potions")]
    public int HealPotions = 0;
    public int ShieldPotions = 0;

    [Header("Current Weapon")]
    public WeaponType currentWeapon = WeaponType.Melee;

    [Header("UI References")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI shieldLeft;
    public TextMeshProUGUI healLeft;
    public TextMeshProUGUI goldLeft;

    public Image swordBackground;
    public Image spearBackground;
    public Image bowBackground;

    [Header("Potions UI")]
    public Image healPotionImage;
    public Image shieldPotionImage;

    [HideInInspector] public bool usingHealPotion = true;

    private float energyAccumulator = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        currentEnergy = maxEnergy;
        UpdateAllStatsText();
    }

    void Update()
    {
        RegenerateEnergy();
    }

    private void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            energyAccumulator += energyRegenRate * Time.deltaTime;

            if (energyAccumulator >= 1f)
            {
                int energyToAdd = Mathf.FloorToInt(energyAccumulator);
                currentEnergy += energyToAdd;
                currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
                energyAccumulator -= energyToAdd;

                UpdateAllStatsText();
            }
        }
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
        UpdateAllStatsText();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateAllStatsText();
    }

    public void GainShield(int amount)
    {
        currentShield += amount;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
        UpdateAllStatsText();
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        UpdateAllStatsText();
    }

    public void UpdateAllStatsText()
    {
        if (hpText != null)
            hpText.text = currentHealth + " / " + maxHealth;
        if (shieldText != null)
            shieldText.text = currentShield + " / " + maxShield;
        if (energyText != null)
            energyText.text = currentEnergy + " / " + maxEnergy;
        if (shieldLeft != null)
            shieldLeft.text = ShieldPotions.ToString();
        if (healLeft != null)
            healLeft.text = HealPotions.ToString();
        if (goldLeft != null)
            goldLeft.text = Gold.ToString();

        if (swordBackground != null)
            swordBackground.color = Color.white;

        if (spearBackground != null)
            spearBackground.color = hasSpear ? Color.white : Color.red;

        if (bowBackground != null)
            bowBackground.color = hasBow ? Color.white : Color.red;

        if (healPotionImage != null)
            healPotionImage.gameObject.SetActive(false);
        if (shieldPotionImage != null)
            shieldPotionImage.gameObject.SetActive(false);

        switch (currentWeapon)
        {
            case WeaponType.Melee:
                if (swordBackground != null)
                    swordBackground.color = Color.green;
                break;

            case WeaponType.Spear:
                if (spearBackground != null)
                    spearBackground.color = hasSpear ? Color.green : Color.red;
                break;

            case WeaponType.Bow:
                if (bowBackground != null)
                    bowBackground.color = hasBow ? Color.green : Color.red;
                break;

            case WeaponType.Potions:
                if (healPotionImage != null)
                    healPotionImage.gameObject.SetActive(usingHealPotion);
                if (shieldPotionImage != null)
                    shieldPotionImage.gameObject.SetActive(!usingHealPotion);
                break;
        }
    }
}

