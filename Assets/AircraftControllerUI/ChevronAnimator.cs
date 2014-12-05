using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ChevronAnimator : MonoBehaviour
{
	public Sprite[] sprites;
	public float delay = 0.5f;
	private float timeSinceLast;
	private int index;

	// Update is called once per frame
	void Update()
	{
		timeSinceLast += Time.deltaTime;

		if (timeSinceLast > delay)
		{
			index++;
			if (index >= sprites.Length)
			{
				index = 0;
			}

			timeSinceLast = 0f;
			GetComponent<Image>().sprite = sprites[index];
		}
	}

	
	public void SetAscentAmount(float amount)
	{
		amount = Mathf.Clamp(amount, -1f, 1f);

		transform.parent.localScale = new Vector3(1f, 1f, Mathf.Sign(amount));

		Color c = GetComponent<Image>().color;
		c.a = Mathf.Abs(amount);
		GetComponent<Image>().color = c;
	}
}
