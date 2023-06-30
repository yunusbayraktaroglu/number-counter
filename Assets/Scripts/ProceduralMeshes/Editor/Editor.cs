using UnityEngine;
using UnityEditor;

namespace ProceduralMeshes
{
    [CustomEditor(typeof(ProceduralMesh))]
    public class ExtendEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ProceduralMesh script = (ProceduralMesh)target;

            if ( script.mesh != null)
            {
                if (script.canBake && GUILayout.Button("Bake Mesh"))
                {
                    script.BakeMesh();
                }
                if (GUILayout.Button("Apply Scale"))
                {
                    script.ApplyScale();
                }
            }
        }

        // Add a menu item to create custom GameObjects.
        // Priority 10 ensures it is grouped with the other menu items of the same kind
        // and propagated to the hierarchy dropdown and hierarchy context menus.
        [MenuItem("GameObject/Pronotron/Procedural Mesh", false, 0)]
        static void CreateCounterGameObject(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("ProceduralMesh");
            go.AddComponent<ProceduralMesh>();

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            Selection.activeObject = go;
        }
    }

}