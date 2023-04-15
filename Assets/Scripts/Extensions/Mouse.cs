using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Utils;

namespace Extensions
{
	
	[ExecuteInEditMode]
	public static class Mouse
	{

		/// <summary>
		/// Returns UnityEngine.InputSystem.Mouse.current. This value can be null if no mouse is connected.
		/// </summary>
		public static UnityEngine.InputSystem.Mouse Current => UnityEngine.InputSystem.Mouse.current;

		#if UNITY_EDITOR
		/// <summary>
		/// Stores the type of the GameView for retrieval of the mouse position over the Game View in the editor.
		/// </summary>
		public static Type GameViewType;
		/// <summary>
		/// Stores the type of the SceneView for retrieval of the mouse position over the Scene View in the editor.
		/// </summary>
		public static Type SceneViewType;
		#endif
		
		static Mouse() {
			#if UNITY_EDITOR
			// Get the GameView and SceneView types for use in the editor
			GameViewType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.GameView");
			SceneViewType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.SceneView");
			#endif
		}

		[RuntimeInitializeOnLoadMethod]
		private static void RuntimeInitialization()
		{
			// add a callback OnLateUpdate to save the mouse position
			Runtime.ONLateUpdate += SaveLastMousePosition;
			// add a callback OnFixedUpdate to save the mouse position
			Runtime.ONFixedUpdate += SaveFixedUpdateMousePosition;
			// Initialize saved mouse position buffers
			for (int i = 0; i < mousePositionsBufferSize; i++) {
				positions.PushFront(Vector2.zero); worldPositions.PushFront(Vector2.zero);
				fixedPositions.PushFront(Vector2.zero); fixedWorldPositions.PushFront(Vector2.zero);
			}
		}

		#region Position and Position Queries

		/// <summary> Returns the current mouse position.<br/><br/>
		/// Within player code, the coordinates are in the coordinate space of Unity's Display.<br/><br/>
		/// Within editor code, the coordinates are in the coordinate space of the current EditorWindow.<br/><br/>
		/// This means that if you query the Mouse position in EditorWindow.OnGUI, for example,<br/>
		/// the returned Vector2 will be in the coordinate space of your local GUI (like Event.mousePosition).
		/// </summary>
		public static Vector2 Position => Current?.position.value.Validate() ?? Vector2.zero;

		/// <summary>
		/// Current mouse position in worldspace. 
		/// </summary>
		public static Vector2 WorldPosition {
			get {
				#if UNITY_EDITOR
				if (worldPositionCalculatedThisFrame) { worldPositionCalculatedThisFrame = false; }
				try {
					if (Application.isFocused) { worldPosition = WorldPositionForCamera(Camera.main); }
					else { worldPosition = WorldPositionForCamera(UnityEditor.SceneView.lastActiveSceneView.camera); }
				}
				catch { return Vector2.zero; }
#else
					if (!worldPositionCalculatedThisFrame) {
						try { worldPosition = WorldPositionForCamera(Camera.main); }
						catch { worldPosition = Vector2.zero; }
						worldPositionCalculatedThisFrame = true;
					}
#endif
				return worldPosition;
			}
		}
		private static bool worldPositionCalculatedThisFrame = false;
		private static Vector2 worldPosition;

		/// <summary>
		/// Returns camera.ScreenToWorldPoint(Position), or Vector2.zero if the camera is null.
		/// </summary>
		public static Vector2 WorldPositionForCamera(Camera camera)
		{
			if (!camera) { return Vector2.zero; }
			return camera.ScreenToWorldPoint(Position);
		}

		/// <summary>
		/// Mouse position in worldspace during the previous frame. 
		/// </summary>
		public static Vector2 WorldPositionLastFrame => worldPositionLastFrame;
		private static Vector2 worldPositionLastFrame;
		private static void SaveLastMousePosition()
		{
			worldPositionLastFrame = WorldPosition;
#if !UNITY_EDITOR
			worldPositionCalculatedThisFrame = false;
#endif
			worldPositions.PushFront(worldPositionLastFrame);
			positions.PushFront(Position);
		}

