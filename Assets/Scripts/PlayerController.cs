using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private float moveSpeed = 10f;
	private bool moving = false;

	[SerializeField]
	private float speedBoostDecay = 1;
	[SerializeField]
	private float speedBoostStoppedDecay = 4;
	private float speedBoost = 0;

	[SerializeField]
	private float minSquareSpeed = 0.01f;

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
	private float maxFallSpeed = -15;

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
	private float jumpSpeedBoost = 10f; // Speed boost when jumping
	[SerializeField]
	private float bounceWindow = 0.1f; // The window of time after hitting the ground for the player to bounce
	[SerializeField]
	private float bounceVelocityCut = 11; // How much speed is cut from the previous fall speed when determining bounce speed boost
	[SerializeField]
	private float bounceSpeedMultiplier = 0.75f; // Speed boost proportional to velocity.y when jumping right after landing
	private float timeSinceLanded;
	private float previousFallSpeed = 0;

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

	[SerializeField]
	float aimAssistPerMeter = 0.05f;

	[SerializeField]
	ParticleSystem enemyHitParticles;
	[SerializeField]
	ParticleSystem floorHitParticles;

	EnemyManager enemies;

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
		coyoteTime = coyoteDuration;
		timeSinceLanded = bounceWindow;
		jumpBufferTime = jumpBufferDuration;

		moveAction = InputSystem.actions.FindAction("Move");
		lookAction = InputSystem.actions.FindAction("Look");
		jumpAction = InputSystem.actions.FindAction("Jump");
		attackAction = InputSystem.actions.FindAction("Attack");

		enemies = FindObjectsByType<EnemyManager>(FindObjectsSortMode.InstanceID)[0];

		cam = GetComponentInChildren<Camera>();
		rb = GetComponent<Rigidbody>();
		camScript = GetComponentInChildren<PlayerCamera>();
	}

	// Update is called once per frame
	void Update()
	{
		// Movement
		Rotate();
		FlatMotion();
		Jump();
		//MovementWrapup();

		// Misc
		Shoot();
	}

	private void FixedUpdate()
	{
		MovementWrapup();
	}

	public void AddForce(Vector3 force)
	{
		velocity += force;
	}

	public Vector3 GetVelocity()
	{
		return velocity;
	}

	private void Rotate()
	{
		// Camera rotation (up and down)
		Vector2 lookDirection = lookAction.ReadValue<Vector2>();
		float currentRotation = cam.transform.rotation.eulerAngles.x;
		float newX = currentRotation - lookDirection.y * lookSensitivity;
		if (currentRotation <= 90 && newX > lookLimitV) newX = lookLimitV;
		if (currentRotation >= 180 && newX < 360 - lookLimitV) newX = 360 - lookLimitV;
		cam.transform.localRotation = Quaternion.Euler(new Vector3(newX, 0, 0));

		// Player rotation (left and right)
		float newY = transform.rotation.eulerAngles.y + lookDirection.x * lookSensitivity;
		transform.rotation = Quaternion.Euler(new Vector3(0, newY, 0));
	}

	private void FlatMotion()
	{
		MoveBySpeed(moveSpeed * Time.deltaTime);
	}

	private void MoveBySpeed(float speed)
	{
		Vector3 moveDirection = moveAction.ReadValue<Vector2>().normalized;
		moving = !moveDirection.Equals(Vector3.zero);
		moveDirection = transform.forward * moveDirection.y + transform.right * moveDirection.x;
		velocity += new Vector3(moveDirection.x, 0, moveDirection.z) * speed;
	}

	private void Jump()
	{
		// Ground check
		if (groundedCooldown > 0)
		{
			grounded = false;
		}
		else
		{
			bool preGrounded = grounded;
			RaycastHit hit; // I don't know how to call SphereCast with a maxDistance without a hitInfo param.
			float checkDistance = grounded ? groundCheckDistance + 0.05f : groundCheckDistance;
			grounded = Physics.SphereCast(transform.position, groundedRadius, -transform.up, out hit, checkDistance);

			if (grounded && !preGrounded)
			{
				camScript.Dip(velocity.y);
				previousFallSpeed = Mathf.Abs(velocity.y);
				timeSinceLanded = 0;
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
				// Y velocity
				if (velocity.y < 0) velocity.y = 0;
				velocity.y += minJumpStrength;

				// XZ velocity
				speedBoost += jumpSpeedBoost;
				if (timeSinceLanded < bounceWindow) speedBoost += Mathf.Max(0, previousFallSpeed-bounceVelocityCut) * bounceSpeedMultiplier;

				// Everything else or something
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
	}

	public float GetFallSpeed()
	{
		if (grounded && timeSinceLanded < bounceWindow) return -previousFallSpeed;
		else return velocity.y;
	}

	private void MovementWrapup()
	{
		rb.linearVelocity = new Vector3(velocity.x * (1 + speedBoost), velocity.y, velocity.z * (1 + speedBoost));
		Debug.Log(speedBoost);

		speedBoost /= 1 + (moving ? speedBoostDecay : speedBoostStoppedDecay) * Time.deltaTime;
		if (speedBoost < 0.01) speedBoost = 0;
		float frameFriction = 1 + friction * Time.deltaTime;
		velocity.x /= frameFriction;
		velocity.z /= frameFriction;
		if (!moving && velocity.x * velocity.x + velocity.z * velocity.z < minSquareSpeed)
		{
			velocity.x = 0;
			velocity.z = 0;
		}

		if (grounded && velocity.y < 0) velocity.y = -0.5f;
		else velocity.y -= gravity * Time.deltaTime;
		if (velocity.y < maxFallSpeed) velocity.y = maxFallSpeed;

		if (groundedCooldown > 0) groundedCooldown -= Time.deltaTime;
		timeSinceLanded += Time.deltaTime;
	}

	private void Shoot()
	{
		if (attackAction.WasPressedThisFrame()) // Has the shoot button been pressed?
		{
			RaycastHit hit;
			bool hasHit;

			// Aim assisted hit check
			Vector3 direction = cam.transform.forward;
			foreach (Transform enemy in enemies.GetEnemyList())
			{
				Vector3 positionDifference = enemy.position - cam.transform.position;
				float distance = positionDifference.magnitude;

				// Is the enemy close enough?
				if (distance > shootDistance) continue;

				// Is the angle close enough?
				Vector3 flattenedDirection = new Vector3(direction.x, direction.y / 2, direction.z);
				Vector3 flattenedPositionDifference = new Vector3(positionDifference.x, positionDifference.y / 2, positionDifference.z);
				float angleDisparity = Vector3.Distance(flattenedPositionDifference, Vector3.Project(flattenedPositionDifference, flattenedDirection));
				if (angleDisparity > 0.75 + distance * aimAssistPerMeter) continue;

				// Is there line of sight?
				
				hasHit = Physics.Raycast(cam.transform.position, positionDifference, out hit, distance);
				if (hasHit && hit.collider.transform == enemy)
				{
					enemy.GetComponentInParent<Hitable>().Hit();
					Quaternion particleRotation = Quaternion.FromToRotation(Vector3.up, Vector3.Reflect(cam.transform.forward, hit.normal));
					Instantiate(enemyHitParticles, hit.point, particleRotation);
					return;
				}
			}

			// Wall hit check
			hasHit = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, shootDistance);
			if (hasHit)
			{
				Quaternion particleRotation = Quaternion.FromToRotation(Vector3.up, Vector3.Reflect(cam.transform.forward, hit.normal));
				Instantiate(floorHitParticles, hit.point, particleRotation);
			}
		}
	}
}