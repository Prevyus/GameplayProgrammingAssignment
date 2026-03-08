using UnityEngine;
using System.Collections.Generic;

public class CharacterModel : MonoBehaviour
{// SETS THE COLOR OF THE CHARACTER MODEL

    [Header("Model")]
    [SerializeField] bool useSkinnedMesh = false;
    [SerializeField] List<Renderer> BoxMeshRenderers = new List<Renderer>();
    [SerializeField] List<Renderer> SkinnedMeshRenderers = new List<Renderer>();
    [SerializeField] List<Renderer> CamShow = new List<Renderer>();
    [SerializeField] List<Color> colors = new List<Color>();

    private Color MeshesColor;

    private void Start()
    {
        if (colors.Count > 0)
        {
            int colorNum = Random.Range(0, colors.Count);
            Color randColor = colors[colorNum];
            //Color color = new Color(Random.Range(160, 255), randColor.g, randColor.b);
            MeshesColor = randColor;
        }

        ApplyColors();

        foreach (Renderer mesh in SkinnedMeshRenderers)
        {
            mesh.enabled = useSkinnedMesh;
        }
        foreach (Renderer mesh in BoxMeshRenderers)
        {
            mesh.enabled = !useSkinnedMesh;
        }
    }

    void OnBodyColorChanged(Color oldValue, Color newValue)
    {
        foreach (Renderer mesh in (useSkinnedMesh ? SkinnedMeshRenderers : BoxMeshRenderers))
        {
            if (mesh)
            {
                foreach (Material mat in mesh.materials)
                {
                    mat.color = newValue;
                }
            }
        }
        foreach (Renderer mesh in CamShow)
        {
            if (mesh)
            {
                foreach (Material mat in mesh.materials)
                {
                    mat.color = newValue;
                }
            }
        }
    }

    void ApplyColors()
    {
        foreach (Renderer mesh in (useSkinnedMesh ? SkinnedMeshRenderers : BoxMeshRenderers))
        {
            if (mesh) mesh.material.color = MeshesColor;
        }
        foreach (Renderer mesh in CamShow)
        {
            if (mesh) mesh.material.color = MeshesColor;
        }
    }
}
