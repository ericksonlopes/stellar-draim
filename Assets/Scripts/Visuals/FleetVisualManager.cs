using System.Collections.Generic;
using UnityEngine;

public class FleetVisualManager : MonoBehaviour
{
    [Header("Configurações da Frota")]
    public GameObject collectorShipPrefab;
    public Transform motherShip;
    public float shipSpeed = 2.5f;

    private List<CollectorShip> activeShips = new List<CollectorShip>();

    void Update()
    {
        if (EconomyManager.Instance == null || collectorShipPrefab == null || motherShip == null) return;

        // O número de naves na tela é sempre collectorCount + 1 (pois já começa com 1)
        int targetShipCount = EconomyManager.Instance.collectorCount + 1;

        // Instancia naves adicionais se necessário
        while (activeShips.Count < targetShipCount)
        {
            SpawnShip();
        }

        // Remove naves se o número alvo diminuir (por exemplo, após um reset de progresso)
        while (activeShips.Count > targetShipCount)
        {
            int lastIndex = activeShips.Count - 1;
            if (activeShips[lastIndex] != null)
            {
                Destroy(activeShips[lastIndex].gameObject);
            }
            activeShips.RemoveAt(lastIndex);
        }
    }

    void SpawnShip()
    {
        GameObject newShipObj = Instantiate(collectorShipPrefab, motherShip.position, Quaternion.identity);
        CollectorShip shipComponent = newShipObj.GetComponent<CollectorShip>();
        
        if (shipComponent == null)
        {
            shipComponent = newShipObj.AddComponent<CollectorShip>();
        }

        shipComponent.Initialize(motherShip, shipSpeed);
        activeShips.Add(shipComponent);
    }
}
