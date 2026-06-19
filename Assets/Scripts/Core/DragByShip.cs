using UnityEngine;
using UnityEngine.EventSystems;

public class DragByShip : MonoBehaviour
{
    private Camera mainCamera;
#if UNITY_EDITOR
    private Vector3 offset;
    private Vector2 lastMousePos;
#endif

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        TransparentWindow.DragWindow();

#if UNITY_EDITOR
        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            Vector3 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            mousePos.z = 10f; // Distância fixa para a câmera ortográfica
            offset = transform.position - mainCamera.ScreenToWorldPoint(mousePos);
            lastMousePos = mousePos;
        }
#endif
    }

#if UNITY_EDITOR
    private void OnMouseDrag()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            Vector3 currentMousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            currentMousePos.z = 10f;

            // 1. A Nave acompanha o mouse exatamente onde ele está colado
            Vector3 targetWorldPos = mainCamera.ScreenToWorldPoint(currentMousePos) + offset;
            transform.position = targetWorldPos;

            // 2. A UI (UIParent) acompanha na mesma proporção de pixels
            Vector2 deltaPixels = new Vector2(currentMousePos.x, currentMousePos.y) - lastMousePos;
            lastMousePos = new Vector2(currentMousePos.x, currentMousePos.y);

            ScreenBoundaryManager boundaryManager = FindObjectOfType<ScreenBoundaryManager>();
            if (boundaryManager != null && boundaryManager.uiParent != null)
            {
                // Multiplicador menor na UI apenas para não escapar da tela se a resolução for diferente
                boundaryManager.uiParent.anchoredPosition += deltaPixels * 0.5f; 
            }
        }
    }
#endif
}
