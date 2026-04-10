using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCamera : MonoBehaviour
{
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
        
    }

    // Update is called once per frame
    void Update()
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
            float progress = 1 - timeLeft/duration;
            return Vector3.Lerp(base_, Vector3.zero, progress*progress);
        }

        public bool IsDone()
        {
            return timeLeft == 0;
        }
    }
}
