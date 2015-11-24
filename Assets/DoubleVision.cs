using UnityEngine;

public class DoubleVision : CC_Base
{
	public Vector2 displace = new Vector2(0.7f, 0.0f);
	
	[Range(0f, 1f)]
	public float amount = 1.0f;

	public Vector2 target_displace = new Vector2(0f,0f);
	public float target_amount = 0f;
	public float change_interval = 0.3f;
	public float current_interval = 0f;

	public float timer;
	public float current_timer;
	protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (amount == 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}
		
		material.SetVector("_Displace", new Vector2(displace.x / Screen.width, displace.y / Screen.height));
		material.SetFloat("_Amount", amount);
		Graphics.Blit(source, destination, material);
	}

	public void resetEffect(float t){
		amount = 0.4f;
		timer = t;
		current_timer = 0f;
		target_displace = Random.insideUnitCircle*1.2f;
	}

	void Update(){
		current_interval += Time.deltaTime;
		current_timer += Time.deltaTime;
		if (current_interval >= change_interval) {
			current_interval = 0;
			target_amount = (current_timer > timer*0.7)?0f:(Random.value*0.4f+0.6f);

		}
		if (current_timer > timer * 0.6) {
			target_displace = new Vector2 ();
		}
		if (current_timer > timer) {
			this.enabled = false;
		}
		amount += (amount > target_amount)?(-0.2f*Time.deltaTime):(1f*Time.deltaTime);
		displace += (target_displace - displace) * Time.deltaTime*1.5f;
		if (amount < 0)
			amount = 0;
		if (amount > 1)
			amount = 1f;
	}
}
