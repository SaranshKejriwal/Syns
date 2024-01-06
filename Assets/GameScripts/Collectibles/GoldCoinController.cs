using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoinController : GenericCollectibleItem
{

    private float goldCoinValue = 1f;//this value can be tweaked if we want to make gold coins more valuable.

    // Start is called before the first frame update
    void Start()
    {
        //subscribe to event of MazeRenderer. Destroy old prefab objects if a new maze is going to be rendered.
        MazeRenderer.Instance.OnNewMazeRender += DestroySelfOnNewMazeRender;

        this.destroyObjectOnCollect = true;//coin should be destroyed on collection
        this.isObjectMovable = true;
        this.correctCollectingPlayer = isCollectableBy.BothActivePlayers;
        //Coins can be collected by Both
    }


    // Update is called once per frame
    void Update()
    {
        if (!GameMaster.Instance.IsLevelPlaying())
        {
            return;//do nothing if game is paused or level has ended.
        }

        if (base.IsActivePlayerInVicinityForCollection(PlayerTwoController.Instance))
        {
            PlayerTwoController.Instance.CollectGold(goldCoinValue);
            Destroy(this.gameObject);
        }
        if (base.IsActivePlayerInVicinityForCollection(PlayerOneController.Instance))
        {
            PlayerOneController.Instance.CollectGold(goldCoinValue);
            Destroy(this.gameObject);//increment only once.
        }
    }

    private void DestroySelfOnNewMazeRender(object obj, EventArgs e)
    {
        if (this != null && this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }
}
