using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
	[SerializeField]
	private float moveSpeed = 10f;

	[SerializeField]
	private float friction = 1f;

	[SerializeField]
	private float lookLimitV = 70f;

	[SerializeField]
	private float lookSensitivity = 0.1f;

	[SerializeField]
	private float minJumpStrength = 10;

	[SerializeField]
	private float addtionalJumpStrength = 10;

	[SerializeField]
	private float maxJumpTime = 0.5f;

	[SerializeField]
	private float gravity = 10;

	[SerializeField]
	private float groundedRadius = 0.5f;

	[SerializeField]
	private float groundCheckDistance = 1.1f;

	[SerializeField]
	private float groundedCooldownDuration = 0.1f;
	private float groundedCooldown = 0;

	private bool grounded = true;
	private float jumpTimeRemaining = 0;

	[SerializeField]
	private float coyoteDuration = 0.1f;
	private float coyoteTime;

	[SerializeField]
	private float jumpBufferDuration = 0.1f;
	private float jumpBufferTime;

	private Vector3 velocity = new Vector3(0, 0, 0);

	private InputAction moveAction;
	private InputAction lookAction;
	private InputAction jumpAction;

	private Camera cam;

	private Rigidbody rb;

	private void Awake()
	{
		// Turning v-sync on. I don't feel like making an entire script for this right now.
		QualitySettings.vSyncCount = 1;

		UnityEngine.Cursor.lockState = CursorLockMode.Locked;
		UnityEngine.Cursor.visible = false;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		moveAction = InputSystem.actions.FindAction("Move");
		lookAction = InputSystem.actions.FindAction("Look");
		jumpAction = InputSystem.actions.FindAction("Jump");

		cam = GetComponentInChildren<Camera>();
		rb = GetComponent<Rigidbody>();

		coyoteTime = coyoteDuration;
		jumpBufferTime = jumpBufferDuration;
	}

	// Update is called once per frame
	void Update()
	{
		// Rotation
		Vector2 lookDirection = lookAction.ReadValue<Vector2>();
		float currentRotation = cam.transform.rotation.eulerAngles.x;
		float newX = currentRotation - lookDirection.y * lookSensitivity;
		if (currentRotation < lookLimitV && newX > lookLimitV) newX = lookLimitV;
		if (currentRotation > 360 - lookLimitV && newX < 360 - lookLimitV) newX = 360 - lookLimitV;
		cam.transform.localRotation = Quaternion.Euler(new Vector3(newX, 0, 0));

		float newY = transform.rotation.eulerAngles.y + lookDirection.x * lookSensitivity;
		transform.rotation = Quaternion.Euler(new Vector3(0, newY, 0));

		// Flat motion
		Vector3 moveDirection = moveAction.ReadValue<Vector2>().normalized;
		moveDirection = transform.forward * moveDirection.y + transform.right * moveDirection.x;
		rb.AddForce(new Vector3(moveDirection.x, 0, moveDirection.z) * moveSpeed * Time.deltaTime);

		// Ground check
		if (groundedCooldown > 0)
		{
			grounded = false;
		}
		else
		{
			RaycastHit hit; // I don't know how to call SphereCast with a maxDistance without a hitInfo param.
			grounded = Physics.SphereCast(transform.position, groundedRadius, -transform.up, out hit, groundCheckDistance);
			Debug.Log(hit.distance);
		}

		// Jumping
		if (grounded) coyoteTime = coyoteDuration;
		else if (coyoteTime > 0) coyoteTime -= Time.deltaTime;

		if (jumpAction.WasPressedThisFrame()) jumpBufferTime = jumpBufferDuration;
		else if (jumpBufferTime > 0) jumpBufferTime -= Time.deltaTime;

		if (groundedCooldown > 0) groundedCooldown -= Time.deltaTime;

		if (jumpAction.IsPressed())
		{
			if ( jumpTimeRemaining <= 0)//jumpBufferTime > 0 && coyoteTime > 0 &&
			{
				rb.AddForce(Vector3.up * minJumpStrength);
				jumpTimeRemaining = maxJumpTime;

				groundedCooldown = groundedCooldownDuration;
				jumpBufferTime = 0;
				coyoteTime = 0;
				grounded = false;
			}
			else if (jumpTimeRemaining > 0)
			{
				jumpTimeRemaining -= Time.deltaTime;
				rb.AddForce(Vector3.up * addtionalJumpStrength * (jumpTimeRemaining / maxJumpTime) * Time.deltaTime);
			}
		}
		else
		{
			jumpTimeRemaining = 0;
		}

		// Friction
		Vector3 force = rb.GetAccumulatedForce();
		rb.AddForce(force / (1 + friction * Time.deltaTime) - force);
	}
}
