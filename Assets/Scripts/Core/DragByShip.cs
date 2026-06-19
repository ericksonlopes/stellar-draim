using UnityEngine;
using UnityEngine.EventSystems;

public class DragByShip : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            isDragging = true;
            Vector3 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            mousePos.z = 10f; // Distância fixa para a câmera ortográfica
            offset = transform.position - mainCamera.ScreenToWorldPoint(mousePos);
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse != null)
            {
                // Se soltou o botão ou não está mais pressionado, para de arrastar
                if (mouse.leftButton.wasReleasedThisFrame || !mouse.leftButton.isPressed)
                {
                    isDragging = false;
                    return;
                }

                Vector3 currentMousePos = mouse.position.ReadValue();
                currentMousePos.z = 10f;

                // A Nave acompanha o mouse exatamente onde ele está colado
                Vector3 targetWorldPos = mainCamera.ScreenToWorldPoint(currentMousePos) + offset;
                transform.position = targetWorldPos;
            }
        }
    }
}
