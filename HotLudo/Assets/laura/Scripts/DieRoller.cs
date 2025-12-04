using UnityEngine;

public class DieRoller : MonoBehaviour
{
    [SerializeField] private Sprite[] dieSprites; // Asigna Die_0 a Die_5 en el inspector
    private SpriteRenderer sr;
    private Animator anim;

    private int pendingRoll; // resultado que se aplicará al terminar la animación

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (sr == null)
            Debug.LogError("No se encontró SpriteRenderer en el objeto");

        if (anim == null)
            Debug.LogError("No se encontró Animator en el objeto");

        // Al inicio, mostrar Die_0
        if (dieSprites.Length > 0)
            sr.sprite = dieSprites[0];
    }

    void OnMouseDown()
    {
        RollDie();
    }

    void RollDie()
    {
        // Generar resultado aleatorio y guardarlo
        pendingRoll = Random.Range(0, dieSprites.Length);

        // Reactivar Animator para reproducir la animación
        anim.enabled = true;
        anim.SetTrigger("RollTrigger");
    }

    // Este método lo llamará un Animation Event al final del clip "Roll"
    public void ApplyResult()
    {
        sr.sprite = dieSprites[pendingRoll];

        // Desactivar Animator para que no sobrescriba el sprite final
        anim.enabled = false;

        Debug.Log("Dado: " + (pendingRoll + 1));
    }
}
