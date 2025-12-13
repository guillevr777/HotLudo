using UnityEngine;
using UnityEditor;

/// <summary>
/// Script de Editor para automatizar la asignación de posA y posB en el script Casilla.
/// Debe estar en una carpeta llamada 'Editor'.
/// </summary>
public class CasillaAssignerEditor : EditorWindow
{
    // NO ES NECESARIO CAMBIAR ESTO:
    // private const string PosAName = "posA"; 
    // private const string PosBName = "posB";

    [MenuItem("Tools/Parchis/Asignar Posiciones de Casillas (PosA/PosB)")]
    public static void ShowWindow()
    {
        GetWindow<CasillaAssignerEditor>("Asignar Posiciones de Casillas");
    }

    void OnGUI()
    {
        GUILayout.Label("Automatización de Asignación de Posiciones", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        if (GUILayout.Button("Buscar y Asignar en todas las Casillas"))
        {
            AssignPositionsToAllCasillas();
        }

        EditorGUILayout.Space(20);
        EditorGUILayout.HelpBox("Busca GameObjects con el script 'Casilla' y les asigna sus hijos que empiecen con 'posA' o 'posB' a las variables correspondientes.", MessageType.Info);
    }

    // HECHO PRIVADO PARA RESOLVER EL ERROR DE ACCESIBILIDAD.
    // También he limpiado la búsqueda de FindObjectsOfType.
    private static void AssignPositionsToAllCasillas()
    {
        // El tipo de objeto a buscar es Casilla
        Casilla[] casillas = FindObjectsOfType<Casilla>();

        int assignedCount = 0;
        int totalCount = casillas.Length;

        Debug.Log($"Iniciando asignación automática en {totalCount} casillas...");

        Undo.SetCurrentGroupName("Asignar Posiciones A y B automáticamente");
        int undoGroup = Undo.GetCurrentGroup();

        foreach (Casilla casilla in casillas)
        {
            Transform casillaTransform = casilla.transform;
            bool changed = false;

            Transform posAChild = null;
            Transform posBChild = null;

            // Iterar sobre los hijos y buscar los que empiezan con "posA" o "posB"
            foreach (Transform child in casillaTransform)
            {
                // Buscamos el primer hijo que comience con 'posA' (ignorando sufijos como '(1)', etc.)
                if (child.name.StartsWith("posA", System.StringComparison.OrdinalIgnoreCase) && posAChild == null)
                {
                    posAChild = child;
                }
                // Buscamos el primer hijo que comience con 'posB'
                else if (child.name.StartsWith("posB", System.StringComparison.OrdinalIgnoreCase) && posBChild == null)
                {
                    posBChild = child;
                }
            }

            // 1. Asignar PosA
            if (posAChild != null && casilla.posA != posAChild)
            {
                // Registra la acción para permitir CTRL+Z
                Undo.RecordObject(casilla, $"Set posA on {casilla.name}");
                casilla.posA = posAChild;
                changed = true;
            }

            // 2. Asignar PosB
            if (posBChild != null && casilla.posB != posBChild)
            {
                // Registra la acción para permitir CTRL+Z
                Undo.RecordObject(casilla, $"Set posB on {casilla.name}");
                casilla.posB = posBChild;
                changed = true;
            }

            if (changed)
            {
                EditorUtility.SetDirty(casilla);
                assignedCount++;
            }
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"Proceso terminado. Se asignaron referencias en {assignedCount} de {totalCount} casillas.");
    }
}