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

    void Update()
    {
        if (EconomyManager.Instance != null)
        {
            txtScrap.text = EconomyManager.Instance.currentScrap.ToString("F0");
            txtPPS.text = "+" + EconomyManager.Instance.scrapPerSecond.ToString("F1") + "/s";
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
}
