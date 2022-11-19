using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeWhenSnakeClose : MonoBehaviour {

	[SerializeField] private CanvasGroup _text1;
	[SerializeField] private CanvasGroup _text2;

	[SerializeField] private CanvasGroup _pause;
	[SerializeField] private CanvasGroup _slider;


	[SerializeField] private CanvasGroup _quitText;

	[SerializeField] private CanvasGroup _weightTextForwardPropagationRELu;



	private void Awake()
	{
		Move.SendPosition += (vec2) => ChangeAlpha(vec2);
	}

	public void SpeedSlider(float v)
	{
		v = -v + (11f / 50f);
		Time.fixedDeltaTime = v;
	}
	public void PauseGame()
	{
		if (Time.timeScale != 0)
		{
			FindObjectOfType<Slider>().interactable = false;
			Time.timeScale = 0;
		}
		else
		{
			FindObjectOfType<Slider>().interactable = true;
			Time.timeScale = 1;
		}
	}

	private void ChangeAlpha(Vector2 pos)
	{
		if (pos.x < -11.83f && pos.y > 4.34f)
		{
			StartCoroutine(FadeCoroutine(_pause, true));
		}
		else
		{

			StartCoroutine(FadeCoroutine(_pause, false));
		}
		if (pos.x < -9.94f && pos.y > 6.42f)
		{
			StartCoroutine(FadeCoroutine(_slider, true));
		}
		else
		{

			StartCoroutine(FadeCoroutine(_slider, false));
		}

		if (pos.x < -5.18f && pos.y < -3.46f)
		{
			StartCoroutine(FadeCoroutine(_text1, true));
		}
		else
		{

			StartCoroutine(FadeCoroutine(_text1, false));
		}

		if (pos.x < -5.96f && pos.y < -5.74f)
		{
			StartCoroutine(FadeCoroutine(_text2, true));
		}
		else
		{

			StartCoroutine(FadeCoroutine(_text2, false));
		}

		if (pos.x > 12.36f && pos.y > 6.2f)
		{

			StartCoroutine(FadeCoroutine(_quitText, true));
		}
		else
		{

			StartCoroutine(FadeCoroutine(_quitText, false));
		}

		if (pos.x > 6.24 && pos.y < -1.6f)
		{

			StartCoroutine(FadeCoroutine(_weightTextForwardPropagationRELu, true));
		}
		else
		{

			StartCoroutine(FadeCoroutine(_weightTextForwardPropagationRELu, false));
		}
	}

	private IEnumerator FadeCoroutine(CanvasGroup graphic, bool fadeOut)
	{
		float t = 0;
		while (t < 1f)
		{
			t += Time.unscaledDeltaTime;
			graphic.alpha = Mathf.Lerp(graphic.alpha, fadeOut ? 0.1f : 1, t / 1f);
			print(graphic.alpha);
			yield return new WaitForEndOfFrame();
		}
	}
}
