using System.Collections.Generic;
using UnityEngine;

public enum TipoCasilla { Normal, Segura, Salida, Meta }

public class Casilla : MonoBehaviour
{
    [Header("Identificador de la casilla")]
    public int index;

    [Header("Tipo de casilla")]
    public TipoCasilla tipo = TipoCasilla.Normal;

    [Header("Posiciones para fichas")]
    public Transform[] posiciones; // PosA, PosB

    [HideInInspector]
    public List<Ficha> fichasEnCasilla = new List<Ficha>();

    // Añadir ficha a esta casilla
    public void AñadirFicha(Ficha ficha)
    {
        fichasEnCasilla.Add(ficha);

        int slot = fichasEnCasilla.Count - 1;
        if (slot >= posiciones.Length)
            slot = posiciones.Length - 1;

        ficha.transform.position = posiciones[slot].position;
    }

    // Quitar ficha al salir
    public void QuitarFicha(Ficha ficha)
    {
        fichasEnCasilla.Remove(ficha);
    }

    // Indica si esta casilla está bloqueada por 2 fichas del mismo color
    public bool EstaBloqueada()
    {
        if (fichasEnCasilla.Count < 2) return false;

        return fichasEnCasilla[0].color == fichasEnCasilla[1].color;
    }
}
