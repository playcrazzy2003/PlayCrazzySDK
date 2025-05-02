using System;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static Action<Transform, float, float, bool, Action> OnMoveToTarget;

    private GameManager gameManager;
    private PlayerController playerController;

    [Header("Camera Movement Settings")] [SerializeField]
    private float minMoveTime = 1f;

    [SerializeField] private float maxMoveTime = 3f;
    [SerializeField] private float maxDistance = 50f;

    private void OnEnable()
    {
        gameManager = GameManager.Instance;
        playerController = gameManager.playerController;
        OnMoveToTarget += MoveToTarget;
    }

    private void OnDisable()
    {
        OnMoveToTarget -= MoveToTarget;
    }

    private void MoveToTarget(Transform target, float overrideMoveTime, float stayTime, bool returnToPlayer = true,
        Action onComplete = null)
    {
        if (!gameManager.IsObjectOffScreen(target))
        {
            onComplete?.Invoke();
            return;
        }

        float moveTime = (overrideMoveTime <= 0f) ? CalculateMoveTime(target.position) : overrideMoveTime;

        transform.parent = null;
        gameManager.PausePlayerControl();

        transform.DOMove(target.position, moveTime).OnComplete(() =>
        {
            onComplete?.Invoke();

            if (returnToPlayer)
            {
                DOVirtual.DelayedCall(stayTime, () =>
                {
                    MoveToTarget(playerController.transform, moveTime, stayTime, false, () =>
                    {
                        transform.parent = playerController.transform;
                        transform.localPosition = Vector3.zero;
                        gameManager.ResumePlayerControl();
                    });
                });
            }
        });
    }

    private float CalculateMoveTime(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        float t = Mathf.InverseLerp(0f, maxDistance, distance);
        return Mathf.Lerp(minMoveTime, maxMoveTime, t);
    }
}