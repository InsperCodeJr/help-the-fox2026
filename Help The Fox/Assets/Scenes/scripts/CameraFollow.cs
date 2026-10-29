using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Limites da sala")]
    [SerializeField] private Transform limiteInferiorEsquerdo;
    [SerializeField] private Transform limiteSuperiorDireito;

    private Camera cameraComponente;
    private float tamanhoDesejado;
    private float ultimoTamanhoAplicado;

    private void Awake()
    {
        InicializarCamera();
    }

    private void InicializarCamera()
    {
        cameraComponente = GetComponent<Camera>();
        tamanhoDesejado = cameraComponente.orthographicSize;
        ultimoTamanhoAplicado = tamanhoDesejado;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        if (cameraComponente == null)
        {
            InicializarCamera();
        }

        if (cameraComponente.orthographic && limiteInferiorEsquerdo != null
            && limiteSuperiorDireito != null)
        {
            Vector2 minimo = Vector2.Min(limiteInferiorEsquerdo.position, limiteSuperiorDireito.position);
            Vector2 maximo = Vector2.Max(limiteInferiorEsquerdo.position, limiteSuperiorDireito.position);
            Vector2 tamanhoSala = maximo - minimo;

            if (tamanhoSala.x > 0f && tamanhoSala.y > 0f)
            {
                // Permite ajustar o zoom pelo Inspector durante o Play.
                if (!Mathf.Approximately(cameraComponente.orthographicSize, ultimoTamanhoAplicado))
                {
                    tamanhoDesejado = cameraComponente.orthographicSize;
                }

                float aspecto = Mathf.Max(cameraComponente.aspect, 0.0001f);
                float tamanhoMaximo = Mathf.Min(tamanhoSala.y * 0.5f, tamanhoSala.x / (2f * aspecto));
                cameraComponente.orthographicSize = Mathf.Min(tamanhoDesejado, tamanhoMaximo);
                ultimoTamanhoAplicado = cameraComponente.orthographicSize;

                float meiaAltura = ultimoTamanhoAplicado;
                float meiaLargura = meiaAltura * aspecto;

                targetPosition.x = LimitarEixo(targetPosition.x,
                    minimo.x + meiaLargura, maximo.x - meiaLargura);
                targetPosition.y = LimitarEixo(targetPosition.y,
                    minimo.y + meiaAltura, maximo.y - meiaAltura);
            }
        }

        transform.position = targetPosition;
    }

    private static float LimitarEixo(float valor, float minimo, float maximo)
    {
        return minimo <= maximo ? Mathf.Clamp(valor, minimo, maximo) : (minimo + maximo) * 0.5f;
    }

    private void OnDrawGizmosSelected()
    {
        if (limiteInferiorEsquerdo == null || limiteSuperiorDireito == null) return;

        Vector3 minimo = Vector3.Min(limiteInferiorEsquerdo.position, limiteSuperiorDireito.position);
        Vector3 maximo = Vector3.Max(limiteInferiorEsquerdo.position, limiteSuperiorDireito.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((minimo + maximo) * 0.5f, maximo - minimo);
    }
}
