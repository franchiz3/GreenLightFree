using UnityEngine;
using System.Collections;

public class _basket_trigger : MonoBehaviour {

	public int _is_trigger = 0;
	public bool _enterball = false;
	public _basket_trigger _another_trigger;

	void Awake(){
		_Player.instance._addtriggers(_is_trigger,this);
	}
	//---------------------------------------
	public void _reset(){
		_enterball = false;
	}
	//---------------------------------------
	void OnTriggerExit(Collider other) {
		
		//if (other.name == "Ball") {testing taking this out because I changed the names of the balls.. might just make them all the same name

			if (_is_trigger == 1 && !_enterball) {

				if (_another_trigger._enterball) {

					_another_trigger._enterball = false;
					_Player.instance._add_point ();
                    
					if (_audio_control.instance._play_sound ()) {
						GetComponent<AudioSource> ().Play ();
					}

				}


			}

			_enterball = true;
		//}
	}
	//---------------------------------------
}
