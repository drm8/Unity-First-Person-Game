using UnityEngine;

public class FixMaterialTiling : MonoBehaviour
{
    private float tileFactor = 0.25f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material material = new Material(meshRenderer.material);
        material.mainTextureScale = new Vector2(transform.localScale.x * tileFactor, transform.localScale.z * tileFactor);
        meshRenderer.material = material;
    }
}
