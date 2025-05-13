using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCModeler : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform visual;

    [Header("Push Settings")] public float pushSpeed = 5f;
    public float maxElasticDistance = 1.5f;
    public float returnSpeed = 4f;

    [Header("Wobble Settings")] public float wobbleSpeed = 8f;
    public float wobbleAmount = 10f;

    private Vector3 originalLocalPos;
    private bool isPushed = false;
    private float wobbleTime = 0f;

    private void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!visual) visual = transform.GetChild(0);
        originalLocalPos = visual.localPosition;
    }

    private void Update()
    {
        // Lock Y movement of the visual
        Vector3 pos = visual.localPosition;
        pos.y = 0f;
        visual.localPosition = pos;

        if (!isPushed)
        {
            // Smoothly return to original local position
            Vector3 target = new Vector3(originalLocalPos.x, 0f, originalLocalPos.z);
            visual.localPosition = Vector3.Lerp(visual.localPosition, target, Time.deltaTime * returnSpeed);

            // If far enough from center, wobble rotation on Y
            float dist = Vector3.Distance(visual.localPosition, target);
            if (dist > 0.01f)
            {
                wobbleTime += Time.deltaTime * wobbleSpeed;
                float yRotation = Mathf.Sin(wobbleTime) * wobbleAmount;
                visual.localRotation = Quaternion.Euler(0f, yRotation, 0f);
            }
            else
            {
                visual.localRotation = Quaternion.identity;
                wobbleTime = 0f;
            }
        }
        else
        {
            // Reset wobble when being pushed
            visual.localRotation = Quaternion.identity;
            wobbleTime = 0f;
        }

        isPushed = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPushed = true;

        // Direction away from player (XZ only)
        Vector3 dir = (visual.position - other.transform.position);
        dir.y = 0f;
        dir.Normalize();

        // Apply push with limit
        Vector3 newOffset = visual.localPosition + dir * Time.deltaTime * pushSpeed;
        if (Vector3.Distance(newOffset, originalLocalPos) < maxElasticDistance)
        {
            visual.localPosition = newOffset;
        }

        // Make visual rotate toward its original (parent) position
        Vector3 lookDir = (transform.position - visual.position).normalized;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);
            visual.rotation = lookRotation;
        }
    }

    [Header("Movement Settings")] public float defaultStoppingDistance = 0.5f;
    
    public void MoveTo(Vector3 target, Action onArrive = null, float? customStoppingDistance = null)
    {
        float stoppingDistanceToUse = customStoppingDistance ?? defaultStoppingDistance;

        agent.stoppingDistance = stoppingDistanceToUse;
        agent.SetDestination(target);
        StartCoroutine(CheckArrivalCoroutine(onArrive));
    }

    private IEnumerator CheckArrivalCoroutine(Action onArrive)
    {
        // Wait until path is fully calculated
        while (agent.pathPending)
            yield return null;

        // Wait until agent reaches close enough to the destination
        while (agent.remainingDistance > agent.stoppingDistance || agent.hasPath == false)
        {
            yield return null;
        }

        Debug.Log($"Arrived: remainingDistance = {agent.remainingDistance}, stoppingDistance = {agent.stoppingDistance}");

        onArrive?.Invoke();
    }
}