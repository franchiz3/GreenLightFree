using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour {

    public static TimeController instance;
    public UnityEngine.UI.Text timeText, score, highScore, scoreT;
    public AudioClip three,two,one, time,second,third,fourth,clutch;
    public GameObject countDown;
    public static GameObject highScoreT; 
    public static UnityEngine.UI.Text timeDisplay;
    public static float startTime;
    public static int timeSetting, remainingTime;
    private bool showing = false;
    public static  bool started,ending,gameover=false;
    public static Coroutine gameTime, gameEnd;
    private bool trigger2, trigger3, trigger4;
    public AudioSource source;
    // Use this for initialization
	void Start () {
        timeDisplay = timeText;
        ending = false;
        instance = this;
    }
	
	// Update is called once per frame
	void Update () {
       
        
	}

    public static void StopRoutines()
    {
        instance.StopAllCoroutines();
        started = false;
        ending = false;
    }
    private void DisplayStats()
    {
        started = false;
        ending = false;
        scoreT.text = "Score " + timeSetting + "s: ";
        score.text = _Player.score.ToString();
        GameObject.Find("_GameControl").GetComponent<hud_control>()._objects_hud_control[2].SetActive(true);
        GameObject.Find("_GameControl").GetComponent<_audio_control>()._gameover_sound();
        highScoreT = GameObject.Find("_NewRecord");
        
        if (highScoreT)
            highScoreT.SetActive(false);
        if (PlayerPrefs.GetInt("highScore" + timeSetting) < _Player.score)
        {
            PlayerPrefs.SetInt("highScore" + timeSetting, _Player.score);
            dreamloLeaderBoard.SwitchLinks(timeSetting);
            ButtonMaster.dl.AddScore(ButtonMaster.playerName, PlayerPrefs.GetInt("highScore"+timeSetting));
            highScoreT.SetActive(true);
        }
        highScore.text = PlayerPrefs.GetInt("highScore" + timeSetting).ToString();
        _Player.score = 0;
        if (ButtonMaster.shotsLeft <= 0)
        {
            ButtonMaster.shotsLeft = 0;
            StartCoroutine(ButtonMaster.DisplayTip("Sorry You're Out Of Shots Today :(. Have No Fear, You Can Earn More By Watching An Ad! (Main Menu)", false, true));
            ButtonMaster.playActive = false;
            //System.DateTime end = System.DateTime.Now.AddDays(1);
            //if (!PlayerPrefs.HasKey("coolEnd"))
            //  PlayerPrefs.SetString("coolEnd", end.ToString());
            //StartCoroutine(ButtonMaster.RefreshCountDown());
            _Player.shotEnabled = false;
            return;
        }
    }
    public IEnumerator CountItDown()
    {
        trigger2 = false;
        trigger3 = false;
        trigger4 = false;
        _Player.score = 0;
        for (int t = timeSetting; t>0; t--)
        {
            remainingTime = t;
            if (!trigger2 && GetQuarter() == 2)
            {
                trigger2 = true;
                source.clip = second;
                if(!_audio_control.noFX)
                    source.Play();
            }
            if(!trigger3 && GetQuarter() == 3)
            {
                trigger3 = true;
                source.clip = third;
                if (!_audio_control.noFX)
                    source.Play();
            }
            if (!trigger4 && GetQuarter() == 4)
            {
                trigger4 = true;
                source.clip = fourth;
                if (!_audio_control.noFX)
                    source.Play();
            }
            if (t == 5)
            {
                source.clip = clutch;
                if (!_audio_control.noFX)
                    source.Play();
            }

            if (t == 3)
                gameEnd = StartCoroutine(CountingDown());
            timeText.text = "Q: " +GetQuarter()+"\tTime: " + t + "s";
            yield return new WaitForSeconds(1);
        }
    }

    public static int GetQuarter()
    {
        float temp = remainingTime;
        if (temp > timeSetting * .75)
            return 1;
        else if (temp > timeSetting * .50)
            return 2;
        else if (temp > timeSetting * .25)
            return 3;
        else
            return 4;
    }

    private IEnumerator CountingDown()
    {
        ending = true;
        countDown.SetActive(true);
        countDown.GetComponentInChildren<UnityEngine.UI.Text>().text = "3";
        _audio_control.PlayClip(three);
        yield return new WaitForSeconds(1f);
        countDown.GetComponentInChildren<UnityEngine.UI.Text>().text = "2";
        _audio_control.PlayClip(two);
        yield return new WaitForSeconds(1f);
        countDown.GetComponentInChildren<UnityEngine.UI.Text>().text = "1";
        _audio_control.PlayClip(one);
        yield return new WaitForSeconds(1f);
        _audio_control.PlayClip(time);
        countDown.GetComponentInChildren<UnityEngine.UI.Text>().text = "TIME!";
        gameover = true;
        _Player.shotEnabled = false;
        _Player.instance.KillThemAll();
        yield return new WaitForSeconds(1f);
        showing = false;
        countDown.SetActive(false);
        ButtonMaster.ShowBannerAd();
        DisplayStats();
       
    }
   
}
