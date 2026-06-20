using UnityEngine;

public class CollectorShip : MonoBehaviour
{
    private Transform motherShip;
    private float speed = 2f;
    private Vector2 targetPosition;
    private bool returning = false;
    private float stateTimer = 0f;

    public enum State { Idle, Outward, Collecting, Returning }
    private State currentState = State.Idle;

    public void Initialize(Transform motherShipTransform, float shipSpeed)
    {
        motherShip = motherShipTransform;
        speed = shipSpeed;
        transform.position = motherShip.position;
        SetRandomTarget();
    }

    void Update()
    {
        if (motherShip == null) return;

        // Atualiza a velocidade atual baseada nos upgrades de Thruster (+15% por nível)
        float currentSpeed = speed;
        if (EconomyManager.Instance != null)
        {
            currentSpeed = speed * (1.0f + (EconomyManager.Instance.thrusterLevel * 0.15f));
        }

        switch (currentState)
        {
            case State.Idle:
                SetRandomTarget();
                break;

            case State.Outward:
                // Move em direção ao ponto de coleta
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
                
                // Rotaciona para olhar para o alvo
                LookAt(targetPosition);

                if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
                {
                    currentState = State.Collecting;
                    
                    // Calcula tempo de coleta base (entre 0.5s e 1.5s) reduzido em -10% por nível de Magnet
                    float collectDuration = Random.Range(0.5f, 1.5f);
                    if (EconomyManager.Instance != null)
                    {
                        collectDuration *= Mathf.Max(0.1f, 1.0f - (EconomyManager.Instance.magnetLevel * 0.10f));
                    }
                    stateTimer = collectDuration;
                }
                break;

            case State.Collecting:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    currentState = State.Returning;
                }
                break;

            case State.Returning:
                // Retorna para a Nave-Mãe
                transform.position = Vector2.MoveTowards(transform.position, motherShip.position, currentSpeed * Time.deltaTime);
                
                // Rotaciona para olhar para a Nave-Mãe
                LookAt(motherShip.position);

                if (Vector2.Distance(transform.position, motherShip.position) < 0.1f)
                {
                    currentState = State.Idle;
                }
                break;
        }
    }

    void SetRandomTarget()
    {
        if (Camera.main == null) return;

        // Escolhe um ponto aleatório nos cantos ou tela geral (viewport entre 0.15 e 0.85 para não sair dos limites visíveis)
        float randomX = Random.Range(0.15f, 0.85f);
        float randomY = Random.Range(0.15f, 0.85f);

        Vector3 targetWorld = Camera.main.ViewportToWorldPoint(new Vector3(randomX, randomY, 0));
        targetPosition = new Vector2(targetWorld.x, targetWorld.y);
        currentState = State.Outward;
    }

    void LookAt(Vector2 target)
    {
        Vector2 direction = target - (Vector2)transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // Ajusta o sprite dependendo da rotação original (geralmente assume-se apontando para a direita no sprite)
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
