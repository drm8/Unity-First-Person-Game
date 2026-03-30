using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
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
	private InputAction attackAction;

	[SerializeField]
	float shootDistance = 500;

	private Camera cam;

	private PlayerCamera camScript;

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
		attackAction = InputSystem.actions.FindAction("Attack");

		cam = GetComponentInChildren<Camera>();
		rb = GetComponent<Rigidbody>();
		camScript = GetComponentInChildren<PlayerCamera>();

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
		Debug.Log(newX + ", " + currentRotation + ", " + (newX > lookLimitV));
		if (currentRotation <= 90 && newX > lookLimitV) newX = lookLimitV;
		if (currentRotation >= 180 && newX < 360 - lookLimitV) newX = 360 - lookLimitV;
		cam.transform.localRotation = Quaternion.Euler(new Vector3(newX, 0, 0));

		float newY = transform.rotation.eulerAngles.y + lookDirection.x * lookSensitivity;
		transform.rotation = Quaternion.Euler(new Vector3(0, newY, 0));

		// Flat motion
		Vector3 moveDirection = moveAction.ReadValue<Vector2>().normalized;
		moveDirection = transform.forward * moveDirection.y + transform.right * moveDirection.x;
		velocity += new Vector3(moveDirection.x, 0, moveDirection.z) * moveSpeed * Time.deltaTime;

		// Ground check
		if (groundedCooldown > 0)
		{
			grounded = false;
		}
		else
		{
			bool preGrounded = grounded;
			RaycastHit hit; // I don't know how to call SphereCast with a maxDistance without a hitInfo param.
			grounded = Physics.SphereCast(transform.position, groundedRadius, -transform.up, out hit, groundCheckDistance);

			if (grounded && !preGrounded)
			{
				camScript.Dip(velocity.y);
			}
		}

		// Jumping
		if (grounded) coyoteTime = coyoteDuration;
		else if (coyoteTime > 0) coyoteTime -= Time.deltaTime;

		if (jumpAction.WasPressedThisFrame()) jumpBufferTime = jumpBufferDuration;
		else if (jumpBufferTime > 0) jumpBufferTime -= Time.deltaTime;

		if (jumpAction.IsPressed())
		{
			if (jumpBufferTime > 0 && coyoteTime > 0)
			{
				if (velocity.y < 0) velocity.y = 0;
				velocity.y += minJumpStrength;
				jumpTimeRemaining = maxJumpTime;

				groundedCooldown = groundedCooldownDuration;
				jumpBufferTime = 0;
				coyoteTime = 0;
				grounded = false;
			}
			else if (jumpTimeRemaining > 0)
			{
				jumpTimeRemaining -= Time.deltaTime;
				velocity.y += addtionalJumpStrength * (jumpTimeRemaining / maxJumpTime) * Time.deltaTime;
			}
		}
		else
		{
			jumpTimeRemaining = 0;
		}

		// Final movement logic
		rb.position += velocity * Time.deltaTime;

		float frameFriction = 1 + friction * Time.deltaTime;
		velocity.x /= frameFriction;
		velocity.z /= frameFriction;

		if (grounded && velocity.y < 0) velocity.y = -0.5f;
		else velocity.y -= gravity * Time.deltaTime;

		if (groundedCooldown > 0) groundedCooldown -= Time.deltaTime;

		// Shoot
		if (attackAction.WasPressedThisFrame())
		{
			RaycastHit hit;
			bool hasHit = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, shootDistance);
			if (hasHit)
			{
				if (hit.collider.CompareTag("Hitable"))
				{
					hit.collider.GetComponentInParent<Hitable>().Hit();
				}
			}	
		}
	}

	public void AddForce(Vector3 force)
	{
		velocity += force;
	}

	public Vector3 GetVelocity()
	{
		return velocity;
	}
}