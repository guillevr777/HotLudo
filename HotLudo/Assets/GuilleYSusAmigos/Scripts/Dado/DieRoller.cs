using UnityEngine;

/// <summary>
/// Lanzamiento, animación, sonido y resultado
/// </summary>
public class DieRoller : MonoBehaviour
{
    [Header("Sprites del dado")]
    [SerializeField] private Sprite[] dieSprites;

    [Header("Audio")]
    [SerializeField] private AudioClip diceRollingClip;

    private SpriteRenderer sr;
    private Animator anim;
    private AudioSource audioSource;

    [Header("Referencias")]
    public BoardManager boardManager;

    // Valor aleatorio 
    private int pendingRoll;
    // Estado actual del dado
    private bool isRolling = false;

    /// <summary>
    /// Inicialización de componentes
    /// </summary>
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

        // Mostrar el primer sprite del dado al inicio
        if (dieSprites.Length > 0)sr.sprite = dieSprites[0];
    }

    /// <summary>
    /// Detecta el clic del ratón sobre el dado y lanza el dado
    /// </summary>
    void OnMouseDown()
    {
        // Evitar múltiples lanzamientos simultáneos
        if (isRolling) return;  
        RollDie();
    }

    /// <summary>
    /// Inicia la animación y el sonido del dado, eligiendo un valor aleatorio
    /// </summary>
    void RollDie()
    {
        isRolling = true;

        // Seleccionar número aleatorio entre 0 y cantidad de sprites
        pendingRoll = Random.Range(0, dieSprites.Length);

        // Activar animación
        anim.enabled = true;
        anim.SetTrigger("RollTrigger");

        // Reproducir sonido del dado mientras gira
        if (diceRollingClip != null && audioSource != null)
        {
            audioSource.clip = diceRollingClip;
            audioSource.loop = true; 
            audioSource.Play();
        }
    }

    /// <summary>
    /// Aplica el resultado del dado y llama al BoardManager
    /// </summary>
    public void ApplyResult()
    {
        // Detener sonido de dado girando
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }

        // Mostrar la cara final del dado
        sr.sprite = dieSprites[pendingRoll];

        // Apagar animador para mantener la cara fija
        anim.enabled = false;

        int rollNumber = pendingRoll + 1;
        Debug.Log("DieRoller: Valor del dado = " + rollNumber);

        // Mandar al BoardManager el resultado
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