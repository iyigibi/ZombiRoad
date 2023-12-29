using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
public class VertIdxToUv : MonoBehaviour
{
    public int channel = 2;

    // Start is called before the first frame update
    void Start()
    {
    }

    [ContextMenu("bake vertices")]
    public void BakeVertIdxToUv()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.hideFlags = HideFlags.DontSave;
        var uv = new List<Vector2>();
        var count = mesh.vertexCount;
        for (var i = 0; i < count; i++)
            uv.Add(new Vector2(i + 0.5f, 0) / Mathf.NextPowerOfTwo(count));
        mesh.SetUVs(channel, uv);
        SaveMesh(mesh, "NewInstance", false, false);
    }
    public static void SaveMeshInPlace(MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = mf.sharedMesh;
        SaveMesh(m, m.name, false, true);
    }

    public static void SaveMeshNewInstanceItem(MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = mf.sharedMesh;
        SaveMesh(m, m.name, true, true);
    }

    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }
}
