using UnityEngine;

public class DieRoller : MonoBehaviour
{
    [SerializeField] private Sprite[] dieSprites;
    [SerializeField] private AudioClip diceRollingClip;

    private SpriteRenderer sr;
    private Animator anim;
    private AudioSource audioSource;


    // Referencia a otros scripts
    public BoardManager boardManager;

    // Guardará el número aleatorio (0–5)
    private int pendingRoll;
    // Estado del dado
    private bool isRolling = false;


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (sr == null) Debug.LogError("DieRoller: No se encontró SpriteRenderer");
        if (anim == null) Debug.LogError("DieRoller: No se encontró Animator");
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("DieRoller: Se añadió AudioSource automáticamente");
        }

        // Mostrar sprite inicial
        if (dieSprites.Length > 0)sr.sprite = dieSprites[0];
    }

    /// <summary>
    /// // Detecta el clic del ratón sobre el dado
    /// </summary>
    void OnMouseDown()
    {
        if (isRolling) return;  // Evitar múltiples lanzamientos simultáneos

        RollDie();
    }

    /// <summary>
    /// // Acciona la animación del dado
    /// </summary>
    void RollDie()
    {
        isRolling = true;

        // Selecciona un número aleatorio entre 0 y cantidad de sprites
        pendingRoll = Random.Range(0, dieSprites.Length);

        // Activo animación
        anim.enabled = true;
        anim.SetTrigger("RollTrigger");

        // Reproducir sonido del dado
        if (diceRollingClip != null && audioSource != null)
        {
            audioSource.clip = diceRollingClip;
            audioSource.loop = true; // Loop mientras gira
            audioSource.Play();
        }
    }

    /// <summary>
    /// Llamamos Animation Event al terminar la animación
    /// </summary>
    public void ApplyResult()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }

        // Mostrar la cara final del dado
        sr.sprite = dieSprites[pendingRoll];

        // Apagar animador para que se quede la cara fija
        anim.enabled = false;

        int rollNumber = pendingRoll + 1;
        Debug.Log("DieRoller: Valor final del dado = " + rollNumber);

        // Mandar el número al BoardManager
        if (boardManager != null)
        {
            boardManager.OnDieRolled(rollNumber);
        }
        else
        {
            Debug.LogError("DieRoller: No hay BoardManager asignado");
        }

        isRolling = false;
    }
}
