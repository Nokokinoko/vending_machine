using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using DG.Tweening;

namespace Teacups
{
	public class ReloadSceneManager : MonoBehaviour
	{
		public static ReloadSceneManager _Instance;

		[SerializeField] private CanvasGroup m_GrpCanvas;

		private readonly float TIME_FADE = 0.2f;

		private void Awake()
		{
			m_GrpCanvas.alpha = 0.0f;

			if (_Instance == null)
			{
				_Instance = this;
				DontDestroyOnLoad(this);

				if (Application.isEditor)
				{
					Observable.EveryUpdate()
						.Where(_ => Input.GetKey(KeyCode.Space))
						.ThrottleFirst(TimeSpan.FromSeconds(2.0f))
						.Subscribe(_ => FadeIn())
						.AddTo(this)
					;
				}
			}
			else
			{
				Destroy(this);
			}
		}

		public void FadeIn()
		{
			m_GrpCanvas.DOFade(1.0f, TIME_FADE)
				.OnComplete(() => {
					SceneManager.activeSceneChanged += (Scene pBefore, Scene pAfter) => FadeOut();

					var _Active = SceneManager.GetActiveScene();
					SceneManager.LoadScene(_Active.buildIndex);
				})
			;
		}

		private void FadeOut()
		{
			m_GrpCanvas.DOFade(0.0f, TIME_FADE);
		}
	}
}
