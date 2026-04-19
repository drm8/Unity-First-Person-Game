using UnityEngine;

public class LittleGuyUIManager : MonoBehaviour
{
    private Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID)[0].transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate towards player
		transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
    }
}
