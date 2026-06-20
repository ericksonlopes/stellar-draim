using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FleetCollectorUIManager : MonoBehaviour
{
    [Header("UI - Collector Quantity (Fleet)")]
    public TextMeshProUGUI txtCollectorName;
    public TextMeshProUGUI txtCollectorInfo;
    public Button btnCollectorUpgrade;

    [Header("UI - Scanner Precision")]
    public TextMeshProUGUI txtScannerName;
    public TextMeshProUGUI txtScannerInfo;
    public Button btnScannerUpgrade;

    [Header("UI - Thruster Overdrive")]
    public TextMeshProUGUI txtThrusterName;
    public TextMeshProUGUI txtThrusterInfo;
    public Button btnThrusterUpgrade;

    [Header("UI - Cargo Expansion")]
    public TextMeshProUGUI txtCargoName;
    public TextMeshProUGUI txtCargoInfo;
    public Button btnCargoUpgrade;

    [Header("UI - Scrap Magnet")]
    public TextMeshProUGUI txtMagnetName;
    public TextMeshProUGUI txtMagnetInfo;
    public Button btnMagnetUpgrade;

    [Header("UI - Auto-Pilot AI")]
    public TextMeshProUGUI txtAIName;
    public TextMeshProUGUI txtAIInfo;
    public Button btnAIUpgrade;

    [Header("UI - Status do Coletor")]
    public TextMeshProUGUI txtActiveProbes;
    public TextMeshProUGUI txtTotalBonus;

    void Start()
    {
        // Define os nomes dos upgrades por código ao iniciar
        if (txtCollectorName != null) txtCollectorName.text = "Collector Fleet";
        if (txtScannerName != null) txtScannerName.text = "Scanner Precision";
        if (txtThrusterName != null) txtThrusterName.text = "Thruster Overdrive";
        if (txtCargoName != null) txtCargoName.text = "Cargo Expansion";
        if (txtMagnetName != null) txtMagnetName.text = "Scrap Magnet";
        if (txtAIName != null) txtAIName.text = "Auto-Pilot AI";
    }

    void Update()
    {
        if (EconomyManager.Instance != null)
        {
            // Atualiza status gerais do Coletor
            if (txtActiveProbes != null) 
            {
                txtActiveProbes.text = "Probes: " + (EconomyManager.Instance.collectorCount + 1);
            }
            if (txtTotalBonus != null) 
            {
                double collectorMultiplier = (EconomyManager.Instance.collectorCount) * (0.10 + (EconomyManager.Instance.scannerLevel * 0.02) + (EconomyManager.Instance.cargoLevel * 0.05));
                txtTotalBonus.text = "Bonus: +" + (collectorMultiplier * 100.0).ToString("F0") + "%";
            }

            // Atualiza os 6 Upgrades do Collector na aba Fleet com textos de nome e info unificados na cor
            UpdateUpgradeUI(EconomyManager.Instance.GetCollectorCost(), EconomyManager.Instance.collectorCount, txtCollectorName, txtCollectorInfo, btnCollectorUpgrade);
            UpdateUpgradeUI(EconomyManager.Instance.GetScannerCost(), EconomyManager.Instance.scannerLevel, txtScannerName, txtScannerInfo, btnScannerUpgrade);
            UpdateUpgradeUI(EconomyManager.Instance.GetThrusterCost(), EconomyManager.Instance.thrusterLevel, txtThrusterName, txtThrusterInfo, btnThrusterUpgrade);
            UpdateUpgradeUI(EconomyManager.Instance.GetCargoCost(), EconomyManager.Instance.cargoLevel, txtCargoName, txtCargoInfo, btnCargoUpgrade);
            UpdateUpgradeUI(EconomyManager.Instance.GetMagnetCost(), EconomyManager.Instance.magnetLevel, txtMagnetName, txtMagnetInfo, btnMagnetUpgrade);
            UpdateUpgradeUI(EconomyManager.Instance.GetAICost(), EconomyManager.Instance.aiLevel, txtAIName, txtAIInfo, btnAIUpgrade);
        }
    }

    void UpdateUpgradeUI(double cost, int level, TextMeshProUGUI txtName, TextMeshProUGUI txtInfo, Button btnBuy)
    {
        bool canAfford = EconomyManager.Instance != null && EconomyManager.Instance.currentScrap >= cost;
        
        // Define as cores: #4C4C4C quando disponível, vermelho suave quando indisponível
        Color textColor;
        if (canAfford)
        {
            ColorUtility.TryParseHtmlString("#4C4C4C", out textColor);
        }
        else
        {
            textColor = new Color(0.9f, 0.3f, 0.3f);
        }

        if (txtName != null)
        {
            txtName.color = textColor;
        }
        if (txtInfo != null)
        {
            txtInfo.text = "Lvl " + level + " | " + cost.ToString("F0") + " Scrap";
            txtInfo.color = textColor;
        }
        if (btnBuy != null)
        {
            btnBuy.interactable = canAfford;

            // Escurece/esmaece a imagem do botão inteiro por código se não puder comprar
            Image btnImage = btnBuy.GetComponent<Image>();
            if (btnImage != null)
            {
                btnImage.color = canAfford ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.6f);
            }
        }
    }

    // --- MÉTODOS DE COMPRA ---

    public void PurchaseCollectorUpgrade()
    {
        Debug.Log("COMPRA: Clique registrado no botão do Collector!");
        if (EconomyManager.Instance != null) EconomyManager.Instance.BuyCollectorUpgrade();
    }

    public void PurchaseScannerUpgrade()
    {
        if (EconomyManager.Instance != null) EconomyManager.Instance.BuyScannerUpgrade();
    }

    public void PurchaseThrusterUpgrade()
    {
        if (EconomyManager.Instance != null) EconomyManager.Instance.BuyThrusterUpgrade();
    }

    public void PurchaseCargoUpgrade()
    {
        if (EconomyManager.Instance != null) EconomyManager.Instance.BuyCargoUpgrade();
    }

    public void PurchaseMagnetUpgrade()
    {
        if (EconomyManager.Instance != null) EconomyManager.Instance.BuyMagnetUpgrade();
    }

    public void PurchaseAIUpgrade()
    {
        if (EconomyManager.Instance != null) EconomyManager.Instance.BuyAIUpgrade();
    }
}
