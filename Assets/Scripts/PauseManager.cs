using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;

    private PlayerController player;
    private InputAction pauseAction;

    [SerializeField] private GameObject menu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID)[0];
        pauseAction = InputSystem.actions.FindAction("Pause");
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseAction.WasPerformedThisFrame())
        {
            isPaused = !isPaused;

            if (isPaused) Pause();
            else Unpause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;

        EventSystem.current.SetSelectedGameObject(null); // Fixes buttons getting stuck as selected.

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        player.OnPause();
        menu.SetActive(true);
    }

    public void Unpause()
    {
        Time.timeScale = 1f;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        player.OnUnpause();
        menu.SetActive(false);
    }
}
