using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class WorkerBehaviour : MonoBehaviour
{
    [SerializeField] private int MaxCaptiy = 5;
    private WorkerManager manager;
    private ItemBatchHandler batchHandler;
    private NPCModeler npcModeler;
    private int carried = 0;
    private ItemBatchStacker giver, receiver;
    private bool hasWorked = false;
    private void OnEnable()
    {
        manager = GameManager.Instance.workerManager;
        npcModeler = gameObject.GetComponent<NPCModeler>();
    }

    [Button("Init")]
    public void Init()
    {
        TryFindWork();
        //StartCoroutine(WorkLoop());
    }

    void TryFindWork()
    {
        
    }

    IEnumerator FindWorkLoop()
    {
        while (!hasWorked)
        {
            if (!manager.TryGetWork(out giver, out receiver))
            {
                hasWorked = true;
                MoveToTarget(giver.transform.position, giver, receiver);
                yield break;
            }
            
            yield return new WaitForSeconds(2f);
        }
    }

    void MoveToTarget(Vector3 target, ItemBatchStacker giver, ItemBatchStacker receiver)
    {
        npcModeler.MoveTo(target, () =>
        {
            batchHandler.HandleItemsReceive(giver);
        });
    }
    private IEnumerator WorkLoop()
    {
        while (true)
        {
            yield return TryFindWorkk();
           
            if (giver == null || receiver == null)
            {
                yield return new WaitForSeconds(2f);
                continue;
            }

            yield return MoveTo(giver.transform.position);
            yield return PickupFromGiver();

            if (carried > 0)
            {
                yield return MoveTo(receiver.transform.position);
                yield return DeliverToReceiver();
            }
            carried = 0;
        }
    }

    private IEnumerator TryFindWorkk()
    {
        while (!manager.TryGetWork(out giver, out receiver))
        {
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator PickupFromGiver()
    {
        while (carried < MaxCaptiy && giver.CanGive())
        {
            carried++;
            giver.GiveItem();
            yield return new WaitForSeconds(0.2f); // Simulate pick-up time
        }
    }

    private IEnumerator DeliverToReceiver()
    {
        float waitTime = 3f;
        float timer = 0f;

        while (carried > 0)
        {
            if (receiver.CanReceive())
            {
                ///receiver.ReceiveItem();
                carried--;
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                if (timer >= waitTime)
                {
                    ItemBatchStacker altReceiver = FindAlternateReceiver(receiver.itemType);
                    if (altReceiver != null)
                    {
                        receiver = altReceiver;
                        yield return MoveTo(receiver.transform.position);
                        timer = 0;
                    }
                    else
                    {
                        break;
                    }
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    private ItemBatchStacker FindAlternateReceiver(ItemType itemType)
    {
        foreach (var stacker in manager.GetAllReceivers())
        {
            if (stacker.itemType == itemType && stacker.CanReceive())
            {
                return stacker;
            }
        }

        return null;
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        bool hasArrived = false;
        npcModeler.MoveTo(destination, () => hasArrived = true);

        while (!hasArrived)
            yield return null;
    }
}
