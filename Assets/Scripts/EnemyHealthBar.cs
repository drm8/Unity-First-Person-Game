using UnityEngine;

public class EnemyHealthBar : HealthBar
{
    [SerializeField] private float stayDuration = 2;
    private float stayTimeLeft = 0;
    private bool visible;
    private float visibleDelta = 0;
    [SerializeField] private float appearSpeed = 3;
    private float distanceToCamera;
    [SerializeField] private float hideWithinDistanceStart = 1.5f;
    [SerializeField] private float hideWithinDistanceEnd = 0.5f;

    private Hitable enemy;
    private Transform playerCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetCropMask();
        transform.localScale = Vector3.zero;

        enemy = GetComponentInParent<Hitable>();
        playerCam = FindObjectsByType<PlayerCamera>(FindObjectsSortMode.InstanceID)[0].transform;
    }

    // Update is called once per frame
    new void Update()
    {
        if (GetHealth() < previousHealth)
        {
            visible = true;
            stayTimeLeft = stayDuration;
        }

        base.Update();

        float scale = 0;
        if (visible)
        {
            if (visibleDelta < 1)
            {
                // Appear
                visibleDelta += Time.deltaTime * appearSpeed;
                if (visibleDelta >= 1)
                {
                    visibleDelta = 1;
                    scale = 1;
                }
                else scale = GetScale(visibleDelta);
            }
            else
            {
                // Stay
                scale = 1;
                stayTimeLeft -= Time.deltaTime;
                if (stayTimeLeft <= 0) visible = false;
            }
        }
        else if (visibleDelta > 0)
        {
            // Disappear
            visibleDelta -= Time.deltaTime * appearSpeed;
            if (visibleDelta <= 0)
            {
                visibleDelta = 0;
                transform.localScale = Vector3.zero;
            }
            else scale = GetScale(visibleDelta);
        }

        if (scale > 0)
        {
            distanceToCamera = Vector3.Distance(playerCam.position, transform.position);
            if (distanceToCamera < hideWithinDistanceStart)
            {
                scale *= (distanceToCamera - hideWithinDistanceEnd) / (hideWithinDistanceStart - hideWithinDistanceEnd);
            }
        }

        transform.localScale = Vector3.one * scale;
    }

    private float GetScale(float delta)
    {
        return Mathf.Pow(Mathf.Sin(0.6f * Mathf.PI * delta), 4) / 0.8f;
    }

    protected override float GetHealth() { return enemy.GetHealth(); }
}
