using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class SaveDataWrapper
{
    public List<SaveComponentEntry> value = new();
}

[Serializable]
public class SaveComponentEntry
{
    public string componentName;
    public List<SaveFieldEntry> fields = new();
}

[Serializable]
public class SaveFieldEntry
{
    public string fieldName;
    public string fieldValue;
}

public static class SaveDataStorage
{
    public static void Save(SaveDataComponent comp)
    {
        SaveDataWrapper wrapper = new();

        foreach (var entry in comp.componentEntries)
        {
            if (!entry.saveComponent) continue;

            var type = entry.component.GetType();
            SaveComponentEntry componentData = new()
            {
                componentName = type.Name
            };

            if (type == typeof(Transform))
            {
                Transform t = (Transform)entry.component;

                componentData.fields.Add(new SaveFieldEntry { fieldName = "position", fieldValue = SerializeField(t.position) });
                componentData.fields.Add(new SaveFieldEntry { fieldName = "rotation", fieldValue = SerializeField(t.eulerAngles) });
                componentData.fields.Add(new SaveFieldEntry { fieldName = "scale", fieldValue = SerializeField(t.localScale) });

                wrapper.value.Add(componentData);
                continue;
            }

            foreach (var fieldName in entry.saveableFields)
            {
                FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null) continue;

                object value = field.GetValue(entry.component);
                string serializedValue = SerializeField(value);

                componentData.fields.Add(new SaveFieldEntry
                {
                    fieldName = fieldName,
                    fieldValue = serializedValue
                });
            }

            wrapper.value.Add(componentData);
        }

        string json = JsonUtility.ToJson(wrapper, true);
        PlayerPrefs.SetString("Save_" + comp.gameObject.name, json);
        PlayerPrefs.Save();

        Debug.Log("Saved JSON:\n" + json);
    }

    public static void Load(SaveDataComponent comp)
    {
        string key = "Save_" + comp.gameObject.name;
        if (!PlayerPrefs.HasKey(key))
        {
            Debug.LogWarning("No save data found.");
            return;
        }

        string json = PlayerPrefs.GetString(key);
        SaveDataWrapper wrapper = JsonUtility.FromJson<SaveDataWrapper>(json);

        foreach (var entry in comp.componentEntries)
        {
            var type = entry.component.GetType();
            var componentData = wrapper.value.Find(e => e.componentName == type.Name);
            if (componentData == null) continue;

            if (type == typeof(Transform))
            {
                Transform t = (Transform)entry.component;

                foreach (var fieldEntry in componentData.fields)
                {
                    switch (fieldEntry.fieldName)
                    {
                        case "position":
                            t.position = (Vector3)DeserializeField(fieldEntry.fieldValue, typeof(Vector3));
                            break;
                        case "rotation":
                            t.eulerAngles = (Vector3)DeserializeField(fieldEntry.fieldValue, typeof(Vector3));
                            break;
                        case "scale":
                            t.localScale = (Vector3)DeserializeField(fieldEntry.fieldValue, typeof(Vector3));
                            break;
                    }
                }

                continue;
            }

            foreach (var fieldEntry in componentData.fields)
            {
                FieldInfo field = type.GetField(fieldEntry.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null) continue;

                object deserializedValue = DeserializeField(fieldEntry.fieldValue, field.FieldType);
                if (deserializedValue != null)
                    field.SetValue(entry.component, deserializedValue);
            }
        }

        Debug.Log("Loaded data from PlayerPrefs.");
    }

    private static string SerializeField(object value)
    {
        switch (value)
        {
            case int i: return i.ToString();
            case float f: return f.ToString("R");
            case double d: return d.ToString("R");
            case bool b: return b.ToString();
            case Vector2 v2: return $"{v2.x},{v2.y}";
            case Vector3 v3: return $"{v3.x},{v3.y},{v3.z}";
            default:
                Debug.LogWarning("Unsupported type: " + value?.GetType());
                return string.Empty;
        }
    }

    private static object DeserializeField(string value, Type type)
    {
        try
        {
            if (type == typeof(int)) return int.Parse(value);
            if (type == typeof(float)) return float.Parse(value);
            if (type == typeof(double)) return double.Parse(value); 
            if (type == typeof(bool)) return bool.Parse(value);
            if (type == typeof(Vector2))
            {
                string[] parts = value.Split(',');
                return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
            }

            if (type == typeof(Vector3))
            {
                string[] parts = value.Split(',');
                return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Deserialization failed for type {type}: {ex.Message}");
        }

        return null;
    }
}
