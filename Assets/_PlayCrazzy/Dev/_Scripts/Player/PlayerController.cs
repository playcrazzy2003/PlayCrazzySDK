using EasyCharacterMovement;
using UnityEngine;

[System.Serializable]
public class PlayerControllerData
{
    public FloatingJoystick joystick;
    public float rotationRate = 540.0f;
    public float maxSpeed = 5f;
    public float customAngle = 45f;
    public float acceleration = 20.0f;
    public float deceleration = 20.0f;
    public float groundFriction = 8.0f;
    public float airFriction = 0.5f;
    [Range(0.0f, 1.0f)] public float airControl = 0.3f;
    public Vector3 gravity = Vector3.down * 9.81f;
    internal CharacterMovement characterMovement;
}

public class PlayerController : MonoBehaviour
{
    [Tooltip("Player DataRefs")] 
    public PlayerControllerData playerControllerData;
    [HideInInspector]
    public AnimationController animationController;


    //  Movement 
    [SaveableField] public float velocity;
    private float horizontal;
    private float vertical;
   [SaveableField] private Vector3 movementDirection;
    private Vector3 desiredVelocity;

    private void Awake()
    {
        Initializetion();
    }

    private void Initializetion()
    {
        playerControllerData.characterMovement = GetComponent<CharacterMovement>();
        animationController = GetComponentInChildren<AnimationController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    #region Movement

    public void HandleMovement()
    {
        horizontal = playerControllerData.joystick.Horizontal;
        vertical = playerControllerData.joystick.Vertical;
        //playerControllerData.isDragging = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
        movementDirection = Vector3.zero;
        movementDirection += Vector3.right * horizontal;
        movementDirection += Vector3.forward * vertical;

        movementDirection = Quaternion.AngleAxis(playerControllerData.customAngle, Vector3.up) * movementDirection;
        movementDirection = Vector3.ClampMagnitude(movementDirection, 1.0f);

        playerControllerData.characterMovement.RotateTowards(movementDirection,
            playerControllerData.rotationRate * Time.deltaTime * 2f);

        desiredVelocity = movementDirection * playerControllerData.maxSpeed;

        float actualAcceleration = playerControllerData.characterMovement.isGrounded
            ? playerControllerData.acceleration
            : playerControllerData.acceleration * playerControllerData.airControl;
        float actualDeceleration = playerControllerData.characterMovement.isGrounded
            ? playerControllerData.deceleration
            : 0.0f;

        float actualFriction = playerControllerData.characterMovement.isGrounded
            ? playerControllerData.groundFriction
            : playerControllerData.airFriction;

        playerControllerData.characterMovement.SimpleMove(desiredVelocity, playerControllerData.maxSpeed,
            actualAcceleration, actualDeceleration, actualFriction, actualFriction, playerControllerData.gravity);
    }

    public void FrizePlayer()
    {
        
    }
    public bool IsMoving()
    {
        return desiredVelocity.sqrMagnitude > 0.0001f;
    }
    public float GetVelocity()
    {
        velocity = new Vector2(playerControllerData.joystick.Horizontal, playerControllerData.joystick.Vertical)
            .magnitude;
        return velocity = Mathf.Clamp01(velocity);
    }

    #endregion

    #region Animation

    public void HandleAnimation()
    {
        if (animationController == null)
        {
            Debug.LogError("No animation controller found"); return;}
        if (IsMoving())
        {
            animationController.PlayAnimation(AnimType.Move);
            animationController.SetFloat("Velocity", GetVelocity());
        }
        else
        {
            animationController.PlayAnimation(AnimType.Idle);
        }
    }

    #endregion
}