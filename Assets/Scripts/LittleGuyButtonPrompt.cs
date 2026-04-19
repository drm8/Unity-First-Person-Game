using UnityEngine;

public class LittleGuyButtonPrompt : MonoBehaviour
{
    private Vector3 initialScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialScale = transform.localScale;
        transform.localScale = Vector3.one * 0;
    }

    public void show()
    {
        transform.localScale = initialScale;
    }

    public void hide()
    {
        transform.localScale = Vector3.one * 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