		/// <summary>
		/// Mouse position in worldspace during the previous fixed update. 
		/// </summary>
		public static Vector2 WorldPositionLastFixedUpdate => worldPositionLastFixedUpdate;
		private static Vector2 worldPositionLastFixedUpdate = Vector2.zero;
		private static void SaveFixedUpdateMousePosition()
		{
			worldPositionLastFixedUpdate = WorldPosition;
			fixedWorldPositions.PushFront(worldPositionLastFixedUpdate);
			fixedPositions.PushFront(Position);
		}

		private const int mousePositionsBufferSize = 1000;
		
		/// <summary>
		/// Stores the last 1000 mouse positions. The most recent mouse position can always be accesed at index 0. <br/><br/>
		/// A new mouse position is added every LateUpdate, so the current frame's mouse position is not available until after LateUpdate.
		/// </summary>
		public static CircularBuffer<Vector2> Positions => positions;
		private static CircularBuffer<Vector2> positions = new CircularBuffer<Vector2>(mousePositionsBufferSize);
		
		/// <summary>
		/// Stores the last 1000 mouse worldspace positions. The most recent mouse position can always be accesed at index 0. <br/><br/>
		/// A new mouse position is added every LateUpdate, so the current frame's mouse position is not available until after LateUpdate.
		/// </summary>
		public static CircularBuffer<Vector2> WorldPositions => worldPositions;
		private static CircularBuffer<Vector2> worldPositions = new CircularBuffer<Vector2>(mousePositionsBufferSize);
		
		/// <summary>
		/// Stores the last 1000 FixedUpdate mouse positions. The most recent mouse position can always be accesed at index 0.
		/// </summary>
		public static CircularBuffer<Vector2> FixedPositions => fixedPositions;
		private static CircularBuffer<Vector2> fixedPositions = new CircularBuffer<Vector2>(mousePositionsBufferSize);
		
		/// <summary>
		/// Stores the last 1000 FixedUpdate mouse worldspace positions. The most recent mouse position can always be accesed at index 0.
		/// </summary>
		public static CircularBuffer<Vector2> FixedWorldPositions => fixedWorldPositions;
		private static CircularBuffer<Vector2> fixedWorldPositions = new CircularBuffer<Vector2>(mousePositionsBufferSize);

		/// <summary>
		/// Returns true if the mouse is within the Application's game window. 
		/// </summary>
		public static bool IsInsideGameWindow => IsMouseInsideGameWindow();
		/// <summary>
		/// Returns true if the mouse is within the Application's game window. 
		/// </summary>
		private static bool IsMouseInsideGameWindow()
		{
			if (Current == null) { return false; }
			// determine if the mouse is within the application's game view
#if UNITY_EDITOR
			if (!UnityEditor.EditorWindow.mouseOverWindow) { return false; }
			// check if EditorWindow.mouseOverWindow is a GameView
			try { return UnityEditor.EditorWindow.mouseOverWindow.GetType() == GameViewType; }
			catch { return false; }
#else
			return 
				Position.x >= 0f && Position.x <= Screen.width && 
				Position.y >= 0f && Position.y <= Screen.height;
#endif
		}

#if UNITY_EDITOR
		/// <summary>
		/// Returns true if the mouse is over a SceneView window. 
		/// </summary>
		public static bool IsWithinSceneViewRect => IsMouseWithinSceneViewRect();
		/// <summary>
		/// Returns true if the mouse is over a SceneView window. 
		/// </summary>
		private static bool IsMouseWithinSceneViewRect()
		{
			if (Current == null || !UnityEditor.EditorWindow.mouseOverWindow) { return false; }
			try { return UnityEditor.EditorWindow.mouseOverWindow.GetType() == SceneViewType; }
			catch { return false; }
		}
#endif

