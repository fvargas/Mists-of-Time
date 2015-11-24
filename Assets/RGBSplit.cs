using UnityEngine;

public class RGBSplit : CC_Base
{
	public float amount = 0f;
	public float angle = 0f;

	public float timer;
	public float current_timer;

	public float target_amount;
	public float target_angle;

	protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (amount == 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}

		material.SetFloat("_RGBShiftAmount", amount * 0.001f);
		material.SetFloat("_RGBShiftAngleCos", Mathf.Cos(angle));
		material.SetFloat("_RGBShiftAngleSin", Mathf.Sin(angle));
		Graphics.Blit(source, destination, material);
	}
	public void resetEffect(float t){
		target_amount = Random.value*7+5;
		target_angle = Random.value*360;
		amount = 0;
		angle = target_angle;
		timer = t;
		current_timer = 0f;
	}

	void Update(){
		current_timer += Time.deltaTime;
		if (current_timer > timer * 0.3) {
			amount -= 20 * Time.deltaTime;
		} else {
			amount += 40 * Time.deltaTime;
		}
		if (current_timer > timer) {
			this.enabled = false;
		}
		if (amount < 0)
			amount = 0;
		if (amount > 12f)
			amount = 12f;
	}
}
