using UnityEngine;
using System.Collections;
using System;

public class _pausegame : MonoBehaviour {
    public static bool paused, unpaused = false;
    //---------------------------------------
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && paused)
        {
            _resume();
            StartCoroutine(Unpause());//had to add a lag so _Player doesnt also back out to the main menu on esc
        }

    }
    public static IEnumerator Unpause()
    {
        unpaused = true;
        yield return new WaitForSeconds(5f);
        unpaused = false;
    }
    public static void AdPause(object sender, EventArgs args)
    {
        if (ButtonMaster.inMenu)
            return;
        if (_Player.ballUsing.Count > 0 || TimeController.ending)
            return;
        Time.timeScale = 0f;
        paused = true;
        GameObject.Find("_GameControl").GetComponent<hud_control>()._objects_hud_control[6].SetActive(true);
    }
    public static  void ADResume(object sender, EventArgs args)
    {
        if (!paused)
            return;
        Time.timeScale = 1f;
        paused = false;
        GameObject.Find("_GameControl").GetComponent<hud_control>()._objects_hud_control[6].SetActive(false);
    }
    public void _pause () {
        if (_Player.ballUsing.Count>0 || TimeController.ending)
            return;
		Time.timeScale = 0f;
        paused = true;
		GetComponent<hud_control> ()._objects_hud_control [6].SetActive (true);
	}
	//---------------------------------------
	public void _resume () {
		Time.timeScale = 1f;
        paused = false;
		GetComponent<hud_control> ()._objects_hud_control [6].SetActive (false);
    }
	//---------------------------------------
}
