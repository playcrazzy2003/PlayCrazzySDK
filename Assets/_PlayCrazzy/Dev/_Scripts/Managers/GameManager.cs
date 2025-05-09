using UnityEngine;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Space(3)]
    [Header("Public Obj References")]
    public GameObject moneyBreak;
    
    [Space(3)]
    [Header("References")]
    [SerializeField] Camera activeCamera;
    [SerializeField] ItemsData itemData; 
    
    public PlayerController playerController { get; private set; }
    public UiManager uiManager { get; private set; }
    public CameraController cameraController { get; private set; }
    public EconomyManager economyManager { get; private set; }
    public BaseUnlockManager baseUnlockManager { get; private set; }
    public WorkerManager workerManager { get; private set; }

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
        baseUnlockManager = FindObjectOfType<BaseUnlockManager>();
        workerManager = FindObjectOfType<WorkerManager>();
        
        itemData.SetAllVelus();
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

    public void SetObjectsStates(GameObject[] objects, bool state)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = objects[i];
            if (obj != null)
            {
                obj.SetActive(!state);
                if (state)
                {
                    Vector3 startSCALE = obj.transform.localScale;
                    obj.transform.localScale = Vector3.zero;
                    obj.transform.DOScale(startSCALE, .8f).SetEase(Ease.OutBounce);
                }

                obj.SetActive(state);
            }
        }
    }

    public void PlayParticles(ParticleSystem[] particles)
    {
        foreach (var particle in particles)
        {
            particle.Play();
        }
    }

    public void SetObjectsState(GameObject objects, bool state)
    {
        if (objects.activeInHierarchy != state)
            objects.SetActive(state);
    }

    public void PlayParticles(ParticleSystem particles)
    {
        particles.Play();
    }
}
