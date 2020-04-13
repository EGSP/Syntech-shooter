using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using TMPro;



public class HUD_Resources : MonoBehaviour, IPlayerObserver
{
    
    [SerializeField] private TMP_Text detailCountText;

    private DetailData detailData;

    void Awake()
    {
        UIController.Instance.OnPlayerChanged += ChangePlayerController;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateDetails(int count)
    {
        detailCountText.text = count.ToString();
    }

    private void UnsubscribeFromDetails()
    {
        if (detailData != null)
        {
            detailData.OnCountChanged -= UpdateDetails;
            detailData = null;
        }
    }

    public void ChangePlayerController(PlayerControllerComponent playerControllerComponent)
    {
        UnsubscribeFromDetails();

        var inventory = playerControllerComponent.InventoryComponent.InventorySystem;

        var detailList = inventory.GetListOfInventoryItem(InventoryItemType.Detail);

        if(detailList.Count > 0)
        {
            detailData = detailList[0] as DetailData;

            detailData.OnCountChanged += UpdateDetails;
            detailData.OnForceUnsubscribe += UnsubscribeFromDetails;

            UpdateDetails(detailData.Count);
        }
    }

    public void PlayerControllerNull()
    {
        throw new System.NotImplementedException();
    }

    public void Unsubscribe(IObservable observable)
    {
        UnsubscribeFromDetails();
    }
}
