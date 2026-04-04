using UnityEngine;

public class textRender : MonoBehaviour
{

    public TMPro.TextMeshPro text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.fontMaterial = new Material(text.fontMaterial);
        text.fontMaterial.renderQueue = 4000;
    }
}
