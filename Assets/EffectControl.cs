using UnityEngine;
using System.Collections;

public class EffectControl : MonoBehaviour {
	private float effect_timer;
	private string effect_name;
	SoundManager sm;
	public AudioClip time_travel_clip;
	// Use this for initialization
	void Start () {
		effect_timer = 0;
		effect_name = null;
		sm = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
	}

	public void showParticleEffect(string name,float t){
		effect_name = name;
		effect_timer = t;
		this.transform.Find (effect_name).gameObject.SetActive (true);

		if (name == "holy") {
			this.transform.Find (effect_name).gameObject.GetComponent<ParticleSystem> ().playbackSpeed = 3.0f;
			sm.PlaySingle (time_travel_clip);
		}
	}

	

	// Update is called once per frame
	void Update () {
		if (effect_timer > 0)
			effect_timer -= Time.deltaTime;
		if (effect_timer < 0)
			effect_timer = 0f;
		if (effect_timer == 0 && this.transform.Find (effect_name).gameObject.activeSelf && effect_name != null) {
			this.transform.Find (effect_name).gameObject.SetActive (false);
		}
	}
}
