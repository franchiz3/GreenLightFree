using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class ButtonMaster : MonoBehaviour {
    public static string playerName;
    public static dreamloLeaderBoard dl;
    private static int shot = 125;
    public static ButtonMaster instance;
    public GameObject master, exitBanner;
    public Button shotParamsToggler,longToMid,discoverToggler, muteMusic, muteFX, exitButton;
    public Sprite chosen, notChosen;
    public GameObject go, newShot, cancelShot, timeButtons, barHolder,x,canvas,xShadow, tips, dailyShots, timeDisplay;
    public FixedJoystick stick;
    public static Text dailyText;
    public static GameObject tipsP, timeDisplayO;
    public static int shotsLeft;
    public static float refreshTime = 0;
    public GameObject options, statView;
    public SimpleHealthBar shotBar, lastShot;
    private Coroutine current;
    private string tempShot = null;
    private static bool paramsActive, editActive, soundActive, musicMuted, FXMuted,waiting, naming ;
    public static bool inMenu, UIActive, displaying = false, tipsEnabled, playActive = true;
    private Color tempColor;
    public GameObject[] shotParams, newShotParams;
    public static List<string[]> jumpShotData;
    private static InterstitialAd interstitial;
    private RewardBasedVideoAd rewardVideo;
    private static BannerOptions defaults;

    void Start ()
    {
        AdInit();
        //////////
        instance = master.GetComponent<ButtonMaster>();
        dl = GameObject.Find("dreamloPrefab").GetComponent<dreamloLeaderBoard>();
        SetPlayerVariables();
        playerName = PlayerPrefs.GetString("name","");
        tempColor = GameObject.Find("Regular Mode").GetComponent<Image>().color;
        GameObject.Find("Regular Mode").GetComponent<Image>().color = Color.gray;
        Instantiate();
        timeDisplayO = timeDisplay;
        StartCoroutine(RefreshCountDown());
        tipsP = tips;
        dailyText = dailyShots.GetComponentInChildren<Text>();
    }
    private void AdInit()
    {
        #if UNITY_ANDROID
                string appId = "ca-app-pub-3472480102756008~9555381110";
                // #elif UNITY_IPHONE don't have this id yet
                //           string appId = "ca-app-pub-3940256099942544~1458002511";
        #else
        string appId = "unexpected_platform";
        #endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
        rewardVideo = RewardBasedVideoAd.Instance;
        LoadRewards();
        LoadInters();
        rewardVideo.OnAdRewarded += Reward;
        rewardVideo.OnAdClosed += RewardReload;
        rewardVideo.OnAdFailedToLoad += Failed;
        interstitial.OnAdClosed += BannerDestroy;
        interstitial.OnAdClosed += _pausegame.ADResume;
        interstitial.OnAdOpening += _pausegame.AdPause;
        //Advertisement.Banner.Load("banner");
    }
    private void Awake()
    {
        //Screen.orientation = ScreenOrientation.LandscapeLeft;
        //ca-app-pub-3472480102756008~9555381110 ADMOB id. switching advertising
        Advertisement.Initialize("2914734", false);
    }
    private void SetPlayerVariables()
    {
        _Player.shotChanged = false;
        _Player.timeAttack = false;
        _Player.discoverMode = false;
        _Player.shotBar = shotBar;
        _Player.lastShot = lastShot;
        _Player.barHolder = barHolder;
        _Player.xButton = x;
        _Player.canvas = canvas;
        _Player.xShadow = xShadow;
    }
    private void OnDestroy()
    {
       //PlayerPrefs.DeleteAll();
       //PlayerPrefs.SetInt("remaining", 1);
      // PlayerPrefs.SetInt("cash", 999999);
        PlayerPrefs.Save();
    }
    private void Update()
    {
        if (inMenu && Input.GetKeyUp(KeyCode.Escape)) // adding back button function to game
        {
            if (soundActive) // options menu
            {
                ToggleSoundOptions();
            }
            else if (editActive)
            {
                ToggleNewShotOptions("cancel");
            }
            else if (paramsActive)
            {
                ToggleShotOptions();
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }
            else
            {
                Application.Quit();
            }
        }
    }

    //-----------------------------------------------------------------------------

    void Instantiate()
    {
        
        if (PlayerPrefs.GetInt("tips", 1) == 0)
        {
            GameObject.Find("Tips Toggler").GetComponentInChildren<Text>().text = "Tips:\nDisabled";
            tipsEnabled = false;
        }            
        else
        {
            GameObject.Find("Tips Toggler").GetComponentInChildren<Text>().text = "Tips:\nEnabled";
            tipsEnabled = true;
        }   
        UIActive = false;
        if (PlayerPrefs.GetInt("shotBar", 1) == 0)
            GameObject.Find("Remove Bar").GetComponentInChildren<Text>().text = "Activate\nShot Bar";
        PopulateShots();
        Dropdown temp = shotParams[1].GetComponent<Dropdown>();
        Dropdown temp2 = shotParams[2].GetComponent<Dropdown>();
        for (int x = 67; x < 100; x++)//might change to 67 have to check lowest possible
        {
            temp.options.Add(new Dropdown.OptionData(x.ToString()));
            temp2.options.Add(new Dropdown.OptionData(x.ToString()));
        }
        go.SetActive(false);
        if (IsShot(PlayerPrefs.GetString("shot"))) //if preferences load, disable shot params and set the dropdowns to the saved values
        {
            _Player.rLong = PlayerPrefs.GetInt("rLong");
            _Player.rMid = PlayerPrefs.GetInt("rMid");
            _Player.shot = SetShot(PlayerPrefs.GetString("shot"));
            shotParams[0].GetComponent<Dropdown>().value = FindIndex(_Player.shot.displayName);
            shotParams[0].GetComponentInChildren<Text>().text = _Player.shot.displayName;
            shotParams[1].GetComponent<Dropdown>().value = _Player.rLong - 67;
            shotParams[1].GetComponentInChildren<Text>().text = _Player.rLong.ToString();
            shotParams[2].GetComponent<Dropdown>().value = _Player.rMid - 67;
            shotParams[2].GetComponentInChildren<Text>().text = _Player.rMid.ToString();
            paramsActive = false;
            foreach (GameObject obj in shotParams)
                obj.SetActive(false);
            shotParamsToggler.GetComponentInChildren<Text>().text = "Edit\nJumpshot";
            if (PlayerPrefs.GetInt("long") == 0)
            {
                _Player.longRange = false;
                longToMid.gameObject.GetComponentInChildren<Text>().text = "Mid Range";
            }
            if(playActive && playerName != "")
                go.SetActive(true);
            else
            {
                if (playerName != "")
                    go.SetActive(true);
                else
                {
                    naming = true;
                    //Debug.Log("Naming Activated");
                    shotParamsToggler.GetComponentInChildren<Text>().text = "Accept";
                    newShotParams[0].GetComponentInChildren<Text>().text = "Username...";
                    newShotParams[0].SetActive(true);
                    paramsActive = true;
                }
            }
        }
        else
        {
            shotParams[0].GetComponentInChildren<Text>().text = "Kevin Durant";
            shotParams[1].GetComponentInChildren<Text>().text = "67";
            shotParams[2].GetComponentInChildren<Text>().text = "67";
            newShot.gameObject.SetActive(false);
            paramsActive = true;
        }


        options.SetActive(false);
        soundActive = false;
        editActive = false;
        timeButtons.SetActive(false);
        cancelShot.SetActive(false);

        foreach (GameObject field in newShotParams) //adding ability to have a name for the leaderboard. name has to be set whether the person is new or returning to the game after the update
            if (naming)
            {
                if(field.name != "DisplayName")
                    field.SetActive(false);
            }
            else
            {
                field.SetActive(false);
            }


       
            if (PlayerPrefs.GetInt("_music") == 0)
                musicMuted = false;
            else
            {
                musicMuted = true;
                muteMusic.gameObject.GetComponent<Image>().color = Color.gray;
            }

            if (PlayerPrefs.GetInt("_fx") == 0)
                FXMuted = false;
            else
            {
                FXMuted = true;
                muteFX.gameObject.GetComponent<Image>().color = Color.gray;
            }

        if (PlayerPrefs.GetInt("tracking") == 1)//all for ads and balls left
        {            
            shotsLeft = PlayerPrefs.GetInt("remaining");
            if (shotsLeft < 0)
                shotsLeft = 0;
            if (PlayerPrefs.GetString("coolEnd",null) !=null && DateTime.Now > Convert.ToDateTime(PlayerPrefs.GetString("coolEnd")))
            {
                PlayerPrefs.DeleteKey("coolEnd");
                DateTime end = DateTime.Now.AddDays(1);
                PlayerPrefs.SetString("coolEnd", end.ToString());
                // Debug.Log("Issue Here");
                shotsLeft += shot;
                PlayerPrefs.SetInt("tracking", 0);
            }
            if (shotsLeft <= 0)
            {
                dailyShots.GetComponentInChildren<Text>().text = "Get Shots!";
                playActive = false;
                go.SetActive(false);
            }
            else
            {
                dailyShots.GetComponentInChildren<Text>().text = "Shots Left: " + shotsLeft; // this is if the countdown is going but the person earned more shots
                playActive = true;
            }

        }
        else
        {
            shotsLeft = shot;
            //Debug.Log("Happening");
            PlayerPrefs.SetString("coolEnd", DateTime.Now.AddDays(1).ToString());
            PlayerPrefs.SetFloat("remaining", shotsLeft);
            // Debug.Log("Issue Here");
            dailyShots.GetComponentInChildren<Text>().text = "Shots Left: " + shotsLeft;
        }
        if(playerName != "")
            GameObject.Find("_Instructions").GetComponent<Text>().text = "Hi "+ playerName + "! Accept A JumpShot, Select A Stage & Touch Go!";
    } 

    //-------------------------------------------------------------------------------

    int FindIndex(string shot)
    {
        List<Dropdown.OptionData> data = shotParams[0].GetComponent<Dropdown>().options;
        for (int index = 0; 0 < data.Count; index++)
        {
            if (data[index].text == shot)
                return index;
        }
        Debug.LogError("Indexing of Names Interpreted Incorrectly");
        return -1;
    }

    //-------------------------------------------------------------------------------
    #region Toggles
    public void SetMode(int choice)
    {
        if(choice == 0)
        {
            _Player.normalMode = true;
            GameObject.Find("0").GetComponent<Image>().sprite = chosen;
            GameObject.Find("1").GetComponent<Image>().sprite = notChosen;
        }
        else
        {
            _Player.normalMode = false;
            GameObject.Find("0").GetComponent<Image>().sprite = notChosen;
            GameObject.Find("1").GetComponent<Image>().sprite = chosen;
        }
    }
    //------------------------------------------------------------------------------
    public void ViewStats()
    {
        if (editActive ) 
        {
            if (!displaying)
                StartCoroutine(DisplayTip("Finish Creating Your Jumpshot Or Press Cancel To View Stats", false, false));
            return;
        }
        if(soundActive)
        {
            if (!displaying)
                StartCoroutine(DisplayTip("Accept Options Changes Before You Can View Stats", false,false));
            return;
        }
        if(paramsActive)
        {
            if (!displaying)
                StartCoroutine(DisplayTip("Accept Your Jumpshot To View Stats", false,false));
            return;
        }
        if (statView.activeSelf)
        {
            statView.SetActive(false);
            UIActive = false;
            AddedItemsController.ToggleAddedItems();
        }
        else
        {
            statView.SetActive(true);
            if (!PlayerPrefs.HasKey("shootingStats"))
                GameObject.Find("_Back").GetComponentInChildren<Text>().text = "Take A Few Shots And Come Back Shakah Brah!";
            else
                GameObject.Find("_Back").GetComponentInChildren<Text>().text = PlayerPrefs.GetString("shootingStats");
            AddedItemsController.UI = true;
            AddedItemsController.ToggleAddedItems();
            UIActive = true;
        }
    }
    //-------------------------------------------------------------------------------
    public void ToggleMusic()
    {
        if (musicMuted)
        {
            _audio_control.noMusic = false;
            _audio_control.instance._music = true;
            _audio_control.instance.GetComponent<AudioSource>().mute = false;
            _audio_control.instance.GetComponent<AudioSource>().Play();
            GameObject.Find("Music Sound").GetComponent<Image>().color = Color.green;
            PlayerPrefs.SetInt("_music", 0);
            musicMuted = false;
        }
        else
        {
            _audio_control.noMusic = true;
            _audio_control.instance._music = false;
            _audio_control.instance.GetComponent<AudioSource>().Stop();
            GameObject.Find("Music Sound").GetComponent<Image>().color = Color.gray;
            PlayerPrefs.SetInt("_music", 1);
            musicMuted = true;
        }
        
    }

    //-------------------------------------------------------------------------------

    public void ToggleFX()
    {
        if (FXMuted)
        {
            _audio_control.noFX = false;
            _audio_control.instance._fx = true;
            GameObject.Find("FX Sound").GetComponent<Image>().color = Color.yellow;
            PlayerPrefs.SetInt("_fx", 0);
            FXMuted = false;
        }
        else
        {
            _audio_control.noFX = true;
            _audio_control.instance._fx = false;
            GameObject.Find("FX Sound").GetComponent<Image>().color = Color.gray;
            PlayerPrefs.SetInt("_fx", 1);
            FXMuted = true;
        }
    }

    //---------------------------------------------------------------------------------

    public void ToggleShotBar(Text button)
    {
        if(button.text == "Remove\nShot Bar")
        {
            button.text = "Activate\nShot Bar";
            _Player.barActive = false;
            PlayerPrefs.SetInt("shotBar", 0);
        }
        else
        {
            button.text = "Remove\nShot Bar";
            _Player.barActive = true;
            PlayerPrefs.SetInt("shotBar", 1);
        }
    }

    //-----------------------------------------------------------------------------

    public void ToggleTips()
    {
        if (tipsEnabled)
        {
            tipsEnabled = false;
            GameObject.Find("Tips Toggler").GetComponentInChildren<Text>().text = "Tips:\nDisabled";
            PlayerPrefs.SetInt("tips", 0);
        }
        else
        {
            tipsEnabled = true;
            GameObject.Find("Tips Toggler").GetComponentInChildren<Text>().text = "Tips:\nEnabled";
            PlayerPrefs.SetInt("tips", 1);
        }
    }
    
    //-----------------------------------------------------------------------------
    public void ToggleShotOptions()
    {
        if (editActive)
        {
            if (!displaying)
                StartCoroutine(DisplayTip("Finish Creating Your Jumpshot Or Press Cancel To Change Your Shot", false, false));
            return;
        }
        if (soundActive)
        {
            return;
        }
        if (UIActive)
            return;

        if (!paramsActive)//activate dropdowns
        {
            foreach (GameObject dropdown in shotParams)
                dropdown.SetActive(true);
            paramsActive = true;
            longToMid.gameObject.SetActive(true);
            newShot.SetActive(false);
            shotParamsToggler.GetComponentInChildren<Text>().text = "Accept";
            if (_Player.timeAttack)
                timeButtons.SetActive(false);
            else
                go.SetActive(false); //attempting to erase bug where game starts without jumpshot selected
            //add call to edit green window based on params entered<--done see else
            _Player.shot.displayName = tempShot;
        }
        else//deactivate dropdowns, change jumpshot, and reactivate gameplay
        {
            if (!naming)
            {
                _Player.rLong = shotParams[1].GetComponent<Dropdown>().value + 67;//<----- number here has to be the lowest possible value, makes going from input field to file easy
                _Player.rMid = shotParams[2].GetComponent<Dropdown>().value + 67;
                _Player.shot = SetShot(shotParams[0].GetComponent<Dropdown>().captionText.text);
                PlayerPrefs.SetInt("rLong", _Player.rLong);
                PlayerPrefs.SetInt("rMid", _Player.rMid);
                PlayerPrefs.SetString("shot", shotParams[0].GetComponent<Dropdown>().captionText.text);
                if (tempShot != null && tempShot != _Player.shot.displayName)
                    _Player.shotChanged = true;
                if (!_Player.longRange)
                    PlayerPrefs.SetInt("long", 0);
                else
                    PlayerPrefs.SetInt("long", 1);
                //_Player.CalculateGreenWindow(rLong, rMid);
                longToMid.gameObject.SetActive(false);
                newShot.SetActive(true);
                foreach (GameObject dropdown in shotParams)
                    dropdown.SetActive(false);
            }
            else
            {
                if(newShotParams[0].GetComponent<InputField>().text.Length > 12)
                {
                    StartCoroutine(DisplayTip("Name Must Be Fewer Than 12 Characters!", false, true));
                    return;
                }
                else if (dl.NameExists(newShotParams[0].GetComponent<InputField>().text))
                {
                    StartCoroutine(DisplayTip("Sorry But That Name Is Taken! Please Try Another Name Or Throw Some Numbers In There. Make Sure Not To Use An * In Your Name!", false, true));
                    return;
                }
                else if(newShotParams[0].GetComponent<InputField>().text == "")
                {
                    StartCoroutine(DisplayTip("You Must Enter A Name To Continue! Be Sure Not To Use Any Asterisks * !", false, true));
                    return;
                }
                else if (newShotParams[0].GetComponent<InputField>().text.Contains("*"))
                {
                    StartCoroutine(DisplayTip("Make Sure Not To Use An * In Your Name!", false, true));
                    return;
                }
                else
                {
                    playerName = newShotParams[0].GetComponent<InputField>().text;
                    newShotParams[0].GetComponentInChildren<Text>().text = "Shot Name...";
                    newShotParams[0].SetActive(false);
                    PlayerPrefs.SetString("name", playerName);
                    dl.AddScore(playerName, PlayerPrefs.GetInt("highScore30"), 30);
                    dl.AddScore(playerName, PlayerPrefs.GetInt("highScore60"), 60);
                    dl.AddScore(playerName, PlayerPrefs.GetInt("highScore90"), 90);
                    GameObject.Find("_Instructions").GetComponent<Text>().text = "Hi " + playerName + "! Accept A JumpShot, Select A Stage & Touch Go!";
                    naming = false;
                }
            }

            if (playerName != "")
            {
                paramsActive = false;
                shotParamsToggler.GetComponentInChildren<Text>().text = "Edit\nJumpshot";
            }
            
            if (playActive)
            {
                if (_Player.timeAttack)
                    timeButtons.SetActive(true);
                else
                {
                    if (playerName != "")
                        go.SetActive(true);
                    else
                    {
                        naming = true;
                        StartCoroutine(DisplayTip("Welcome To The New LeaderBoard System! Please Set Your Username To Display Your Scores!", false, true));
                        newShotParams[0].SetActive(true);
                    }
                }
            }
            else
            {
                if(playerName == "")
                {
                    naming = true;
                    StartCoroutine(DisplayTip("You're Out Of Shots For Now Buuuuuut; Welcome To The New LeaderBoard System! Please Set Your Username To Display Your Scores!", false, true));
                    newShotParams[0].SetActive(true);
                }
            }
            
        }
    }
    //------------------------------------------------------

    public void ToggleNewShotOptions(string action)
    {
        if (soundActive)
        {
            return;
        }
        if (paramsActive)
        {
            if (!displaying)
                StartCoroutine(DisplayTip("Click \"Accept\" Before Creating A New Shot!", false, false));
            return;
        }
        if (UIActive)
            return;

        if (editActive)
        {
            if (action == "accept")
            {
                if (GetAcceptableParameters())
                {
                    string[] temp = {newShotParams[0].GetComponent<InputField>().text, newShotParams[1].GetComponent<InputField>().text, newShotParams[2].GetComponent<InputField>().text,
                    "1",newShotParams[3].GetComponent<InputField>().text,newShotParams[4].GetComponent<InputField>().text};
                    jumpShotData.Add(temp);
                    shotParams[0].GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(temp[0]));
                    string prefs =  temp[0] + "," + temp[1] + "," + temp[2] + ",1," + temp[3] + "," + temp[4] +"\n";
                    PlayerPrefs.SetString("customShots", PlayerPrefs.GetString("customShots", "") + prefs);

                    //add acceptance parameters<-- done 10/29/18
                    //add dropdown for which rating the person was at when they took the pull of the release<--done 10/29/18<--undone decided to scrap function and let them choose their window
               
                    foreach (GameObject field in newShotParams)
                        field.SetActive(false);
                    cancelShot.SetActive(false);
                    editActive = false;
                    newShot.GetComponentInChildren<Text>().text = "New Jumpshot";
                    if (playActive)
                    {
                        if (_Player.timeAttack)
                            timeButtons.SetActive(true);
                        else
                            go.SetActive(true);
                    }
                }
            }
            else
            {
                foreach (GameObject field in newShotParams)
                    field.SetActive(false);
                cancelShot.SetActive(false);
                editActive = false;
                newShot.GetComponentInChildren<Text>().text = "New Jumpshot";
                if (playActive)
                {
                    if (_Player.timeAttack)
                        timeButtons.SetActive(true);
                    else
                        go.SetActive(true);
                }
            }
        }
        else
        {
            editActive = true;
            cancelShot.SetActive(true);
            newShot.GetComponentInChildren<Text>().text = "Accept";
            foreach (GameObject field in newShotParams)
                field.SetActive(true);
            if (_Player.timeAttack)
                timeButtons.SetActive(false);
            else
                go.SetActive(false);
        }
    }

    //---------------------------------------

    public void ToggleSoundOptions()
    {
        if (editActive)
        {
            if (!displaying)
                StartCoroutine(DisplayTip("Finish Creating Your Jumpshot Or Press Cancel To Open The Options Menu", false,false));
            return;
        }
        if (paramsActive)
        {
            if (!displaying)
                StartCoroutine(DisplayTip("Accept Your Jumpshot To Open The Options Menu", false,false));
            return;
        }
        if (UIActive)
            return;


        if (soundActive)
        {
            soundActive = false;
            options.SetActive(false);
            if (playActive)
            {
                if (_Player.timeAttack)
                    timeButtons.SetActive(true);
                else
                    go.SetActive(true);
            }
            AddedItemsController.ToggleAddedItems();
            GameObject.Find("Sound Options").GetComponentInChildren<Text>().text = "Options";
        }
        else
        {
            soundActive = true;
            options.SetActive(true);
            if (_Player.timeAttack)
                timeButtons.SetActive(false);
            else
                go.SetActive(false);
            AddedItemsController.ToggleAddedItems();
            GameObject.Find("Sound Options").GetComponentInChildren<Text>().text = "Accept";
        }
    }

    //---------------------------------------


    public void ToggleLongMid()
    {
        if (_Player.longRange)
        {
            longToMid.GetComponentInChildren<Text>().text = "Mid Range";
            _Player.longRange = false;
        }
        else
        {
            longToMid.GetComponentInChildren<Text>().text = "Long Range";
            _Player.longRange = true;          
        }
    }


    #endregion
    //----------------------------------------

    //----------------------------------------
    #region Game Modes

    public void TimeAttackMode()
    {
        if (UIActive || paramsActive || editActive || soundActive)
        {
            if (GameObject.Find("_Instructions").GetComponent<Text>().color != Color.red)
                StartCoroutine(Instruct("Close All Menus Before Switching Modes!"));
            return;
        }

        if (!_Player.discoverMode && !_Player.regularMode)
            return;

        if(shotsLeft < 10)
        {
            StartCoroutine(DisplayTip("Sorry You Can't Enter Time Attack With Fewer Than 10 Shots",false,true));
            Debug.Log(shotsLeft);
            return;
        }
        if (GameObject.Find("_Instructions").GetComponent<Text>().color != Color.red)
            StopCoroutine(Instruct("Close All Menus Before Switching Modes!"));
        GameObject.Find("_Instructions").GetComponent<Text>().color = Color.white;
        if (_Player.discoverMode)
        {
            _Player.discoverMode = false;
            discoverToggler.GetComponent<Image>().color = tempColor;
        }
        else if (_Player.regularMode)
        {
            _Player.regularMode = false;
            GameObject.Find("Regular Mode").GetComponent<Image>().color = tempColor;
        }
        go.SetActive(false);
        if(playActive)
            timeButtons.SetActive(true);
        GameObject.Find("_Instructions").GetComponent<Text>().text = "Pick A Time To Start!";
        tempColor = GameObject.Find("Time Mode").GetComponent<Image>().color;
        GameObject.Find("Time Mode").GetComponent<Image>().color = Color.gray;
        _Player.timeAttack = true;
    }
    //------------------------------------------------------------------------------
    public void DiscoverMode()
    {
        if (UIActive || paramsActive || editActive || soundActive)
        {
            if (GameObject.Find("_Instructions").GetComponent<Text>().color != Color.red)
                StartCoroutine(Instruct("Close All Menus Before Switching Modes!"));
            return;
        }

        if (!_Player.timeAttack && !_Player.regularMode)
            return;
        if (GameObject.Find("_Instructions").GetComponent<Text>().color != Color.red)
            StopCoroutine(Instruct("Close All Menus Before Switching Modes!"));
        GameObject.Find("_Instructions").GetComponent<Text>().color = Color.white;
        if (_Player.timeAttack)
        {
            timeButtons.SetActive(false);
            _Player.timeAttack = false;
            GameObject.Find("Time Mode").GetComponent<Image>().color = tempColor;
        }
        else if (_Player.regularMode)
        {
            _Player.regularMode = false;
            GameObject.Find("Regular Mode").GetComponent<Image>().color = tempColor;
        }

        shotParamsToggler.gameObject.SetActive(false);
        newShot.SetActive(false);
        if(playActive)
            go.SetActive(true);
        _Player.discoverMode = true;
        tempColor = discoverToggler.gameObject.GetComponent<Image>().color;
        discoverToggler.gameObject.GetComponent<Image>().color = Color.gray;
        GameObject.Find("_Instructions").GetComponent<Text>().text = "Select A Stage And Press Go!";
        shotParamsToggler.gameObject.SetActive(true);
        newShot.SetActive(true);
        //might be editing to make params not active when the app starts
        //if that happens this must be updated to not activate params <--happened. completely rewrote modes 11/11/18^^^
    }

    public void RegularMode()
    {
        if (UIActive || paramsActive || editActive || soundActive)
        {
            if(GameObject.Find("_Instructions").GetComponent<Text>().color != Color.red)
                StartCoroutine(Instruct("Close All Menus Before Switching Modes!"));
            return;
        }
        if (!_Player.timeAttack && !_Player.discoverMode)
            return;

        StopCoroutine(Instruct("Close All Menus Before Switching Modes!"));
        GameObject.Find("_Instructions").GetComponent<Text>().color = Color.white;
        tempColor = GameObject.Find("Regular Mode").GetComponent<Image>().color;
        GameObject.Find("Regular Mode").GetComponent<Image>().color = Color.gray;
        if (_Player.timeAttack)
        {
            timeButtons.SetActive(false);
            if(playActive)
                go.SetActive(true);
            _Player.timeAttack = false;
            GameObject.Find("Time Mode").GetComponent<Image>().color = tempColor;
        }
        else if (_Player.discoverMode)
        {
            _Player.discoverMode = false;
            discoverToggler.gameObject.GetComponent<Image>().color = tempColor;
        }
        _Player.regularMode = true;
        GameObject.Find("_Instructions").GetComponent<Text>().text = "Select A Stage And Press Go!";
    }

    #endregion
    //----------------------------------------

    #region ADD/CHANGE Shot
    bool GetAcceptableParameters()
    {
        if (newShotParams[0].GetComponent<InputField>().text == "" || newShotParams[1].GetComponent<InputField>().text == "" || newShotParams[2].GetComponent<InputField>().text == "" ||
            newShotParams[3].GetComponent<InputField>().text == "" || newShotParams[4].GetComponent<InputField>().text == "")
        {
            newShotParams[5].GetComponent<Text>().color = Color.red;
            newShotParams[5].GetComponent<Text>().text = "No Field May Be Left Empty!";
            return false;
        }

        int[] releases = new int[4]; //too lazy to change from ratings to releases
        int.TryParse(newShotParams[1].GetComponent<InputField>().text, out releases[0]);
        int.TryParse(newShotParams[2].GetComponent<InputField>().text, out releases[1]);
        int.TryParse(newShotParams[3].GetComponent<InputField>().text, out releases[2]);
        int.TryParse(newShotParams[4].GetComponent<InputField>().text, out releases[3]);

        if (releases[0] > releases[1] || releases[2] > releases[3])
        {
            newShotParams[5].GetComponent<Text>().color = Color.red;
            newShotParams[5].GetComponent<Text>().text = "Low Release Must Be Lower Than High Release!";
            return false;
        }

        if(releases[1]-releases[0]> 75 || releases[3] - releases[2] > 75)
        {
            newShotParams[5].GetComponent<Text>().color = Color.red;
            newShotParams[5].GetComponent<Text>().text = "Green Window Cannot Be Bigger Than 75ms!";
            return false;
        }

        newShotParams[5].GetComponent<Text>().color = Color.white;
        newShotParams[5].GetComponent<Text>().text = "Insert Lowest Then Highest Green Release in Milliseconds";
        return true;    
    }

    //--------------------------------------- take jumpshot name as string shot and edit jumpshot to associated green window values
    public JumpShot SetShot(string shot)
    {
        int[] times = new int[5];
        float window;
        int customFlag;
        foreach (string[] data in jumpShotData)
        {

            if (data[0] == shot)
            {
                int.TryParse(data[3], out customFlag);
                //Debug.Log(data[3]);
                if(customFlag != 1)
                {
                    int.TryParse(data[1], out times[0]);
                    int.TryParse(data[2], out times[1]);
                    int.TryParse(data[4], out times[3]);
                    float.TryParse(data[5], out window);
                    int.TryParse(data[7], out times[4]);
                    return new JumpShot(shot, times[0], times[1], true, times[3], window, times[4]);
                }
                else
                {

                    int.TryParse(data[1], out times[0]);
                    int.TryParse(data[2], out times[1]);
                    int.TryParse(data[4], out times[2]);
                    int.TryParse(data[5], out times[3]);
                    return new JumpShot(shot, times[0], times[1], times[2], times[3]);
                }   
            }

        }
        return null;
    }

    public bool IsShot(string shot)
    {
        foreach (string[] data in jumpShotData)
        {
            if (data[0] == shot)
                return true;
        }
        return false;
    }


    //----------------------------------------------------------------------------------------

    private void PopulateShots()
    {
        TextAsset fileData = Resources.Load<TextAsset>("data/ShotChart");
        //string fileData = ;
        string[] lines = fileData.text.Split("\n"[0]);
        string[] cLines = PlayerPrefs.GetString("customShots").Split("\n"[0]); //custom data
        jumpShotData = new List<string[]>();
        List<Dropdown.OptionData> shotList = shotParams[0].GetComponent<Dropdown>().options;
        shotList.Clear();
        for (int line = 0; line < cLines.Length; line++)
        {
            string[] separated = (cLines[line].Trim()).Split(","[0]);
            if (separated[0].Length == 0)//eliminates unnecessary empty option at end. Also could do Length-1 in conditional
                 break;
            jumpShotData.Add(separated);
            shotList.Add(new Dropdown.OptionData(separated[0]));
        }
        for (int line = 1; line < lines.Length; line++)
        {
            string[] separated = (lines[line].Trim()).Split(","[0]);
            if (separated[0].Length == 0)//eliminates unnecessary empty option at end. Also could do Length-1 in conditional
                return;
            jumpShotData.Add(separated);
            //Debug.Log(separated[0]+ separated[1]+ separated[2]+ separated[3]+ separated[4]+ separated[5]);
            shotList.Add(new Dropdown.OptionData(separated[0]));
        }        
        //testing having all data in memory and accessible through this n-dimensional array
        //will be tested when a better method arises. For now the parsing will occur each time the player tries to change shots.
        //can't declare separated array before now because number of lines is unknown List<string[4]> possible?
    }
    #endregion
    //---------------------------------------
    #region Tips

    public static IEnumerator DisplayTip(string tip, bool random, bool adRelated)
    {
        if (!tipsEnabled && !adRelated)
            yield break;
        displaying = true;
        tipsP.SetActive(true);
        if (!random)
            tipsP.GetComponentInChildren<Text>().text = FormatTip(tip);
        else
            tipsP.GetComponentInChildren<Text>().text = FormatTip(RandomTip());
        yield return new WaitForSeconds(15);
        tipsP.GetComponentInChildren<Text>().text = "";
        yield return new WaitForSeconds(.5f);
        tipsP.SetActive(false);
        displaying = false;
    }
    
    public void CloseTip()
    {
        if (inMenu)
        {
            tips.SetActive(false);
        }
        else
        {
            tipsP.SetActive(false);
        }
    }

    public static string FormatTip(string tip)
    {
        if(tip.Length < 20)
        {
            tipsP.GetComponent<RectTransform>().sizeDelta = new Vector2(1100, 125);
            GameObject.Find("TipsText").GetComponent<RectTransform>().sizeDelta = new Vector2(800,75);
        }
        else if (tip.Length < 40)
        {
            tipsP.GetComponent<RectTransform>().sizeDelta = new Vector2(1100, 175);
            GameObject.Find("TipsText").GetComponent<RectTransform>().sizeDelta = new Vector2(800, 125);
        }
        else if (tip.Length < 60)
        {
            tipsP.GetComponent<RectTransform>().sizeDelta = new Vector2(1100, 225);
            GameObject.Find("TipsText").GetComponent<RectTransform>().sizeDelta = new Vector2(800, 175);
        }
        else if (tip.Length < 80)
        {
            tipsP.GetComponent<RectTransform>().sizeDelta = new Vector2(1100, 300);
            GameObject.Find("TipsText").GetComponent<RectTransform>().sizeDelta = new Vector2(800, 225);
        }
        else if(tip.Length < 100)
        {
            tipsP.GetComponent<RectTransform>().sizeDelta = new Vector2(1100, 500);
            GameObject.Find("TipsText").GetComponent<RectTransform>().sizeDelta = new Vector2(800, 300);
        }
        else
        {
            String tips = tip;// just needed filler here for tips
        }
        return tip;
    }
    public void ShowTip(string tip)
    {
        StartCoroutine(DisplayTip(tip, false, true));
    }

    public static string RandomTip()
    {
        switch (UnityEngine.Random.Range(1, 15))
        {
            case 1:
                return "Only A Shot In The Perfect Green Window is Guaranteed To Go In";

            case 2:
                return "Any Shot In the Green Window That Isn't In The Perfect Green Window Can Go Full White";

            case 3:
                return "Full White Releases Tend To Go In Even Less Often Than Late or Early :O";

            case 4:
                return "The Idea Is To Get Either As Close To OR Inside The Green Window..";

            case 5:
                return "The Further From The Green Window Your Release Is, The Less Likely The Shot Will Go In!";

            case 6:
                return "Take Your Time In Time Attack.. You Don't Want To Miss Those Streak Multipliers!";

            case 7:
                return "In Time Attack, The Closer The Game Is To Ending, The More Points You Get Per Shot!";

            case 8:
                return "Disable Music And Sound Effects In The Options Menu, Feel Free To Play Your Music In The Background!";

            case 9:
                return "You Can Move The X Button!Just Press Move Button(Bottom Left In The Game)";

            case 10:
                return "Achievement Unlocked!\nCheck Out Your Achievements By Clicking The Medal Icon In The Main Menu!";

            case 11:
                return "Clutch Factor!The Last Three Seconds Bring A Gigantic Bonus. Make Those Last Shots Count!";

            case 12:
                return "Arcade Mode Makes It Much Easier To Hit Shots And Get Streaks If You're Struggling!";

            case 13:
                return "Tips Regarding Ads Will Show Whether Tips Are On Or Off";

            case 14:
                return "Don't Worry! If You Start Time Attack or Discover Shot Without Enough Balls, You'll Be Allowed To Finish That Round";

            default:
                return "Turn These Tips Off In The Options Menu";
        }
    }
    #endregion
    //---------------------------------------

    #region Ad-Related
    public static void TrackTheBalls()
    {
        PlayerPrefs.SetInt("tracking", 1);
        //Debug.Log("Issue Here");
        shotsLeft = shot;
    }
    #region ADMOB
    private void Skipped(object sender, EventArgs args)
    {
        StartCoroutine(DisplayTip("If You Skip The Ad You Get No Shots :(", false, true));
    }

    private void Failed(object sender, EventArgs args)
    {
        StartCoroutine(DisplayTip("Ad Failed To Load. Please Try Again. If The Problem Persists Contact The Developer", false, true));
        //LoadRewards();
    }
    private void RewardReload(object sender, EventArgs args)
    {
        LoadRewards();
    }
    private void Reward(object sender, EventArgs args) //this is for admob
    {
        //LoadRewards();
        StartCoroutine(DisplayTip("Congratulations! You Just Earned 20 Shots!", false, true));
        if (shotsLeft < 0)
            shotsLeft = 20;
        else
            shotsLeft += 20;
        PlayerPrefs.SetInt("remaining", shotsLeft);
        dailyShots.GetComponentInChildren<Text>().text = "Shots Left: " + shotsLeft;
        if (_Player.timeAttack)
            timeButtons.SetActive(true);
        else
            go.SetActive(true);
        playActive = true;     
    }
    #endregion

    private void UnityReward(ShowResult result)
    {
        if(result == ShowResult.Skipped)
            StartCoroutine(DisplayTip("If You Skip The Ad You Get No Shots :(", false, true));
        //else if(result == ShowResult.Failed)
            //StartCoroutine(DisplayTip("Ad Failed To Load. Please Try Again. If The Problem Persists Contact The Developer", false, true));
        else if(result == ShowResult.Finished)
        {
            StartCoroutine(DisplayTip("Congratulations! You Just Earned 20 Shots!", false, true));
            if (shotsLeft < 0)
                shotsLeft = 20;
            else
                shotsLeft += 20;
            PlayerPrefs.SetInt("remaining", shotsLeft);
            dailyShots.GetComponentInChildren<Text>().text = "Shots Left: " + shotsLeft;
            if (_Player.timeAttack)
                timeButtons.SetActive(true);
            else
                go.SetActive(true);
            playActive = true;
        }
    }
    public static IEnumerator RefreshCountDown()
    {
        if (PlayerPrefs.GetInt("tracking") == 0 || playActive)
            yield break;
        
        timeDisplayO.SetActive(true);
        Text text = timeDisplayO.GetComponent<Text>();
        //Debug.Log(Time.time);
        DateTime end = Convert.ToDateTime(PlayerPrefs.GetString("coolEnd"));

        while(DateTime.Now < end)
        {
            if (shotsLeft > 0)
            {
                if (_Player.timeAttack)
                    instance.timeButtons.SetActive(true);
                else
                    instance.go.SetActive(true);
                instance.dailyShots.GetComponentInChildren<Text>().text = "Shots Left: " + shotsLeft;
                timeDisplayO.SetActive(false);
                playActive = true;
                yield break;
            }
            TimeSpan down = end.Subtract(DateTime.Now);
            refreshTime = (float)down.TotalSeconds;
            text.text = down.Hours + " : " + down.Minutes + " : " + down.Seconds ;
            yield return new WaitForSecondsRealtime(1);
        }
        timeDisplayO.SetActive(false);
        ResetCoolDown();
    }

    public static void ResetCoolDown()
    {
        PlayerPrefs.SetString("coolEnd", DateTime.Now.AddDays(1).ToString()); // now the countdown only resets once a day based on when you first launch it or if you wait for the timer to run out.
        //PlayerPrefs.SetInt("remaining", shot);
        PlayerPrefs.SetInt("tracking", 0);
        //Debug.Log("Issue Here");
        if (shotsLeft < 0)
            shotsLeft = shot;
        else
            shotsLeft += shot;
        PlayerPrefs.SetInt("remaining", shotsLeft);
        if (_Player.timeAttack)
            instance.timeButtons.SetActive(true);
        else
            instance.go.SetActive(true);
        playActive = true;
        instance.dailyShots.GetComponentInChildren<Text>().text = "Shots Left: " + shotsLeft;
        instance.StopAllCoroutines();
    }

    private void LoadRewards()
    {        
        #if UNITY_ANDROID
                    string adUnitId = "ca-app-pub-3472480102756008/8541682348";
        #elif UNITY_IPHONE
                    string adUnitId = "ca-app-pub-3472480102756008/8541682348";
        #else
                    string adUnitId = "unexpected_platform";
        #endif

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        rewardVideo.LoadAd(request, adUnitId); 
    }
    public void StartAds()
    {
        if (rewardVideo.IsLoaded())
            rewardVideo.Show();
        else
        {
            if (Advertisement.IsReady("rewardedVideo"))
            {
                var options = new ShowOptions { resultCallback = UnityReward };
                Advertisement.Show("rewardedVideo",options);
            }                
            else
            {
                StartCoroutine(WaitForAd("rewardedVideo"));
            }
        }
    }
    private void LoadInters()
    {
        string adUnitId;
        #if UNITY_ANDROID
                adUnitId = "ca-app-pub-3472480102756008/6766276687";
        #elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3472480102756008/6766276687";
        #else
                       adUnitId = "unexpected_platform";
        #endif
        interstitial = new InterstitialAd(adUnitId);
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }
    public static void ShowBannerAd()
    {
        //removing all unity ad stuff and switching to ADMOB
        //AdMob+Unity+Possibly Facebook 12/3/18
        if (interstitial.IsLoaded())
            interstitial.Show();
        else//reload google Ad but display Unity Ad
        {
            instance.LoadInters();
            if (!inMenu)
            {
                if (Advertisement.IsReady("banner"))
                {
                    var defaults = new BannerOptions { showCallback = instance.ExitAds };
                    Advertisement.Banner.Show("banner",defaults);
                    //_Player.instance.StartCoroutine(EnableAdExit());
                }

                else
                {
                    if (!inMenu)
                        _Player.instance.StartCoroutine(WaitForAd("banner"));
                    //  else removing banner inmenu
                    // instance.StartCoroutine(WaitForAd("banner"));
                }
            }
        }
        
    }
    private void ExitAds()
    {
        _Player.instance.StartCoroutine(EnableAdExit());
    }
    private static IEnumerator EnableAdExit()
    {
       // waiting = true;
        //instance.exitBanner.SetActive(true);
        for (int x = 15; x > 0; x--)
        {
            instance.exitBanner.GetComponentInChildren<Text>().text = x.ToString();
            yield return new WaitForSeconds(1);
        }
        //waiting = false;
        instance.exitBanner.GetComponentInChildren<Text>().text = "";
        Advertisement.Banner.Hide();
      
    }
    public void HideBanner()
    {
        if (waiting)
            return;
        Advertisement.Banner.Hide();
        //Advertisement.Banner.Load("banner");
    }
    private void BannerDestroy(object sender, EventArgs args)
    {
        interstitial.Destroy();
        LoadInters();
    }
    public static IEnumerator WaitForAd(string ad)
    {
        while (!Advertisement.IsReady("banner"))//I'll most likely add a call to FB ads here or alternate between FB & unity as a last resort when ads won't load.
            yield return new WaitForSeconds(.5f);
        var defaults = new BannerOptions { showCallback = instance.ExitAds };
        Advertisement.Banner.Show("banner", defaults);
    }
    public static void RewardB(ShowResult result) // might not need any of this. leaving just in case
    {
        
        if (result == ShowResult.Finished)
        {
            if (UnityEngine.Random.Range(0, 100) < 80.2)
            {
                if (UnityEngine.Random.Range(0, 100) < 50)
                {
                    int reward = UnityEngine.Random.Range(10, 25);
                    if(inMenu)
                        instance.StartCoroutine(DisplayTip("Congrats You Just Earned " + reward + " Shots! Enjoy Mah Bois!", false, true));
                    else
                        _Player.instance.StartCoroutine(DisplayTip("Congrats You Just Earned " + reward + " Shots! Enjoy Mah Bois!", false, true));
                    if (shotsLeft <= 0)
                        shotsLeft = reward;
                    else
                        shotsLeft += reward;
                    PlayerPrefs.SetInt("remaining", shotsLeft);
                    instance.dailyShots.GetComponentInChildren<Text>().text = "Shots Left: " + shotsLeft;
                    playActive = true;
                }
                else
                {
                    int reward = UnityEngine.Random.Range(1000, 2500);
                    if(!inMenu)
                        _Player.instance.StartCoroutine(DisplayTip("Congrats You Just Earned $" + reward + "! Don't Spend It All In One Place!", false, true));
                    else
                        instance.StartCoroutine(DisplayTip("Congrats You Just Earned $" + reward + "! Don't Spend It All In One Place!", false, true));
                    _Player.cash += reward;
                    GameObject.Find("_GameControl").GetComponent<hud_control>()._update_money();
                    //PlayerPrefs.SetInt("cash", PlayerPrefs.GetInt("cash") + reward);
                }
            }
            
        }
        else if(result == ShowResult.Skipped)
        {
            //Debug.Log("REACHED");
            if (PlayerPrefs.GetInt("warned", 0) == 0)
            {
                if(inMenu)
                    instance.StartCoroutine(DisplayTip("Whoa! Watching Those Ads Will Give You A Chance To Get Extra Shots, Free Money And More! Think Twice. But Here's 10 Free Shots Just For You. Shhhh! It's A Secret..", false, true));
                else
                    _Player.instance.StartCoroutine(DisplayTip("Whoa! Watching Those Ads Will Give You A Chance To Get Extra Shots, Free Money And More! Think Twice. But Here's 10 Free Shots Just For You. Shhhh! It's A Secret..", false, true));
                shotsLeft += 10;
                PlayerPrefs.SetInt("remaining", shotsLeft);
                PlayerPrefs.SetInt("warned", 1);
                //Debug.Log("REACHED");
                //return;
            }
            else if (PlayerPrefs.GetInt("destroyed", 0) != 1 && PlayerPrefs.GetInt("warned", 0) == 1)
            {
                if(!inMenu)
                    _Player.instance.StartCoroutine(DisplayTip("Thought You Could Skip Again And I'd Give You Free Shots Huh? Nope I'm Taking Your Shots! Muhaha! Just Kidding But No Shots For You!", false, true));
                else
                    instance.StartCoroutine(DisplayTip("Thought You Could Skip Again And I'd Give You Free Shots Huh? Nope I'm Taking Your Shots! Muhaha! Just Kidding But No Shots For You!", false, true));
                PlayerPrefs.SetInt("destroyed", 1);
                //Debug.Log("REACHED");
                //return;
            }
            else
            {
                if (inMenu)
                     instance.StartCoroutine(DisplayTip("You're Missing Out On All Of These Free Shooooots! (Oprah Voice)", false, false));
                else
                    _Player.instance.StartCoroutine(DisplayTip("You're Missing Out On All Of These Free Shooooots! (Oprah Voice)", false, false));
                //Debug.Log(inMenu);
                // return;
            }
        }


    }
    #endregion

    //---------------------------------------
    #region misc
    public void OpenFile()
    {
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            if(File.Exists(Application.dataPath + "/Resources/data/stats.txt"))
                Application.OpenURL(Application.dataPath + "/Resources/data/stats.txt");
            else
                GameObject.Find("Open Doc").GetComponentInChildren<Text>().text = "No File";
        }
        else if (File.Exists(Application.persistentDataPath + "/Resources/data/stats.txt"))
        {
            Application.OpenURL(Application.persistentDataPath + "/Resources/data/stats.txt");   
        }
        else
            GameObject.Find("Open Doc").GetComponentInChildren<Text>().text = "No File"; //Attempt to find bug in loading file


    }

    //------------------------------------------

    public void ExitMenu()
    {
        inMenu = false;
        PlayerPrefs.Save();
    }

    //------------------------------------------

    public static bool AnythingActive()
    {
        if (paramsActive || soundActive || editActive)
            return true;
        else
            return false;
    }

    public void SetTime(int time)
    {
        TimeController.timeSetting = time;
    }

    IEnumerator Instruct(string instructions)
    {
        GameObject messenger = GameObject.Find("_Instructions");
        if (messenger.GetComponent<Text>().color == Color.red)
            yield break;
        string temp = messenger.GetComponent<Text>().text;
        messenger.GetComponent<Text>().color = Color.red;
        messenger.GetComponent<Text>().text = instructions;
        yield return new WaitForSeconds(3f);
        messenger.GetComponent<Text>().text = temp;
        messenger.GetComponent<Text>().color = Color.white;
    }

    public void GoToFull()
    {
        //Debug.Log("Attempting");
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.franchiz3.GreenLight19f");
    }
    #endregion
}
