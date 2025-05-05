using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera activeCamera;
    
    
    public static GameManager Instance;
    public PlayerController playerController { get; private set; }
    public UiManager uiManager { get; private set; }
    public CameraController cameraController { get; private set; }
    public EconomyManager economyManager { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (activeCamera == null)
        {
            activeCamera = Camera.main;
        }

        Initializetion();
    }

    void Initializetion()
    {
        uiManager = FindObjectOfType<UiManager>();
        economyManager = FindObjectOfType<EconomyManager>();
        playerController = FindObjectOfType<PlayerController>();
        cameraController = FindObjectOfType<CameraController>();
    }
    
    
    public bool IsObjectOffScreen(Transform obj)
    {
        Vector3 viewportPos = activeCamera.WorldToViewportPoint(obj.position);
        return viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;
    }
    
    public void PausePlayerControl()
    {
        playerController.playerControllerData.characterMovement.enabled = false;
        playerController.enabled = false;
        playerController.animationController.PlayAnimation(AnimType.Idle, 0);
    }

    public void ResumePlayerControl()
    {
        playerController.playerControllerData.characterMovement.enabled = true;
        playerController.enabled = true;
        playerController.animationController.PlayAnimation(AnimType.Idle, 0);
    }
}
