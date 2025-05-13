using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class WorkerBehaviour : MonoBehaviour
{
    [SerializeField] private int maxCapacity = 5;

    private WorkerManager manager;
    private ItemBatchHandler batchHandler;
    private ItemBatchStacker giver, receiver;
    private NPCModeler npcModeler;

    private int carried = 0;
    private Coroutine workRoutine;

    private void OnEnable()
    {
        manager = GameManager.Instance.workerManager;
        npcModeler = GetComponent<NPCModeler>();
        batchHandler = GetComponentInChildren<ItemBatchHandler>();

    }

    [Button("Start Working")]
    private void StartWorking()
    {
        if (workRoutine != null)
            StopCoroutine(workRoutine);

        workRoutine = StartCoroutine(WorkLoop());
    }

    private IEnumerator WorkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f)); // simulate idle thinking

            if (!manager.TryGetWork(out giver, out receiver))
            {
                Debug.Log($"{name} couldn't find work.");
                continue;
            }

            // Move to giver
            bool hasArrived = false;
            npcModeler.MoveTo(giver.transform.position, () => hasArrived = true);
            
            yield return new WaitUntil(() => hasArrived);

            // Pick items
            carried = 0;
            while (carried < maxCapacity)
            {
                var item = giver.GiveItemOfType(receiver.itemType);
                if (item == null) break;

                batchHandler.ReceiveItem(item);
                carried++;
                yield return new WaitForSeconds(0.2f); // simulate pickup delay
            }

            if (carried == 0) continue; // nothing picked

            // Move to receiver
            hasArrived = false;
            npcModeler.MoveTo(receiver.GetClosestPoint(transform.position), () => hasArrived = true);
            yield return new WaitUntil(() => hasArrived);

            // Drop items
            for (int i = 0; i < carried; i++)
            {
                var item = batchHandler.GiveItem();
                if (item == null) break;

                receiver.ReceiveItem(item);
                yield return new WaitForSeconds(0.2f); // simulate drop delay
            }

            carried = 0;
        }
    }
}
