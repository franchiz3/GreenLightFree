using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddedItemsController : MonoBehaviour {

    public static GameObject addedItems,options;
    public static bool UI = false;
    private static bool itemsActive;
    // Use this for initialization
	void Start () {
        addedItems = GameObject.Find("Added Items");
        options = GameObject.Find("Sound Options");
        itemsActive = true;
    }
	
	// Update is called once per frame
	public static void ToggleAddedItems()
    {
        if (itemsActive)
        {
            addedItems.SetActive(false);
            itemsActive = false;
            if (UI)
                options.SetActive(false);
        }   
        else
        {
            addedItems.SetActive(true);
            itemsActive = true;
            options.SetActive(true);
            UI = false;
        }
    }

}
