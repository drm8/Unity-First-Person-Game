using UnityEngine;
using UnityEngine.InputSystem;

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

	[SerializeField]
	private float headJumpDamageBase = 1f;
    [SerializeField]
    private float headJumpDamageVelocityMultiplier = 0.025f;
    [SerializeField]
	private float headJumpRadius = 0.85f;
	[SerializeField]
	private float headJumpScanDistance = 0.5f;
	[SerializeField]
	private float halfHeight = 1;

	[SerializeField]
	private float headJumpCooldownDuration = 0.1f;
	private float headJumpCooldown;

	private bool teleportFlag = false;
	private Vector3 teleportPosition;

	// I'm making this public as a little treat to myself, the accessor methods were getting a little excessive.
	public Vector3 velocity = new Vector3(0, 0, 0);

	private InputAction moveAction;
	private InputAction jumpAction;

	private PlayerCombat combat;
	private Camera cam;
	private PlayerCamera camScript;
	private Rigidbody rb;

	[SerializeField]
	private LayerMask defaultOnlyLayermask;

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
		jumpAction = InputSystem.actions.FindAction("Jump");

		combat = GetComponent<PlayerCombat>();
        cam = GetComponentInChildren<Camera>();
		camScript = GetComponentInChildren<PlayerCamera>();
        rb = GetComponent<Rigidbody>();
    }

    public void OnPause()
	{
        moveAction.Disable();
        jumpAction.Disable();
		combat.OnPause();
        camScript.OnPause();
    }
    public void OnUnpause() 
	{
        moveAction.Enable();
        jumpAction.Enable();
        combat.OnUnpause();
        camScript.OnUnpause();
    }

    // Update is called once per frame
    void Update()
	{
        MoveBySpeed(moveSpeed * Time.deltaTime);
        Jump();
	}

	private void FixedUpdate()
	{
		Teleport();
		MovementWrapup();
	}

	private void LateUpdate()
	{
		
	}

	private void Teleport()
	{
		if (teleportFlag)
		{
			teleportFlag = false;
			camScript.Shift(transform.position - teleportPosition);
			camScript.UpdatePositionEarly();
			transform.position = teleportPosition;
		}
	}

    public void MoveBySpeed(float speed)
	{
		Vector3 moveDirection = moveAction.ReadValue<Vector2>().normalized;
		moving = !moveDirection.Equals(Vector3.zero);
		moveDirection = GetForward() * moveDirection.y + GetRight() * moveDirection.x;
		velocity += new Vector3(moveDirection.x, 0, moveDirection.z) * speed;
	}

    public void AddSpeedBoost(float boost) { speedBoost += boost; }

    public float GetFallSpeed()
    {
        if (grounded && timeSinceLanded < bounceWindow) return -previousFallSpeed;
        else return velocity.y;
    }

    public float GetSpeed()
	{
		Vector2 flatVel = new Vector2(velocity.x, velocity.z) * (1 + speedBoost);
		return flatVel.magnitude;
    }

    private Vector3 GetForward()
    {
        Vector3 forward = cam.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetRight()
    {
        Vector3 right = cam.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void GroundCheck()
	{
		if (groundedCooldown > 0)
		{
			grounded = false;
		}
		else
		{
			bool preGrounded = grounded;
			RaycastHit hit; // I don't know how to call SphereCast with a maxDistance without a hitInfo param.
			float checkDistance = grounded ? groundCheckDistance + 0.05f : groundCheckDistance;
			grounded = Physics.SphereCast(transform.position, groundedRadius, -transform.up, out hit, checkDistance, defaultOnlyLayermask);

			if (grounded && !preGrounded)
			{
				camScript.Dip(velocity.y);
				previousFallSpeed = Mathf.Abs(velocity.y);
				timeSinceLanded = 0;
			}
		}
	}

	private Hitable GetHeadJumpEnemy()
	{
		if (headJumpCooldown > 0)
			return null;
        RaycastHit hit;
        if (!Physics.SphereCast(transform.position, headJumpScanDistance, -transform.up, out hit, headJumpRadius))
			return null;
		Hitable enemy = hit.transform.GetComponent<Hitable>();
		if (enemy == null || !enemy.GetJumpable())
			return null;
        return enemy;
    }

	private void Jump()
	{
		GroundCheck();

		if (grounded) coyoteTime = coyoteDuration;
		else if (coyoteTime > 0) coyoteTime -= Time.deltaTime;

		if (jumpAction.WasPressedThisFrame()) jumpBufferTime = jumpBufferDuration;
		else if (jumpBufferTime > 0) jumpBufferTime -= Time.deltaTime;

		if (jumpAction.IsPressed())
		{
			// Head jump check
			Hitable jumpedEnemy = GetHeadJumpEnemy();
            bool headJump = (jumpedEnemy != null);

			if (jumpBufferTime > 0 && (coyoteTime > 0 || headJump))
			{
                // Head jump
                if (headJump)
                {
					combat.ReplenishAmmo();

                    headJumpCooldown = headJumpCooldownDuration;
                    teleportFlag = true;
                    teleportPosition = GetHeadJumpPosition(jumpedEnemy);

					if (!grounded)
					{
						previousFallSpeed = Mathf.Abs(velocity.y);
						timeSinceLanded = 0;
					}

                    float sqrXZMagnitude = (velocity.x * velocity.x + velocity.z * velocity.z) * (1 + speedBoost);
					float headJumpMagnitude = sqrXZMagnitude + GetFallSpeed();
                    jumpedEnemy.Hit(headJumpDamageBase * (1 + headJumpMagnitude * headJumpDamageVelocityMultiplier), "jump");
                }

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

	private Vector3 GetHeadJumpPosition(Hitable enemy)
	{
        Vector3 center = enemy.HeadPosition() + Vector3.up * halfHeight;
        if (velocity.sqrMagnitude == 0) return center; // Zero velocity edge case

        if (Vector3.Angle(center - transform.position, velocity) >= 0)
		{
            // Teleport the player above the enemy's center.
            return center;
        }
		else
		{
            // If the player has passed the center of the enemy already, teleporting to the usual place would send the player
            // backwards, which feels bad. Instead, project onto the player's velocity with the usual place as an origin.
            return Vector3.Project(transform.position - center, velocity) + center;
        }
	}

	private void MovementWrapup()
	{
		rb.linearVelocity = new Vector3(velocity.x * (1 + speedBoost), velocity.y, velocity.z * (1 + speedBoost));

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

		if (headJumpCooldown > 0) headJumpCooldown -= Time.deltaTime;
		if (groundedCooldown > 0) groundedCooldown -= Time.deltaTime;
		timeSinceLanded += Time.deltaTime;
	}
}