		#endregion

		#region Relative Position Queries

		/// <summary>
		/// Returns a Vector2 from position to the mouse worldspace position.
		/// </summary>
		public static Vector2 ToMouse(this Transform t) { return WorldPosition - (Vector2)t.position; }
		/// <summary>
		/// Returns a Vector2 from this vector to the mouse worldspace position.
		/// </summary>
		public static Vector2 ToMouse(this Vector2 vec) { return WorldPosition - vec; }
		/// <summary>
		/// Returns a Vector2 from this vector to the mouse worldspace position.
		/// </summary>
		public static Vector2 ToMouse(this Vector3 vec) { return WorldPosition - (Vector2)vec; }

		/// <summary>
		/// Returns the normalized direction from position to the mouse worldspace position.
		/// </summary>
		public static Vector2 DirToMouse(this Transform t) { return (WorldPosition - (Vector2)t.position).normalized; }
		/// <summary>
		/// Returns the normalized direction from this vector to the mouse worldspace position.
		/// </summary>
		public static Vector2 DirToMouse(this Vector2 vec) { return (WorldPosition - vec).normalized; }
		/// <summary>
		/// Returns the normalized direction from this vector to the mouse worldspace position.
		/// </summary>
		public static Vector2 DirToMouse(this Vector3 vec) { return (WorldPosition - (Vector2)vec).normalized; }

		/// <summary>
		/// Returns the distance from position to the mouse worldspace position.
		/// </summary>
		public static float DistToMouse(this Transform t) { return (WorldPosition - (Vector2)t.position).magnitude; }
		/// <summary>
		/// Returns the distance from this vector to the mouse worldspace position.
		/// </summary>
		public static float DistToMouse(this Vector2 vec) { return (WorldPosition - vec).magnitude; }
		/// <summary>
		/// Returns the distance from this vector to the mouse worldspace position.
		/// </summary>
		public static float DistToMouse(this Vector3 vec) { return (WorldPosition - (Vector2)vec).magnitude; }

		#endregion

		#region Mouse Utilities and Scene Queries

		/// <summary>
		/// A hidden GameObject with a kinematic Rigidbody2D component attached that follows the mouse position.<br/>
		/// This is useful for things like attaching a SpringJoint2D to the mouse, and connecting it to a Rigidbody2D in the scene.
		/// </summary>
		public static Rigidbody2D Body {
			get {
				if (!body) {
#if UNITY_EDITOR
					if (!Application.isPlaying) { return null; }
#endif
					GameObject mouseBodyGO = new GameObject("MouseRigidbody2D", typeof(Rigidbody2D));
					mouseBodyGO.hideFlags = HideFlags.HideAndDontSave;
					body = mouseBodyGO.GetComponent<Rigidbody2D>();
					body.bodyType = RigidbodyType2D.Kinematic;
					Runtime.ONFixedUpdate += UpdateMouseRigidbody2D;
				}
				return body;
			}
		}
		private static Rigidbody2D body = null;
		/// <summary>
		/// This method is called every FixedUpdate, and updates the position of the MouseRigidbody2D to match the mouse position.
		/// </summary>
		private static void UpdateMouseRigidbody2D() { Body.position = WorldPosition; }

		/// <summary>
		/// Checks all Collider2Ds under the mouse, if a Rigidbody2D is found it is assigned to the out parameter and the method returns true.<br/>
		/// If no body is found, returns false and assigns null to the out parameter.
		/// </summary>
		public static bool HoveredRigidbody2D(out Rigidbody2D body)
		{
			if (HoveredRigidbody2Ds(out List<Rigidbody2D> bodies)) {
					body = bodies[0]; return true;
			}
			else {
					body = null;	  return false; 
			}
		}

