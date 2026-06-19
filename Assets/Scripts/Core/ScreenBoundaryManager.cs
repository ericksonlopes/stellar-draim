using UnityEngine;

public class ScreenBoundaryManager : MonoBehaviour
{
    [Header("Referências")]
    public Transform shipTransform;
    public RectTransform uiParent;

    [Header("Posição da UI")]
    [Tooltip("Distância da UI abaixo da nave (valor positivo).")]
    public float uiOffsetY = 130f;

    private Camera mainCamera;
    private Canvas parentCanvas;
    private RectTransform canvasRect;

    void Start()
    {
        mainCamera = Camera.main;
        if (uiParent != null)
        {
            parentCanvas = uiParent.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                canvasRect = parentCanvas.transform as RectTransform;
            }
        }
        // Começa com a UI abaixo da nave
        UpdateUIPosition();
    }

    void LateUpdate()
    {
        UpdateUIPosition();
    }

    private void UpdateUIPosition()
    {
        if (uiParent == null || shipTransform == null || mainCamera == null || parentCanvas == null || canvasRect == null) return;

        // Posição da nave em coordenadas de tela
        Vector2 screenPos = mainCamera.WorldToScreenPoint(shipTransform.position);

        // Conversão para coordenadas locais do Canvas
        Vector2 localPoint;
        
        // O terceiro parâmetro deve ser a câmera se o Canvas não for Overlay
        Camera cam = (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : mainCamera;
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out localPoint))
        {
            // UI sempre fica ABAIXO da nave (Y negativo)
            float posY = -uiOffsetY;
            // Alinha o X com a nave e o Y com o offset correspondente
            uiParent.anchoredPosition = new Vector2(localPoint.x, localPoint.y + posY);
        }
    }
}
