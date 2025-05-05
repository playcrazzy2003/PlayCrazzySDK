using UnityEngine;
using UnityEditor;

public class BaseUnlockerManagerWindow : EditorWindow
{
    private BaseUnlockerTable table;
    private Vector2 scrollPos;

    private float colNoWidth = 40f;
    private float colBaseUnlockerWidth = 150f;
    private float colNameWidth = 150f;
    private float colIconWidth = 80f;
    private float colCostWidth = 100f;
    private float colSubRowWidth = 100f;
    private const float splitterWidth = 5f;

    private int draggingColumn = -1;
    private Rect[] columnRects;

    [MenuItem("GameTools/Base Unlocker Manager")]
    public static void ShowWindow()
    {
        GetWindow<BaseUnlockerManagerWindow>("Base Unlocker Manager");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        table = (BaseUnlockerTable)EditorGUILayout.ObjectField("Unlocker Table", table, typeof(BaseUnlockerTable),
            false);
        if (table == null) return;

        GUILayout.Space(10);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        HandleResize();
        DrawHeader();

        for (int i = 0; i < table.entries.Count; i++)
        {
            DrawRow(table.entries[i], i);
        }

        EditorGUILayout.EndScrollView();
        GUILayout.Space(10);

        if (GUILayout.Button("Add New Row", GUILayout.Height(30)))
        {
            table.entries.Add(new BaseUnlockerEntry());
        }

        EditorUtility.SetDirty(table);
    }

   private void DrawRow(BaseUnlockerEntry entry, int index, int indent = 0)
{
    EditorGUILayout.BeginHorizontal("box");
    GUILayout.Space(indent * 15);

    // Foldout to toggle visibility of sub-rows

    // Base Unlocker Row fields
    entry.currntIndex = EditorGUILayout.IntField(entry.currntIndex, GUILayout.Width(colNoWidth));
    entry.baseUnlocker = (BaseUnlocker)EditorGUILayout.ObjectField(entry.baseUnlocker, typeof(BaseUnlocker), true, GUILayout.Width(colBaseUnlockerWidth));
    entry.baseName = EditorGUILayout.TextField(entry.baseName, GUILayout.Width(colNameWidth));
    entry.icon = (Sprite)EditorGUILayout.ObjectField(entry.icon, typeof(Sprite), false, GUILayout.Width(colIconWidth), GUILayout.Height(40));
    entry.baseUnlockCost = EditorGUILayout.FloatField(entry.baseUnlockCost, GUILayout.Width(colCostWidth));
    entry.showSubRows = EditorGUILayout.Foldout(entry.showSubRows, "Sub Rows", true);

    // Remove row button
    if (GUILayout.Button("X", GUILayout.Width(50)))
    {
        if (indent == 0)
            table.entries.RemoveAt(index);
        EditorGUILayout.EndHorizontal();
        return;
    }

    EditorGUILayout.EndHorizontal();

    // If sub-rows are visible, draw them
    if (entry.showSubRows)
    {
        for (int j = 0; j < entry.subRows.Count; j++)
        {
            // Draw sub-row with adjusted indentation
            DrawRow(entry.subRows[j], j, indent + 1);

            // Add a remove button for each sub-row
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space((indent + 1) * 15);
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                entry.subRows.RemoveAt(j);
                break; // Break after removing to avoid index issues during iteration
            }
            EditorGUILayout.EndHorizontal();
        }

        // Add Sub Row Button
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space((indent + 1) * 15);

        // Add Sub Row button with width adjustment
        if (GUILayout.Button("Add Sub Row", GUILayout.Width(colSubRowWidth)))
        {
            entry.subRows.Add(new BaseUnlockerEntry());
        }
        EditorGUILayout.EndHorizontal();
    }
}


    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(12);
        GUILayout.Label("No.", GUILayout.Width(colNoWidth));
        GUILayout.Label("BaseUnlocker", GUILayout.Width(colBaseUnlockerWidth));
        GUILayout.Label("Name", GUILayout.Width(colNameWidth));
        GUILayout.Label("Icon", GUILayout.Width(colIconWidth));
        GUILayout.Label("Cost", GUILayout.Width(colCostWidth));
        GUILayout.Space(25);
        EditorGUILayout.EndHorizontal();
    }

    private void HandleResize()
    {
        if (columnRects == null || columnRects.Length != 5)
            columnRects = new Rect[5];

        float x = 12;
        columnRects[0] = new Rect(x + colNoWidth, 0, splitterWidth, position.height);
        x += colNoWidth + splitterWidth;
        columnRects[1] = new Rect(x + colBaseUnlockerWidth, 0, splitterWidth, position.height);
        x += colBaseUnlockerWidth + splitterWidth;
        columnRects[2] = new Rect(x + colNameWidth, 0, splitterWidth, position.height);
        x += colNameWidth + splitterWidth;
        columnRects[3] = new Rect(x + colIconWidth, 0, splitterWidth, position.height);
        x += colIconWidth + splitterWidth;
        columnRects[4] = new Rect(x + colCostWidth, 0, splitterWidth, position.height);

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