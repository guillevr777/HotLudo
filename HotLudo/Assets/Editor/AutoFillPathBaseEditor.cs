using UnityEngine;
using UnityEditor;

public class AutoFillAllPathsEditor : EditorWindow
{
    BoardManager boardManager;

    // Parents para cada path
    Transform parentBase;
    Transform parentFinalAzul;
    Transform parentFinalAmarillo;
    Transform parentFinalVerde;
    Transform parentFinalRojo;

    // Auto-detect by name options
    bool autoDetect = true;
    string baseParentName = "CasillasBase";
    string finalAzulName = "CasillasAzules";
    string finalAmarilloName = "CasillasAmarillas";
    string finalVerdeName = "CasillasVerdes";
    string finalRojoName = "CasillasRojas";

    [MenuItem("Tools/Board → Auto Fill All Paths")]
    static void OpenWindow()
    {
        GetWindow<AutoFillAllPathsEditor>("AutoFill All Paths");
    }

    void OnGUI()
    {
        GUILayout.Label("Auto Fill BoardManager paths from parent children", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        boardManager = (BoardManager)EditorGUILayout.ObjectField("BoardManager", boardManager, typeof(BoardManager), true);

        GUILayout.Space(6);
        autoDetect = EditorGUILayout.ToggleLeft("Auto-detect parents by name", autoDetect);
        if (autoDetect)
        {
            baseParentName = EditorGUILayout.TextField("Base parent name", baseParentName);
            finalAzulName = EditorGUILayout.TextField("Final Azul parent name", finalAzulName);
            finalAmarilloName = EditorGUILayout.TextField("Final Amarillo parent name", finalAmarilloName);
            finalVerdeName = EditorGUILayout.TextField("Final Verde parent name", finalVerdeName);
            finalRojoName = EditorGUILayout.TextField("Final Rojo parent name", finalRojoName);
            EditorGUILayout.HelpBox("Se buscarán GameObjects en la escena por nombre y se usarán sus hijos en el orden de la Hierarchy.", MessageType.Info);
        }
        else
        {
            parentBase = (Transform)EditorGUILayout.ObjectField("Base parent", parentBase, typeof(Transform), true);
            parentFinalAzul = (Transform)EditorGUILayout.ObjectField("Final Azul parent", parentFinalAzul, typeof(Transform), true);
            parentFinalAmarillo = (Transform)EditorGUILayout.ObjectField("Final Amarillo parent", parentFinalAmarillo, typeof(Transform), true);
            parentFinalVerde = (Transform)EditorGUILayout.ObjectField("Final Verde parent", parentFinalVerde, typeof(Transform), true);
            parentFinalRojo = (Transform)EditorGUILayout.ObjectField("Final Rojo parent", parentFinalRojo, typeof(Transform), true);
            EditorGUILayout.HelpBox("Arrastra aquí los GameObjects padres que contienen las casillas como hijos.", MessageType.Info);
        }

        EditorGUILayout.Space();
        GUI.enabled = boardManager != null;
        if (GUILayout.Button("Fill All Paths"))
        {
            FillAllPaths();
        }
        GUI.enabled = true;

        EditorGUILayout.Space();
        if (GUILayout.Button("Select BoardManager in Hierarchy"))
        {
            if (boardManager != null) Selection.activeObject = boardManager.gameObject;
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Esto asigna las propiedades (pathBase, finalPathAzul, finalPathAmarillo, finalPathVerde, finalPathRojo) en el BoardManager seleccionadoo.", MessageType.None);
    }

    void FillAllPaths()
    {
        if (boardManager == null)
        {
            Debug.LogError("AutoFillAllPaths: asigna un BoardManager en la ventana.");
            return;
        }

        // resolve parents if auto-detect
        if (autoDetect)
        {
            if (string.IsNullOrEmpty(baseParentName) == false)
            {
                var go = GameObject.Find(baseParentName);
                if (go != null) parentBase = go.transform;
                else parentBase = null;
            }
            if (string.IsNullOrEmpty(finalAzulName) == false)
            {
                var go = GameObject.Find(finalAzulName);
                if (go != null) parentFinalAzul = go.transform;
                else parentFinalAzul = null;
            }
            if (string.IsNullOrEmpty(finalAmarilloName) == false)
            {
                var go = GameObject.Find(finalAmarilloName);
                if (go != null) parentFinalAmarillo = go.transform;
                else parentFinalAmarillo = null;
            }
            if (string.IsNullOrEmpty(finalVerdeName) == false)
            {
                var go = GameObject.Find(finalVerdeName);
                if (go != null) parentFinalVerde = go.transform;
                else parentFinalVerde = null;
            }
            if (string.IsNullOrEmpty(finalRojoName) == false)
            {
                var go = GameObject.Find(finalRojoName);
                if (go != null) parentFinalRojo = go.transform;
                else parentFinalRojo = null;
            }
        }

        int filled = 0;
        Undo.RecordObject(boardManager, "AutoFillAllPaths");

        if (parentBase != null)
        {
            AssignTransformArrayToProperty(boardManager, "pathBase", parentBase);
            filled++;
        }
        else Debug.LogWarning("AutoFillAllPaths: parentBase no asignado (se omitirá).");

        if (parentFinalAzul != null)
        {
            AssignTransformArrayToProperty(boardManager, "finalPathAzul", parentFinalAzul);
            filled++;
        }
        else Debug.LogWarning("AutoFillAllPaths: parentFinalAzul no asignado (se omitirá).");

        if (parentFinalAmarillo != null)
        {
            if (!AssignTransformArrayToProperty(boardManager, "finalPathAmarillo", parentFinalAmarillo))
                AssignTransformArrayToProperty(boardManager, "pathAmarillo", parentFinalAmarillo);
            filled++;
        }
        else Debug.LogWarning("AutoFillAllPaths: parentFinalAmarillo no asignado (se omitirá).");

        if (parentFinalVerde != null)
        {
            if (!AssignTransformArrayToProperty(boardManager, "finalPathVerde", parentFinalVerde))
                AssignTransformArrayToProperty(boardManager, "pathVerde", parentFinalVerde);
            filled++;
        }
        else Debug.LogWarning("AutoFillAllPaths: parentFinalVerde no asignado (se omitirá).");

        if (parentFinalRojo != null)
        {
            if (!AssignTransformArrayToProperty(boardManager, "finalPathRojo", parentFinalRojo))
                AssignTransformArrayToProperty(boardManager, "pathRojo", parentFinalRojo);
            filled++;
        }
        else Debug.LogWarning("AutoFillAllPaths: parentFinalRojo no asignado (se omitirá).");

        EditorUtility.SetDirty(boardManager);
        Debug.Log($"AutoFillAllPaths: completado. Arrays rellenados: {filled}");
    }

    bool AssignTransformArrayToProperty(BoardManager bm, string propertyName, Transform parent)
    {
        if (bm == null || parent == null) return false;

        SerializedObject so = new SerializedObject(bm);
        SerializedProperty prop = so.FindProperty(propertyName);

        if (prop == null)
        {
            Debug.LogWarning($"AutoFillAllPaths: BoardManager no tiene la propiedad '{propertyName}'.");
            return false;
        }

        int count = parent.childCount;
        prop.arraySize = count;
        for (int i = 0; i < count; i++)
        {
            SerializedProperty element = prop.GetArrayElementAtIndex(i);
            element.objectReferenceValue = parent.GetChild(i);
        }

        so.ApplyModifiedProperties();
        Debug.Log($"AutoFillAllPaths: '{propertyName}' rellenado con {count} hijos de '{parent.name}'.");
        return true;
    }
}
