using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Configurações do Parallax")]
    [Tooltip("Velocidade de movimento do fundo")]
    public float speed = 1f;
    
    [Tooltip("Largura exata da sua imagem (para saber quando repetir)")]
    public float spriteWidth = 10f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Movimenta para a esquerda e repete quando atinge 'spriteWidth'
        float newPos = Mathf.Repeat(Time.time * speed, spriteWidth);
        transform.position = startPosition + Vector3.left * newPos;
    }
}
