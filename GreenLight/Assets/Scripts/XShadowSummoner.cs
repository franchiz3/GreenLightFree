using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XShadowSummoner : MonoBehaviour {

    _Player player;
    // Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleShots(int index)
    {
        player = _Player.instance;
        player.ToggleMoveShotButton(index);
    }
}
