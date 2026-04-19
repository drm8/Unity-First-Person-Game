using UnityEngine;

public class Billboard : MonoBehaviour
{
    private PlayerCamera playerCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCam = FindObjectsByType<PlayerCamera>(FindObjectsSortMode.InstanceID)[0];
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(playerCam.transform);
    }
}
