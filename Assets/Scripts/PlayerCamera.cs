using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
	[SerializeField]
	private float lookLimitV = 70f;

	[SerializeField]
	private float lookSensitivity = 0.1f;

	private Camera cam;

	private InputAction lookAction;

	[SerializeField]
	private float dipAccel = 2;

    [SerializeField]
	private float dipDrag = 10;

	[SerializeField]
    private float dipEndTolerance = -0.05f;

    private float dipY = 0;
    private float dipVel = 0;
    private bool dipActive = false;

    [SerializeField]
    private float defaultOffsetDuration = 1;
    private List<Offset> offsets = new List<Offset>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		lookAction = InputSystem.actions.FindAction("Look");

		cam = GetComponentInChildren<Camera>();
	}

    // Update is called once per frame
    void Update() {}

    void LateUpdate()
    {
		Rotate();
		UpdatePosition();
	}

	private void Rotate()
	{
		// Camera rotation (up and down)
		Vector2 lookDirection = lookAction.ReadValue<Vector2>();
		float currentRotation = cam.transform.rotation.eulerAngles.x;
		float newX = currentRotation - lookDirection.y * lookSensitivity;
		if (currentRotation <= 90 && newX > lookLimitV) newX = lookLimitV;
		if (currentRotation >= 180 && newX < 360 - lookLimitV) newX = 360 - lookLimitV;

		// Camera rotation (left and right)
		float newY = cam.transform.rotation.eulerAngles.y + lookDirection.x * lookSensitivity;
		cam.transform.rotation = Quaternion.Euler(new Vector3(newX, newY, 0));
	}

    private void UpdatePosition()
    {
		// Initial offset
		DipUpdate();
		Vector3 currentOffset = Vector3.up * dipY;

		// Adding from offsets list
		for (int i = offsets.Count - 1; i >= 0; i--)
		{
			currentOffset += offsets[i].Update();
			if (offsets[i].IsDone()) offsets.RemoveAt(i);
		}

		// Applying final offset
		if (!transform.localPosition.Equals(Vector3.zero) || !currentOffset.Equals(Vector3.zero))
		{
			transform.position = transform.parent.position + currentOffset;
		}
	}

	public void UpdatePositionEarly()
    {
		// Initial offset
		DipUpdate();
		Vector3 currentOffset = Vector3.up * dipY;

		// Adding from offsets list
		for (int i = offsets.Count - 1; i >= 0; i--)
		{
			currentOffset += offsets[i].GetCurrent();
			if (offsets[i].IsDone()) offsets.RemoveAt(i);
		}

		// Applying final offset
		if (!transform.localPosition.Equals(currentOffset))
		{
			transform.localPosition = currentOffset;
		}
	}


	private void DipUpdate()
    {
        if (dipActive)
        {
            dipVel += dipAccel * Time.deltaTime;
            dipVel /= 1 + dipDrag * Time.deltaTime;
            dipY = dipY + dipVel * Time.deltaTime;

            if (dipY > dipEndTolerance && dipVel > 0)
            {
                dipActive = false;
				dipVel = 0;
                dipY = 0;
			}
		}
    }

    public void Dip(float velocity)
    {
        dipVel = velocity;
        dipActive = true;
	}

    public void Shift(Vector3 offsetVector)
    {
        offsets.Add(new Offset(offsetVector, defaultOffsetDuration));
    }
    public void Shift(Vector3 offsetVector, float duration)
    {
        offsets.Add(new Offset(offsetVector, duration));
    }

    private class Offset
    {
        private Vector3 base_;
        private float duration;
        private float timeLeft;
        public Offset(Vector3 base_, float duration)
        {
            this.base_ = base_;
            this.duration = duration;
            timeLeft = duration;
        }

        public Vector3 Update()
        {
            timeLeft = Mathf.Max(0, timeLeft - Time.deltaTime);
            return GetCurrent();
        }

		public Vector3 GetCurrent()
		{
			float progress = timeLeft / duration;
			return Vector3.Lerp(base_, Vector3.zero, 1 - progress * progress);
		}

		public bool IsDone()
        {
            return timeLeft == 0;
        }
    }
}
