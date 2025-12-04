using System.Collections.Generic;
using UnityEngine;

public enum ColorJugador { Rojo, Azul, Amarillo, Verde }

public class Tablero : MonoBehaviour
{
    [Header("Recorrido principal (0–69) en orden")]
    public List<Casilla> casillas; // 69 casillas

    [Header("Caminos finales")]
    public List<Casilla> caminoRojoFinal;
    public List<Casilla> caminoAzulFinal;
    public List<Casilla> caminoAmarilloFinal;
    public List<Casilla> caminoVerdeFinal;

    // Devuelve una casilla del camino principal
    public Casilla GetCasilla(int index)
    {
        return casillas[index % casillas.Count];
    }

    // Devuelve la casilla inicial del camino final
    public Casilla GetInicioCaminoFinal(ColorJugador color)
    {
        switch (color)
        {
            case ColorJugador.Rojo:
                return caminoRojoFinal[0];
            case ColorJugador.Azul:
                return caminoAzulFinal[0];
            case ColorJugador.Amarillo:
                return caminoAmarilloFinal[0];
            case ColorJugador.Verde:
                return caminoVerdeFinal[0];
        }
        return null;
    }

    // Devuelve una casilla interna del camino final
    public Casilla GetCasillaFinal(ColorJugador color, int paso)
    {
        switch (color)
        {
            case ColorJugador.Rojo:
                return caminoRojoFinal[paso];
            case ColorJugador.Azul:
                return caminoAzulFinal[paso];
            case ColorJugador.Amarillo:
                return caminoAmarilloFinal[paso];
            case ColorJugador.Verde:
                return caminoVerdeFinal[paso];
        }
        return null;
    }
}
