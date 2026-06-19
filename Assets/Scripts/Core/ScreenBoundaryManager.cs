using UnityEngine;

public class ScreenBoundaryManager : MonoBehaviour
{
    [Header("Referências")]
    public Transform shipTransform;
    public RectTransform uiParent;

    [Header("Posição da UI")]
    [Tooltip("Distância da UI abaixo da nave (valor positivo). Quando inverter, ela vai para cima com a mesma distância.")]
    public float uiOffsetY = 80f;

    [Header("Limite para Inverter")]
    [Tooltip("Quando a nave passar dessa porcentagem da tela (0 = fundo, 1 = topo), inverte.")]
    [Range(0.1f, 0.5f)]
    public float flipThreshold = 0.35f;

    private bool isFlipped = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        // Começa com a UI abaixo da nave
        UpdateUIPosition();
    }

    void Update()
    {
        if (shipTransform == null || mainCamera == null) return;

        // Posição da nave na tela (0 = fundo, 1 = topo)
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(shipTransform.position);

        bool shouldFlip = viewportPos.y < flipThreshold;

        if (shouldFlip && !isFlipped)
        {
            isFlipped = true;
            ApplyFlip();
        }
        else if (!shouldFlip && isFlipped)
        {
            isFlipped = false;
            ApplyFlip();
        }
    }

    private void ApplyFlip()
    {
        // Inverte a nave de cabeça para baixo
        if (shipTransform != null)
        {
            Vector3 scale = shipTransform.localScale;
            scale.y = isFlipped ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            shipTransform.localScale = scale;
        }

        UpdateUIPosition();
    }

    private void UpdateUIPosition()
    {
        if (uiParent == null) return;

        // Normal: UI fica ABAIXO da nave (Y negativo)
        // Invertido: UI fica ACIMA da nave (Y positivo)
        float posY = isFlipped ? uiOffsetY : -uiOffsetY;

        uiParent.anchoredPosition = new Vector2(uiParent.anchoredPosition.x, posY);
    }
}
