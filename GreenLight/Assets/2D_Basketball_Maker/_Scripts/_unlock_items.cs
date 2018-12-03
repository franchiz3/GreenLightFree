using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class _unlock_items : MonoBehaviour {
	//---------------------------------------
	int _price = 0;
	int _ID = 0;
	bool _is_ball = false, buyToggled = false;
    public Text unlockText;
	//---------------------------------------
	void Awake(){
        //_check_balls_locked ();
        //_check_stages_locked ();
        _load_unlockeables(true); //looks like the developer couldn't fully get it to work. I'll have to fix it because the way they have it crashes
        //UnlockAll();
        _check_stages_ui ();
		//_load_unlockeables ();
	}
    //---------------------------------------

    void UnlockAll()
    {
        foreach (_ball_config ball in GetComponent<_design_control>()._ball_materials)
            ball._locked = false;
        foreach (_level_config level in GetComponent<_design_control>()._levels)
            level._locked = false;
    }

    //--------------------------------------
    void _check_balls_locked (){
		for (int i = 0; i < GetComponent<_design_control> ()._ball_materials.Length; i++) {
			if(GetComponent<_design_control> ()._ball_materials[i]._price_to_unlock > 0){
				//---------------------------------------
				GetComponent<_design_control> ()._ball_materials [i]._locked = true;
				//---------------------------------------
			}
		}
	}
	//---------------------------------------
	void _check_stages_locked (){
		//---------------------------------------
		// CHECK LOCKED STAGES
		//---------------------------------------
		for (int i = 0; i < GetComponent<_design_control> ()._levels.Length; i++) {
			//---------------------------------------
			if (GetComponent<_design_control> ()._levels [i]._price_to_unlock > 0) {
				//---------------------------------------
				GetComponent<_design_control> ()._levels [i]._locked = true;
				//---------------------------------------
			} else {
				GetComponent<_design_control> ()._levels [i]._locked = false;
			}
			//---------------------------------------
		}
	}
	//---------------------------------------
	public void _check_stages_ui (){
		//---------------------------------------
		// CHECK LOCKED STAGES
		//---------------------------------------
		for (int i = 0; i < GetComponent<_design_control> ()._levels.Length; i++) {
			//---------------------------------------
			if (GetComponent<_design_control> ()._levels [i]._locked) {
				GetComponent<_design_control> ()._levels [i]._UI_locked_sprite.gameObject.SetActive (true);
			} else {
				GetComponent<_design_control> ()._levels [i]._UI_locked_sprite.gameObject.SetActive (false);
			}
			//---------------------------------------
		}
	}
	//---------------------------------------
	public void _buy_item(){
		//---------------------------------------
		Debug.Log ("BUY ITEM");
		//---------------------------------------
		_audio_control.instance._buy_sound(); // Play Sound
		GetComponent<hud_control>()._unlock_close();
		//---------------------------------------
		int _t = _Game_Control.instance._money;
		_t = _t - _price;
		_Game_Control.instance._money = _t;
        _Player.cash = _t;
        PlayerPrefs.SetInt("cash", _Player.cash);
		//---------------------------------------
		if (_is_ball) {
			//---------------------------------------
			_achievements_config.instance._check (_conditions.In_Total_Game, _achievement_type.Unlock_Ball, _ID);
			GetComponent<_design_control> ()._ball_materials [_ID]._locked = false;
			GetComponent<_design_control> ()._update_img ();
			//---------------------------------------
		} else {
			//---------------------------------------
			_achievements_config.instance._check (_conditions.In_Total_Game, _achievement_type.Unlock_Stage, _ID);
			GetComponent<_design_control> ()._levels [_ID]._locked = false;
			_check_stages_ui ();
			//---------------------------------------
		}
		GetComponent<hud_control> ()._update_money ();
		//---------------------------------------
		_save_unlock(_is_ball);
        //---------------------------------------
        ButtonMaster.UIActive = false;
        AddedItemsController.UI = false;
    }
	//---------------------------------------
	public void _unlock_ball (Image ID) {
        if (ButtonMaster.AnythingActive())
            return;
        ButtonMaster.UIActive = true;
        AddedItemsController.UI = true;
        if(!GetComponent<hud_control>()._objects_hud_control[5].activeSelf)
            AddedItemsController.ToggleAddedItems();
        /*if (!buyToggled)
        {
           
            buyToggled = true; 
        }*/

        //---------------------------------------
        _is_ball = true;
		_ID = int.Parse (ID.name);
        //---------------------------------------
        unlockText.text = "unlock";
        GetComponent<hud_control> ()._icon.rectTransform.sizeDelta = new Vector2(300,300);
		GetComponent<hud_control> ()._icon.sprite = GetComponent<_design_control> ()._ball_materials [_ID]._icon;
		GetComponent<hud_control> ()._price.text = GetComponent<_design_control> ()._ball_materials [_ID]._price_to_unlock.ToString();
		GetComponent<hud_control> ()._unlock.SetActive (true);
		//---------------------------------------
		_price = GetComponent<_design_control> ()._ball_materials [_ID]._price_to_unlock;
		//---------------------------------------
		if (_Game_Control.instance._money < GetComponent<_design_control> ()._ball_materials [_ID]._price_to_unlock) {
			GetComponent<hud_control> ()._button_buy.SetActive (false);
			GetComponent<hud_control> ()._low_money.SetActive (true);
		} else {
			GetComponent<hud_control> ()._button_buy.SetActive (true);
			GetComponent<hud_control> ()._low_money.SetActive (false);
		}
		//---------------------------------------
	}
	//---------------------------------------
	public void _unlock_stage (int _IDT) {
        if (ButtonMaster.AnythingActive())
            return;
        ButtonMaster.UIActive = true;
        AddedItemsController.UI = true;
        AddedItemsController.ToggleAddedItems();
        GetComponent<hud_control>()._low_money.GetComponentInChildren<Text>().text = "You're Broke As A Joke! :P";
        /*if (buyToggled)
        {
            
            buyToggled = false; ;
        }*/
        _is_ball = false;
		_ID = _IDT;
        //---------------------------------------
        hud_control hud = GetComponent<hud_control>();
		hud._icon.rectTransform.sizeDelta = new Vector2(300,200);
		hud._icon.sprite = GetComponent<_design_control> ()._levels[_ID]._image_for_select_level;
		hud._price.text = GetComponent<_design_control> ()._levels[_ID]._price_to_unlock.ToString();
		hud._unlock.SetActive (true);
        GameObject.Find("Unlock Text").GetComponent<Text>().text = GetComponent<_design_control>()._levels[_ID]._stage_name;
		//---------------------------------------
		_price = GetComponent<_design_control> ()._levels[_ID]._price_to_unlock;
        int neededShots = GetComponent<_design_control>()._levels[_ID]._points_to_unlock; // shotsMade necessary to buy a stage
        //---------------------------------------
        if (_Game_Control.instance._money < GetComponent<_design_control> ()._levels[_ID]._price_to_unlock ) {
            
            hud._button_buy.SetActive (false);
			hud._low_money.SetActive (true);
		} else {
            if (neededShots > PlayerPrefs.GetInt("shotsMade"))
            {
                hud._button_buy.SetActive(false);
                hud._low_money.SetActive(true);
                hud._low_money.GetComponentInChildren<Text>().text = "Not Enough Shots Taken!\nNeed:"+neededShots+" Have:" + PlayerPrefs.GetInt("shotsMade");
            }
            else
            {
                hud._button_buy.SetActive(true);
                hud._low_money.SetActive(false);
            }
		}
		//---------------------------------------
	}
	//---------------------------------------
	void _save_unlock(bool _isball = true){
		string _t = "";// this string has all of the values for a stage being locked or unlocked as true and false. It's a rudimentary binary system

		if (_isball) { // Is Ball <---trying to figure out the point of this bool. seems to just make itself true regawrdless G.O.T :P <-- ok so only a ball and stage can be bought and thats the bool
			for (int i = 0; i < GetComponent<_design_control> ()._ball_materials.Length; i++) {
				if (GetComponent<_design_control> ()._ball_materials [i]._locked) {
					_t += "true";
				} else {
					_t += "false";
				}
				_t += "/";
			}
			//---------------------------------------
			PlayerPrefs.SetString ("_ball_locked", _t);
			//---------------------------------------
		} else {
			for (int i = 0; i < GetComponent<_design_control> ()._levels.Length; i++) {
				if (GetComponent<_design_control> ()._levels[i]._locked) {
					_t += "true";
				} else {
					_t += "false";
				}
				_t += "/";
			}
			//---------------------------------------
			PlayerPrefs.SetString ("_stage_locked", _t);
			//---------------------------------------
		}
        PlayerPrefs.Save();
	}
	//---------------------------------------
	void _load_unlockeables(bool _isball = true){
        //---------------------------------------
        if (PlayerPrefs.HasKey("_ball_locked"))
        {
            //---------------------------------------
            string _t = PlayerPrefs.GetString("_ball_locked");
            string[] _arr = _t.Split(new string[] { "/" }, System.StringSplitOptions.None);

            //---------------------------------------
            for (int i = 0; i < GetComponent<_design_control>()._ball_materials.Length; i++)
            {
                GetComponent<_design_control>()._ball_materials[i]._locked = _string_to_bool(_arr[i]);
            }
            //---------------------------------------
        }
        else //fix for not loading if the parameter doesn't exist 11/7/18
        {
            string values = "false/";
            GetComponent<_design_control>()._ball_materials[0]._locked = false;
            for (int i = 1; i < GetComponent<_design_control>()._ball_materials.Length; i++)
            {
                GetComponent<_design_control>()._ball_materials[i]._locked = true;
                values += "/true";
            }
            PlayerPrefs.SetString("_ball_locked",values);
        }
		//---------------------------------------
		if(PlayerPrefs.HasKey("_stage_locked")){
			//---------------------------------------
			string _t = PlayerPrefs.GetString ("_stage_locked");
            //Debug.Log(_t);
			string[] _arr = _t.Split(new string[] {"/"}, System.StringSplitOptions.None);

			//---------------------------------------
			for (int i = 0; i < GetComponent<_design_control> ()._levels.Length; i++) {
				//Debug.Log (_arr [i]);
				GetComponent<_design_control> ()._levels [i]._locked = _string_to_bool(_arr [i]);
			}
			//---------------------------------------
		}
        else //fix for not loading if the parameter doesn't exist 11/7/18
        {
            string values = "false";
            GetComponent<_design_control>()._levels[0]._locked = false;
            for (int i = 1; i < GetComponent<_design_control>()._levels.Length; i++)
            {
                GetComponent<_design_control>()._levels[i]._locked = true;
                values += "/true";
            }
            PlayerPrefs.SetString("_stage_locked", values);
        }
        //---------------------------------------
        PlayerPrefs.Save();
    }
	//---------------------------------------
	bool _string_to_bool(string _s){
		bool _r = true;

		if (_s == "false") {
			_r = false;
		}
		return _r;
	}
	//---------------------------------------
}
