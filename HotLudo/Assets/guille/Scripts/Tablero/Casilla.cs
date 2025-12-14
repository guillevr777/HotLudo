using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    Normal,
    Safe,
    Exit,
    Final // Casilla dentro del camino final (Meta)
}

public class Casilla : MonoBehaviour
{
    [Header("Tipo de casilla")]
    public CellType cellType = CellType.Normal;

    [Header("Posiciones para fichas (Solo Normal/Safe/Exit)")]
    public Transform posA;
    public Transform posB;

    // Slots fijos para casillas NO finales
    public Pawn pawnA;
    public Pawn pawnB;

    // Lista para casillas FINALES (permite más de 2)
    private List<Pawn> finalPawns = new List<Pawn>();

    // ──────────────
    // Ocupación
    // ──────────────
    public bool HasFreeSlot()
    {
        if (cellType == CellType.Final)
        {
            // En la meta, siempre hay espacio hasta que estén las 4
            return finalPawns.Count < 4;
        }

        // Para el resto de casillas (Normal/Safe/Exit)
        return pawnA == null || (posB != null && pawnB == null);
    }

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

    public bool IsBridge()
    {
        // La casilla final NUNCA es un puente
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

    // ──────────────
    // Gestión de fichas
    // ──────────────
    public Vector3 GetFreePosition(Pawn pawn)
    {
        if (cellType == CellType.Final)
        {
            // En la meta, añadimos a la lista y devolvemos la posición central de la Casilla.
            if (!finalPawns.Contains(pawn))
            {
                finalPawns.Add(pawn);
            }
            // Retorna la posición de la Casilla, ya que no usamos posA/posB en la meta.
            return transform.position;
        }

        // Casillas Normales/Seguras/Salida
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
    /// Quita la referencia a la ficha de esta casilla.
    /// </summary>
    public void RemovePawn(Pawn pawn)
    {
        if (cellType == CellType.Final)
        {
            finalPawns.Remove(pawn);
            return;
        }

        // Casillas Normales/Seguras/Salida
        if (pawnA == pawn)
        {
            pawnA = null;

            // CONSOLIDACIÓN
            if (posB != null && pawnB != null)
            {
                pawnA = pawnB;
                pawnB = null;
                // Mueve visualmente la ficha que estaba en B a la posición de A
                pawnA.transform.position = posA.position;
            }
        }
        else if (pawnB == pawn)
        {
            pawnB = null;
        }
    }

    // Solo busca rivales en casillas NO finales
    public Pawn GetRivalPawn(int playerIndex)
    {
        if (cellType == CellType.Final) return null;

        if (pawnA != null && pawnA.playerIndex != playerIndex) return pawnA;
        if (pawnB != null && pawnB.playerIndex != playerIndex) return pawnB;
        return null;
    }
}