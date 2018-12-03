using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class _audio_control : MonoBehaviour {
	private static _audio_control _instance;
	public static _audio_control instance{get{if (!_instance){
				_instance = FindObjectOfType(typeof(_audio_control)) as _audio_control;}
			return _instance;}}
    public static bool noMusic = false, noFX = false;
	public AudioClip _song_main_theme;
    public  AudioClip[] goodClips, badClips, outliers, levelSpecific;
    private static bool clipsLoaded = false;
	public bool _music,_fx = true;
	public AudioSource _gameover;
	public AudioSource _click;
	public AudioSource _buy;
    public AudioSource achievement, inGame, inMenu;
	[HideInInspector]
	public Sprite[] _textures_sound_buttons = new Sprite[4];
	[HideInInspector]
	public Button[] _buttons = new Button[2];
	void Awake()
    {
		_loadprefs ();
		_play_song ();
	}

    //---------------------------------------

    public  string PlayGood()
    {
        AudioClip clipperDipper; ;
        if (Random.Range(0, 99) < 65 || _level_select.currentLevel == 6)
        {
            clipperDipper = goodClips[Random.Range(0, goodClips.Length - 1)];
        }
        else
        {
            //different sounds based on which level
            switch (_level_select.currentLevel)
            {
                case 0:
                    clipperDipper = levelSpecific[Random.Range(0,2)];
                    break;

                case 1:
                    clipperDipper = levelSpecific[Random.Range(3, 5)];
                    break;

                case 2:
                    clipperDipper = levelSpecific[Random.Range(8, 9)];
                    break;

                case 3:
                    clipperDipper = levelSpecific[Random.Range(12, 13)];
                    break;

                case 4:
                    clipperDipper = levelSpecific[Random.Range(18, 19)];
                    break;

                default:
                    clipperDipper = levelSpecific[Random.Range(20,21)];
                    break;
            }
        }
        PlayClip(clipperDipper);
        return clipperDipper.name;
    }

    //---------------------------------------

    public string PlayBad(bool airball)
    {
        AudioClip clipperDipper;
        string stageName = GetComponent<_design_control>()._levels[_level_select.currentLevel]._stage_name;
        if (airball && Random.Range(0, 100) < 60)
        {
            if (stageName == "Marsy Barsy" && Random.Range(0, 100) < 70)
                clipperDipper = levelSpecific[6];
            else if (stageName == "Just Deserts" && Random.Range(0, 100) < 70)
                clipperDipper = levelSpecific[25];
            else
                clipperDipper = outliers[Random.Range(16, 19)];
        }
        else
        {
            if (Random.Range(0, 99) < 65 || _level_select.currentLevel == 0 || _level_select.currentLevel == 5)
            {
                clipperDipper = badClips[Random.Range(0, badClips.Length - 1)];
            }
            else
            {
                switch (_level_select.currentLevel)
                {
                    case 1:
                        clipperDipper = levelSpecific[7];
                        break;

                    case 2:
                        clipperDipper = levelSpecific[Random.Range(10,11)];
                        break;

                    case 3:
                        clipperDipper = levelSpecific[Random.Range(14, 17)];
                        break;

                    case 5:
                        clipperDipper = levelSpecific[Random.Range(22,23)];
                        break;

                    default:
                        clipperDipper = levelSpecific[26];
                        break;
                }
            }
  
        }
        PlayClip(clipperDipper);
        return clipperDipper.name;
    }

    public string PlayGreen()
    {
        AudioClip clipperDipper;
        clipperDipper = outliers[Random.Range(1, 4)];
        PlayClip(clipperDipper);
        return clipperDipper.name;
    }

    //---------------------------------------

    public static string PlayClip(AudioClip clip)
    {
        if (noFX)
            return null;
        _Player.player.clip = clip;
        _Player.player.mute = false;
        _Player.player.Play();
        return clip.name;
    }

	//--------------------------------------- loading volume preferences

	void _loadprefs(){

		if (PlayerPrefs.GetInt ("_music") == 0) {
			_music = true;
            noMusic = false;
		} else {
			_music = false;
            noMusic = true;
		}
		//---------------------------------------
		if (PlayerPrefs.GetInt ("_fx") == 0) {
			_fx = true;
            noFX = false;
		} else {
			_fx = false;
            noFX = true;
		}

		// Update Buttons
		_change_sprite (0, _music);
		_change_sprite (1, _fx);

	}


	/*/---------------------------------------  mute toggle<-- obsolete and couldnt find button so I added my own
	public void _b_music() {

		if (_music || noMusic) {
			_music = false;
			PlayerPrefs.SetInt ("_music", 1); // Save option
			GetComponent<AudioSource> ().mute = true; // Mute True
		} else {
			_music = true;
			PlayerPrefs.SetInt ("_music", 0); // Save option

			if(!GetComponent<AudioSource> ().isPlaying && !noMusic){
				GetComponent<AudioSource> ().Play ();
			}
			GetComponent<AudioSource> ().mute = false; // Mute False
		}

		// Update Buttons
		_change_sprite (0, _music);

	}

	///--------------------------------------- sound effects mute also obsolete

	public void _b_fx() {
		
		if (_fx || noFX) {
			_fx = false;
			PlayerPrefs.SetInt ("_fx", 1); // Save option
		} else {
			_fx = true;
			PlayerPrefs.SetInt ("_fx", 0); // Save option
		}

		// Update Buttons
		_change_sprite (1, _fx);

	}

	*/
    
    //--------------------------------------- main theme

	public void _play_song(AudioClip _s = null){

		if (_s != null) { // Load Song
			GetComponent<AudioSource> ().clip = _s;
		} else {
			GetComponent<AudioSource> ().clip = _song_main_theme;
		}

		if (_music) {
			GetComponent<AudioSource> ().Play (); // Play Song
		}

	}

	//---------------------------------------

	public bool _play_sound(){
		return _fx;
	}

	//---------------------------------------

	public void _gameover_sound(){
		if (_fx) {
			_gameover.Play ();
		}
	}

	//---------------------------------------

	public void _hud_click(){
		if (_fx) {
			_click.Play ();
		}
	}
	//---------------------------------------

	public void _buy_sound(){
		if (_fx) {
			_buy.Play ();
		}
	}
	//---------------------------------------

	void _change_sprite(int _b,bool _action){

		if (_b == 0) {

			if (_action) {
				_buttons [0].image.sprite = _textures_sound_buttons [0];
			} else {
				_buttons [0].image.sprite = _textures_sound_buttons [1];
			}

		} else {
			
			if (_action) {
				_buttons [1].image.sprite = _textures_sound_buttons [2];
			} else {
				_buttons [1].image.sprite = _textures_sound_buttons [3];
			}

		}

	}

	//---------------------------------------
    private IEnumerator MuteForSeconds(AudioSource source, float sec)
    {
        source.Pause();
        yield return new WaitForSeconds(sec);
        source.UnPause();
    }

    public void AchievementSound()
    {
        if (_fx)
        {
            StartCoroutine(MuteForSeconds(GetComponent<AudioSource>(),2.5f));
            achievement.Play();
        }
            
    }

    public void PlayInGame()
    {
        if (_fx)
        {
            StartCoroutine(MuteForSeconds(GetComponent<AudioSource>(), 4));
            inGame.Play();
        }
            
    }

    public void PlayInMenu()
    {
        if (_fx)
        {
            StartCoroutine(MuteForSeconds(GetComponent<AudioSource>(), 4));
            inMenu.Play();
        }
           
    }
}