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
    public Transform[] posiciones; 

    [HideInInspector]
    public List<Ficha> fichasEnCasilla = new List<Ficha>();


    // Añadir ficha a esta casilla
    public void AñadirFicha(Ficha ficha)
    {
        fichasEnCasilla.Add(ficha);

        int slot = fichasEnCasilla.Count - 1;
        if (slot >= posiciones.Length)
            slot = posiciones.Length - 1;

        Vector3 nuevaPos = posiciones[slot].position;
        nuevaPos.z = -1f; 
        ficha.transform.position = nuevaPos;

        ficha.casillaActual = this;   
        ComprobarColision(ficha);     
    }


    // Quitar ficha de esta casilla
    public void QuitarFicha(Ficha ficha)
    {
        fichasEnCasilla.Remove(ficha);
    }


    // ¿Casilla bloqueada?
    public bool EstaBloqueada()
    {
        if (fichasEnCasilla.Count < 2)
            return false;

        return fichasEnCasilla[0].color == fichasEnCasilla[1].color;
    }

    public Ficha ObtenerFichaEnemiga(ColorJugador color)
    {
        if (fichasEnCasilla.Count == 1 &&
            fichasEnCasilla[0].color != color)
        {
            return fichasEnCasilla[0];
        }

        return null;
    }


    // Comprobar colisión al entrar una ficha
    private void ComprobarColision(Ficha fichaEntrante)
    {
        if (tipo == TipoCasilla.Segura)
            return;

        if (fichasEnCasilla.Count == 2)
        {
            if (EstaBloqueada())
                return;
        }
    }
}
