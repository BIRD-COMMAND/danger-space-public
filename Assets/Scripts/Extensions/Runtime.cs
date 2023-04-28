using System;
using UnityEngine;

namespace Extensions
{

	/// <summary>
	/// A class with static event callbacks for MonoBehaviour events such as Update, FixedUpdate, etc.<br/>
	/// By referencing this class, non-MonoBehaviour classes can receive callbacks for MonoBehaviour events.
	/// </summary>
	public class Runtime : MonoBehaviour
	{

		/// <summary>
		/// Static instance of this class. Receives events like Update, FixedUpdate, etc. and triggers callbacks.
		/// </summary>
		private static Runtime s_Instance;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void TryCreateInstance()
		{

			if (s_Instance) { return; }

			Runtime[] runtimeInstances = Resources.FindObjectsOfTypeAll<Runtime>();

			if (runtimeInstances.Length > 0) { 
				s_Instance = runtimeInstances[0]; 
			}
			else { 
				s_Instance = new GameObject("RuntimeManager", typeof(Runtime)).GetComponent<Runtime>(); 
				s_Instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
			}

		}

		/// <summary>
		/// The value of Time.time at the beginning of the last Fixed Update.
		/// </summary>
		public static float FixedUpdateLastTimestamp { get; private set; } = 0f;

		#region Callback Delegates

		/// <summary>
		/// Triggers callback when the OnApplicationPause event occurs.
		/// </summary>
		public static event Action<bool> ONApplicationPause = delegate {};
		/// <summary>
		/// Triggers callback when the OnApplicationFocus event occurs.
		/// </summary>
		public static event Action<bool> ONApplicationFocus = delegate {};
		/// <summary>
		/// Triggers callback when the OnApplicationQuit event occurs.
		/// </summary>
		public static event Action ONApplicationQuit = delegate {};

		/// <summary>
		/// Triggers callback when the OnGui event occurs.
		/// </summary>
		public static event Action ONGui = delegate {};
		/// <summary>
		/// Triggers callback when the OnUpdate event occurs.
		/// </summary>
		public static event Action ONUpdate = delegate {};
		/// <summary>
		/// Triggers callback when the OnLateUpdate event occurs.
		/// </summary>
		public static event Action ONLateUpdate = delegate {};
		/// <summary>
		/// Triggers callback when the OnFixedUpdate event occurs.
		/// </summary>
		public static event Action ONFixedUpdate = delegate {};

		/// <summary>
		/// Triggers callback when the OnPreCull event occurs.
		/// </summary>
		public static event Action ONPreCull = delegate {};
		/// <summary>
		/// Triggers callback when the OnPreRender event occurs.
		/// </summary>
		public static event Action ONPreRender = delegate {};
		/// <summary>
		/// Triggers callback when the OnPostRender event occurs.
		/// </summary>
		public static event Action ONPostRender = delegate {};
		/// <summary>
		/// Triggers callback when the OnRenderImage event occurs.
		/// </summary>
		public static event Action<RenderTexture, RenderTexture> ONRenderImage = delegate {};

		/// <summary>
		/// Triggers callback when the OnYieldNull Coroutine event occurs.
		/// </summary>
		public static event Action ONYieldNull = delegate {};
		/// <summary>
		/// Triggers callback when the OnWaitForFixedUpdate Coroutine event occurs.
		/// </summary>
		public static event Action ONWaitForFixedUpdate = delegate {};
		/// <summary>
		/// Triggers callback when the OnWaitForEndOfFrame Coroutine event occurs.
		/// </summary>
		public static event Action ONWaitForEndOfFrame = delegate {};

		#endregion

		#region Application callback Events

		private void OnApplicationPause(bool pause) { ONApplicationPause(pause); }
		private void OnApplicationFocus(bool focus) { ONApplicationFocus(focus); }
		private void OnApplicationQuit() { ONApplicationQuit(); }

		#endregion

		#region GUI and Update Events

		private void OnGUI() { ONGui(); }
		private void Update() { ONUpdate(); }
		private void LateUpdate() { ONLateUpdate(); }
		private void FixedUpdate() { FixedUpdateLastTimestamp = Time.time; ONFixedUpdate(); }

		#endregion

		#region Render Callback Events

		private void OnPreCull() { ONPreCull(); }
		private void OnPreRender() { ONPreRender(); }
		private void OnPostRender() { ONPostRender(); }
		private void OnRenderImage(RenderTexture source, RenderTexture destination) { 
			ONRenderImage(source, destination); 
		}

		#endregion

		#region Coroutine Callbacks

		// Initialization of callback coroutines
		private void Start() {
			yieldNullCoroutine = StartCoroutine(YieldNull());
			waitForFixedUpdateCoroutine = StartCoroutine(WaitForFixedUpdate());
			waitForEndOfFrameCoroutine  = StartCoroutine(WaitForEndOfFrame());
		}
		// Stop all callback coroutines
		private void OnDestroy() {
			if (yieldNullCoroutine != null)				{ StopCoroutine(yieldNullCoroutine); }
			if (waitForFixedUpdateCoroutine != null)	{ StopCoroutine(waitForFixedUpdateCoroutine); }
			if (waitForEndOfFrameCoroutine != null)		{ StopCoroutine(waitForEndOfFrameCoroutine); }
		}

		/// <summary>
		/// Coroutine for triggering the ONYieldNull event.
		/// </summary>
		private System.Collections.IEnumerator YieldNull() {
			while (true) {
				yield return null;
				ONYieldNull();
			}
		}
		private static Coroutine yieldNullCoroutine;

		/// <summary>
		/// Coroutine for triggering the ONWaitForFixedUpdate event.
		/// </summary>
		private System.Collections.IEnumerator WaitForFixedUpdate() {
			while (true) {
				yield return waitForFixedUpdate;
				ONWaitForFixedUpdate();
			}
		}
		private static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
		private static Coroutine waitForFixedUpdateCoroutine;

		/// <summary>
		/// Coroutine for triggering the ONWaitForEndOfFrame event.
		/// </summary>
		private System.Collections.IEnumerator WaitForEndOfFrame() {
			while (true) {
				yield return waitForEndOfFrame;
				ONWaitForEndOfFrame();
			}
		}
		private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
		private static Coroutine waitForEndOfFrameCoroutine;

		/// <summary>
		/// If both parameters are valid, uses a coroutine to execute the specified action after the specified number of seconds.<br/>
		/// If either parameter is invalid the action is not executed
		/// </summary>
		public static void DoInSeconds(Action action, float seconds) { s_Instance.StartCoroutine(s_Instance.M_DoInSeconds(action, seconds)); }
		private System.Collections.IEnumerator M_DoInSeconds(Action action, float seconds) {
			// if action is null throw exception
			if (action == null) { throw new ArgumentNullException("action"); }
			// if seconds is not valid perform action immediately
			if (!float.IsNormal(seconds) || seconds < 0f) { action(); }
			// else WaitForSeconds and perform action
			else { yield return new WaitForSeconds(seconds); action(); }
		}

		#endregion

	}
}
