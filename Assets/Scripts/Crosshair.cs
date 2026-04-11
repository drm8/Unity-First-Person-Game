using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private int activeOpacity = 192;
	[SerializeField] private int inactiveOpacity = 64;

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
		int opacity = active ? activeOpacity : inactiveOpacity;
		image.CrossFadeAlpha(opacity, 0.2f, true);
	}
}
