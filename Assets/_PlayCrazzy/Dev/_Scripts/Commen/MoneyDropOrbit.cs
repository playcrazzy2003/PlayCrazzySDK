using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyDropOrbit : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public float orbitRadius = 5f;
    public float yPositionOffset = 2f;
    public float maxDistance = 20f;
    public float minXRotation = 40f;
    public float maxXRotation = -40f;

    ParticleSystem moneyDrop;
    private void Start()
    {
        moneyDrop = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (player == null || target == null)
        {
            if (moneyDrop.isPlaying)
            {
                moneyDrop.Stop();
            }
            return;
        }

        if (!moneyDrop.isPlaying)
        {
            moneyDrop.Play();
        }
        // --- Direction to target (only horizontal) ---
        Vector3 dirToTarget = (target.position - player.position).normalized;
        dirToTarget.y = 0f; // Keep orbit on horizontal plane

        // --- Calculate orbit position with Y offset ---
        Vector3 orbitPosition = player.position + dirToTarget * orbitRadius;
        orbitPosition.y += yPositionOffset;

        transform.position = orbitPosition;

        // --- Look at the target ---
        Vector3 lookDirection = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        float targetY = lookRotation.eulerAngles.y;

        // --- X Rotation based on distance ---
        float distance = Vector3.Distance(player.position, target.position);
        distance = Mathf.Clamp(distance, 0f, maxDistance);
        float t = distance / maxDistance;
        float mappedX = Mathf.Lerp(minXRotation, maxXRotation, t);

        // --- Apply rotation ---
        transform.rotation = Quaternion.Euler(mappedX, targetY, 0f);
    }
}

