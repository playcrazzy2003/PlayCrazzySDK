using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains grid construction settings.
/// </summary>
[System.Serializable]
public class GridData
{
    public Vector3 startPointOffset = new(0f, 0.213f, 0f);
    public GridPatternType selectedPatternType;

    [Header("Advanced Layering")]
    public int forwardLayerCount = 1;
    public float forwardLayerSpacing = 3.0f;
    public LayeringDirection layeringDirection = LayeringDirection.Forward;

    [Header("Spacing Controls")]
    public float rowSpacing = 1.5f;
    public float colSpacing = 1.5f;
    public float layerSpacing = 2.0f;

    [Header("Rotation Offset")]
    public Vector3 blockRotationOffset;

    [Header("Scale Offset")]
    public Vector3 scale;

    public List<Transform> transforms = new();
}

/// <summary>
/// A structured layout containing multiple layers of patterns.
/// </summary>
[System.Serializable]
public class LayeredPattern
{
    public string name;
    public List<string[]> layers;

    public LayeredPattern(GridPatternType type, List<string[]> layers)
    {
        this.name = type.ToString();
        this.layers = layers;
    }
}

/// <summary>
/// Builds and destroys 3D patterns based on selected grid configuration.
/// </summary>
public class ItemGridBuilder : MonoBehaviour
{
    [Header("Grid Settings")]
    public Transform parentTransform;
    public GameObject blockPrefab;
    public GridData gridData = new();

    private List<LayeredPattern> patterns = new();

    /// <summary>
    /// Converts the layering direction enum to a vector offset.
    /// </summary>
    Vector3 GetDirectionOffset(LayeringDirection dir)
    {
        return dir switch
        {
            LayeringDirection.Forward => parentTransform.forward,
            LayeringDirection.Right => parentTransform.right,
            LayeringDirection.Up => parentTransform.up,
            _ => Vector3.forward
        };
    }

    /// <summary>
    /// Generates the pattern defined in gridData.
    /// </summary>
    [ContextMenu("Build Selected Pattern")]
    public void BuildSelectedPattern()
    {
        LoadPatterns();

        if (parentTransform == null)
        {
            Debug.LogError("Parent transform not assigned.");
            return;
        }

        LayeredPattern pattern = GetPattern(gridData.selectedPatternType);
        if (pattern == null)
        {
            Debug.LogError("Pattern not found: " + gridData.selectedPatternType);
            return;
        }

        DestroyGeneratedBlocks();

        Vector3 directionOffset = GetDirectionOffset(gridData.layeringDirection);

        for (int forwardLayer = 0; forwardLayer < gridData.forwardLayerCount; forwardLayer++)
        {
            Vector3 forwardOffset = directionOffset * forwardLayer * gridData.forwardLayerSpacing;

            for (int layerIndex = 0; layerIndex < pattern.layers.Count; layerIndex++)
            {
                var layer = pattern.layers[layerIndex];
                Vector3 layerOffset = Vector3.up * layerIndex * gridData.layerSpacing;

                for (int row = 0; row < layer.Length; row++)
                {
                    string rowData = layer[row];
                    for (int col = 0; col < rowData.Length; col++)
                    {
                        if (rowData[col] != '*') continue;

                        Vector3 localPos = new Vector3(col * gridData.colSpacing, 0, row * gridData.rowSpacing);
                        Vector3 worldPos = parentTransform.TransformPoint(localPos + layerOffset + forwardOffset);

                        GameObject block = blockPrefab != null
                            ? Instantiate(blockPrefab, worldPos, Quaternion.identity, parentTransform)
                            : new GameObject("EmptyBlock");

                        block.transform.position = worldPos;
                        block.transform.localScale = gridData.scale;
                        block.transform.Rotate(gridData.blockRotationOffset);
                        block.transform.SetParent(parentTransform, true);

                        gridData.transforms.Add(block.transform);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Removes all generated blocks under the parent transform.
    /// </summary>
    [ContextMenu("Destroy Generated Blocks")]
    public void DestroyGeneratedBlocks()
    {
        if (parentTransform == null) return;

        for (int i = parentTransform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parentTransform.GetChild(i).gameObject);
        }

        gridData.transforms.Clear();
    }

    /// <summary>
    /// Loads built-in patterns into memory.
    /// </summary>
    void LoadPatterns()
    {
        patterns.Clear();

        patterns.Add(new LayeredPattern(GridPatternType.Cube, new List<string[]>
        {
            new[] { "***", "***", "***" },
            new[] { "***", "***", "***" },
            new[] { "***", "***", "***" }
        }));

        patterns.Add(new LayeredPattern(GridPatternType.Cube_2, new List<string[]>
        {
            new[] { "**", "**" },
            new[] { "**", "**" },
            new[] { "**", "**" }
        }));

        patterns.Add(new LayeredPattern(GridPatternType.Pyramid3D, new List<string[]>
        {
            new[] { "****" },
            new[] { " ** " },
            new[] { "  * " }
        }));

        patterns.Add(new LayeredPattern(GridPatternType.Doubleline, new List<string[]>
        {
            new[] { "*", "*" },
            new[] { "*", "*" },
            new[] { "*", "*" }
        }));

        patterns.Add(new LayeredPattern(GridPatternType.SingleLine, new List<string[]>
        {
            new[] { "*" },
            new[] { "*" },
            new[] { "*" }
        }));

        patterns.Add(new LayeredPattern(GridPatternType.Cross, new List<string[]>
        {
            new[] { "   *   " },
            new[] { "   *   " },
            new[] { "*******" },
            new[] { "   *   " },
            new[] { "   *   " }
        }));

        patterns.Add(new LayeredPattern(GridPatternType.Wedge, new List<string[]>
        {
            new[] { "    *    " },
            new[] { "   ***   " },
            new[] { "  *****  " },
            new[] { " ******* " },
            new[] { "*********" }
        }));
    }

    /// <summary>
    /// Returns the pattern corresponding to the selected type.
    /// </summary>
    LayeredPattern GetPattern(GridPatternType type)
    {
        return patterns.Find(p => p.name == type.ToString());
    }
}

/// <summary>
/// Available pattern types.
/// </summary>
public enum GridPatternType
{
    Cube,
    Pyramid3D,
    SingleLine,
    Cross,
    Wedge,
    Cube_2,
    Doubleline
}

/// <summary>
/// Direction used for forward layering.
/// </summary>
[System.Serializable]
public enum LayeringDirection
{
    Forward,
    Right,
    Up
}
