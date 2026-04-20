using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;

    [SerializeField] private float minOffset = 0.1f;
    [SerializeField] private float maxOffset = 0.4f;
    [SerializeField] private float minXZMagnitude = 2f;
    [SerializeField] private float maxXZMagnitude = 6f;
    [SerializeField] private float minYMagnitude = 4f;
    [SerializeField] private float maxYMagnitude = 10f;

    [SerializeField] private float minDuration = 0.35f;
    [SerializeField] private float maxDuration = 0.7f;
    private float duration;
    private float timeLeft;

    [SerializeField] private float baseSize = 0.8f;
    [SerializeField] private float damageSizeMultiplier = 0.25f;
    private float size = 1;

    public void SetNumber(float value)
    {
        damageText.text = "" + (int)(value * 10);
        size = baseSize + Mathf.Sqrt(value) * damageSizeMultiplier;
        transform.localScale = Vector3.one * size;
    }

    private Vector3 Velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float random = Random.Range(0, 2*Mathf.PI);
        Vector2 direction = new Vector2(Mathf.Cos(random), Mathf.Sin(random));
        Vector2 positionOffset = direction * Random.Range(minOffset, maxOffset);
        Vector2 horizontalMagnitude = direction * Random.Range(minXZMagnitude, maxXZMagnitude);
        transform.position += new Vector3(positionOffset.x, 0.5f, positionOffset.y);
        Velocity = new Vector3(horizontalMagnitude.x, Random.Range(minYMagnitude, maxYMagnitude), horizontalMagnitude.y);

        duration = Random.Range(minDuration, maxDuration);
        timeLeft = duration;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            transform.localScale = Vector3.one * size * Mathf.Sqrt(timeLeft / duration);
            transform.position += Velocity * Mathf.Sqrt(timeLeft / duration) * Time.deltaTime;
        }
    }
}
