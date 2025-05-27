using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Energy Regen")]
    public float energyRegenRate = 5f;

    [Header("Weapons Owned")]
    public bool hasMelee = true;
    public bool hasSpear = false;
    public bool hasBow = false;

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

    public Image healPotionImage;
    public Image shieldPotionImage;

    [Header("Respawn")]
    public Transform respawnPoint;

    [Header("Death Screen")]
    public GameObject deathScreen;

    [HideInInspector] public bool usingHealPotion = true;

    private float energyAccumulator = 0f;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        currentEnergy = maxEnergy;
        UpdateAllStatsText();

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    private void Update()
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
                currentEnergy = Mathf.Clamp(currentEnergy + energyToAdd, 0, maxEnergy);
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

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            if (deathScreen != null)
                deathScreen.SetActive(true);

            StartCoroutine(RespawnAfterDelay(3f));
        }
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Respawn();
    }

    public void Respawn()
    {
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            Debug.Log("Player respawned at checkpoint.");
        }
        else
        {
            Debug.LogWarning("No respawn point set for the player!");
        }

        currentHealth = maxHealth;
        currentShield = maxShield;
        currentEnergy = maxEnergy;
        isDead = false;

        if (deathScreen != null)
            deathScreen.SetActive(false);

        UpdateAllStatsText();
    }

    public void SetRespawnPoint(Transform newPoint)
    {
        respawnPoint = newPoint;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateAllStatsText();
    }

    public void GainShield(int amount)
    {
        currentShield = Mathf.Clamp(currentShield + amount, 0, maxShield);
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
            hpText.text = $"{currentHealth} / {maxHealth}";
        if (shieldText != null)
            shieldText.text = $"{currentShield} / {maxShield}";
        if (energyText != null)
            energyText.text = $"{currentEnergy} / {maxEnergy}";
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
                swordBackground.color = Color.green;
                break;
            case WeaponType.Spear:
                spearBackground.color = hasSpear ? Color.green : Color.red;
                break;
            case WeaponType.Bow:
                bowBackground.color = hasBow ? Color.green : Color.red;
                break;
            case WeaponType.Potions:
                healPotionImage?.gameObject.SetActive(usingHealPotion);
                shieldPotionImage?.gameObject.SetActive(!usingHealPotion);
                break;
        }
    }
}
