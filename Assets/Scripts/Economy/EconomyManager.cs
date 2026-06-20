using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Recursos")]
    public double currentScrap = 0;
    
    [Header("Upgrades da Nave Mãe")]
    public double basePPS = 1.0;          // PPS Base da Nave-Mãe
    public int baseUpgradeLevel = 0;      // Nível do upgrade base (Warp Drive / Engines)
    
    [Header("Upgrades do Collector (Frota)")]
    public int collectorCount = 0;        // Lvl 1: Quantidade de sondas (+10% por sonda, começa com 1 sonda base no Lvl 0)
    public int scannerLevel = 0;          // Lvl 2: Scanner Precision (+2% eficiência por sonda)
    public int thrusterLevel = 0;         // Lvl 3: Thruster Overdrive (+15% velocidade, +5% PPS)
    public int cargoLevel = 0;            // Lvl 4: Cargo Expansion (+5% eficiência por sonda)
    public int magnetLevel = 0;           // Lvl 5: Scrap Magnet (-10% tempo coleta, +8% PPS)
    public int aiLevel = 0;               // Lvl 6: Auto-Pilot AI (+12% PPS)

    // PPS Total calculado dinamicamente com base nos 6 upgrades
    public double scrapPerSecond
    {
        get
        {
            double baseMultiplier = 1.0;
            // O número de coletores ativos é collectorCount + 1 (pois já começa com 1)
            double collectorMultiplier = (collectorCount) * (0.10 + (scannerLevel * 0.02) + (cargoLevel * 0.05));
            double thrusterMultiplier = thrusterLevel * 0.05;
            double magnetMultiplier = magnetLevel * 0.08;
            double aiMultiplier = aiLevel * 0.12;

            return basePPS * (baseMultiplier + collectorMultiplier + thrusterMultiplier + magnetMultiplier + aiMultiplier);
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        currentScrap += scrapPerSecond * Time.deltaTime;
    }

    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        
        currentScrap = 0;
        basePPS = 1.0;
        baseUpgradeLevel = 0;
        collectorCount = 0;
        scannerLevel = 0;
        thrusterLevel = 0;
        cargoLevel = 0;
        magnetLevel = 0;
        aiLevel = 0;

        SaveData();
        Debug.Log("Progresso zerado com sucesso!");
    }

    [ContextMenu("Add 1000 Scrap")]
    public void AddTestScrap()
    {
        currentScrap += 1000;
        SaveData();
        Debug.Log("Adicionado 1000 Scrap de teste!");
    }

    // --- CUSTOS DOS UPGRADES ---
    public double GetBaseUpgradeCost() => 15 * Mathf.Pow(1.5f, baseUpgradeLevel);
    public double GetCollectorCost() => 50 * Mathf.Pow(1.8f, collectorCount);
    public double GetScannerCost() => 40 * Mathf.Pow(1.5f, scannerLevel);
    public double GetThrusterCost() => 60 * Mathf.Pow(1.6f, thrusterLevel);
    public double GetCargoCost() => 80 * Mathf.Pow(1.7f, cargoLevel);
    public double GetMagnetCost() => 100 * Mathf.Pow(1.75f, magnetLevel);
    public double GetAICost() => 150 * Mathf.Pow(1.9f, aiLevel);

    // --- AÇÕES DE COMPRA ---
    public bool BuyBaseUpgrade()
    {
        double cost = GetBaseUpgradeCost();
        if (SpendScrap(cost))
        {
            baseUpgradeLevel++;
            basePPS += 0.5;
            SaveData();
            return true;
        }
        return false;
    }

    public bool BuyCollectorUpgrade()
    {
        double cost = GetCollectorCost();
        if (SpendScrap(cost))
        {
            collectorCount++;
            SaveData();
            return true;
        }
        return false;
    }

    public bool BuyScannerUpgrade()
    {
        double cost = GetScannerCost();
        if (SpendScrap(cost))
        {
            scannerLevel++;
            SaveData();
            return true;
        }
        return false;
    }

    public bool BuyThrusterUpgrade()
    {
        double cost = GetThrusterCost();
        if (SpendScrap(cost))
        {
            thrusterLevel++;
            SaveData();
            return true;
        }
        return false;
    }

    public bool BuyCargoUpgrade()
    {
        double cost = GetCargoCost();
        if (SpendScrap(cost))
        {
            cargoLevel++;
            SaveData();
            return true;
        }
        return false;
    }

    public bool BuyMagnetUpgrade()
    {
        double cost = GetMagnetCost();
        if (SpendScrap(cost))
        {
            magnetLevel++;
            SaveData();
            return true;
        }
        return false;
    }

    public bool BuyAIUpgrade()
    {
        double cost = GetAICost();
        if (SpendScrap(cost))
        {
            aiLevel++;
            SaveData();
            return true;
        }
        return false;
    }

    public bool SpendScrap(double amount)
    {
        if (currentScrap >= amount)
        {
            currentScrap -= amount;
            SaveData();
            return true;
        }
        return false;
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("CurrentScrap", currentScrap.ToString("F2"));
        PlayerPrefs.SetString("BasePPS", basePPS.ToString("F2"));
        PlayerPrefs.SetInt("BaseUpgradeLevel", baseUpgradeLevel);
        
        PlayerPrefs.SetInt("CollectorCount", collectorCount);
        PlayerPrefs.SetInt("ScannerLevel", scannerLevel);
        PlayerPrefs.SetInt("ThrusterLevel", thrusterLevel);
        PlayerPrefs.SetInt("CargoLevel", cargoLevel);
        PlayerPrefs.SetInt("MagnetLevel", magnetLevel);
        PlayerPrefs.SetInt("AILevel", aiLevel);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("CurrentScrap"))
            double.TryParse(PlayerPrefs.GetString("CurrentScrap"), out currentScrap);

        if (PlayerPrefs.HasKey("BasePPS"))
            double.TryParse(PlayerPrefs.GetString("BasePPS"), out basePPS);

        baseUpgradeLevel = PlayerPrefs.GetInt("BaseUpgradeLevel", 0);
        collectorCount = PlayerPrefs.GetInt("CollectorCount", 0);
        scannerLevel = PlayerPrefs.GetInt("ScannerLevel", 0);
        thrusterLevel = PlayerPrefs.GetInt("ThrusterLevel", 0);
        cargoLevel = PlayerPrefs.GetInt("CargoLevel", 0);
        magnetLevel = PlayerPrefs.GetInt("MagnetLevel", 0);
        aiLevel = PlayerPrefs.GetInt("AILevel", 0);
    }

    void OnApplicationQuit()
    {
        SaveData();
    }
}
