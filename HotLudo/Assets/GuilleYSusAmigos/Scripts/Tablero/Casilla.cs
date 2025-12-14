using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tipos de casilla posibles
/// </summary>
public enum CellType
{
    Normal,
    Safe,
    Exit,
    Final
}

/// <summary>
/// Controla ocupación, posiciones de fichas y capturas
/// </summary>
public class Casilla : MonoBehaviour
{
    [Header("Tipo de casilla")]
    public CellType cellType = CellType.Normal;

    [Header("Posiciones para fichas (Solo Normal/Safe/Exit)")]
    public Transform posA;
    public Transform posB;

    // Fichas ocupando la casilla (Solo Normal/Safe/Exit)
    public Pawn pawnA;
    public Pawn pawnB;

    // Fichas en la casilla final (Solo Final)
    private List<Pawn> finalPawns = new List<Pawn>();

    /// <summary>
    /// Devuelve si hay espacio libre en la casilla
    /// </summary>
    /// <returns></returns>
    public bool HasFreeSlot()
    {
        if (cellType == CellType.Final)
        {
            // En la meta siempre hay espacio hasta que estén las 4
            return finalPawns.Count < 4;
        }

        // Para el resto de casillas
        return pawnA == null || (posB != null && pawnB == null);
    }

    /// <summary>
    /// Retorna el número de fichas actualmente en la casilla
    /// </summary>
    public int PawnCount()
    {
        if (cellType == CellType.Final)
        {
            return finalPawns.Count;
        }

        // Para el resto de casillas
        int count = 0;
        if (pawnA != null) count++;
        if (pawnB != null) count++;
        return count;
    }

    /// <summary>
    /// Comprueba si la casilla es un puente
    /// </summary>
    public bool IsBridge()
    {
        // La casilla final no son un puente
        if (cellType == CellType.Final) return false;

        // El resto de casillas
        if (pawnA != null && pawnB != null)
        {
            if (pawnA.playerIndex == pawnB.playerIndex)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Devuelve la posición libre para una ficha que llega a la casilla
    /// </summary>
    /// <param name="pawn">Ficha que se mueve</param>
    public Vector3 GetFreePosition(Pawn pawn)
    {
        if (cellType == CellType.Final)
        {
            if (!finalPawns.Contains(pawn))
            {
                finalPawns.Add(pawn);
            }
            return transform.position;
        }

        if (pawnA == null)
        {
            pawnA = pawn;
            return posA.position;
        }

        if (posB != null && pawnB == null)
        {
            pawnB = pawn;
            return posB.position;
        }

        return posA.position;
    }

    /// <summary>
    /// Quita la referencia a la ficha de esta casilla
    /// </summary>
    /// <param name="pawn">Ficha a remover</param>
    public void RemovePawn(Pawn pawn)
    {
        if (cellType == CellType.Final)
        {
            finalPawns.Remove(pawn);
            return;
        }

        if (pawnA == pawn)
        {
            pawnA = null;

            if (posB != null && pawnB != null)
            {
                pawnA = pawnB;
                pawnB = null;
                pawnA.transform.position = posA.position;
            }
        }
        else if (pawnB == pawn)
        {
            pawnB = null;
        }
    }

    /// <summary>
    /// Devuelve una ficha rival en casillas normales/seguras/salida
    /// </summary>
    /// <param name="playerIndex">Índice del jugador que llega</param>
    /// <returns>Ficha rival o null</returns>
    public Pawn GetRivalPawn(int playerIndex)
    {
        if (cellType == CellType.Final) return null;

        if (pawnA != null && pawnA.playerIndex != playerIndex) return pawnA;
        if (pawnB != null && pawnB.playerIndex != playerIndex) return pawnB;
        return null;
    }
}