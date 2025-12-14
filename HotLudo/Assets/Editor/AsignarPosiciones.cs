using UnityEngine;
using UnityEditor;

public class CasillaAssignerEditor : EditorWindow
{
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

    private static void AssignPositionsToAllCasillas()
    {
        Casilla[] casillas = FindObjectsByType<Casilla>(FindObjectsSortMode.None);

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

            foreach (Transform child in casillaTransform)
            {
                if (child.name.StartsWith("posA", System.StringComparison.OrdinalIgnoreCase) && posAChild == null)
                {
                    posAChild = child;
                }
                else if (child.name.StartsWith("posB", System.StringComparison.OrdinalIgnoreCase) && posBChild == null)
                {
                    posBChild = child;
                }
            }

            if (posAChild != null && casilla.posA != posAChild)
            {
                Undo.RecordObject(casilla, $"Set posA on {casilla.name}");
                casilla.posA = posAChild;
                changed = true;
            }

            if (posBChild != null && casilla.posB != posBChild)
            {
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