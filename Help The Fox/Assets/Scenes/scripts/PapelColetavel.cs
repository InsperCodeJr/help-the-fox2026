using TMPro;
using UnityEngine;

public class PapelColetavel : MonoBehaviour
{
    [SerializeField] private string idPapel = "papel_labirinto_escuro";
    [SerializeField] private string nomePapel = "Papel";
    [SerializeField] private Sprite spriteMochila;
    [SerializeField] private Sprite spriteLeitura;
    [SerializeField, TextArea(3, 12)] private string texto = "texto aqui";

    [Header("Interacao")]
    [SerializeField] private Transform jogador;
    [SerializeField, Min(0f)] private float distanciaInteracao = 2f;
    [SerializeField] private TMP_Text textoInteracao;

    private Collider2D colisorPapel;
    private Renderer imagemPapel;
    private InterfaceMochila interfaceMochila;
    private bool avisoTextoEmitido;

    public PapelInventario DadosPapel => new PapelInventario
    {
        Id = idPapel,
        Nome = nomePapel,
        SpriteMochila = spriteMochila,
        SpriteLeitura = spriteLeitura,
        Texto = texto
    };

    public bool JogadorPerto
    {
        get
        {
            PrepararReferencias();
            if (jogador == null) return false;

            Vector2 pontoPapel = PontoMaisProximoDoPapel(jogador.position);
            Collider2D colisorJogador = jogador.GetComponent<Collider2D>();
            Vector2 pontoJogador = colisorJogador != null && colisorJogador.enabled
                ? colisorJogador.ClosestPoint(pontoPapel)
                : (Vector2)jogador.position;
            pontoPapel = PontoMaisProximoDoPapel(pontoJogador);

            float alcance = Mathf.Max(0f, distanciaInteracao);
            return (pontoJogador - pontoPapel).sqrMagnitude <= alcance * alcance;
        }
    }

    void Awake()
    {
        PrepararReferencias();
        if (textoInteracao == null && !avisoTextoEmitido)
        {
            avisoTextoEmitido = true;
            Debug.LogWarning("PapelColetavel: atribua o texto de interacao no Inspector.", this);
        }
    }

    void OnEnable()
    {
        if (EstadoPapeis.Possui(idPapel))
        {
            OcultarPapel();
            return;
        }

        DefinirPrompt(false);
    }

    void Update()
    {
        if (EstadoPapeis.Possui(idPapel))
        {
            OcultarPapel();
            return;
        }

        bool podeColetar = !InterfaceBloqueada() && JogadorPerto;
        DefinirPrompt(podeColetar);
        if (podeColetar && Input.GetKeyDown(KeyCode.P))
        {
            TentarColetar();
        }
    }

    void OnDisable()
    {
        DefinirPrompt(false);
    }

    public bool TentarColetar()
    {
        if (EstadoPapeis.Possui(idPapel))
        {
            OcultarPapel();
            return false;
        }

        if (!isActiveAndEnabled || InterfaceBloqueada() || !JogadorPerto)
        {
            return false;
        }

        if (!EstadoPapeis.Guardar(DadosPapel))
        {
            return false;
        }

        OcultarPapel();
        return true;
    }

    private void PrepararReferencias()
    {
        if (jogador == null)
        {
            GameObject objetoJogador = GameObject.FindGameObjectWithTag("Player");
            if (objetoJogador != null) jogador = objetoJogador.transform;
        }

        if (colisorPapel == null) colisorPapel = GetComponent<Collider2D>();
        if (imagemPapel == null) imagemPapel = GetComponent<Renderer>();
        if (interfaceMochila == null) interfaceMochila = FindAnyObjectByType<InterfaceMochila>();
    }

    private bool InterfaceBloqueada()
    {
        PrepararReferencias();
        return interfaceMochila != null && (interfaceMochila.EstaAberta || interfaceMochila.EstaLendo);
    }

    private Vector2 PontoMaisProximoDoPapel(Vector2 ponto)
    {
        if (colisorPapel != null && colisorPapel.enabled)
        {
            return colisorPapel.ClosestPoint(ponto);
        }

        if (imagemPapel != null)
        {
            return imagemPapel.bounds.ClosestPoint(new Vector3(ponto.x, ponto.y, transform.position.z));
        }

        return transform.position;
    }

    private void DefinirPrompt(bool visivel)
    {
        if (textoInteracao == null) return;

        textoInteracao.text = "aperte P para pegar o papel";
        if (textoInteracao.gameObject.activeSelf != visivel)
        {
            textoInteracao.gameObject.SetActive(visivel);
        }
    }

    private void OcultarPapel()
    {
        DefinirPrompt(false);
        gameObject.SetActive(false);
    }
}
