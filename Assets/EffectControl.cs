using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EffectControl : MonoBehaviour {
	private Dictionary<string, float> effect_timers;
	SoundManager sm;
	public AudioClip time_travel_clip;
	// Use this for initialization
	void Start () {
		effect_timers = new Dictionary<string, float> ();
		sm = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
	}

	public void showParticleEffect(string name,float t){
		if (effect_timers.ContainsKey (name)) {
			effect_timers [name] = t;
		} else {
			effect_timers.Add (name, t);
		}
		this.transform.Find (name).gameObject.SetActive (true);

		if (name == "holy") {
			this.transform.Find (name).gameObject.GetComponent<ParticleSystem> ().playbackSpeed = 3.0f;
			sm.PlaySingle (time_travel_clip);
		}
	}

	

	// Update is called once per frame
	void Update () {
		List<string> keys_to_remove = new List<string> ();
		foreach(KeyValuePair<string, float> entry in effect_timers)
		{
			string effect_name = entry.Key;
			float effect_timer = entry.Value;
			if (effect_timer > 0){
				effect_timer -= Time.deltaTime;
				if (effect_timer <= 0){
					keys_to_remove.Add(effect_name);
				}else{
					effect_timers[effect_name] = effect_timer;
				}
			}

			if (effect_timer <= 0 && this.transform.Find (effect_name).gameObject.activeSelf && effect_name != null) {
				this.transform.Find (effect_name).gameObject.SetActive (false);
			}
		}
		foreach (string n in keys_to_remove) {
			effect_timers.Remove (n);
		}
	}
}
