
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    // Lista de sprites donde guardaremos las caras del dado
    [SerializeField]
    List<Sprite> die;

    // Variable para almacenar el resultado del dado
    int roll;

    public void RollRandom()
    {
        // Obtenemos el componente SpriteRenderer
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        // Cambiamos la cara del dado a una aleatoria
        renderer.sprite = die[Random.Range(0, die.Count)];
    }

    public void RollDie(int temp)
    {
        // Guardamos el resultado del dado
        roll = temp;

        // Reproducimos la animación de lanzamiento del dado (RollDie)
        Animator animator = GetComponent<Animator>();
        animator.Play("RollDie", -1, 0f);
    }

    public void SetRoll()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = die[roll - 1];
    }
}
