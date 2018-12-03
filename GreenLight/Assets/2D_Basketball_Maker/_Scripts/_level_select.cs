using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class _level_select : MonoBehaviour {

	// VAR LIST
	//---------------------------------------
		[HideInInspector]
		public bool _is_menu_selection = true;
	//---------------------------------------
	// VAR UI
	//---------------------------------------
		[HideInInspector]
		public Image _background;
		[HideInInspector]
		public Text _name_stage;
		public Button[] _button_list;
		public Sprite[] _difficulty_icons = new Sprite[2];
		Image[] _difficulty_list = new Image[2];
		Transform _parentHud;

    //---------------------------------------
    public static int currentLevel=0;
	//---------------------------------------
	void Awake () {
		//_create_buttons_dif (GetComponent<_design_control>()._levels[GetComponent<_game_options>()._level_p]._Difficulty_levels.Length);
		//_update_level_info (GetComponent<_game_options>()._difficulty_l);
	}

	//---------------------------------------

	void _create_buttons_dif(int _l){
		Transform _p = GameObject.Find ("_Difficulty").transform;

		if (!_p) {
			Debug.Log ("File not found (_Difficulty)");

		} else if (_difficulty_list.Length != GetComponent<_design_control>()._levels[GetComponent<_game_options>()._level_p]._Difficulty_levels.Length){

			//---------------------------------------

			float _icon_size = 100;
			float _total_size = _icon_size*_l;
			float _x = -_total_size/2;
			_x = _x+_icon_size/2;
			_difficulty_icons[0] = Resources.Load<Sprite>("_level_difficulty");
			_difficulty_icons[1] = Resources.Load<Sprite>("_level_difficulty_selected");
			_difficulty_list = new Image[GetComponent<_design_control>()._levels[GetComponent<_game_options>()._level_p]._Difficulty_levels.Length];

			//---------------------------------------

			for(int i=0;i<_l;i++){

				//---------------------------------------

				GameObject _d = new GameObject();
				_d.AddComponent<RectTransform>();
				_d.AddComponent<CanvasRenderer>();
				_d.AddComponent<Image>();
				_d.AddComponent<Button>();
				_d.name = i.ToString();

				// ADD TO ARRAY
				//---------------------------------------
				_difficulty_list[i] = _d.GetComponent<Image>();
				//---------------------------------------

				Button _b = _d.GetComponent<Button>();
				_b.transform.parent = _p;
				_b.transform.localPosition = new Vector3(_x,-27,0);
				_b.transform.localScale = new Vector3(1,1,1);
				_b.onClick.AddListener( delegate{ this.gameObject.GetComponent<_level_select>()._update_difficulty(_b); } );

				// ADD NEW POSITION X
				//---------------------------------------
				_x = _x+_icon_size;
				//---------------------------------------

				// ADD SPRITES
				//---------------------------------------
				if(i == 0){
					_b.GetComponent<Image>().sprite = _difficulty_icons[1];
				}else{
					_b.GetComponent<Image>().sprite = _difficulty_icons[0];
				}
				//---------------------------------------
			}

		}

	}

	//---------------------------------------

	public void _update_level_info(int _s){
        if (GetComponent<_design_control>()._levels[_s]._locked)
            return;
        _reset_all_buttons_outline ();
		GetComponent<_game_options> ()._level_p = _s;
        currentLevel = _s;
		_background.sprite = GetComponent<_design_control>()._levels[_s]._image_for_select_level;
		_name_stage.text = "Stage:\n"+ GetComponent<_design_control>()._levels[_s]._stage_name;
		_button_list[_s].GetComponent<Outline>().enabled = true;
	}

	//---------------------------------------

	public void _update_difficulty(Button _level){
		int _d =  Convert.ToInt32(_level.name);
		GetComponent<_game_options>()._difficulty_l = _d;
		_reset_difficulty (_d);

	}

	//---------------------------------------
	
	void _reset_all_buttons_outline(){
		for (int i=0; i<_button_list.Length; i++) {
			_button_list[i].GetComponent<Outline>().enabled = false;
		}
	}

	//---------------------------------------
	
	void _reset_difficulty(int _l){

		for (int i=0; i<_difficulty_list.Length; i++) {
			//---------------------------------------
			if(i <= _l){
				_difficulty_list[i].sprite = _difficulty_icons[1];
			}else{
				_difficulty_list[i].sprite = _difficulty_icons[0];
			}
			//---------------------------------------
		}
	}

	//---------------------------------------
	
	public void _retry_game(){

        //_Player.instance._destroyball (_Player.ballUsing[_Player.ballUsing.Count-1]);
        //_pausegame._resume ();
        _Player player = _Player.instance;
        _Player.score = 0;
        _Player.shotEnabled = true;
        _Player.instance.mouseDown = 0;
        _Player.instance.pressed = false;
        _Player.ballUsing = new System.Collections.Generic.List<GameObject>();
        _Player.instance.ball = new GameObject[10];
        TimeController.gameover = false;
        if (_Player.barActive)
        {
            _Player.shotBar.UpdateBar(0, 1);
            _Player.lastShot.UpdateBar(0, 1);
        }
        GameObject.Find("Cash").GetComponent<Text>().text = "Score:\t\t0";
        if(TimeController.highScoreT)
            TimeController.highScoreT.SetActive(false);
        PlayerPrefs.SetInt("shotsMade", PlayerPrefs.GetInt("shotsMade") + _Player.instance.shotsMade);
        PlayerPrefs.SetInt("shotsTaken", PlayerPrefs.GetInt("shotsTaken") + _Player.instance.shotsTaken);
        _Player.instance.shotsMade = 0;
        _Player.instance.shotsTaken = 0;
        _Player.instance.shotStreak = 0;
        _Player.instance.greenStreak = 0;
        _Player.annoucement.text = "";
        TimeController.timeDisplay.text = "Time: " + TimeController.timeSetting;
		GetComponent<hud_control> ()._start_game ();		
		_Player.instance.createBall (1f);
		_Player.instance._score = 0f;
        //---------------------------------------
        _Player.release.text = "Relese:";
        _Player.offBy.text = "Off By: ";
        _Player.chance.text = "Chance of Making:";
        _Player.value.text = "Release: ";
        _Player.avg.text = "Average: ";
        _Player.shotStats.text = "0/0";
        _Player.shotStats2.text = "0%";
        player.shotsMade = 0;
        player.shotsTaken = 0;
        player.releases = new System.Collections.Generic.List<float>();
        GameObject.Find("Green Streak").GetComponent<Text>().text = "Green Streak: 0";
        GameObject.Find("Shot Streak").GetComponent<Text>().text = "Shot Streak: 0";

        _Game_Control.instance._is_gameover = false;
        if (!ButtonMaster.playActive)
        {
            _Game_Control.instance._main_menu();
            return;
        }
        //ButtonMaster.ShowBannerAd();
    }


	//---------------------------------------

	public void _start_game(){
        // CREATE ATTEMPS HUD
        //---------------------------------------
        if (ButtonMaster.AnythingActive())
        {
            GameObject.Find("_Instructions").GetComponent<Text>().text = "You Must Close All Menus Before Starting!";
            GameObject.Find("_Instructions").GetComponent<Text>().color = Color.red;
        }
		int _at = GetComponent<_design_control> ()._levels [GetComponent<_game_options> ()._level_p]._Difficulty_levels [GetComponent<_game_options> ()._difficulty_l]._attempts;
		Image[] _arr = new Image[_at];
			GetComponent<_Game_Control>()._start_game(_at,_arr);
			//---------------------------------------
		//}
		//---------------------------------------
		GameObject _backg = GameObject.Find ("BackgroundGame");

		if (_backg) {
			Destroy(_backg);
		}

		_backg = Instantiate(GetComponent<_design_control>()._levels[GetComponent<_game_options>()._level_p]._background);
		_backg.transform.localPosition = new Vector3 (0,0,5);
		_backg.name = "BackgroundGame";

		GameObject _player = Instantiate(Resources.Load("_Player", typeof(GameObject))) as GameObject; //where the basket and player are instantiated
		GameObject _basket = Instantiate(GetComponent<_design_control>()._levels[GetComponent<_game_options>()._level_p]._basket); //levelp is the level picked
		_basket.transform.position = new Vector3 (GetComponent<_design_control>()._levels[GetComponent<_game_options>()._level_p]._basket_position_x,GetComponent<_design_control>()._levels[GetComponent<_game_options>()._level_p]._basket_position_y,1.78f);


		_player.name = "_Player";
		_basket.name = "_Basket";

		_player.GetComponent<_Player> ()._basketgo = _basket.transform;
		_player.GetComponent<_Player> ()._chek_basket_dats();

		GetComponent<hud_control> ()._start_game ();

		if (!GetComponent<_game_options>()._touch_mode) {
			GameObject.Find ("_CirclePlayer").GetComponent<SpriteRenderer> ().sprite = _Game_Control.instance._sprt_player;
		}
		//Music Play
		//---------------------------------------
		GetComponent<_audio_control>()._play_song(GetComponent<_design_control>()._levels[GetComponent<_game_options>()._level_p]._theme);
		//---------------------------------------
		_Player.instance.newplay();
		//---------------------------------------
	}
	//---------------------------------------
}