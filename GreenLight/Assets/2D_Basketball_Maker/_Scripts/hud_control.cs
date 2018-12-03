using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class hud_control : MonoBehaviour {
	//---------------------------------------
	
	public GameObject[] _objects_hud_control;
	//---------------------------------------
	//NEW
	//---------------------------------------
	[HideInInspector]
	public GameObject _achievement;
	[HideInInspector]
	public GameObject _unlock;
	[HideInInspector]
	public Text[] _money;
	[HideInInspector]
	public Text _price;
	[HideInInspector]
	public Image _icon;
	public GameObject _button_buy;
	public GameObject _low_money;
	//---------------------------------------
	// Time Attack
	[HideInInspector]
	public Text _time_selected;
	[HideInInspector]
	public GameObject _timepack;
	[HideInInspector]
	public Text _time_game;
	//---------------------------------------
	[HideInInspector]
	public Text _score;
	[HideInInspector]
	public Text _best_score;
	//---------------------------------------
	// GAME OVER
	[HideInInspector]
	public Text _gov_score;
	[HideInInspector]
	public Text _gov_best_score;
	//---------------------------------------
	// Menu
	[HideInInspector]
	public Image _ballicon_default;
	[HideInInspector]
	public Image _player_default;
	[HideInInspector]
	public Image _trajectory_default;
	//---------------------------------------
	public Image _touchm;
	public Sprite[] _touchb;
	public Text _T_touch;
    //---------------------------------------
    public static bool pageSwitch = false;
    private bool unlockTriggered = false;
	void Awake(){
        /*if (PlayerPrefs.HasKey ("bestscore")) {
			_gov_best_score.text = PlayerPrefs.GetInt ("bestscore").ToString ("0000000");
			_best_score.text = PlayerPrefs.GetInt ("bestscore").ToString ("0000000");
		} else {
			PlayerPrefs.GetInt("bestscore",0);
		}
		//---------------------------------------*/
        
    }
	//---------------------------------------
	public void _start_game(){//in game
		//_score.text = "0000000";
		_objects_hud_control [0].SetActive (false);
		_objects_hud_control [2].SetActive (false);
		_objects_hud_control [1].SetActive (true);
        ButtonMaster.instance.StopAllCoroutines();
        if(ButtonMaster.instance.tips)
            ButtonMaster.instance.tips.SetActive(false);
	}
	//---------------------------------------
	public void update_score(float _s){
		_score.text = _s.ToString("0000000");
	}
	//---------------------------------------
	public void _gameover(){
        _Player.shotEnabled = false;
        _gov_score.text = _Player.instance._score.ToString("0000000");
		_objects_hud_control[0].SetActive(false);
		_objects_hud_control[1].SetActive(false);
		_objects_hud_control[4].SetActive(false);
		_objects_hud_control[2].SetActive(true);
		_objects_hud_control[3].SetActive(true);
		_objects_hud_control[3].GetComponent<Animator> ().enabled = true;

		if (PlayerPrefs.HasKey ("bestscore")) {
			if (_Player.instance._score > PlayerPrefs.GetInt ("bestscore")) {
				_objects_hud_control [4].SetActive (true);
				PlayerPrefs.SetInt ("bestscore", (int)_Player.instance._score);
				_gov_best_score.text = _Player.instance._score.ToString ("0000000");
				_best_score.text = _Player.instance._score.ToString ("0000000");
			}
		} else {
			_gov_best_score.text = _Player.instance._score.ToString ("0000000");
			_best_score.text = _Player.instance._score.ToString ("0000000");
		}
	}
    //---------------------------------------
    public void _changemenu(int _s)
    {
        if (!ButtonMaster.AnythingActive() && !ButtonMaster.UIActive)
        {
            AddedItemsController.UI = true;
            GetComponent<_design_control>()._selectmode = _s;
            AddedItemsController.ToggleAddedItems();
            //if(_objects_hud_control[5].activeSelf) //|| !_button_buy.activeSelf || !_low_money.activeSelf)
              //  AddedItemsController.ToggleAddedItems();
            _objects_hud_control[5].SetActive(true);
            ButtonMaster.UIActive = true;
            GetComponent<_design_control>()._next(0);
        }
    }
    //--------------------------------------

	public void _view_time() {
		_timepack.SetActive (true);
	}
	//---------------------------------------
	public void _achievement_view() {
        if (!ButtonMaster.AnythingActive() && !ButtonMaster.UIActive)
        {
            _achievements_config.instance._load_page();
            ButtonMaster.UIActive = true;
            AddedItemsController.UI = true;
            AddedItemsController.ToggleAddedItems();
            _achievement.SetActive(true);
            if (_Player.timeAttack)
                ButtonMaster.instance.timeButtons.SetActive(false);
            else
                ButtonMaster.instance.go.SetActive(false);
        }
	}
	//---------------------------------------
	public void _achievement_close() {
		_achievement.SetActive (false);
        ButtonMaster.UIActive = false;
        AddedItemsController.ToggleAddedItems();
        AddedItemsController.UI = false;
        if (ButtonMaster.playActive)
        {
            if (_Player.timeAttack)
                ButtonMaster.instance.timeButtons.SetActive(true);
            else
                ButtonMaster.instance.go.SetActive(true);
        }
    }
	//---------------------------------------
	public void _unlock_open() {
        if (!ButtonMaster.AnythingActive() || unlockTriggered)
        {
            _unlock.SetActive(true);
            unlockTriggered = true;
            ButtonMaster.UIActive = true;
            AddedItemsController.UI = true;
            AddedItemsController.ToggleAddedItems();
            if (_Player.timeAttack)
                ButtonMaster.instance.timeButtons.SetActive(false);
            else
                ButtonMaster.instance.go.SetActive(false);
        }
    }
	//---------------------------------------
	public void _unlock_close() {
		_unlock.SetActive (false);
        unlockTriggered = false;
        
        if (!GetComponent<hud_control>()._objects_hud_control[5].activeSelf)
        {
            AddedItemsController.ToggleAddedItems();
            ButtonMaster.UIActive = false;
        }
        if (ButtonMaster.playActive)
        {
            if (_Player.timeAttack)
                ButtonMaster.instance.timeButtons.SetActive(true);
            else
                ButtonMaster.instance.go.SetActive(true);
        }
        

    }
	//---------------------------------------
	public void _update_money(){
		_money[0].text = "$ "+_Game_Control.instance._money;
		_money[1].text = "$ " +_Game_Control.instance._money;
		_money[2].text = "$ " +_Game_Control.instance._money;
	}
	//---------------------------------------
	public void _add_money(int _t){
		StartCoroutine (_add_m (_t));
	}
	//---------------------------------------
	IEnumerator _add_m(int _total){
		_Player.cash += _total;
	    _Game_Control.instance._money = _Player.cash;
	    PlayerPrefs.SetInt("cash",_Player.cash);
	    _update_money ();
        yield break;
	}
	//---------------------------------------
	public void _touch_mode(){
		//---------------------------------------
		if(GetComponent<_game_options>()._touch_mode){
			GetComponent<_game_options>()._touch_mode = false;
			_touchm.sprite = _touchb[0];
			_T_touch.text = "Normal Mode";
		}else{
			GetComponent<_game_options>()._touch_mode = true;
			_touchm.sprite = _touchb[1];
			_T_touch.text = "Touch Mode";
		}
		//---------------------------------------
	}
	//---------------------------------------
}
