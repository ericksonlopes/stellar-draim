using UnityEngine;

public class CollectorShip : MonoBehaviour
{
    private Transform motherShip;
    private float speed = 0.3f; // Reduzida a velocidade padrão de 1f para 0.3f
    private Vector2 targetPosition;
    private bool returning = false;
    private float stateTimer = 0f;

    // Variáveis para órbita Idle
    private float orbitAngle = 0f;
    private float orbitRadius = 0.6f;
    private float orbitSpeed = 2f;

    public enum State { Idle, Outward, Collecting, Returning }
    private State currentState = State.Idle;

    public void Initialize(Transform motherShipTransform, float shipSpeed)
    {
        motherShip = motherShipTransform;
        speed = shipSpeed;
        transform.position = motherShip.position;
        orbitAngle = Random.Range(0f, Mathf.PI * 2f); // Inicializa com ângulo aleatório
        SetRandomTarget();
    }

    void Update()
    {
        if (motherShip == null) return;

        // Calcula a distância atual da sonda até a Nave-Mãe no mundo
        float distanceToMother = Vector2.Distance(transform.position, motherShip.position);
        float distanceMultiplier = 1.0f;

        // Se estiver fora da órbita próxima (distância > 1.5 unidades), acelera progressivamente baseado na distância
        if (distanceToMother > 1.5f)
        {
            distanceMultiplier = distanceToMother * 2.5f; // Quanto mais distante, mais rápido viaja de volta
        }

        // Calcula velocidade final com upgrades de Thruster e o multiplicador de distância
        float currentSpeed = speed;
        if (EconomyManager.Instance != null)
        {
            currentSpeed = speed * (1.0f + (EconomyManager.Instance.thrusterLevel * 0.15f));
        }
        currentSpeed *= distanceMultiplier;

        // --- DESACELERAÇÃO INTELIGENTE (Ease Out ao aproximar dos alvos) ---
        if (currentState == State.Outward)
        {
            float distToTarget = Vector2.Distance(transform.position, targetPosition);
            if (distToTarget < 0.6f)
            {
                currentSpeed *= Mathf.Lerp(0.25f, 1.0f, distToTarget / 0.6f);
            }
        }
        else if (currentState == State.Returning)
        {
            if (distanceToMother < 0.6f)
            {
                currentSpeed *= Mathf.Lerp(0.25f, 1.0f, distanceToMother / 0.6f);
            }
        }

        // --- SISTEMA DE SEPARAÇÃO (Evita que as sondas ocupem o mesmo espaço) ---
        Vector2 separationForce = Vector2.zero;
        CollectorShip[] allShips = FindObjectsOfType<CollectorShip>();
        int neighborCount = 0;
        float separationRadius = 0.4f; // Raio de segurança para evitar colisão

        foreach (CollectorShip other in allShips)
        {
            if (other != null && other != this)
            {
                float dist = Vector2.Distance(transform.position, other.transform.position);
                if (dist < separationRadius)
                {
                    Vector2 away;
                    if (dist < 0.01f)
                    {
                        float angle = Random.Range(0f, Mathf.PI * 2f);
                        away = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                    }
                    else
                    {
                        away = ((Vector2)transform.position - (Vector2)other.transform.position).normalized;
                    }
                    // Força mais intensa quanto mais perto
                    float strength = 1.0f - (dist / separationRadius);
                    separationForce += away * strength;
                    neighborCount++;
                }
            }
        }

        switch (currentState)
        {
            case State.Idle:
                // --- MOVIMENTO DE ÓRBITA IDLE ---
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    SetRandomTarget();
                }
                else
                {
                    // Incrementa o ângulo da órbita
                    orbitAngle += orbitSpeed * Time.deltaTime;
                    // Calcula posição orbital ao redor da nave-mãe
                    Vector2 orbitTarget = (Vector2)motherShip.position + new Vector2(Mathf.Cos(orbitAngle), Mathf.Sin(orbitAngle)) * orbitRadius;
                    
                    // Alvo de mira combinado com força de separação
                    Vector2 finalTarget = orbitTarget;
                    if (neighborCount > 0)
                    {
                        finalTarget += separationForce * 0.3f;
                    }
                    
                    LookAt(finalTarget);
                    // Move sempre para a FRENTE (sem dar ré)
                    transform.position += transform.up * currentSpeed * Time.deltaTime;
                }
                break;

            case State.Outward:
                // Calcula direção e vetor perpendicular para a ondulação
                Vector2 dirOut = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
                Vector2 perpOut = new Vector2(-dirOut.y, dirOut.x);
                // Alvo temporário ondulado (onda senoidal)
                Vector2 tempTargetOut = (Vector2)targetPosition + perpOut * Mathf.Sin(Time.time * 4f) * 0.5f;

                // Alvo de mira combinado com força de separação
                Vector2 finalTargetOut = tempTargetOut;
                if (neighborCount > 0)
                {
                    finalTargetOut += separationForce * 0.3f;
                }

                LookAt(finalTargetOut);
                // Move sempre para a FRENTE (sem dar ré)
                transform.position += transform.up * currentSpeed * Time.deltaTime;

                if (Vector2.Distance(transform.position, targetPosition) < 0.25f)
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
                // Calcula direção e vetor perpendicular para a ondulação no retorno
                Vector2 dirRet = ((Vector2)motherShip.position - (Vector2)transform.position).normalized;
                Vector2 perpRet = new Vector2(-dirRet.y, dirRet.x);
                Vector2 tempTargetRet = (Vector2)motherShip.position + perpRet * Mathf.Sin(Time.time * 4f) * 0.5f;

                // Alvo de mira combinado com força de separação
                Vector2 finalTargetRet = tempTargetRet;
                if (neighborCount > 0)
                {
                    finalTargetRet += separationForce * 0.3f;
                }

                LookAt(finalTargetRet);
                // Move sempre para a FRENTE (sem dar ré)
                transform.position += transform.up * currentSpeed * Time.deltaTime;

                if (Vector2.Distance(transform.position, motherShip.position) < 0.25f)
                {
                    currentState = State.Idle;
                    stateTimer = Random.Range(2f, 5f); // Orbita por 2 a 5 segundos
                    orbitRadius = Random.Range(0.5f, 0.8f); // Raio orbital
                    orbitSpeed = Random.Range(1.5f, 3.0f) * (Random.value > 0.5f ? 1f : -1f); // Velocidade de órbita
                }
                break;
        }
    }

    void SetRandomTarget()
    {
        if (Camera.main == null || motherShip == null) return;

        // Converte a posição da Nave-Mãe para coordenadas de Viewport (0 a 1)
        Vector3 motherViewport = Camera.main.WorldToViewportPoint(motherShip.position);

        // Limita o X a um raio bem curto em relação à Nave-Mãe (-12% a +12% da largura da tela)
        float randomX = motherViewport.x + Random.Range(-0.12f, 0.12f);
        randomX = Mathf.Clamp(randomX, 0.15f, 0.85f);

        // Limita o Y a um raio muito curto acima da Nave-Mãe (+3% a +9% da altura da tela)
        float randomY = motherViewport.y + Random.Range(0.03f, 0.09f);
        randomY = Mathf.Clamp(randomY, 0.15f, 0.85f);

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
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 240f * Time.deltaTime);
        }
    }
}