		/// <summary>
		/// Checks all Collider2Ds under the mouse, if any Rigidbody2Ds are found they are added to the out parameter List and the method returns true.<br/>
		/// If no Rigidbody2Ds are found, returns false and assigns an empty List&lt;Rigidbody2D&gt; to the out parameter.
		/// </summary>
		public static bool HoveredRigidbody2Ds(out List<Rigidbody2D> bodies)
		{
			bodies = HoveredItems<Rigidbody2D>(); return bodies.Count == 0;
		}

		/// <summary>
		/// Returns all items of type <typeparamref name="T"/> under the mouse attached to any non-Trigger Collider2D.
		/// </summary>
		public static List<T> HoveredItemsWithCollider<T>() where T : Component
		{
			bool queriesHitTriggers = Physics2D.queriesHitTriggers;
			Physics2D.queriesHitTriggers = false;
			IEnumerable<Collider2D> filtered =
				Physics2D.OverlapPointAll(WorldPosition)
				.Where(c => !c.isTrigger && c.attachedRigidbody != Body && c.transform.root.GetComponentInChildren<T>());
			Physics2D.queriesHitTriggers = queriesHitTriggers;
			return filtered.SelectMany(c => c.transform.root.GetComponentsInChildren<T>()).ToList();
		}

		/// <summary>
		/// Returns all items of type <typeparamref name="T"/> under the mouse attached to any Trigger Collider2D.
		/// </summary>
		public static List<T> HoveredItemsWithTrigger<T>() where T : Component
		{
			bool queriesHitTriggers = Physics2D.queriesHitTriggers;
			Physics2D.queriesHitTriggers = true;
			IEnumerable<Collider2D> filtered =
				Physics2D.OverlapPointAll(WorldPosition)
				.Where(c => c.isTrigger && c.attachedRigidbody != Body && c.transform.root.GetComponentInChildren<T>());
			Physics2D.queriesHitTriggers = queriesHitTriggers;
			return filtered.SelectMany(c => c.transform.root.GetComponentsInChildren<T>()).ToList();
		}

		/// <summary>
		/// Returns all items of type <typeparamref name="T"/> under the mouse attached to any type of Collider2D.
		/// </summary>
		public static List<T> HoveredItems<T>() where T : Component
		{
			bool queriesHitTriggers = Physics2D.queriesHitTriggers;
			Physics2D.queriesHitTriggers = true;
			IEnumerable<Collider2D> filtered =
				Physics2D.OverlapPointAll(WorldPosition)
				.Where(c => c.attachedRigidbody != Body && c.transform.root.GetComponentInChildren<T>());
			Physics2D.queriesHitTriggers = queriesHitTriggers;
			return filtered.SelectMany(c => c.transform.root.GetComponentsInChildren<T>()).ToList();
		}

		#endregion

		#region Mouse Button and Movement Queries

		/// <summary>
		/// Returns the value of Mouse.current.scroll.value.y, or 0f if Mouse.current is null. 
		/// </summary>
		public static float ScrollY => Current?.scroll.value.y ?? 0f;
		/// <summary>
		/// Returns true if Mouse.current.scroll.value.y does not equal 0f, or false if Mouse.current is null. 
		/// </summary>
		public static bool DidScrollY => !Mathf.Approximately(Current?.scroll.y.value ?? 0f, 0f);
		/// <summary>
		/// Returns the value of Mouse.current.scroll.value.x, or 0f if Mouse.current is null. 
		/// </summary>
		public static float ScrollX => Current?.scroll.value.x ?? 0f;
		/// <summary>
		/// Returns true if Mouse.current.scroll.value.x does not equal 0f, or false if Mouse.current is null. 
		/// </summary>
		public static bool DidScrollX => !Mathf.Approximately(Current?.scroll.x.value ?? 0f, 0f);

		/// <summary>
		/// Returns true if the Left mouse button is currently held down. 
		/// </summary>
		public static bool LeftDown => Current?.leftButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the Left mouse button was pressed down this frame. 
		/// </summary>
		public static bool LeftClick => Current?.leftButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the Left mouse button was released this frame. 
		/// </summary>
		public static bool LeftRelease => Current?.leftButton.wasReleasedThisFrame ?? false;

