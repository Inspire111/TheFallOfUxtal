using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShop : MonoBehaviour
{
    public GameObject shopUI;

    private bool shopOpen = false;
    private InputSystem_Actions inputActions;
    private PlayerStats stats;
    private Player_mvt movement;

    [Header("Prices")]
    public int spearCost = 10;
    public int bowCost = 10;
    public int energyCost = 15;
    public int healPotionCost = 5;
    public int shieldPotionCost = 5;

    [Header("UI References")]
    public TextMeshProUGUI spearPrice;
    public TextMeshProUGUI bowPrice;
    public TextMeshProUGUI energyPrice;
    public TextMeshProUGUI healPotionPrice;
    public TextMeshProUGUI shieldPotionPrice;

    public Button buySpear;
    public Button buyBow;
    public Button buyEnergy;
    public Button buyHealPotion;
    public Button buyShieldPotion;
    public Button EXIT;

    private Image spearImage;
    private Image bowImage;
    private Image energyImage;
    private Image healPotionImage;
    private Image shieldPotionImage;

    private void Start()
    {
        movement = GetComponent<Player_mvt>();
        inputActions = movement.GetInputActions();
        stats = GetComponent<PlayerStats>();

        shopUI.SetActive(false);

        // Hook up button listeners
        buySpear.onClick.AddListener(BuySpear);
        buyBow.onClick.AddListener(BuyBow);
        buyEnergy.onClick.AddListener(BuyEnergyUpgrade);
        buyHealPotion.onClick.AddListener(BuyHealPotion);
        buyShieldPotion.onClick.AddListener(BuyShieldPotion);
        EXIT.onClick.AddListener(CloseShop);

        // Cache the button backgrounds for color manipulation
        spearImage = buySpear.GetComponent<Image>();
        bowImage = buyBow.GetComponent<Image>();
        energyImage = buyEnergy.GetComponent<Image>();
        healPotionImage = buyHealPotion.GetComponent<Image>();
        shieldPotionImage = buyShieldPotion.GetComponent<Image>();
    }

    private void Update()
    {
        if (shopOpen && inputActions != null)
        {
            if (inputActions.Player.Interact.WasPressedThisFrame())
            {
                Debug.Log("Interact input detected while shop is open.");
                CloseShop();
            }

            UpdateButtonStates();
        }
    }

    public void OpenShop()
    {
        if (shopOpen) return;

        shopOpen = true;
        shopUI.SetActive(true);
        movement.SetMovementEnabled(false);

        // Set prices with "or"
        spearPrice.text = spearCost + " or";
        bowPrice.text = bowCost + " or";
        energyPrice.text = energyCost + " or";
        healPotionPrice.text = healPotionCost + " or";
        shieldPotionPrice.text = shieldPotionCost + " or";

        UpdateButtonStates();
    }

    public void CloseShop()
    {
        Debug.Log("Closing shop...");
        shopOpen = false;
        shopUI.SetActive(false);
        movement.SetMovementEnabled(true);
    }

    private void UpdateButtonStates()
    {
        SetButtonState(buySpear, spearImage, !stats.hasSpear, stats.Gold >= spearCost);
        SetButtonState(buyBow, bowImage, !stats.hasBow, stats.Gold >= bowCost);
        SetButtonState(buyEnergy, energyImage, true, stats.Gold >= energyCost);
        SetButtonState(buyHealPotion, healPotionImage, true, stats.Gold >= healPotionCost);
        SetButtonState(buyShieldPotion, shieldPotionImage, true, stats.Gold >= shieldPotionCost);
    }

    private void SetButtonState(Button button, Image image, bool notOwned, bool canAfford)
    {
        bool interactable = notOwned && canAfford;
        button.interactable = interactable;

        if (image != null)
        {
            image.color = interactable ? Color.white : Color.red;
        }
    }

    public bool IsShopOpen() => shopOpen;
    public InputSystem_Actions GetInputActions() => inputActions;

    // ---- Shop Button Methods ----

    public void BuySpear()
    {
        if (stats.Gold >= spearCost && !stats.hasSpear)
        {
            stats.Gold -= spearCost;
            stats.hasSpear = true;
            stats.UpdateAllStatsText();
            UpdateButtonStates();
        }
    }

    public void BuyBow()
    {
        if (stats.Gold >= bowCost && !stats.hasBow)
        {
            stats.Gold -= bowCost;
            stats.hasBow = true;
            stats.UpdateAllStatsText();
            UpdateButtonStates();
        }
    }

    public void BuyHealPotion()
    {
        if (stats.Gold >= healPotionCost)
        {
            stats.Gold -= healPotionCost;
            stats.HealPotions += 1;
            stats.UpdateAllStatsText();
            UpdateButtonStates();
        }
    }

    public void BuyShieldPotion()
    {
        if (stats.Gold >= shieldPotionCost)
        {
            stats.Gold -= shieldPotionCost;
            stats.ShieldPotions += 1;
            stats.UpdateAllStatsText();
            UpdateButtonStates();
        }
    }

    public void BuyEnergyUpgrade()
    {
        if (stats.Gold >= energyCost)
        {
            stats.Gold -= energyCost;
            stats.maxEnergy += 5;
            stats.currentEnergy += 5;
            stats.UpdateAllStatsText();
            UpdateButtonStates();
        }
    }
}
