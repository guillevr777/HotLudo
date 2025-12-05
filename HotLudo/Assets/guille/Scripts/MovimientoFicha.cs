using System.Collections;
using UnityEngine;

public class MovimientoFicha : MonoBehaviour
{
    public Tablero tablero;
    public int pasosARecorrer = 0;
    public float velocidad = 0.2f;

    public Ficha ficha;  // la ficha a mover


    // FUNCIÓN PRINCIPAL PARA MOVER UNA FICHA
    public void MoverFicha(Ficha fichaAMover, int pasos)
    {
        ficha = fichaAMover;
        pasosARecorrer = pasos;

        StartCoroutine(MoverPasoAPaso());
    }


    // CORUTINA: mueve la ficha una casilla por paso
    private IEnumerator MoverPasoAPaso()
    {
        for (int i = 0; i < pasosARecorrer; i++)
        {
            Casilla siguiente = ObtenerSiguienteCasilla();

            if (siguiente.EstaBloqueada())
            {
                Debug.Log("Movimiento bloqueado por dos fichas del mismo color");
                yield break;
            }

            yield return StartCoroutine(MoverSuavemente(siguiente.posiciones[0].position));

            ficha.MoverACasilla(siguiente);

            yield return new WaitForSeconds(0.05f);
        }

        Debug.Log("Movimiento completado");
    }


    // MOVER SUAVEMENTE (animación)
    private IEnumerator MoverSuavemente(Vector3 destino)
    {
        destino.z = -1; 

        while (Vector3.Distance(ficha.transform.position, destino) > 0.01f)
        {
            ficha.transform.position = Vector3.MoveTowards(
                ficha.transform.position,
                destino,
                velocidad
            );

            yield return null;
        }
    }


    // Incluye entrada a camino final
    private Casilla ObtenerSiguienteCasilla()
    {
        Casilla actual = ficha.casillaActual;

        if (DebeEntrarAlCaminoFinal(actual))
        {
            return tablero.GetInicioCaminoFinal(ficha.color);
        }

        return tablero.GetCasilla(actual.index + 1);
    }


    private bool DebeEntrarAlCaminoFinal(Casilla actual)
    {
        switch (ficha.color)
        {
            case ColorJugador.Azul:
                return actual.index == 47;

            case ColorJugador.Rojo:
                return actual.index == 65;

            case ColorJugador.Verde:
                return actual.index == 12;

            case ColorJugador.Amarillo:
                return actual.index == 30;
        }

        return false;
    }

    void Update()
    {
    }

}