		/// <summary>
		/// Returns true if the Right mouse button is currently held down. 
		/// </summary>
		public static bool RightDown => Current?.rightButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the Right mouse button was pressed down this frame. 
		/// </summary>
		public static bool RightClick => Current?.rightButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the Right mouse button was released this frame. 
		/// </summary>
		public static bool RightRelease => Current?.rightButton.wasReleasedThisFrame ?? false;

		/// <summary>
		/// Returns true if the Middle mouse button is currently held down. 
		/// </summary>
		public static bool MiddleDown => Current?.middleButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the Middle mouse button was pressed down this frame. 
		/// </summary>
		public static bool MiddleClick => Current?.middleButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the Middle mouse button was released this frame. 
		/// </summary>
		public static bool MiddleRelease => Current?.middleButton.wasReleasedThisFrame ?? false;
		
		/// <summary>
		/// Mouse movement in worldspace since the last frame. 
		/// </summary>
		public static Vector2 DeltaFrame => WorldPositionLastFrame.To(WorldPosition);
		/// <summary>
		/// Mouse movement in worldspace since the last fixed update. 
		/// </summary>
		public static Vector2 DeltaFixed => WorldPositionLastFixedUpdate.To(WorldPosition);

		/// <summary>
		/// Mouse movement velocity in worldspace units/second since the last frame. 
		/// </summary>
		public static Vector2 VelocityFrame => DeltaFrame / Time.deltaTime;
		/// <summary>
		/// Mouse movement velocity in worldspace units/second since the last fixed update. 
		/// </summary>
		public static Vector2 VelocityFixed => DeltaFixed / Mathf.Max(0.0001f, Time.time - Runtime.FixedUpdateLastTimestamp);

		#endregion

		#region Average Movement and Velocity Queries

		/// <summary>
		/// Returns the average movement of the mouse (in mousePosition space) over the past <paramref name="frames"/> frames.
		/// </summary>
		public static Vector2 AverageVelocity(int frames)
		{
			if (frames < 1) { throw new ArgumentOutOfRangeException(nameof(frames)); }
			Vector2 average = Vector2.zero; int count = frames;
			if (positions[0] != Position) { average = Position - Positions[0]; count--; }
			for (int i = 0; i < count; i++) { average += positions[i] - positions[i + 1]; }
			return average / frames;
		}
		/// <summary>
		/// Returns the average movement of the mouse (in worldspace) over the past <paramref name="frames"/> frames.
		/// </summary>
		public static Vector2 AverageWorldVelocity(int frames)
		{
			if (frames < 1) { throw new ArgumentOutOfRangeException(nameof(frames)); }
			Vector2 average = Vector2.zero; int count = frames;
			if (worldPositions[0] != WorldPosition) { average = WorldPosition - worldPositions[0]; count--; }
			for (int i = 0; i < count; i++) { average += worldPositions[i] - worldPositions[i + 1]; }
			return average / frames;
		}

		/// <summary>
		/// Returns the average movement of the mouse (in mousePosition space) over the past <paramref name="frames"/> frames.
		/// </summary>
		public static Vector2 AverageFixedVelocity(int frames)
		{
			if (frames < 1) { throw new ArgumentOutOfRangeException(nameof(frames)); }
			Vector2 average = Vector2.zero;
			for (int i = 0; i < frames; i++) { average += fixedPositions[i] - fixedPositions[i + 1]; }
			return average / frames;
		}
		/// <summary>
		/// Returns the average movement of the mouse (in worldspace) over the past <paramref name="frames"/> frames.
		/// </summary>
		public static Vector2 AverageFixedWorldVelocity(int frames)
		{
			if (frames < 1) { throw new ArgumentOutOfRangeException(nameof(frames)); }
			Vector2 average = Vector2.zero;
			for (int i = 0; i < frames; i++) { average += fixedWorldPositions[i] - fixedWorldPositions[i + 1]; }
			return average / frames;
		}

