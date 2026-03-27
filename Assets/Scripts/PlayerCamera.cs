using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	[SerializeField]
	private float dipAccel = 2;

    [SerializeField]
	private float dipDrag = 10;

	[SerializeField]
    private float dipEndTolerance = -0.05f;

    private float dipVel = 0;
    private bool dipActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dipActive)
        {
            dipVel += dipAccel * Time.deltaTime;
            dipVel /= 1 + dipDrag * Time.deltaTime;
			transform.localPosition = new Vector3(0, transform.localPosition.y + dipVel * Time.deltaTime, 0);

            if (transform.localPosition.y > dipEndTolerance && dipVel > 0)
            {
				transform.localPosition = new Vector3(0, 0, 0);
                dipActive = false;
				dipVel = 0;
			}
		}
    }

    public void Dip(float velocity)
    {
        dipVel = velocity;
        dipActive = true;
	}
}
