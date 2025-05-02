using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveDataComponent))]
public class SaveDataComponentEditor : Editor
{
    private SaveDataComponent saveDataComp;

    public override void OnInspectorGUI()
    {
        saveDataComp = (SaveDataComponent)target;

        if (GUILayout.Button("Scan Components"))
        {
            ScanComponents();
        }

        foreach (var entry in saveDataComp.componentEntries)
        {
            EditorGUILayout.BeginVertical("box");
            entry.saveComponent = EditorGUILayout.ToggleLeft("+" + entry.component.GetType().Name, entry.saveComponent);

            if (entry.saveComponent)
            {
                var fields = entry.component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (field.GetCustomAttribute<SaveableFieldAttribute>() != null)
                    {
                        bool save = entry.saveableFields.Contains(field.Name);
                        bool newSave = EditorGUILayout.ToggleLeft(" - " + field.Name, save);

                        if (newSave && !save) entry.saveableFields.Add(field.Name);
                        else if (!newSave && save) entry.saveableFields.Remove(field.Name);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Save To PlayerPrefs"))
        {
            SaveDataStorage.Save(saveDataComp);
        }

        if (GUILayout.Button("Load From PlayerPrefs"))
        {
            SaveDataStorage.Load(saveDataComp);
        }
    }

    private void ScanComponents()
    {
        saveDataComp.componentEntries.Clear();
        foreach (var comp in saveDataComp.gameObject.GetComponents<Component>())
        {
            if (comp is SaveDataComponent) continue;
            saveDataComp.componentEntries.Add(new SaveDataComponent.ComponentDataEntry { component = comp });
        }
    }
}
