using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum que representa los colores de los jugadores
/// </summary>
public enum ColorJugador { Rojo, Azul, Amarillo, Verde }

/// <summary>
/// Gestiona el tablero del juego
/// Contiene las casillas del recorrido principal y los caminos finales de cada jugador
/// </summary>
public class Tablero : MonoBehaviour
{
    [Header("Recorrido principal")]
    public List<Casilla> casillas;

    [Header("Caminos finales")]
    public List<Casilla> caminoRojoFinal;
    public List<Casilla> caminoAzulFinal;
    public List<Casilla> caminoAmarilloFinal;
    public List<Casilla> caminoVerdeFinal;

    /// <summary>
    /// Devuelve la casilla del recorrido principal según el índice
    /// </summary>
    /// <param name="index">Índice de la casilla en el recorrido principal</param>
    /// <returns>Casilla correspondiente en el tablero</returns>
    public Casilla GetCasilla(int index)
    {
        return casillas[index % casillas.Count];
    }

    /// <summary>
    /// Devuelve la casilla inicial del camino final de un jugador
    /// </summary>
    /// <param name="color">Color del jugador</param>
    /// <returns>Primera casilla del camino final del jugador</returns>
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

    /// <summary>
    /// Devuelve una casilla interna del camino final de un jugador
    /// </summary>
    /// <param name="color">Color del jugador</param>
    /// <param name="paso">Paso dentro del camino final</param>
    /// <returns>Casilla correspondiente en el camino final</returns>
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