		/// <summary>
		/// Returns the smoothed movement of the mouse (in mousePosition space) over the past <paramref name="frames"/> frames.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="frames"/> must be &gt;= 1 and &lt;= 1000</exception>
		public static Vector2 SmoothedMovement(int frames)
		{
			if (frames < 1 || frames > 1000) { throw new ArgumentOutOfRangeException(nameof(frames)); }
			if (positions[0] != Position) { return positions[Mathf.Max(0, frames - 2)].To(Position) * frames; }
			else { return positions[frames - 1].To(positions[0]) * frames; }
		}
		/// <summary>
		/// Returns the smoothed movement of the mouse (in worldspace) over the past <paramref name="frames"/> frames.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="frames"/> must be &gt;= 1 and &lt;= 1000</exception>
		public static Vector2 SmoothedVelocity(int frames)
		{
			if (frames < 1 || frames > 1000) { throw new ArgumentOutOfRangeException(nameof(frames)); }
			if (worldPositions[0] != WorldPosition) { return worldPositions[Mathf.Max(0, frames - 2)].To(WorldPosition) * frames; }
			else { return worldPositions[frames - 1].To(worldPositions[0]) * frames; }
		}

		/// <summary>
		/// Returns the smoothed movement of the mouse (in mousePosition space) over the past <paramref name="frames"/> FixedUpdate frames.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="frames"/> must be &gt;= 1 and &lt;= 1000</exception>
		public static Vector2 SmoothedFixedMovement(int frames)
		{
			if (frames < 1 || frames > 1000) { throw new ArgumentOutOfRangeException(nameof(frames)); }
			return fixedPositions[frames - 1].To(fixedPositions[0]) * frames;
		}
		/// <summary>
		/// Returns the smoothed movement of the mouse (in worldspace) over the past <paramref name="frames"/> FixedUpdate frames.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="frames"/> must be &gt;= 1 and &lt;= 1000</exception>
		public static Vector2 SmoothedFixedVelocity(int frames)
		{
			if (frames < 1 || frames > 1000) { throw new ArgumentOutOfRangeException(nameof(frames)); }
			return fixedWorldPositions[frames - 1].To(fixedWorldPositions[0]) * frames;
		}

		#endregion

		#region Windows OS Hooks

		/// <summary>
		/// Windows OS method to set the cursor position to the specified screen coordinates.<br/>
		/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setcursorpos">
		/// Learn.microsoft.com - SetCursorPos function (winuser.h)</a>
		/// </summary>
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool SetCursorPos(int X, int Y);
		/// <summary>
		/// Windows OS method to set the cursor position to the specified screen coordinates.<br/>
		/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setcursorpos">
		/// Learn.microsoft.com - SetCursorPos function (winuser.h)</a>
		/// </summary>
		public static bool SetCursorPos(Vector2Int screenCoordinates) {
			return SetCursorPos(screenCoordinates.x, screenCoordinates.y);
		}

		/// <summary>
		/// Windows OS method to retrieve the position of the mouse cursor, in screen coordinates.<br/>
		/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getcursorpos">
		/// Learn.microsoft.com - GetCursorPos function (winuser.h)</a>
		/// </summary>
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool GetCursorPos(out int X, out int Y);
		/// <summary>
		/// Windows OS method to retrieve the position of the mouse cursor, in screen coordinates - returns the result as a Vector2Int.<br/>
		/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getcursorpos">
		/// Learn.microsoft.com - GetCursorPos function (winuser.h)</a>
		/// </summary>
		public static Vector2Int GetCursorPosition()
		{
			GetCursorPos(out getCursorPosX, out getCursorPosY);
			return new Vector2Int(getCursorPosX, getCursorPosY);
		}
		private static int getCursorPosX, getCursorPosY;

		#endregion

	}
}
