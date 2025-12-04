using UnityEngine;

public class Ficha : MonoBehaviour
{
    public ColorJugador color;
    public Casilla casillaActual;

    private void Start()
    {
        if (casillaActual != null)
        {
            casillaActual.AñadirFicha(this);
        }
    }


    public void MoverACasilla(Casilla nuevaCasilla)
    {
        if (casillaActual != null)
            casillaActual.QuitarFicha(this);

        casillaActual = nuevaCasilla;

        nuevaCasilla.AñadirFicha(this);
    }
}
