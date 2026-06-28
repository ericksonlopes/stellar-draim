using UnityEngine;
using UnityEngine.EventSystems;

public class DragByShip : MonoBehaviour
{
    [SerializeField] private float sizeMultiplier = 1.3f;

    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging = false;

    private void Start()
    {
        mainCamera = Camera.main;
        transform.localScale *= sizeMultiplier;
        LoadPosition();
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
                    SavePosition();
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

    private void SavePosition()
    {
        PlayerPrefs.SetFloat("ShipPosX", transform.position.x);
        PlayerPrefs.SetFloat("ShipPosY", transform.position.y);
        PlayerPrefs.Save();
    }

    private void LoadPosition()
    {
        if (PlayerPrefs.HasKey("ShipPosX") && PlayerPrefs.HasKey("ShipPosY"))
        {
            float x = PlayerPrefs.GetFloat("ShipPosX");
            float y = PlayerPrefs.GetFloat("ShipPosY");
            Vector3 targetPos = new Vector3(x, y, transform.position.z);

            // Validação de segurança: se a posição estiver muito fora da tela (ex: monitor desconectado)
            // reseta para a posição padrão do jogo para não perder a nave.
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(targetPos);
            if (viewportPos.x >= -0.1f && viewportPos.x <= 1.1f && viewportPos.y >= -0.1f && viewportPos.y <= 1.1f)
            {
                transform.position = targetPos;
            }
            else
            {
                transform.position = new Vector3(0f, 0.5f, transform.position.z);
            }
        }
    }
}
