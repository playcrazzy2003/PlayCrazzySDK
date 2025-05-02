using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataComponent : MonoBehaviour
{
    [Serializable]
    public class ComponentDataEntry
    {
        public Component component;
        public bool saveComponent;
        public List<string> saveableFields = new List<string>();
    }

    public List<ComponentDataEntry> componentEntries = new();
}
