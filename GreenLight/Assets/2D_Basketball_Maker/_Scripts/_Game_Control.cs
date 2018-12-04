using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.Collections;

public class _Game_Control : MonoBehaviour {
	private static _Game_Control _instance;
	public static _Game_Control instance{get{if (!_instance){_instance = GameObject.FindObjectOfType(typeof(_Game_Control)) as _Game_Control;}return _instance;}}
	//---------------------------------------
	[HideInInspector]
	public int _time_attack_time = 0;
	[HideInInspector]
	//public Image[] _hud_attemps = new Image[2];
	int _attemps = 0;
	[HideInInspector]
	public bool _is_gameover = false;
	[HideInInspector]
	public int _money = 0;
	public int _basket_money = 25;
	//---------------------------------------
	public Vector3 _power_click_on_touch_mode = new Vector3(1,2,0);
	//---------------------------------------
	// Achievements
	int _total_matches = 0;
	int _total_baskets = 0;
	int _total_money = 0;
	int _total_score = 0;
	[HideInInspector]
	int _matches = 0;
	[HideInInspector]
	public int _baskets = 0;
	//---------------------------------------
	// Sprites to gameplay
	//---------------------------------------
	[Header("Default Design")]
	public Material _ball_material; // Default Ball
	public Sprite _sprt_trajectory; // Default Trajectory Poing
	public Sprite _sprt_player; // Default Player
	//---------------------------------------
	void Awake(){
        
        //---------------------------------------
        _total_matches = PlayerPrefs.GetInt("_total_matches");
		_total_baskets = PlayerPrefs.GetInt("_total_baskets");
		_total_money = PlayerPrefs.GetInt("_total_money");
		_total_score = PlayerPrefs.GetInt("_total_score");
		//---------------------------------------
		_money = PlayerPrefs.GetInt("cash");
		GetComponent<hud_control> ()._update_money ();
		//---------------------------------------
		GetComponent<_unlock_items>()._check_stages_ui ();
		//---------------------------------------
	}
    private void Start()
    {
        Screen.fullScreen = true;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        //Camera.main.aspect = (Screen.currentResolution.width / Screen.currentResolution.height);
        if (Screen.orientation != ScreenOrientation.LandscapeLeft)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }
    //---------------------------------------
    public void _main_menu(){
        if (_Player.timeAttack)
        {
            //if(_Player.instance.shotsTaken <5) //for videos. if banners start working there's no need to check.
            TimeController.StopRoutines();
            ButtonMaster.ShowBannerAd();
        }
        ButtonMaster.instance.exitBanner.GetComponentInChildren<Text>().text = "";
        Advertisement.Banner.Hide();
        Time.timeScale = 1;
        _pausegame.paused = false;
        _Player.annoucement.text = "";
        _Player.score = 0;
        //---------------------------------------
        GetComponent<_unlock_items>()._check_stages_ui ();
        _achievements_config.instance._check(_conditions.In_Total_Game, _achievement_type.Matches, PlayerPrefs.GetInt("sessions"));
        _achievements_config.instance._check(_conditions.In_Total_Game, _achievement_type.Money, PlayerPrefs.GetInt("cash"));
        //---------------------------------------
        GetComponent<hud_control>()._add_money(_Player.instance.sessionCash);
        _Player.instance.sessionCash = 0;
        if (_Player.barActive)
        {
            _Player.shotBar.UpdateBar(0, 1);
            _Player.lastShot.UpdateBar(0, 1);
        }
        TimeController.gameover = false;
        // Destroy all objects from scene gameplay
        Destroy (GameObject.Find ("BackgroundGame"));
		Destroy (GameObject.Find ("_Player"));
		Destroy (GameObject.Find ("_Basket"));
		Destroy (GameObject.Find ("_shadowball"));
		Destroy (GameObject.Find ("_CirclePlayer"));
		// Destroy trayectori
		Destroy (GameObject.Find ("Trajectory_Points"));
		GetComponent<hud_control> ()._objects_hud_control [6].SetActive (false);
		GetComponent<hud_control> ()._objects_hud_control [2].SetActive (false);
		_is_gameover = false;
		_Player.instance._moveball = false;
		GetComponent<hud_control> ()._objects_hud_control [0].SetActive (true);
		//---------------------------------------
		// Reset Music
		//---------------------------------------
        if(_audio_control.instance._music)
		    GetComponent<_audio_control>()._play_song();
        //---------------------------------------
        if (_Player.xActive)
            _Player.xButton.SetActive(false);
        else
            _Player.canvas.SetActive(false);
        ButtonMaster.inMenu = true;
        
        if ( ButtonMaster.shotsLeft <= 0)
        {
            if (_Player.timeAttack)
                ButtonMaster.instance.timeButtons.SetActive(false);
            else
                ButtonMaster.instance.go.SetActive(false);
            ButtonMaster.playActive = false;
            ButtonMaster.dailyText.text = "Get Shots!";
            StartCoroutine(ButtonMaster.RefreshCountDown());
        }

        if (ButtonMaster.tipsEnabled && Random.Range(0, 100) < 35)
            StartCoroutine(ButtonMaster.DisplayTip("", true, false));
        else
            ButtonMaster.tipsP.SetActive(false);
        if (ButtonMaster.shotsLeft > 0)
            ButtonMaster.dailyText.text = "Shots Left: " + ButtonMaster.shotsLeft;
       

        _audio_control.instance.PlayInMenu();
    }

