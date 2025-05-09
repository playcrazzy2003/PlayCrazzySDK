using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    [SerializeField] private List<ItemBatchStacker> giverBatchStackers;
    [SerializeField] private List<ItemBatchStacker> receiverBatchStackers;
    public bool TryGetWork(out ItemBatchStacker giver, out ItemBatchStacker receiver)
    {
        // Randomly choose strategy
        int strategy = Random.Range(0, 2);
        return strategy switch
        {
            0 => FindWorkByReceiverNeeds(out giver, out receiver),
            1 => FindRandomReceiverAndMatchGiver(out giver, out receiver),
            _ => throw new System.NotImplementedException()
        };
    }

    private bool FindWorkByReceiverNeeds(out ItemBatchStacker giver, out ItemBatchStacker receiver)
    {
        foreach (var rec in receiverBatchStackers)
        {
            if (rec.CanReceive() && rec.gameObject.activeInHierarchy)
            {
                foreach (var giv in giverBatchStackers)
                {
                    if (giv.CanGive() && giv.itemType == rec.itemType)
                    {
                        giver = giv;
                        receiver = rec;
                        return true;
                    }
                }
            }
        }

        giver = null;
        receiver = null;
        return false;
    }

    private bool FindRandomReceiverAndMatchGiver(out ItemBatchStacker giver, out ItemBatchStacker receiver)
    {
        receiver = receiverBatchStackers[Random.Range(0, receiverBatchStackers.Count)];
        if (receiver.gameObject.activeInHierarchy)
        {
            foreach (var giv in giverBatchStackers)
            {
                if (giv.CanGive() && giv.itemType == receiver.itemType)
                {
                    giver = giv;
                    return true;
                }
            }
        }

        giver = null;
        receiver = null;
        return false;
    }

    public List<ItemBatchStacker> GetAllReceivers()
    {
        return receiverBatchStackers;
    }

    public List<ItemBatchStacker> GetAllGivers()
    {
        return giverBatchStackers;
    }
}