using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Textos da HUD")]
    public TextMeshProUGUI txtScrap;
    public TextMeshProUGUI txtPPS;

    [Header("Painéis")]
    public GameObject painelUpgrades;

    [Header("Seta do Menu")]
    public Image iconeSeta;
    public Sprite setaMenuFechado; // A sprite da seta normal
    public Sprite setaMenuAberto;  // A sprite da seta para baixo

    [Header("Textos do Upgrade Base (Nave Mãe)")]
    public TextMeshProUGUI txtBaseCost;
    public TextMeshProUGUI txtBaseLevel;
    public Button btnBaseUpgrade;

    void Update()
    {
        if (EconomyManager.Instance != null)
        {
            // Atualiza HUD principal (com checagem de nulo defensiva)
            if (txtScrap != null) txtScrap.text = EconomyManager.Instance.currentScrap.ToString("F0");
            if (txtPPS != null) txtPPS.text = "+" + EconomyManager.Instance.scrapPerSecond.ToString("F1") + "/s";

            // Atualiza Upgrade Base (Nave Mãe)
            double baseCost = EconomyManager.Instance.GetBaseUpgradeCost();
            bool baseCanAfford = EconomyManager.Instance.currentScrap >= baseCost;
            
            Color baseTextColor;
            if (baseCanAfford)
            {
                ColorUtility.TryParseHtmlString("#4C4C4C", out baseTextColor);
            }
            else
            {
                baseTextColor = new Color(0.9f, 0.3f, 0.3f);
            }
            
            if (txtBaseCost != null) 
            {
                txtBaseCost.text = baseCost.ToString("F0") + " Scrap";
                txtBaseCost.color = baseTextColor;
            }
            if (txtBaseLevel != null) 
            {
                txtBaseLevel.text = "Lvl " + EconomyManager.Instance.baseUpgradeLevel;
                txtBaseLevel.color = baseTextColor;
            }
            
            if (btnBaseUpgrade != null) 
            {
                btnBaseUpgrade.interactable = baseCanAfford;
                Image btnImage = btnBaseUpgrade.GetComponent<Image>();
                if (btnImage != null)
                {
                    btnImage.color = baseCanAfford ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.6f);
                }
            }
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

    // --- MÉTODOS DE COMPRA GLOBAL ---

    public void PurchaseBaseUpgrade()
    {
        if (EconomyManager.Instance != null) EconomyManager.Instance.BuyBaseUpgrade();
    }
}
