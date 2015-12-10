using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	private Animator anim;
	private GameManager gm;
	public float attack_radius = 1.8f;
	public float attack_cooldown = 0f;
	private Movement mv;
	static int atkState = Animator.StringToHash("Base Layer.ark1");
	// Use this for initialization
	void Start () {
		gm = GameObject.Find ("GameManager").GetComponent<GameManager>();
		anim = GetComponent<Animator>();
		mv = GetComponent<Movement> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (anim.GetCurrentAnimatorStateInfo(0).nameHash == atkState && !anim.IsInTransition(0)) {
			//a
		}
		if (attack_cooldown > 0) {

			attack_cooldown -= Time.deltaTime;
			if (attack_cooldown < 0) {
				attack_cooldown = 0;
			}else if(attack_cooldown > 0){
				return;
			}
			if(attack_cooldown == 0){
				mv.ResumeMovement ();
			}
		}

		GameObject player_go = gm.GetPlayer ();
		if (player_go == null)
			return;
		if (mv.current_state != player_go.GetComponent<PlayerControl> ().player_state) {
			return;
		}
		if (player_go.GetComponent<PlayerControl> ().shield_on) {
			return;
		}
		float dis = (player_go.transform.position - transform.position).sqrMagnitude;
		if (dis < attack_radius) {
			anim.SetBool ("Attack", true);
			mv.BlockMovement ();
			player_go.GetComponent<PlayerControl>().Stunned();
			attack_cooldown = 3f;
		} else {
			mv.ResumeMovement ();
			anim.SetBool ("Attack", false);
		}
	}
	
}
