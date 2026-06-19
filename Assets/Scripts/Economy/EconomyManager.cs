using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Recursos")]
    public double currentScrap = 0;
    public double scrapPerSecond = 1; // PPS inicial

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

    public void IncreasePPS(double amount)
    {
        scrapPerSecond += amount;
        SaveData();
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("CurrentScrap", currentScrap.ToString("F2"));
        PlayerPrefs.SetString("ScrapPerSecond", scrapPerSecond.ToString("F2"));
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("CurrentScrap"))
            double.TryParse(PlayerPrefs.GetString("CurrentScrap"), out currentScrap);

        if (PlayerPrefs.HasKey("ScrapPerSecond"))
            double.TryParse(PlayerPrefs.GetString("ScrapPerSecond"), out scrapPerSecond);
    }

    void OnApplicationQuit()
    {
        SaveData();
    }
}
