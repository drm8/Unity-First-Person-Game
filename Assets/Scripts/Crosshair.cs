using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private float activeOpacity = 0.75f;
	[SerializeField] private float inactiveOpacity = 0.25f;
    [SerializeField] private float fadeSpeed = 0.1f;

    private Image image;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		image = GetComponent<Image>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SetActive(bool active)
	{
		float opacity = active ? activeOpacity : inactiveOpacity;
		image.CrossFadeAlpha(opacity, fadeSpeed, true);
	}
}
