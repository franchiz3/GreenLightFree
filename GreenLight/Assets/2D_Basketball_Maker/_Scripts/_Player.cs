using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class _Player : MonoBehaviour 
{
	private static _Player _instance;
	public static _Player instance{get{if (!_instance){
	_instance = FindObjectOfType(typeof(_Player)) as _Player;}
	return _instance;}}
    //---------------------------------------
    public static JumpShot shot;
    public static Color barColor;
    public static SimpleHealthBar shotBar, lastShot;
    public static GameObject barHolder;
    public float mouseDown, mouseUp, madePercentage, timeMultiplier, currentScore;
    public static bool xActive = false, longRange = true, stickCreated = false, discoverMode = false, shotChanged, moving = false, shotEnabled = true, timeAttack = false, regularMode = true, barActive = true, normalMode=true;
    public bool pressed = false;
    public bool justEnded = false;
    bool airball = false;
    bool green, shotMade;
    public int sessionCash;
    private int early = 0, late = 0, inWindow = 0, perfect = 0;
    public static Text release, offBy, chance, value, avg, shotDisplay, shotStats,shotStats2, annoucement;
    public List<float> releases;
    Button shotButtonToggler;
    Vector3 tempPos;
    public static GameObject xButton, canvas;
    public static List<GameObject> ballUsing;
   public List<Coroutine> activeShots;
    private float avgAgg;
    public static GameObject backHud, xMove, xShadow, gem;
    public static int trueMidMin, trueMidMax, trueLongMin, trueLongMax, rMid, rLong, cash, score=0;
    public int shotsMade = 0, shotsTaken = 0, shotStreak=0, greenStreak=0, greenCount=0, missCount = 0, currentBall = 0;
    public static AudioSource player;

    #region old ball stuff
        //---------------------------------------
        bool _touch_mode = false;
	    //---------------------------------------
	    public GameObject TrajectoryPointPrefab;
	    public GameObject BallPrefb;
	    [Range(0,30)]
	    public int TrajectoryPoints = 20;
	    public bool _full_trajectory = false;
	    //---------------------------------------	
	    public GameObject[] ball = new GameObject[10];
	    GameObject _tmpcircle = null;
	    List<GameObject> trajectoryPoints;
	    //---------------------------------------	
	    Vector3 _force;
	    //---------------------------------------
	    [HideInInspector]
	    public bool _moveball,_cparts = true;
	    bool _stop_on_dunk = false;
	    //---------------------------------------	
	    Cloth _basket;
	    //---------------------------------------	
	    public Transform _basketgo;
	    Transform _circleplayer;
	    //---------------------------------------	
	    int _dunks = 0;
	    int _last_anim = -1;
	    //---------------------------------------	
	    public float _score = 0f;
	    //---------------------------------------	
	    hud_control _hud;
	    _game_options _gameopt;
	    _design_control _level_op;
	    Rect _pause_zone = new Rect();
	    //---------------------------------------	
	    _basket_trigger[] _trigg_tmp = new _basket_trigger[2];
	    //---------------------------------------
	    //---------------------------------------	
	    public void _addtriggers(int _i,_basket_trigger _bt){
		    _trigg_tmp [_i] = _bt;
	    }

	    //---------------------------------------	
	    void _resettriggerbasket(){
		    _trigg_tmp [0]._reset ();
		    _trigg_tmp [1]._reset ();
	    }
	    //---------------------------------------	

	    public void _destroyball(GameObject ball){
		    if (_touch_mode) {
			    Destroy (GameObject.Find ("CTouch"));
		    }
		    _resettriggerbasket ();

		    if (ball) {
			    Destroy (ball.GetComponent<_ball> ()._shadow.gameObject);
			    Destroy (ball);
                //shotEnabled = true;
                //gem.SetActive(true);
                //GameObject.Find("Ball Ready").GetComponent<Text>().color = Color.green;
                
		    }

	    }

	    //---------------------------------------	
	    public void _startupdate(Transform _b,Sprite _circle = null){
		    _basketgo = _b;
	    }

        //---------------------------------------	
    
        public void _chek_basket_dats()
        {

            // ONLY FOR CLOTH PHYSICS
            //---------------------------------------
            if (GameObject.Find("_basket_add"))
            {
                _basket = GameObject.Find("_basket_add").GetComponent<Cloth>();
            }
            //---------------------------------------

            if (_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._basket_animation_list.Length > 0)
            {
                _basket_animations();
            }

        }

        //---------------------------------------	

        public void _destroyballobstacles(GameObject ball)
        {

            _resettriggerbasket();
            if (ball)
            {
                _particle_miss(ball.transform.position);

                Destroy(ball.GetComponent<_ball>()._shadow.gameObject);
                Destroy(ball);
            }
           

            /*if (!_Game_Control.instance._is_gameover)
            {
                createBall();
            }*/
        }
        //---------------------------------------	
        void _change_gravity()
        {
            Physics.gravity = new Vector3(0f, -_level_op._levels[_gameopt._level_p]._gravity, 0f);
        }
        #endregion


    //---------------------------------------	

    void Awake()
	{
        activeShots = new List<Coroutine>();
        //---------------------------------------
		// NORMAL MODE
		//---------------------------------------
		if (!_touch_mode) {

			//---------------------------------------
			// TRAJECTORY POINTS
			//---------------------------------------
			trajectoryPoints = new List<GameObject>();
			GameObject _parent_tp = new GameObject();
			_parent_tp.name = "Trajectory_Points";
			_parent_tp.transform.position = new Vector3(0,0,0);

			for(int i=0;i<TrajectoryPoints;i++)
			{
				GameObject D =  Instantiate(TrajectoryPointPrefab);
				trajectoryPoints.Insert(i,D);
				D.transform.parent = _parent_tp.transform;
			}
			//---------------------------------------
		}
		_gameopt = GameObject.Find ("_GameControl").GetComponent<_game_options>();
		_level_op = GameObject.Find ("_GameControl").GetComponent<_design_control>();
		_hud = GameObject.Find ("_GameControl").GetComponent<hud_control>();
		_circleplayer = GameObject.Find ("_CirclePlayer").transform;
        //-----------------------------------------

        MyInstantiate();

		//---------------------------------------
		_change_gravity ();
		_pause_zone = new Rect(Screen.width/1.1f, 0, Screen.width, Screen.height/6);
        //---------------------------------------
        if (timeAttack)
            StartCoroutine(ShotDisplayMessage("Attempt A Shot To Begin!"));
	}

    private void Start()
    {
        if (timeAttack)
            GameObject.Find("Time").GetComponent<Text>().text = "Q1\tTime: " + TimeController.timeSetting + "s";
        else
            GameObject.Find("Time").GetComponent<Text>().text = "";
    }

    //---------------------------------------

    private void OnDestroy()
    {

        if (!discoverMode)
        {
            if (shotsTaken > 5)//adding sessions played to make achievements cohesive
                PlayerPrefs.SetInt("sessions", PlayerPrefs.GetInt("sessions") + 1);
            PlayerPrefs.SetInt("cash", cash + sessionCash);

            PlayerPrefs.SetInt("shotsMade", PlayerPrefs.GetInt("shotsMade") + shotsMade);
            
            PlayerPrefs.SetInt("totalGreens", PlayerPrefs.GetInt("totalGreens") + greenCount);
            greenCount = 0;
            PlayerPrefs.SetInt("shotsTaken", PlayerPrefs.GetInt("shotsTaken") + shotsTaken);
            PlayerPrefs.SetInt("missCount", PlayerPrefs.GetInt("missCount") + missCount);
            missCount = 0;

        }

        if (release != null)
        {
            release.text = "Relese:";
            offBy.text = "Off By: ";
            chance.text = "Chance of Making:";
            value.text = "Release: ";
            avg.text = "Average: ";
            GameObject.Find("Cash").GetComponent<Text>().text = "$0";
            shotStats.text = "0/0";
            shotStats2.text = "0%";
            GameObject.Find("Green Streak").GetComponent<Text>().text = "Green Streak: 0";
            GameObject.Find("Shot Streak").GetComponent<Text>().text = "Shot Streak: 0";
        }
        //some issue seems to be happening when the file is made. Don't know if this has anything to do with the discoverMode Glitch yet
        if (!discoverMode)
        {
            #region makeFile           
            float average;
            if (shotChanged) //erase statistics if the shot was changed
            {
                if (PlayerPrefs.HasKey("early"))
                {
                    PlayerPrefs.DeleteKey("early");
                    PlayerPrefs.DeleteKey("late");
                    PlayerPrefs.DeleteKey("inWindow");
                    PlayerPrefs.DeleteKey("perfect");
                    PlayerPrefs.DeleteKey("releases");
                    PlayerPrefs.DeleteKey("shotsOnShot");
                    PlayerPrefs.DeleteKey("makesOnShot");
                }
            }
            else
            {
                if (avgAgg > 0)
                {
                    average = (SumList(releases) + avgAgg) / (PlayerPrefs.GetInt("shotsOnShot") + shotsTaken);
                }
            }
            
            average = AverageList(releases);
            if (shotsTaken != 0)
            {
                PlayerPrefs.SetString("shootingStats", "Average Release: " + average + "ms\nEarly: " + (PlayerPrefs.GetInt("early") + early) + "\nLate: " + (PlayerPrefs.GetInt("late") + late) + "\nIn Window: " +
                    (PlayerPrefs.GetInt("inWindow") + inWindow) + "\nPerfect Green: " + (PlayerPrefs.GetInt("perfect") + perfect) + "\nShots Made / Taken: " + (PlayerPrefs.GetInt("makesOnShot") + shotsMade) + "/" + (PlayerPrefs.GetInt("shotsOnShot") + shotsTaken)
                    + "\t" + Mathf.Round(((float)shotsMade / shotsTaken) * 100)+"%");

                
                PlayerPrefs.SetInt("early", PlayerPrefs.GetInt("early") + early);
                PlayerPrefs.SetInt("late", PlayerPrefs.GetInt("late") + late);
                PlayerPrefs.SetInt("inWindow", PlayerPrefs.GetInt("inWindow") + inWindow);
                PlayerPrefs.SetInt("perfect", PlayerPrefs.GetInt("perfect") + perfect);
                PlayerPrefs.SetInt("shotsOnShot", PlayerPrefs.GetInt("shotsOnShot") + shotsTaken);
                PlayerPrefs.SetInt("makesOnShot", PlayerPrefs.GetInt("makesOnShot") + shotsMade);
                PlayerPrefs.SetFloat("releaseSum", SumList(releases) +avgAgg);

            }
            #endregion
        }
        if (value)
        {
            value.gameObject.SetActive(true);
            offBy.gameObject.SetActive(true);
            chance.gameObject.SetActive(true);
            shotStats.gameObject.SetActive(true);
            shotStats2.gameObject.SetActive(true);
        }
   
        PlayerPrefs.Save();
        StopAllCoroutines();
        if(ButtonMaster.tipsP)
            ButtonMaster.tipsP.SetActive(false);
        if(ball!=null)
            foreach (GameObject obj in ball)
                if (obj)
                    Destroy(obj);
        if(ballUsing != null)
            foreach (GameObject obj in ballUsing)
                if (obj)
                    Destroy(obj);
        ball = null;
        ballUsing.Clear();
    }

    //---------------------------------------

    private void MyInstantiate()
    {
        if(longRange)
            _audio_control.instance.goodClips[14] = Resources.Load<AudioClip>("sounds/good/3b for meb");
        else
            _audio_control.instance.goodClips[14] = Resources.Load<AudioClip>("sounds/good/2b for youb");
        ball = new GameObject[10];
        TimeController.gameover = false;
        cash = PlayerPrefs.GetInt("cash");
        _hud._objects_hud_control[2].SetActive(false);
        shotEnabled = true;
        shotBar.UpdateColor(barColor);
        if(PlayerPrefs.GetInt("shotBar",1) == 0)
        {
            barActive = false;
            barHolder.SetActive(false);
        }
        else
        {
            barActive = true;
            barHolder.SetActive(true);
        }
        release = GameObject.FindWithTag("release").GetComponent<Text>();
        backHud = GameObject.Find("_backhud");
        avg = GameObject.FindWithTag("avg").GetComponent<Text>();
        if(discoverMode)
            avg.text = "";
        if (!shotChanged && !discoverMode) //loading releases so avg continues
            LoadReleases();
        else
            releases = new List<float>(); // update for grabbing saved data<--done 10/31/18
        if (!discoverMode)
        {
            if (releases.Count > 0)
                avg.text = "Avg. Release: " + AverageList(releases);
        }      
        else
            avg.text = "";
        player = GameObject.Find("_In_Game").GetComponent<AudioSource>(); //source for my custom sounds over originals
        if (player == null)
            Debug.LogError("player null");

        if (PlayerPrefs.GetInt("X") == 0)
            xActive = false;
        else
            xActive = true;
        if (!xActive)
        {
            xButton.SetActive(false);
            canvas.SetActive(true);
            if (PlayerPrefs.HasKey("button.x"))
            {
                canvas.transform.position = new Vector3(PlayerPrefs.GetFloat("button.x"), PlayerPrefs.GetFloat("button.y"), 0);
            }
        }          
        else
        {
            xButton.SetActive(true);
            if (PlayerPrefs.HasKey("button.x"))
            {
                xButton.transform.position = new Vector3(PlayerPrefs.GetFloat("button.x"), PlayerPrefs.GetFloat("button.y"), 0);
            }
            canvas.SetActive(false);
        }

        if (!shotButtonToggler)
        {
            shotButtonToggler = GameObject.Find("Shot Button Toggle").GetComponent<Button>(); //consider which buttons should be here. Could switch some to inspector<--player made at runtime so impossible
            shotButtonToggler.onClick.AddListener(() => { ToggleShotButton(); });
            GameObject.Find("X Move Toggle").GetComponent<Button>().onClick.AddListener(() => { ToggleMoveShotButton(0); });
            shotDisplay = GameObject.FindWithTag("shotDisplay").GetComponent<Text>();
            value = GameObject.Find("Shot Value").GetComponent<Text>();
            offBy = GameObject.FindWithTag("offGreen").GetComponent<Text>();
            chance = GameObject.FindWithTag("chance").GetComponent<Text>();
            shotStats = GameObject.Find("Shot Percentage").GetComponent<Text>();
            shotStats2 = GameObject.Find("Shot Percentage %").GetComponent<Text>();
            annoucement = GameObject.Find("Announcement").GetComponent<Text>();
            shotDisplay.fontSize = 45;      
        }
        if(!gem)
            gem = GameObject.Find("Ready Gem");
        ballUsing = new List<GameObject>();

        if (discoverMode)
        {
            value.gameObject.SetActive(false);
            offBy.gameObject.SetActive(false);
            chance.gameObject.SetActive(false);
            shotDisplay.text = "Take 10 Shots. Don't Worry About The Shot Bar, Just Try To Get Similar Releases";
            shotStats.gameObject.SetActive(false);
            shotStats2.gameObject.SetActive(false);
            GameObject.Find("Green Streak").GetComponent<Text>().text = "";
        }
        else
        {
            CalculateGreenWindow(rLong, rMid);
            //addCash = new List<List<int>>();
            if (longRange)
            {
                int midGreen = (trueLongMin + trueLongMax) / 2;
                shotDisplay.text = shot.displayName + "\tLong:" + rLong + "\nGreen Window: " + trueLongMin + " - " + trueLongMax + "ms\nPerfect Green Window: " + (midGreen - shot.pureGreen) + " - " +
                    (midGreen + shot.pureGreen) + "ms";
            }
            else
            {
                int midGreen = (trueMidMin + trueMidMax) / 2;
                shotDisplay.text = shot.displayName + "\tMid:" + rMid + "\nGreen Window: " + trueMidMin + " - " + trueMidMax + "ms\nPerfect Green Window: " + (midGreen - shot.pureGreen) + " - "
                    + (midGreen + shot.pureGreen) + "ms";
            }
        }
        if (timeAttack)
        {
            GameObject.Find("Cash Add").GetComponent<Text>().color = Color.black;
            GameObject.Find("Cash").GetComponent<Text>().color = Color.black;
            GameObject.Find("Cash").GetComponent<Text>().text = "Score:\t\t0";
        }
        else
        {
            GameObject.Find("Cash Add").GetComponent<Text>().color = Color.green;
            GameObject.Find("Cash").GetComponent<Text>().color = Color.green;
            GameObject.Find("Cash").GetComponent<Text>().text = "$0";
        }
        _audio_control.instance.PlayInGame();
    }
    
    //---------------------------------------	
    void Update () 
	{
        #region NewShooting
        #region shot down
        if(shotEnabled && !pressed && ((xActive && Input.GetButtonDown("Jump")) || (!xActive && !pressed && ButtonMaster.instance.stick.Vertical <= -.4)) && !moving &&!TimeController.gameover )
        {
            if (!ButtonMaster.playActive)
            {
                StartCoroutine(ButtonMaster.DisplayTip("Sorry You're Out Of Shots Today :(. Have No Fear, You Can Earn More By Watching An Ad! (Main Menu)", false, true));
                return;
            }
            if (PlayerPrefs.GetInt("tracking", 0) == 0)
                ButtonMaster.TrackTheBalls();
            else if (ButtonMaster.shotsLeft <= 0)
            {
                if (regularMode || justEnded)
                {
                    StartCoroutine(ButtonMaster.DisplayTip("Sorry You're Out Of Shots Today :(. Have No Fear, You Can Earn More By Watching An Ad! (Main Menu)", false, true));
                    ButtonMaster.playActive = false;
                    //if (!PlayerPrefs.HasKey("coolEnd"))
                      //  PlayerPrefs.SetString("coolEnd", end.ToString());
                    //StartCoroutine(ButtonMaster.RefreshCountDown());
                    shotEnabled = false;
                    return;
                }
            }
            if (timeAttack && !TimeController.started)
            {
                //TimeController.startTime = Time.time;
                TimeController.gameTime = StartCoroutine(GameObject.Find("Time").GetComponent<TimeController>().CountItDown());
                TimeController.started = true;
            }

            ButtonMaster.shotsLeft--;
            PlayerPrefs.SetInt("remaining", ButtonMaster.shotsLeft);
            mouseDown = Time.time;
            pressed = true;
            shotEnabled = false;
            gem.SetActive(false);
            GameObject.Find("Ball Ready").GetComponent<Text>().color = Color.red;
        }
        
        #endregion
        #region update bar
        if (ButtonMaster.playActive && barActive)
        {
            if (pressed && !moving && !TimeController.gameover)
            {
                if (discoverMode)
                {
                    release.text = System.Math.Round((Time.time - mouseDown) * 1000, 1).ToString();
                }
                else
                {
                    if (longRange)
                    {
                        if ((Time.time - mouseDown) * 1000 < trueLongMin)
                            shotBar.UpdateBar((Time.time - mouseDown) * 1000, trueLongMin);
                        else if ((Time.time - mouseDown) * 1000 > trueLongMax)
                            shotBar.UpdateBar(trueLongMin - (((Time.time - mouseDown) * 1000) - trueLongMax), trueLongMin); //consider changing all shot times to floats 9/30/18
                    //^^ this is conversion for the bar so it has the same rate whether it is approaching or leaving the green window. all based on the "scale" of the min green. the time in window is ignored
                    }
                    else //midrange window
                    {
                        if ((Time.time - mouseDown) * 1000 < trueMidMin)
                            shotBar.UpdateBar((Time.time - mouseDown) * 1000, trueMidMin);
                        else if ((Time.time - mouseDown) * 1000 > trueMidMax)
                            shotBar.UpdateBar(trueMidMin - (((Time.time - mouseDown) * 1000) - trueMidMax), trueMidMin);
                    }
                }
            }
        }

        #endregion
        #region shot up
        if (pressed && !moving && ((xActive && Input.GetButtonUp("Jump")) || (!xActive && ButtonMaster.instance.stick.Vertical > -.4)) && !TimeController.gameover)
        {
            if (!ButtonMaster.playActive)
                return;
            mouseUp = Time.time;
            pressed = false;
            shotEnabled = false;
            gem.SetActive(false);
            GameObject.Find("Ball Ready").GetComponent<Text>().color = Color.red;
            StartCoroutine(CreateBallInSeconds(.25f));
            shotsTaken++;
            if (regularMode && shotsTaken % 37 == 0)
                ButtonMaster.ShowBannerAd();
            if (ballUsing.Count == 0 || ballUsing[ballUsing.Count-1] != ball[currentBall] )
                //if either the last ball shot doesn't exist or it doesn't equal the last ball add it to the list
                ballUsing.Add( ball[currentBall]);
            bool green = false;
            GameObject thisBall = ballUsing[ballUsing.Count - 1];
            releases.Add(((mouseUp - mouseDown) * 1000));
            float average = AverageList(releases);

            if (discoverMode)
            {
                
                if (releases.Count == 10)
                {                  
                    FindShot();
                    releases.Clear();
                    ButtonMaster.ShowBannerAd();
                    if(ButtonMaster.shotsLeft <= 0)
                    {
                         //StartCoroutine(ButtonMaster.DisplayTip("Sorry You're Out Of Shots Today :(. Have No Fear, You Can Earn More By Watching An Ad! (Main Menu)", false, true));
                         ButtonMaster.playActive = false;
                        System.DateTime end = System.DateTime.Now.AddDays(1);
                        //if(!PlayerPrefs.HasKey("coolEnd"))
                          //  PlayerPrefs.SetString("coolEnd", end.ToString());
                        //StartCoroutine(ButtonMaster.RefreshCountDown());
                        shotEnabled = false;
                        justEnded = true;
                    }

                    if (ball != null)
                        foreach (GameObject obj in ball)
                            if (obj)
                                Destroy(obj);
                    if (ballUsing != null)
                        foreach (GameObject obj in ballUsing)
                            if (obj)
                                Destroy(obj);
                    ball = new GameObject[10];
                    ballUsing.Clear();
                    return;
                }
                else //shot goes in regardless in this mode
                {
                    shotsMade++;
                    GameObject.Find("Shot Streak").GetComponent<Text>().text = shotsMade.ToString();
                    release.text = "Release: " + System.Math.Round((mouseUp - mouseDown) * 1000, 1) + "ms";
                    avg.text = "Average Release: " + System.Math.Round(average, 1) + "ms";
                    
                    Vector3 vel = GetForce(this.transform.position, new Vector3(-1.1f, 5.6f, -1.3f),thisBall);
                    this.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg);
                    setTrajectory(this.transform.position, vel / thisBall.GetComponent<Rigidbody>().mass, new Vector3(.3f, -5.9f, -1.3f));
                    _force = vel;
                    throwBall(thisBall);
                    StartCoroutine(UpdateDisplay(thisBall,0,0,0,0,0,0));
                }
                
            }
            else
            {               
                if (longRange)
                {
                    if (average >= trueLongMin && average <= trueLongMax)
                        avg.color = Color.green;
                    else
                        avg.color = Color.yellow;

                    if ((mouseUp - mouseDown) * 1000 < trueLongMin) //early
                    {
                        lastShot.UpdateBar((mouseUp - mouseDown) * 1000, trueLongMin);
                        lastShot.UpdateColor(Color.yellow);
                        offBy.text = "Early by: " + Mathf.Round((trueLongMin - ((mouseUp - mouseDown) * 1000))) + "ms";
                        value.color = Color.yellow;
                        value.text = "Release: Early";
                    }
                    else if ((mouseUp - mouseDown) * 1000 > trueLongMax)//late
                    {
                        lastShot.UpdateBar(trueLongMin - ((mouseUp - mouseDown) * 1000 - trueLongMax), trueLongMin);
                        lastShot.UpdateColor(Color.yellow);
                        offBy.text = "Late by: " + Mathf.Round(((((mouseUp - mouseDown) * 1000) - trueLongMax))) + "ms";
                        value.color = Color.yellow;
                        value.text = "Release: Late";
                    }
                    else//green or white
                    {
                        green = true;
                    }
                }
                else
                {
                    if (average >= trueMidMin && average <= trueMidMax)
                        avg.color = Color.green;
                    else
                        avg.color = Color.yellow;

                    if ((mouseUp - mouseDown) * 1000 < trueMidMin) //early
                    {
                        lastShot.UpdateBar((mouseUp - mouseDown) * 1000, trueMidMin);
                        lastShot.UpdateColor(Color.yellow);
                        offBy.text = "Early by: " + System.Math.Round((trueMidMin - ((mouseUp - mouseDown) * 1000)), 3) + "ms";
                        value.color = Color.yellow;
                        value.text = "Release: Early";
                    }
                    else if ((mouseUp - mouseDown) * 1000 > trueMidMax)//late
                    {
                        lastShot.UpdateBar(trueMidMin - ((mouseUp - mouseDown) * 1000 - trueMidMax), trueMidMin);
                        lastShot.UpdateColor(Color.yellow);
                        offBy.text = "Late by: " + System.Math.Round(((((mouseUp - mouseDown) * 1000) - trueMidMax)), 3) + "ms";
                        value.color = Color.yellow;
                        value.text = "Release: Late";
                    }
                    else//green
                    {
                        green = true;
                    }
                }

                if (!green)
                {
                    release.color = Color.yellow;
                }

                avg.text = "Average Release: " + System.Math.Round(average, 1) + "ms";
                pressed = false;
                shotEnabled = false;
                gem.SetActive(false);
                GameObject.Find("Ball Ready").GetComponent<Text>().color = Color.red;
                release.text = "Release: " + System.Math.Round((mouseUp - mouseDown) * 1000, 1) + "ms";

                ShootBall(mouseUp - mouseDown,thisBall);
                
            }

            #endregion
        }
        #endregion

        #region escape sequences

        if (!TimeController.ending && Input.GetKeyUp(KeyCode.Escape) && !moving && !_pausegame.unpaused && ballUsing.Count==0) //need to look into adding more of these for certain options
        {
            _Game_Control.instance._main_menu();
        }

        if(Input.GetKeyUp(KeyCode.Escape) && moving)
        {
            ToggleMoveShotButton(0);
        }

        
        
        #endregion
    }
    //---------------------------------------
    #region makeormiss
    public void ShootBall(float release, GameObject thisBall)
    {
        //jumpshot will come from player choice, for testing  it will be a set time KD release
        //ranges and moving must be added as well here
        release *= 1000; //convert ms release into an int value
        int shotCash = 0;
        int shotScore = 0;
        bool inWindow = (longRange && release >= trueLongMin && release <= trueLongMax) || (!longRange && release >= trueMidMin && release <= trueMidMax);
        if ( inWindow && (!normalMode || Green(release)))
        {
            //green release
            //editing for perfect green vs potential green
            this.inWindow++;
            green = true;
            greenCount++;
            _Player.release.color = Color.green;
            chance.color = Color.green;
            chance.text = "Chance of Making: 100%";
            if (barActive)
            {
                shotBar.UpdateColor(Color.green);
                lastShot.UpdateBar(1, 1);
                lastShot.UpdateColor(Color.green);
            }
            offBy.text = "Off by: Nada!";
            offBy.color = Color.green;
            value.color = Color.green;
            value.text = "Release: Excellent";
            Vector3 vel = GetForce(this.transform.position, new Vector3(-1.1f, 5.6f, -1.3f), thisBall);
            this.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg);
            setTrajectory(this.transform.position, vel / thisBall.GetComponent<Rigidbody>().mass, new Vector3(.3f, -5.9f, -1.3f)); // replace _camera_return with set vector3
            _force = vel;
            throwBall(thisBall);
            shotsMade++;
            shotStreak++;
            greenStreak++;
            _achievements_config.instance._check(_conditions.In_Total_Game, _achievement_type.Greens, PlayerPrefs.GetInt("totalGreens") + greenStreak);
            List<int> temp = new List<int>();
            if (greenStreak >= 5)
            {
                sessionCash += greenStreak * 20;
                shotCash += greenStreak * 20;
                shotScore += greenStreak * 100;
            }               
            if (shotStreak >= 5)
            {
                sessionCash += shotStreak * 10;
                shotCash += shotStreak * 10;
                shotScore += shotStreak * 50;
            }     
            sessionCash += 50;
            shotCash += 50;
            shotScore += 1000;
            //if(activeShots.Count != 0)
                StartCoroutine(UpdateDisplay(thisBall,shotScore,shotCash,shotsTaken,shotsMade,shotStreak,greenStreak));
            //else
            //   StartCoroutine(UpdateDisplay(thisBall, shotScore, shotCash, 0));
        }
        else
        {
            //yellow release with success based on % and random
            //added full white release 10/20/18
            greenStreak = 0;
            float discrepancy=0;
            if (inWindow)
            {
                this.inWindow++;
                chance.color = Color.white;
                if (barActive)
                {
                    shotBar.UpdateColor(Color.white);
                    lastShot.UpdateBar(98, 100);
                    lastShot.UpdateColor(Color.white);
                }                
                _Player.release.color = Color.white;
                chance.color = Color.white;
                offBy.color = Color.white;
                value.color = Color.white;
                value.text = "Release: Full White";
                Vector3 vel;
                float window=0;
                if (longRange)
                {
                    int longAvg = (trueLongMax + trueLongMin)/2;
                    if (release < longAvg)
                        discrepancy = (longAvg - shot.pureGreen) - release;
                    else
                        discrepancy = release - (longAvg+ shot.pureGreen);
                    window = trueLongMax - longAvg;
                }
                else
                {
                    int midAvg = (trueMidMax + trueMidMin)/2;
                    if (release < midAvg)
                        discrepancy = (midAvg - shot.pureGreen-Random.Range(2,5)) - release;
                    else
                        discrepancy = release - (midAvg+  shot.pureGreen+Random.Range(2,5));
                    window = trueMidMax - midAvg;
                }
                offBy.text = "Off Perfect by: " + Mathf.Abs((float)System.Math.Round(discrepancy,2))+ "ms";
                if (WhiteMade(discrepancy,window))
                {
                    sessionCash += 40;
                    shotCash += 40;
                    shotScore += 750;
                    shotStreak++;
                    if (shotStreak >= 5)
                    {
                        sessionCash += shotStreak * 10;
                        shotCash += shotStreak * 10;
                        shotScore += shotStreak * 50;
                    }
                    //if (activeShots.Count != 0)
                        
                    //else
                        //StartCoroutine(UpdateDisplay(thisBall, shotScore, shotCash, 0));
                    vel = GetForce(this.transform.position, new Vector3(-1.1f, 5.6f, -1.3f),thisBall);
                    shotsMade++;
                    StartCoroutine(UpdateDisplay(thisBall, shotScore, shotCash, shotsTaken, shotsMade, shotStreak, greenStreak));
                }
                else
                {
                    vel = GetForce(this.transform.position, new Vector3(1.5f, 8.4f, -1.3f),thisBall);
                    shotStreak = 0;
                    StartCoroutine(MissDisplay(thisBall,shotsTaken,shotsMade));
                }
                    
                this.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg);
                setTrajectory(this.transform.position, vel / thisBall.GetComponent<Rigidbody>().mass, new Vector3(.3f, -5.9f, -1.3f)); // replace _camera_return with set vector3
                _force = vel;
                throwBall(thisBall);
                
            }
            else
            {
                chance.color = Color.yellow;
                offBy.color = Color.yellow;
                bool early;
                if (longRange)
                {
                    if (release < trueLongMin)
                    {
                        early = true;
                        this.early++;
                    }
                    else
                    {
                        early = false;
                        late++;
                    }
                        
                    if (early)
                        discrepancy = trueLongMin - release;
                    else
                        discrepancy = release - trueLongMax;
                }
                else
                {
                    if (release < trueMidMin)
                    {
                        early = true;
                        this.early++;
                    }
                    else
                    {
                        early = false;
                        late++;
                    }
                    if (early)
                        discrepancy = trueMidMin - release;
                    else
                        discrepancy = release - trueMidMax;
                }
                Vector3 vel;
                if (ShotMade(discrepancy))
                {
                    sessionCash += 25;
                    shotCash += 25;
                    shotScore += 500;
                    shotStreak++;
                    if (shotStreak >= 5)
                    {
                        sessionCash += shotStreak * 10;
                        shotCash += shotStreak * 10;
                        shotScore += 50 * shotStreak;
                    }      
                    vel = GetForce(this.transform.position, new Vector3(-1.1f, 5.6f, -1.3f),thisBall); 
                    // here is where the make or miss anim happens. _camera_return replaced by set vector3 that makes shot
                    shotsMade++;
                    
                        StartCoroutine(UpdateDisplay(thisBall, shotScore, shotCash,shotsTaken,shotsMade,shotStreak,greenStreak ));
                  //  else
                       // StartCoroutine(UpdateDisplay(thisBall, shotScore, shotCash, 0));
                }
                else if (discrepancy <= 25)
                {
                    vel = GetForce(this.transform.position, new Vector3(1.5f, 8.4f, -1.3f),thisBall);
                    shotStreak = 0;
                    StartCoroutine(MissDisplay(thisBall,shotsTaken,shotsMade));
                    //money.text = "S" + sessionCash;
                    //moneyAdd.text = "";
                }
                else
                {
                    airball = true;
                    vel = GetForce(this.transform.position, new Vector3(1.8f, 8.3f, -1.3f),thisBall);
                    shotStreak = 0;
                    StartCoroutine(MissDisplay(thisBall, shotsTaken,shotsMade));
                    //money.text = "S" + sessionCash;
                    //moneyAdd.text = "";
                }    
                this.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg);
                setTrajectory(this.transform.position, vel / thisBall.GetComponent<Rigidbody>().mass, new Vector3(.3f, -5.9f, -1.3f)); // replace _camera_return with set vector3
                _force = vel;
                throwBall(thisBall);
            }
        }
    }

    //---------------------------------------

    private bool WhiteMade(float disc, float window)
    {
        float g50 = Random.Range(1, window);
        //shotMade = true;
        if (disc / window < .5 && g50 > disc)
        {
            chance.text = "Chance of Making: "+ System.Math.Round(((window-disc)/window)*100,2) + "%";
            shotMade = true;
            return true;
        }    
        else if (g50 > window / 2)
        {
            float h = (float)System.Math.Round((((window - disc) / window) + .5f)*100 - (.5f * ((window - disc) / window))*100,2);
            if (disc/window>.5)
                chance.text = "Chance of Making: " + h + "%";
            else
                chance.text = "Chance of Making: 50%";
            shotMade = true;
            return true;
        }
        else
        {
            chance.text = "Chance of Making: " + (int)Random.Range(0,49) + "%";
            shotMade = false;
            return false;
        }
            
    }

    //---------------------------------------

    private bool Green(float release)
    {
        float lWin, mWin;   
        lWin = shot.pureGreen;
        mWin = shot.pureGreen+Random.Range(2,4);
        if (longRange)
        {
            int longAvg = (trueLongMax + trueLongMin)/2;
            if (release >= longAvg -lWin && release < longAvg+lWin) // perfect green
            {
                perfect++;
                shotMade = true;
                return true;
            }
            else
            {
                float disc;
                if (release < longAvg)
                    disc = (longAvg - lWin) - release;
                else
                    disc = release - (longAvg + lWin);
                if (Random.Range(0, trueLongMax - longAvg) < disc)
                {
                    //Debug.Log(r + " " + disc);
                    shotMade = true;
                    return true;
                }
                else
                    return shotMade =  false;
            }
        }
        else
        {
            int midAvg = (trueMidMax + trueMidMin) / 2;
            if (release >= midAvg - mWin && release < midAvg + mWin) // perfect green
            {
                perfect++;
                shotMade = true;
                return true;
            }
            else
            {
                float disc;
                if (release < midAvg)
                    disc = (midAvg - mWin) - release;
                else
                    disc = release - (midAvg + mWin);
                if (Random.Range(0, trueMidMax - midAvg) < disc)
                {
                    shotMade = true;
                    return true;
                }
                else
                    return shotMade = false;
            }
        }
    }
    //-----------------------------------------
    
    float AverageList(List<float> nums)
    {
        if (nums.Count == 0)
            return 0;
        float avgRelease= 0;
        foreach(float release in nums)
        {
            avgRelease += release;
        }
        if (avgAgg > 0)
        {
            avgRelease += avgAgg;
            avgRelease /= (nums.Count + PlayerPrefs.GetInt("shotsOnShot"));
        }
        else
            avgRelease /= nums.Count;
        return (float)System.Math.Round(avgRelease,1);
    }

    float SumList(List<float> nums)
    {
        if (nums.Count == 0)
            return 0;
        float avgRelease = 0;
        foreach (float release in nums)
        {
            avgRelease += release;
        }
        return avgRelease;
    }

   //----------------------------------------

    private bool ShotMade(float discrepancy) // take the discrepancy between the green window and the release. for every 5ms off, the random % of making the shot goes down by 10%
    {
        int dice;
        if(!normalMode)
            discrepancy -= 5;
        if (discrepancy <= 2.5)
        {
            chance.text = "Chance of Making: " +(dice= Random.Range(96,100)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 5)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(91, 95)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 7.5)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(86, 90)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 10)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(81, 85)) + "%";
            return Random.Range(0, 9) <= dice;    
        }
        else if (discrepancy <= 12.5)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(76, 80)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 15)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(71, 75)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 17.5)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(66, 70)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 20)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(61, 65)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 22.5)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(56, 60)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 25)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(51, 55)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 27.5)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(46, 50)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 30)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(41, 45)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 32.5)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(36, 40)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 35)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(31, 35)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 37.5)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(26, 30)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 40)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(21, 25)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 42.5)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(16, 20)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 45)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(11, 15)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 47.5)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(6, 10)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else if (discrepancy <= 50)
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(1, 5)) + "%";
            return Random.Range(0, 99) <= dice;
        }
        else
        {
            chance.text = "Chance of Making: " + (dice = Random.Range(0,1)) + "%";
            return false;
        }
    }
