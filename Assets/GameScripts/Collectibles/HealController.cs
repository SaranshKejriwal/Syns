using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealController : GenericCollectibleItem
{
    //Heal will provide a percentage based heal - x% of the difference from MaxHealth
    private float healPercent = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //subscribe to event of MazeRenderer. Destroy old prefab objects if a new maze is going to be rendered.
        MazeRenderer.Instance.OnNewMazeRender += DestroySelfOnNewMazeRender;

        this.destroyObjectOnCollect = true;//Heal should be destroyed on collection
        this.isObjectMovable = true;
        this.correctCollectingPlayer = isCollectableBy.BothActivePlayers;
        //Heals can be collected by Both
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameMaster.Instance.IsLevelPlaying())
        {
            return;//do nothing if game is paused or level has ended.
        }

        //Need to add logic to ignore if Player is at Max Health
        if (!PlayerTwoController.Instance.IsAtMaxHealth() && base.IsActivePlayerInVicinityForCollection(PlayerTwoController.Instance))
        {
            //Heal 50% of player's damage.
            PlayerTwoController.Instance.HealPlayer(healPercent);
            Destroy(this.gameObject);
        }
        if (!PlayerOneController.Instance.IsAtMaxHealth() && base.IsActivePlayerInVicinityForCollection(PlayerOneController.Instance))
        {
            PlayerOneController.Instance.HealPlayer(healPercent);
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