	//---------------------------------------
	public void _quit_game(){
        PlayerPrefs.Save();
		Application.Quit ();
	}
	//---------------------------------------
	public void _retry_game(){//need to change this button functionality. Changing to reset stats 11/4/18
        if (!_Player.regularMode)
            return;
        _Player player = _Player.instance;
        //GetComponent<hud_control>()._add_money(player.sessionCash);
        //_Player.shotChanged = true; until glitch gets fixed
        //player.sessionCash = 0;        
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
    }
	//---------------------------------------
	public void _start_game(int _at,Image[] _arr){
		
	}
	//---------------------------------------
	public void _fail(){
	
	}
	//---------------------------------------

	/*public void _Game_Over(){
		//---------------------------------------
		// UPDATE DATS
		//---------------------------------------
		int _m = _baskets*_basket_money;

		_total_matches = _total_matches+_matches;
		_total_baskets = _total_baskets + _baskets;
		_total_money = _total_money + _m;
		//---------------------------------------
		PlayerPrefs.SetInt("_total_matches",_total_matches);
		PlayerPrefs.SetInt("_total_baskets",_total_baskets);
		PlayerPrefs.SetInt("_total_money",_total_money);
		PlayerPrefs.SetInt("_total_score",_total_score);
		//---------------------------------------
		// UPDATE MONEY
		//---------------------------------------
		GetComponent<hud_control> ()._add_money (_m);
		//---------------------------------------
		// GAMEOVER VAR
		//---------------------------------------
		_Player.instance._moveball = false;
		_is_gameover = true;
		_audio_control.instance._gameover_sound ();
		GetComponent<hud_control> ()._gameover();
		Destroy (GameObject.Find ("_shadowball"));
		//_Player.instance._destroyball (_Player.ballUsing[_Player.ballUsing.Count-1]);
		GetComponent<_counter> ()._stop ();
		//---------------------------------------
		//where I stole achievements from
		_baskets = 0;
		//---------------------------------------
		#if ADMOB
		//---------------------------------------
		// CHEK BANNER
		//---------------------------------------
		_admob_admin.Instance._check_banner_action (_admob_active.Game_Over,_admob_hidden.Game_Over);
		//---------------------------------------
		//---------------------------------------
		#endif
	}
	//---------------------------------------*/
	public void _update_time_attack(int _time){
		GetComponent<hud_control> ()._timepack.SetActive (false);
		if (_time == 0) {
			GetComponent<hud_control> ()._time_selected.text = "No Time";
		} else {
			GetComponent<hud_control> ()._time_selected.text = _time.ToString()+" Seconds";
		}
		_time_attack_time = _time;
	}
	//---------------------------------------
}