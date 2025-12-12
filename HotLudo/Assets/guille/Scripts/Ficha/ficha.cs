using UnityEngine;

public class Ficha : MonoBehaviour
{
    public ColorJugador color;
    public Casilla casillaActual;

    [Header("Spawn (punto al que vuelve si es comida)")]
    public Transform spawnPoint;


    private void Start()
    {
        if (casillaActual != null)
        {
            casillaActual.AñadirFicha(this);
        }
    }


    // MOVER UNA FICHA A OTRA CASILLA
    public void MoverACasilla(Casilla nuevaCasilla)
    {
        if (casillaActual != null)
            casillaActual.QuitarFicha(this);

        casillaActual = nuevaCasilla;

        nuevaCasilla.AñadirFicha(this);
    }
}
