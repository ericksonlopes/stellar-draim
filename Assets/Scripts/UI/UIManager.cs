using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Textos da HUD (Legacy)")]
    public Text txtScrap;
    public Text txtPPS;

    [Header("Painéis")]
    public GameObject painelUpgrades;

    [Header("Seta do Menu")]
    public Image iconeSeta;
    public Sprite setaMenuFechado; // A sprite da seta normal
    public Sprite setaMenuAberto;  // A sprite da seta para baixo

    [Header("Textos dos Upgrades")]
    public Text txtBaseCost;
    public Text txtBaseLevel;
    public Text txtFleetCost;
    public Text txtFleetLevel;

    [Header("Botões dos Upgrades")]
    public Button btnBaseUpgrade;
    public Button btnFleetUpgrade;

    void Update()
    {
        if (EconomyManager.Instance != null)
        {
            // Atualiza HUD principal
            txtScrap.text = EconomyManager.Instance.currentScrap.ToString("F0");
            txtPPS.text = "+" + EconomyManager.Instance.scrapPerSecond.ToString("F1") + "/s";

            // Atualiza UI de Upgrades
            double baseCost = EconomyManager.Instance.GetBaseUpgradeCost();
            double fleetCost = EconomyManager.Instance.GetFleetUpgradeCost();

            if (txtBaseCost != null) txtBaseCost.text = baseCost.ToString("F0") + " Scrap";
            if (txtBaseLevel != null) txtBaseLevel.text = "Lvl " + EconomyManager.Instance.baseUpgradeLevel;

            if (txtFleetCost != null) txtFleetCost.text = fleetCost.ToString("F0") + " Scrap";
            if (txtFleetLevel != null) txtFleetLevel.text = "Lvl " + EconomyManager.Instance.fleetLevel;

            // Habilita/desabilita botões baseado na grana atual
            if (btnBaseUpgrade != null) btnBaseUpgrade.interactable = EconomyManager.Instance.currentScrap >= baseCost;
            if (btnFleetUpgrade != null) btnFleetUpgrade.interactable = EconomyManager.Instance.currentScrap >= fleetCost;
        }
    }

    public void ToggleMenu()
    {
        if (painelUpgrades != null)
        {
            bool vaiAbrir = !painelUpgrades.activeSelf;
            painelUpgrades.SetActive(vaiAbrir);

            // Troca a sprite da seta dependendo se o menu abriu ou fechou
            if (iconeSeta != null)
            {
                iconeSeta.sprite = vaiAbrir ? setaMenuAberto : setaMenuFechado;
            }
        }
    }

    public void PurchaseBaseUpgrade()
    {
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.BuyBaseUpgrade();
        }
    }

    public void PurchaseFleetUpgrade()
    {
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.BuyFleetUpgrade();
        }
    }
}
