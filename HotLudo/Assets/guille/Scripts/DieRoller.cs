using UnityEngine;

public class DieRoller : MonoBehaviour
{
    [SerializeField] private Sprite[] dieSprites;
    private SpriteRenderer sr;
    private Animator anim;

    private int pendingRoll;

    public BoardManager boardManager;

    private bool isRolling = false; // <-- NEW: evita clicks durante animación

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (sr == null) Debug.LogError("No se encontró SpriteRenderer");
        if (anim == null) Debug.LogError("No se encontró Animator");

        if (dieSprites.Length > 0)
            sr.sprite = dieSprites[0];
    }

    void OnMouseDown()
    {
        Debug.Log(dieSprites.Length);
        if (isRolling) return; // <-- NEW: si está rodando, ignorar clic
        RollDie();
    }

    void RollDie()
    {
        isRolling = true; // <-- NEW: bloquear clics

        pendingRoll = Random.Range(0, dieSprites.Length);
        anim.enabled = true;
        anim.SetTrigger("RollTrigger");

        Debug.Log("Dado lanzado, resultado será: " + (pendingRoll + 1));
    }

    // Animation Event debe llamarlo al final de la animación Roll
    public void ApplyResult()
    {
        sr.sprite = dieSprites[pendingRoll];
        anim.enabled = false;

        int rollNumber = pendingRoll + 1;
        Debug.Log("Dado finalizó animación mostrando: " + rollNumber);

        // Notificar a BoardManager
        if (boardManager != null)
            boardManager.OnDieRolled(rollNumber);

        isRolling = false;
    }
}