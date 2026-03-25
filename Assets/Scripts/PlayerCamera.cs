using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float dipBaseStrength = 0.25f;

    [SerializeField]
	private float dipSpeed = 3;

    private float dipCurrentStrength;
	private float dipDelta = -1;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dipDelta != -1)
        {
            dipDelta += Time.deltaTime * dipSpeed;

            if (dipDelta >= 1)
            {
				dipDelta = -1;
                transform.localPosition = new Vector3(0, 0, 0);
			}

			transform.localPosition = new Vector3(0, -Mathf.Sin(dipDelta * Mathf.PI) * dipCurrentStrength, 0);
        }
    }

    public void Dip(float strength)
    {
        dipDelta = 0;
        dipCurrentStrength = strength * dipBaseStrength;
	}
}
