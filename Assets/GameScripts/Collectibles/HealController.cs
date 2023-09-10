using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealController : GenericCollectibleItem
{
    // Start is called before the first frame update
    void Start()
    {
        this.destroyObjectOnCollect = true;//Heal should be destroyed on collection
        this.isObjectMovable = true;
        this.correctCollectingPlayer = isCollectableBy.BothActivePlayers;
        //Heals can be collected by Both
    }

    // Update is called once per frame
    void Update()
    {
        //Need to add logic to ignore if Player is at Max Health
        if (base.IsActivePlayerInVicinityForCollection(PlayerTwoController.Instance))
        {
            //Need to add healing logic to heal 50%
            //PlayerTwoController.Instance.CollectGold();
            Destroy(this.gameObject);
        }
        if (base.IsActivePlayerInVicinityForCollection(PlayerOneController.Instance))
        {
            //PlayerOneController.Instance.CollectGold();
            Destroy(this.gameObject);//increment only once.
        }
    }
}