#endregion
    //---------------------------------------------------

    #region Toggles

    void ToggleShotButton()
    {
        if (moving || _pausegame.paused || TimeController.gameover)
            return;

        if (xActive)
        {
            xButton.SetActive(false);
            canvas.SetActive(true);
            xActive = false;
            shotButtonToggler.GetComponentInChildren<Text>().text = "Button";
            PlayerPrefs.SetInt("X", 0);
        }
        else
        {
            xButton.SetActive(true);
            canvas.SetActive(false);
            xActive = true;
            shotButtonToggler.GetComponentInChildren<Text>().text = "JoyStick";
            PlayerPrefs.SetInt("X", 1);
        }
    }


    public void ToggleMoveShotButton(int choice)
    {
        if (!shotEnabled  || _pausegame.paused || TimeController.gameover)
            return;
        if (!moving)
        {
            /*if (!xActive)
            {
                StartCoroutine(ShotDisplayMessage("Only The X Button Can Be Moved For Now"));
                return;
            }*/
            GameObject.Find("X Move Toggle").GetComponentInChildren<Text>().text = "Accept";
            shotDisplay.text = "Click One of The Circles To Move The Shot Button There";
            //xMove.SetActive(true);
            xShadow.SetActive(true);
            moving = true;
           // shotEnabled = false;
        }
        else
        {
            GameObject.Find("X Move Toggle").GetComponentInChildren<Text>().text = "Move\nShot\nButton";
            if (longRange)
            {
                int midGreen = (trueLongMin + trueLongMax) / 2;
                shotDisplay.text = shot.displayName + "\tLong:" + rLong + "\nGreen Window:" + trueLongMin + " - " + trueLongMax + "ms\nPerfect Green Window: " + (midGreen - shot.pureGreen) + " - " +
                    (midGreen + shot.pureGreen) + "ms";
            }
            else
            {
                int midGreen = (trueMidMin + trueMidMax) / 2;
                shotDisplay.text = shot.displayName + "\tMid:" + rMid + "\nGreen Window:" + trueMidMin + " - " + trueMidMax + "ms\nPerfect Green Window: " + (midGreen - shot.pureGreen) + " - "
                    + (midGreen + shot.pureGreen) + "ms";
            }
            if (choice != 0)
            {
                xButton.transform.position = GameObject.Find("X" + choice).transform.position;
                canvas.transform.position = GameObject.Find("X" + choice).transform.position;
                PlayerPrefs.SetFloat("button.x", xButton.transform.position.x);
                PlayerPrefs.SetFloat("button.y", xButton.transform.position.y);
                // xShadow.transform.position = xButton.transform.position;
            }
            /*else
            {
                xButton.transform.position = tempPos;
                //GameObject.Find("Joystick Canvas").transform.position = tempPos; updating to move joystick in later builds
                //stick.transform.position = tempPos;
                //stickB.transform.position = tempPos;
            }*/
                
           // xMove.SetActive(false);
            xShadow.SetActive(false);
            moving = false;
            //shotEnabled = true;

        }
    }

    //---------------------------------------
    #endregion

    //---------------------------------------

    #region misc
    void FindShot()
    {
        float avg = AverageList(releases);
        int[] hitKeeper = new int[ButtonMaster.jumpShotData.Count];
        int index = 0, superEarly=0, superLate=0;
        float earliest = 100000,latest = 0;
        foreach(string[] release in ButtonMaster.jumpShotData)
        {
            int hits = 0;
            if (release[3] == "0") //in order to skip custom shots
            {
                float early, late;
                foreach (float r in releases)
                {
                    float.TryParse(release[1], out early);
                    float.TryParse(release[2], out late);
                    if (r >= early && r <= late)
                        hits++;
                    if (early < earliest)
                    {
                        earliest = early;
                        superEarly = index;
                    }
                        
                    if (late > latest)
                    {
                        latest = late;
                        superLate = index;
                    }
                }
                hitKeeper[index] = hits;
                index++;
            }
        }
        index = hitKeeper[0];
        for(int i = 1; i<hitKeeper.Length; i++)
        {
            if (hitKeeper[i] > index)
                index = i;
        }
        if(hitKeeper[index] == 0)
        {
            if (avg < 400)
                index = superEarly;
            else
                index = superLate;
        }
        string[] chosen = ButtonMaster.jumpShotData[index];     
        shotDisplay.text = "Your Suggested Jumpshot is " + chosen[0] + " With Green Window: "+ chosen[1] + "-" + chosen[2] + ". Shoot Another 10 Shots To Try Again";
        PlayerPrefs.SetInt("shotsTaken", PlayerPrefs.GetInt("shotsTaken") + shotsMade);
        shotsMade = 0;
        GameObject.Find("Shot Streak").GetComponent<Text>().text = shotsMade.ToString();
    }

    //---------------------------------------

    void LoadReleases()
    {
        if (PlayerPrefs.HasKey("releaseSum"))
            avgAgg = PlayerPrefs.GetFloat("releaseSum");
        else
            avgAgg = 0;
    }

  
    //---------------------------------------

    //------------------------------------

    private IEnumerator ResetTextInSeconds(Text text, float sec)
    {
        yield return new WaitForSeconds(sec);
        text.text = "";
    }

    private IEnumerator ShotDisplayMessage(string message)
    {
        string temp = shotDisplay.text;
        shotDisplay.text = message;
        yield return new WaitForSeconds(4);
        shotDisplay.text = temp;
    }
    #endregion

    //-------------------------------------------------
    #region Shot Displays
    public IEnumerator UpdateDisplay(GameObject thisBall, int sScore, int sCash, int taken, int made, int sStreak, int gStreak)//show all changes based on making or missing after the ball goes in, adding animations presently
    {
        if (barActive)
        {
            shotBar.UpdateBar(0, 1);
            shotBar.UpdateColor(barColor);
        }
        if (discoverMode)
        {
            yield return new WaitForSeconds(2.3f);
            _destroyballobstacles(thisBall);
            ballUsing.Remove(thisBall);
            yield break;
        }
        if (timeAttack && TimeController.remainingTime > 5)//normal
        {
            timeMultiplier = (TimeController.GetQuarter() * 50) + (TimeController.GetQuarter() * 250 * shotStreak) + (TimeController.GetQuarter() * 500 * greenStreak);
            sScore += (int)timeMultiplier;
        }
        else if (timeAttack && TimeController.remainingTime < 5)//in the clutch
        {
            timeMultiplier = (TimeController.GetQuarter() * 100) + (TimeController.GetQuarter() * 500 * shotStreak) + (TimeController.GetQuarter() * 1000 * greenStreak);
            sScore += (int)timeMultiplier;
        }
        //GameObject effect = Resources.Load<GameObject>("UETools/Magic_Particle_Effects/Effects/Cell_02");
        // GameObject effect = GameObject.Find("Cell_02");
        //bool clipSet = false;
        yield return new WaitForSeconds(2.3f);
        _achievements_config.instance._check(_conditions.In_one_play, _achievement_type.Baskets, shotsMade);
        #region clip
        if (!TimeController.ending)
        {
            string clip = null;
            if (sStreak == 21)
            {
                clip = _audio_control.instance.outliers[7].name;
                _audio_control.PlayClip(_audio_control.instance.outliers[7]);
            }

            else if (gStreak == 10)
            {
                clip = _audio_control.instance.outliers[5].name;
                _audio_control.PlayClip(_audio_control.instance.outliers[5]);
            }
            else if (sStreak == 30)
            {
                clip = _audio_control.instance.outliers[15].name;
                _audio_control.PlayClip(_audio_control.instance.outliers[15]); //testing announcing streaks by number
                clip += " STREAK!!";
                annoucement.text = clip;
                StartCoroutine(ResetTextInSeconds(annoucement, 3));
                yield return new WaitForSeconds(1f);
                _audio_control.PlayClip(_audio_control.instance.outliers[0]);
                clip = null;
            }
            else if (sStreak == 30)
            {
                clip = _audio_control.instance.outliers[15].name;
                _audio_control.PlayClip(_audio_control.instance.outliers[15]); //testing announcing streaks by number
                clip += " STREAK!!!!!!";
                annoucement.text = clip;
                StartCoroutine(ResetTextInSeconds(annoucement, 3));
                yield return new WaitForSeconds(1f);
                _audio_control.PlayClip(_audio_control.instance.outliers[0]);
                clip = null;
            }
            else if (sStreak == 25)
            {
                clip = _audio_control.instance.outliers[14].name;
                _audio_control.PlayClip(_audio_control.instance.outliers[14]);
                clip += " STREAK!!!!!";
                annoucement.text = clip;
                StartCoroutine(ResetTextInSeconds(annoucement, 3));
                yield return new WaitForSeconds(1f);
                _audio_control.PlayClip(_audio_control.instance.outliers[0]);
                clip = null;
            }
            else if (sStreak == 20)
            {
                clip = _audio_control.instance.outliers[13].name;
                _audio_control.PlayClip(_audio_control.instance.outliers[13]);
                clip += " STREAK!!!!";
                annoucement.text = clip;
                StartCoroutine(ResetTextInSeconds(annoucement, 3));
                yield return new WaitForSeconds(1f);
                _audio_control.PlayClip(_audio_control.instance.outliers[0]);
                clip = null;
            }
            else if (gStreak == 5)
            {
                clip = _audio_control.instance.outliers[6].name.ToUpper();
                _audio_control.PlayClip(_audio_control.instance.outliers[6]);
            }
            else if (sStreak == 15)
            {
                clip = _audio_control.instance.outliers[12].name.ToUpper();
                _audio_control.PlayClip(_audio_control.instance.outliers[12]);
                clip += " STREAK!!!";
                annoucement.text = clip;
                StartCoroutine(ResetTextInSeconds(annoucement, 3));
                yield return new WaitForSeconds(1f);
                _audio_control.PlayClip(_audio_control.instance.outliers[0]);
                clip = null;
            }
            else if (sStreak == 10)
            {
                clip = _audio_control.instance.outliers[11].name.ToUpper();
                _audio_control.PlayClip(_audio_control.instance.outliers[11]);
                clip += " STREAK!!";
                annoucement.text = clip;
                StartCoroutine(ResetTextInSeconds(annoucement, 3));
                clip = null;
                yield return new WaitForSeconds(1f);
                _audio_control.PlayClip(_audio_control.instance.outliers[0]);

            }
            else if (sStreak == 5)
            {
                clip = _audio_control.instance.outliers[10].name.ToUpper();
                _audio_control.PlayClip(_audio_control.instance.outliers[10]);
            }
            else if (green && Random.Range(0, 99) < 85)
            {
                clip = _audio_control.instance.PlayGreen().ToUpper();
            }
            else if (Random.Range(0, 9) < 7)
            {
                clip = _audio_control.instance.PlayGood().ToUpper();
            }

            if (clip != null)
            {
                for (int x = 0; x < Random.Range(1, 5); x++)
                    clip += "!";
                annoucement.text = clip.ToUpper();
                StartCoroutine(ResetTextInSeconds(annoucement, 3));
                //Destroy(Instantiate(effect, annoucement.gameObject.transform), 3);
            }
        }
        #endregion

        Text money = GameObject.Find("Cash").GetComponent<Text>();
        Text moneyAdd = GameObject.Find("Cash Add").GetComponent<Text>();
        shotStats.text = made + "/" + taken; // switching to event based so these happen after the ball goes in 10/26/18
        shotStats2.text = System.Math.Round((((float)made / taken) * 100), 1) + "%";
        GameObject.Find("Green Streak").GetComponent<Text>().text = "Green Streak: " + gStreak;
        GameObject.Find("Shot Streak").GetComponent<Text>().text = "Shot Streak: " + sStreak;
        /*if (barActive)
        {
            shotBar.UpdateBar(0, 1);
            shotBar.UpdateColor(barColor);
        }*/

        score += sScore;

        if (timeAttack)
        {
            moneyAdd.text = "+" + sScore;
            money.text = "Score: " + score;
        }
        else
        {
            moneyAdd.text = "+" + sCash;
            money.text = "$" + sessionCash;
        }

        yield return new WaitForSeconds(.1f);
        _achievements_config.instance._check(_conditions.In_one_play, _achievement_type.Money, sessionCash);
        _achievements_config.instance._check(_conditions.In_one_play, _achievement_type.Greens, gStreak);
        moneyAdd.text = "";
        //PlayerPrefs.SetInt("cash", cash+sessionCash);
        green = false;
        shotEnabled = true;
        gem.SetActive(true);
        GameObject.Find("Ball Ready").GetComponent<Text>().color = Color.green;
        //createBall();
        _destroyballobstacles(thisBall);
        ballUsing.Remove(thisBall);
        //activeShots.RemoveAt(0);
    }

    //---------------------------------------

    public IEnumerator MissDisplay(GameObject thisBall, int taken, int made)
    {

        if (barActive)
        {
            shotBar.UpdateBar(0, 1);
            shotBar.UpdateColor(barColor);
        }
        if (discoverMode)
            yield break;
        //GameObject effect = Resources.Load<GameObject>("UETools/Magic_Particle_Effects/Effects/Bubbles_02");
        //GameObject effect = GameObject.Find("Cell_02");
        yield return new WaitForSeconds(2.3f);

        if (!TimeController.ending)
        {
            string clip = null; ;
            if (Random.Range(0, 9) < 7)
            {
                clip = _audio_control.instance.PlayBad(airball).ToUpper();
                for (int x = 0; x < Random.Range(1, 5); x++)
                    clip += " :P";
                annoucement.text = clip;
            }
            if (clip != null)
            {
                StartCoroutine(ResetTextInSeconds(annoucement, 1.75f));
                //GameObject.Destroy(GameObject.Instantiate(effect, annoucement.gameObject.transform), 3);
            }
        }
        airball = false;
        // madePercentage = (shotsMade / shotsTaken) * 100f;
        shotStats.text = made + "/" + taken;
        shotStats2.text = System.Math.Round((((float)made / taken) * 100), 1) + "%";
        GameObject.Find("Green Streak").GetComponent<Text>().text = "Green Streak: " + 0;
        GameObject.Find("Shot Streak").GetComponent<Text>().text = "Shot Streak: " + 0;
        /*if (barActive)
        {
            shotBar.UpdateBar(0, 1);
            shotBar.UpdateColor(barColor);
        }*/

        green = false;
        missCount++;
        _achievements_config.instance._check(_conditions.In_Total_Game, _achievement_type.Baskets, PlayerPrefs.GetInt("missCount") + missCount);
        shotEnabled = true;
        gem.SetActive(true);
        GameObject.Find("Ball Ready").GetComponent<Text>().color = Color.green;
        //createBall();
        _destroyballobstacles(thisBall);
        ballUsing.Remove(thisBall);
    }
    #endregion
    //---------------------------------------

    public void KillThemAll() //destroy all balls after time attack ends
    {
       // foreach (Coroutine routine in activeShots)
         //   StopCoroutine(routine);
        StopAllCoroutines();//testing <-- looks like this works more efficiently
        foreach (GameObject ball in ballUsing)
            Destroy(ball);
    }

    //----------------------------------------

    private void CalculateGreenWindow(int lRate, int mRate) //needs more shooting data. preferably how rating affects green windows<-- done
    {
        //use parameters from research to induce shot success based on 3pt rating
        if (shot.custom)
        {
            trueLongMax = shot.longMax;
            trueLongMin = shot.longMin;
            trueMidMax = shot.midMax;
            trueMidMin = shot.midMin;
            return;
        }

        rLong = lRate;
        rMid = mRate;
        int lateRandom = Random.Range(35, 39), earlyRandom = Random.Range(11, 15);
        //Debug.Log(shot.GetMin() + " " + shot.GetMax());
        //long range

        if (lRate == shot.pullRate)
        {
            trueLongMax = shot.GetMax();
            trueLongMin = shot.GetMin();
        }
        else if (lRate > shot.pullRate)
        {
            int minDown = (int)Random.Range(0, (1.6f * (lRate - shot.pullRate)) * .5f);
            trueLongMax = shot.GetMax() + minDown / 2;
            trueLongMin = shot.GetMin() - minDown / 2;
        }
        else
        {
            int minDown = (int)Random.Range(0, (1.6f * (shot.pullRate - lRate)) * .5f); //reverse to fill in missing 
            if (lRate > 75)
            {
                trueLongMax = shot.GetMax() - minDown / 2;
                trueLongMin = shot.GetMin() + minDown / 2;
            }
            else
            {
                trueLongMax = shot.GetMax() - minDown / 2 - lateRandom;
                trueLongMin = shot.GetMin() + minDown / 2 - earlyRandom;
            }

        }
        //mid range
        int bonus = Random.Range(1, 3);
        if (mRate == shot.pullRate)
        {
            trueMidMax = shot.GetMax() + bonus;
            trueMidMin = shot.GetMin() - bonus;
        }
        else if (mRate > shot.pullRate)
        {
            int minDown = (int)Random.Range(0, (1.6f * (mRate - shot.pullRate)) * .5f);
            trueMidMax = shot.GetMax() + minDown / 2 + bonus;
            trueMidMin = shot.GetMin() - minDown / 2 - bonus;
        }
        else
        {
            int minDown = (int)Random.Range(0, (1.6f * (shot.pullRate - mRate)) * .5f); //reverse to fill in missing 
            if (mRate > 75)
            {
                trueMidMax = shot.GetMax() - minDown / 2 + bonus;
                trueMidMin = shot.GetMin() + minDown / 2 - bonus;
            }
            else
            {
                trueMidMax = shot.GetMax() - minDown / 2 - lateRandom + bonus;
                trueMidMin = shot.GetMin() + minDown / 2 - earlyRandom - bonus;
            }

        }
    }

    //-----------------------------------
    #region predecessors
    Vector3 _camera_return(){
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	//---------------------------------------
    
    public IEnumerator CreateBallInSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
        createBall();
    }
	public void createBall(float _t = 0f)
	{
        if(shotsTaken == 0 || currentBall >=9)
        {
            currentBall = 0;
        }
        else
        {
            currentBall++;
        }
        ball[currentBall] = Instantiate(BallPrefb);
		ball[currentBall].name = "Ball";
		Vector3 pos = transform.position;
		pos.z = 1;
		//---------------------------------------
		if (_touch_mode) {
			pos = new Vector3 (Random.Range(-11,11),8,1);
			ball[currentBall].GetComponent<Rigidbody> ().useGravity = true;
			ball[currentBall].GetComponent<Rigidbody> ().drag = 1.5f;
			ball[currentBall].GetComponent<Rigidbody> ().angularDrag = 1.5f;
			ball[currentBall].GetComponent<CapsuleCollider> ().enabled = true;
			Physics.queriesHitTriggers = true;
			GameObject _c = Instantiate (Resources.Load ("_Circletouch")) as GameObject;
			_c.GetComponent<SpriteRenderer> ().sprite = _Game_Control.instance._sprt_player;
			_c.name = "CTouch";
			_c.GetComponent<_followball> ()._ball = ball[currentBall].transform;
		}
		//---------------------------------------
		ball[currentBall].transform.position = pos;
		ball[currentBall].GetComponent<_ball>()._tball.GetComponent<Renderer> ().material = _Game_Control.instance._ball_material;
        shotEnabled = true;
        GameObject.Find("Ball Ready").GetComponent<Text>().color = Color.green;
        gem.SetActive(true);
        //---------------------------------------
        // ONLY FOR CLOTH PHYSICS
        //---------------------------------------
        if (_basket != null) {
			CapsuleCollider[] tmp = new CapsuleCollider[1];
			tmp[0] = ball[currentBall].GetComponent<CapsuleCollider>();
			_basket.capsuleColliders = tmp;
		}
		//---------------------------------------

		_cparts = true;

		if (!_touch_mode) {
			if (_t == 0f) {
				_moveball = true;
			} else {
				_moveball = false;
				StartCoroutine (_reset_time_ball (_t));
			}

			resetrajectory (); // Reset alpha color
		}
        //---------------------------------------
       
    }
	//---------------------------------------

	IEnumerator _reset_time_ball(float _t){
		yield return new WaitForSeconds (_t);
		_moveball = true;
	}

	//---------------------------------------

	void throwBall(GameObject ball) // adding multiple balls so an array and calls to a specific ball will be necessary
	{
		ball.GetComponent<Rigidbody>().useGravity = true;
		ball.GetComponent<Rigidbody>().AddForce(_force,ForceMode.Impulse);
		ball.GetComponent<CapsuleCollider> ().enabled = true;
	}

	//---------------------------------------	
	Vector3 GetForce(Vector3 fromPos, Vector3 toPos, GameObject ball)
	{
        if (ball)
        {
            Vector3 _t = toPos - fromPos;
            _t.y = _t.y + Physics.gravity.magnitude / 2;
            _t.y = _t.y * ball.GetComponent<Rigidbody>().mass;
            _t.x = _t.x * ball.GetComponent<Rigidbody>().mass;
            return _t;
        }
        else
        {
            return new Vector3(-1,-1,-1);
        }
	}

	//---------------------------------------	
	
	void resetrajectory()
	{
		for (int i = 0 ; i < TrajectoryPoints ; i++)
		{
			trajectoryPoints [i].transform.position = new Vector3 (0,500,0);
			trajectoryPoints[i].GetComponent<_trigger_dot>()._resetalpha();
		}
	}

	//---------------------------------------	
	
	public void _hidden_trajectory()
	{
		if (!_touch_mode) {
			for (int i = 0 ; i < TrajectoryPoints ; i++)
			{
				trajectoryPoints[i].GetComponent<_trigger_dot>()._hiddenfast();
			}
		}
	}


	//---------------------------------------	
	void setTrajectory(Vector3 pStartPosition , Vector3 pVelocity, Vector3 _mouseposition)
	{
		float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
		float angle = Mathf.Rad2Deg*(Mathf.Atan2(pVelocity.y , pVelocity.x));
		float fTime = 0;

		fTime += 0.08f;
		for (int i = 0 ; i < TrajectoryPoints ; i++)
		{
			float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
			float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics.gravity.magnitude * fTime * fTime / 2.0f);
			Vector3 pos = new Vector3(pStartPosition.x + dx , pStartPosition.y + dy ,1.75f);

			trajectoryPoints[i].transform.position = pos;
			trajectoryPoints[i].transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude)*fTime,pVelocity.x)*Mathf.Rad2Deg);
			fTime += 0.08f;
		}
	}
	//---------------------------------------	

	public void newplay(){
		if (ball[0]) {
			StartCoroutine(_createnewball(_gameopt._time_to_reset_ball));
		}

		if (!ball[0]) {
			createBall ();
		}
	}

	//---------------------------------------	
	IEnumerator _createnewball(float _t = 3f){
		yield return new WaitForSeconds(_t);

		if (ball[0]) {
			_particle_miss(ball[0].transform.position);
			_destroyball (ball[0]);
		}

		/*if (!_Game_Control.instance._is_gameover) {
			createBall();
		}*/

	}

	//---------------------------------------

	Vector3 _rand_player_p(){
		//---------------------------------------
		Vector3 _r = this.transform.position;
		//---------------------------------------
		if (_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._player_always_left) {
			_r = new Vector3(Random.Range(-10,-2f),Random.Range(-4,4f),0f);
		} else {
			_r = new Vector3(Random.Range(-10,10f),Random.Range(-4,4f),0f);
		}
		//---------------------------------------
		// CHECK POSITION OBSTACLES
		//---------------------------------------
		for(int i=0;i<_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._obstacles.Length;i++){
		
			//---------------------------------------
			float _x = _level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._obstacles[i]._position_x;
			float _y = _level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._obstacles[i]._position_y;
			//---------------------------------------

			//---------------------------------------
			if (_r.x < _x+1.5f && _r.x > _x-1.5f){
				
				_rand_player_p ();

			}else if (_r.y < _y + 1.5f && _r.y > _y - 1.5f) {
				
				_rand_player_p ();

			}
			//---------------------------------------

		}

		//---------------------------------------

		return _r;
	}

	//---------------------------------------
	public void _change_position(bool _isball){
		
		if (_isball) {
			
			// Move Player Position
			//---------------------------------------
			this.transform.position = _rand_player_p();
			_circleplayer.position = this.transform.position;
			//---------------------------------------

		} else {
			
			// Move Basket Position
			//---------------------------------------

			_basketgo.gameObject.SetActive(false);

			if (_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._player_always_left) {
				_basketgo.position = new Vector3(Random.Range(2,9f),Random.Range(-2,3f),1.78f);
			} else {

				if(this.transform.position.x <=0f){
					_basketgo.position = new Vector3(Random.Range(2,9f),Random.Range(-2,3f),1.78f);
				}else{
					_basketgo.position = new Vector3(Random.Range(-2,-9f),Random.Range(-2,3f),1.78f);
				}

			}

			_basketgo.gameObject.SetActive(true);
			//---------------------------------------

		}

	}
	//---------------------------------------	

	// ADD POINT
	//---------------------------------------	
	
	public void _add_point(){
		
		/*if (_level_op._levels [_gameopt._level_p]._destroy_ball_on_dunk) {
			_destroyball ();
		} else {
			ball.GetComponent<_ball>()._set = true;
		}

		_Game_Control.instance._baskets++;
		_dunks++;
		_score = _score + _gameopt._score_on_dunk;

		//---------------------------------------	*/
		if (_level_op._levels[_gameopt._level_p]._particles_dunk != null) {
			if(_cparts){
				_cparts = false;
				GameObject _part = Instantiate(_level_op._levels[_gameopt._level_p]._particles_dunk,ballUsing[ballUsing.Count-1].transform.position,Quaternion.identity) as GameObject;
				Destroy(_part,3f);
			}
		}
        //---------------------------------------	
        _basket_animations();
        //_hud.update_score (_score);

        //StartCoroutine (check_options ());
    }

	/*/---------------------------------------	

	IEnumerator check_options(){
		yield return new WaitForSeconds(1f);

		// CHECK RANDOM POSITIONS
		//---------------------------------------	

		if (_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._random_player_position) {
			_change_position(true);
		}

		//---------------------------------------	
		if (_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._random_basket_position) {
			_change_position(false);
		}
		//---------------------------------------

		_destroyball ();

		// STOP ANIMATIONS
		//---------------------------------------
			if (_stop_on_dunk) {
				StopAllCoroutines ();

				if(_tmpcircle){
					Destroy(_tmpcircle);
					_tmpcircle = null;
				}
			}
		//---------------------------------------

		// CHECK BASKET ANIMATIONS
		//---------------------------------------
		_basket_animations ();

		// CREATE A NEW BALL
		//---------------------------------------
		createBall ();
	}

	//---------------------------------------*/

	void _basket_animations(){

		for(int i=0;i<_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._basket_animation_list.Length;i++){

			if(_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._basket_animation_list[i]._dunks_to_active == shotsMade){
				_start_animation(i);
				break;
			}
		}

		//---------------------------------------

		if (!_stop_on_dunk) {

			// BASKET ANIMATIONS
			//------------------
			if (_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._basket_animation_list.Length > 0) {
				_start_animation (_last_anim);
			}
			//------------------
		}
	}

	//---------------------------------------

	void _start_animation(int _selected){
		StopAllCoroutines ();

		if(_tmpcircle){
			Destroy(_tmpcircle);
			_tmpcircle = null;
		}


		// BASKET ANIMATIONS
		//------------------
		if (_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._basket_animation_list.Length > 0) {

			if(_selected == -1){
				_selected = 0;
			}

			_stop_on_dunk = _level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._basket_animation_list[_selected]._stop_on_dunk;
			_last_anim = _selected;

			float _spd = _speed_a(_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._basket_animation_list[_selected]._speed);
			float _dtc = _distance_a(_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._basket_animation_list[_selected]._distance);


			switch (_level_op._levels[_gameopt._level_p]._Difficulty_levels[_gameopt._difficulty_l]._basket_animation_list[_selected]._type_animation)
			{
				
			case _animations.Horizontal:
				StartCoroutine(_horizontal_animation(_spd,_dtc));
				break;
				
			case _animations.Vertical:
				StartCoroutine(_vertical_animation(_spd,_dtc));
				break;
				
			case _animations.Circle:
				StartCoroutine(_circle_animation(_spd,_dtc));
				break;
				
			}
		}
		//------------------

	}

	//---------------------------------------

	// BASKET ANIMATIONS
	//---------------------------------------	

	float _speed_a (_speed_anim _s){
		float _r = 0f;
		
		switch (_s) {
		case _speed_anim.Very_Slow:
			_r = 0.01f;
			break;
			
		case _speed_anim.Slow:
			_r = 0.025f;
			break;
			
		case _speed_anim.Normal:
			_r = 0.035f;
			break;
			
		case _speed_anim.Fast:
			_r = 0.055f;
			break;
			
		case _speed_anim.Very_Fast:
			_r = 0.08f;
			break;
			
		case _speed_anim.Super_Fast:
			_r = 0.1f;
			break;
		}
		
		return _r;
	}

	//---------------------------------------	

	float _distance_a (_distance_anim _s){
		float _r = 0f;
		
		switch (_s) {
		case _distance_anim.Very_Short:
			_r = 0.3f;
			break;
			
		case _distance_anim.Short:
			_r = 0.5f;
			break;
			
		case _distance_anim.Middle:
			_r = 1f;
			break;
			
		case _distance_anim.Long:
			_r = 1.5f;
			break;
			
		case _distance_anim.Very_Long:
			_r = 2f;
			break;
		}
		
		return _r;
	}


	//---------------------------------------	

	IEnumerator _vertical_animation(float _speed, float _distance){

		float _center = _basketgo.position.y;
		float _top = _center + _distance;
		float _down = _center - _distance;
		
		//---------------------------------------	
		
		while (_basketgo.position.y < _top) 
		{
			_basketgo.position = new Vector3 (_basketgo.position.x, _basketgo.position.y +_speed, _basketgo.position.z);
			yield return new WaitForSeconds(0.01f);
			
		}
		
		//---------------------------------------	

		while(_basketgo.position.y > _down)
		{
			_basketgo.position = new Vector3(_basketgo.position.x,_basketgo.position.y-_speed,_basketgo.position.z);
			yield return new WaitForSeconds(0.01f);
		}
		
		//---------------------------------------	
		
		while(_basketgo.position.y < _center)
		{
			_basketgo.position = new Vector3(_basketgo.position.x,_basketgo.position.y+_speed,_basketgo.position.z);
			yield return new WaitForSeconds(0.01f);
		}
		
		//---------------------------------------

		StartCoroutine(_vertical_animation(_speed,_distance));

	}

	//---------------------------------------	

	IEnumerator _horizontal_animation(float _speed, float _distance){

		float _center = _basketgo.position.x;
		float _right = _center + _distance;
		float _left = _center - _distance;

		//---------------------------------------	

		while (_basketgo.position.x < _right) 
				{
					_basketgo.position = new Vector3 (_basketgo.position.x + _speed, _basketgo.position.y, _basketgo.position.z);
					yield return new WaitForSeconds(0.01f);
			
				}

		//---------------------------------------	
		
			while(_basketgo.position.x > _left)
			{
				_basketgo.position = new Vector3(_basketgo.position.x-_speed,_basketgo.position.y,_basketgo.position.z);
				yield return new WaitForSeconds(0.01f);
			}
		
		//---------------------------------------	
		
			while(_basketgo.position.x < _center)
			{
				_basketgo.position = new Vector3(_basketgo.position.x+_speed,_basketgo.position.y,_basketgo.position.z);
				yield return new WaitForSeconds(0.01f);
			}

		//---------------------------------------	


			StartCoroutine(_horizontal_animation(_speed,_distance));

	}

	//---------------------------------------	

	IEnumerator _circle_animation(float _speed, float _distance){

		_speed = _speed * 25;
		_distance = _distance * 3;

		Transform _center = new GameObject ().transform;
		_center.transform.position = _basketgo.position;

		Transform _pivot = new GameObject ().transform;
		_pivot.parent = _center;
		_pivot.transform.localPosition = new Vector3 (_distance,0,0);

		while (_center.eulerAngles.x < 360f) 
		{
			_center.eulerAngles = new Vector3(_center.eulerAngles.x,_center.eulerAngles.y,_center.eulerAngles.z+_speed);
			_basketgo.position = _pivot.position;
			yield return new WaitForSeconds(0.01f);
			
		}

		//---------------------------------------	
	}

	//---------------------------------------	

	public void _particle_miss(Vector3 _p){
		GameObject _part = Instantiate(_level_op._levels[_gameopt._level_p]._particles_miss,_p,Quaternion.identity) as GameObject;

		if (_audio_control.instance._play_sound ()) {
			_part.GetComponent<AudioSource> ().Play ();
		}
		Destroy(_part,3f);
	}
    #endregion
    //---------------------------------------	


}