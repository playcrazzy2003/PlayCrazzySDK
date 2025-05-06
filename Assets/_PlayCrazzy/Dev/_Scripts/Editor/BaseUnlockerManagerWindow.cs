using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseUnlockManager))]
public class BaseUnlockerManagerWindow : Editor
{
    private BaseUnlockManager table;
    private Vector2 scrollPos;

    private float colNoWidth = 40f;
    private float colBaseUnlockerWidth = 150f;
    private float colNameWidth = 150f;
    private float colIconWidth = 80f;
    private float colCostWidth = 100f;

    private const float splitterWidth = 5f;
    private int draggingColumn = -1;
    private Rect[] columnRects;

    private void OnEnable()
    {
        table = (BaseUnlockManager)target;
        if (table.entries == null)
        {
            table.entries = new System.Collections.Generic.List<BaseUnlockerEntry>();
            Debug.Log("BaseUnlockManager: entries list initialized.");
        }
    }

    public override void OnInspectorGUI()
    {
        if (table == null)
            return;

        serializedObject.Update();

        GUILayout.Space(50);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        HandleResize();
        DrawHeader();

        int mainRowCounter = 1;
        for (int i = 0; i < table.entries.Count; i++)
        {
            DrawRow(table.entries[i], i, ref mainRowCounter, 0);
        }

        EditorGUILayout.EndScrollView();
        GUILayout.Space(50f);

        if (GUILayout.Button("Add New Row", GUILayout.Height(30)))
        {
            table.entries.Add(new BaseUnlockerEntry());
            Debug.Log("New main row added.");
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(table);
    }

    private void DrawRow(BaseUnlockerEntry entry, int index, ref int rowCounter, int indent,
        BaseUnlockerEntry parent = null)
    {
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Space(indent * 40);

        // Row number
        string rowLabel = indent == 0 ? rowCounter.ToString() : "↳ " + rowCounter;
        EditorGUILayout.LabelField(rowLabel, GUILayout.Width(colNoWidth));
        rowCounter++;

        // Fields
        entry.baseUnlocker = (BaseUnlocker)EditorGUILayout.ObjectField(entry.baseUnlocker, typeof(BaseUnlocker), true,
            GUILayout.Width(colBaseUnlockerWidth));
        entry.baseName = EditorGUILayout.TextField(entry.baseName, GUILayout.Width(colNameWidth));
        entry.icon = (Sprite)EditorGUILayout.ObjectField(entry.icon, typeof(Sprite), false,
            GUILayout.Width(colIconWidth), GUILayout.Height(40));
        entry.baseUnlockCost = EditorGUILayout.FloatField(entry.baseUnlockCost, GUILayout.Width(colCostWidth));
        GUILayout.Space(40);
        entry.showSubRows = EditorGUILayout.Foldout(entry.showSubRows, "Sub Rows", true);

        // Delete Buttons
        if (indent == 0)
        {
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                table.entries.RemoveAt(index);
                Debug.Log($"Removed main row at index {index}");
                EditorGUILayout.EndHorizontal();
                return;
            }
        }
        else if (parent != null)
        {
            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                parent.subRows.RemoveAt(index);
                Debug.Log($"Removed sub-row at index {index} from parent {parent.baseName}");
                EditorGUILayout.EndHorizontal();
                return;
            }
        }

        EditorGUILayout.EndHorizontal();

        // Sub-rows
        if (entry.showSubRows)
        {
            if (entry.subRows == null)
                entry.subRows = new System.Collections.Generic.List<BaseUnlockerEntry>();

            int subRowCounter = 1;
            for (int j = 0; j < entry.subRows.Count; j++)
            {
                DrawRow(entry.subRows[j], j, ref subRowCounter, indent + 1, entry);
            }

            // Add Sub-row
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space((indent + 1) * 15);
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                entry.subRows.Add(new BaseUnlockerEntry());
                Debug.Log($"Added sub-row to {entry.baseName}");
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawHeader()
    {
        GUIStyle bold = new GUIStyle(EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(12);
        GUILayout.Label("No.", bold, GUILayout.Width(colNoWidth));
        GUILayout.Label("BaseUnlocker", bold, GUILayout.Width(colBaseUnlockerWidth));
        GUILayout.Label("Name", bold, GUILayout.Width(colNameWidth));
        GUILayout.Label("Icon", bold, GUILayout.Width(colIconWidth));
        GUILayout.Label("Cost", bold, GUILayout.Width(colCostWidth));
        GUILayout.Space(25);
        EditorGUILayout.EndHorizontal();
    }

    private void HandleResize()
    {
        if (columnRects == null || columnRects.Length != 5)
            columnRects = new Rect[5];

        float x = 12;
        float height = 20f; // static height, since we don’t have dynamic headers
        columnRects[0] = new Rect(x + colNoWidth, 0, splitterWidth, height);
        x += colNoWidth + splitterWidth;
        columnRects[1] = new Rect(x + colBaseUnlockerWidth, 0, splitterWidth, height);
        x += colBaseUnlockerWidth + splitterWidth;
        columnRects[2] = new Rect(x + colNameWidth, 0, splitterWidth, height);
        x += colNameWidth + splitterWidth;
        columnRects[3] = new Rect(x + colIconWidth, 0, splitterWidth, height);
        x += colIconWidth + splitterWidth;
        columnRects[4] = new Rect(x + colCostWidth, 0, splitterWidth, height);

        for (int i = 0; i < columnRects.Length; i++)
        {
            EditorGUIUtility.AddCursorRect(columnRects[i], MouseCursor.ResizeHorizontal);
        }

        Event e = Event.current;

        if (e.type == EventType.MouseDown)
        {
            for (int i = 0; i < columnRects.Length; i++)
            {
                if (columnRects[i].Contains(e.mousePosition))
                {
                    draggingColumn = i;
                    e.Use();
                    break;
                }
            }
        }

        if (e.type == EventType.MouseDrag && draggingColumn != -1)
        {
            float delta = e.delta.x;
            switch (draggingColumn)
            {
                case 0: colNoWidth = Mathf.Max(20, colNoWidth + delta); break;
                case 1: colBaseUnlockerWidth = Mathf.Max(60, colBaseUnlockerWidth + delta); break;
                case 2: colNameWidth = Mathf.Max(60, colNameWidth + delta); break;
                case 3: colIconWidth = Mathf.Max(40, colIconWidth + delta); break;
                case 4: colCostWidth = Mathf.Max(40, colCostWidth + delta); break;
            }

            e.Use();
        }

        if (e.type == EventType.MouseUp && draggingColumn != -1)
        {
            draggingColumn = -1;
            e.Use();
        }
    }
}