using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Recursos")]
    public double currentScrap = 0;
    
    [Header("Upgrades")]
    public double basePPS = 1.0;          // PPS Base da Nave-Mãe
    public int baseUpgradeLevel = 0;      // Nível do upgrade base
    public int fleetLevel = 0;            // Nível da Frota (número de naves/sondas)
    
    [Header("Configurações de Multiplicador")]
    public double percentBonusPerFleet = 0.10; // +10% por nave da Frota

    // PPS Total calculado dinamicamente
    public double scrapPerSecond => basePPS * (1.0 + (fleetLevel * percentBonusPerFleet));

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
        // Adiciona sucata passivamente de acordo com o tempo
        currentScrap += scrapPerSecond * Time.deltaTime;
    }

    public double GetBaseUpgradeCost()
    {
        return 10 * Mathf.Pow(1.5f, baseUpgradeLevel);
    }

    public double GetFleetUpgradeCost()
    {
        return 50 * Mathf.Pow(1.8f, fleetLevel);
    }

    public bool BuyBaseUpgrade()
    {
        double cost = GetBaseUpgradeCost();
        if (SpendScrap(cost))
        {
            baseUpgradeLevel++;
            basePPS += 0.5; // Aumenta +0.5 PPS de base
            SaveData();
            return true;
        }
        return false;
    }

    public bool BuyFleetUpgrade()
    {
        double cost = GetFleetUpgradeCost();
        if (SpendScrap(cost))
        {
            fleetLevel++;
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
        PlayerPrefs.SetInt("FleetLevel", fleetLevel);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("CurrentScrap"))
            double.TryParse(PlayerPrefs.GetString("CurrentScrap"), out currentScrap);

        if (PlayerPrefs.HasKey("BasePPS"))
            double.TryParse(PlayerPrefs.GetString("BasePPS"), out basePPS);

        if (PlayerPrefs.HasKey("BaseUpgradeLevel"))
            baseUpgradeLevel = PlayerPrefs.GetInt("BaseUpgradeLevel", 0);

        if (PlayerPrefs.HasKey("FleetLevel"))
            fleetLevel = PlayerPrefs.GetInt("FleetLevel", 0);
    }

    void OnApplicationQuit()
    {
        SaveData();
    }
}
