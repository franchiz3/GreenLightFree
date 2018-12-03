using UnityEngine;
using System.Collections;

public class _clouds_scroll : MonoBehaviour {
	public float scrollSpeed = 1f;

	void Update()
	{
		float offset = Time.time * scrollSpeed;
		GetComponent<Renderer> ().material.SetTextureOffset ("_MainTex", new Vector2 (-offset,0));
	}
}
