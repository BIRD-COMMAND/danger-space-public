using UnityEngine;
using UnityEngine.InputSystem;

namespace Extensions
{

	/// <summary>
	/// Wrapper for accessing values on the current Gamepad (Gamepad.current)
	/// <br>All properties will return false, 0f, or a Vector.zero value if Gamepad.current is null</br>
	/// </summary>
	public static class Pad
	{

		/// <summary>
		/// If a value has been set, it will be returned. Otherwise, Gamepad.current will be returned.<br/>
		/// Make sure you reset this value to null when you are done with it so that future calls can access Gamepad.current (the active Gamepad).
		/// </summary>
		public static Gamepad Current { get => current ?? Gamepad.current; set => current = value; }
		private static Gamepad current = null;

		/// <summary>
		/// Returns true if Gamepad.current is not null (refers to a valid device)
		/// </summary>
		public static bool IsAvailable => Current != null;

		#region Sticks, Triggers, and Axes

		/// <summary>
		/// The current Vector2 ReadValue from the leftStick of the current Gamepad.
		/// </summary>
		public static Vector2 LStick => Current?.leftStick.ReadValue() ?? Vector2.zero;
		/// <summary>
		/// The current Vector2 ReadValue from the rightStick of the current Gamepad.
		/// </summary>
		public static Vector2 RStick => Current?.rightStick.ReadValue() ?? Vector2.zero;

		/// <summary>
		/// The current ReadValue from the leftTrigger of the current Gamepad
		/// </summary>
		public static float LTrigger => Current?.leftTrigger.ReadValue() ?? 0f;
		/// <summary>
		/// The current ReadValue from the rightTrigger of the current Gamepad
		/// </summary>
		public static float RTrigger => Current?.rightTrigger.ReadValue() ?? 0f;

		/// <summary> Returns the current Vector2 value of the current gamepad's D-pad.<br/>
		/// X-component: (Left) -1f (Neutral) 0f (Right) 1f.<br/>
		/// Y-component: (Down) -1f (Neutral) 0f (Up) 1f. </summary>
		public static Vector2 DPadAxes => Current?.dpad.value ?? Vector2.zero;
		/// <summary> Returns the current value of the current gamepad's D-pad horizontal axis.<br/>
		/// (Left) -1f (Neutral) 0f (Right) 1f. </summary>
		public static float DPadAxisX => Current?.dpad.x.value ?? 0f;
		/// <summary> Returns the current value of the current gamepad's D-pad vertical axis.<br/>
		/// (Down) -1f (Neutral) 0f (Up) 1f. </summary>
		public static float DPadAxisY => Current?.dpad.y.value ?? 0f;

		#endregion

		#region Pressed: Gamepad.current.button.wasPressedThisFrame

		/// <summary>
		/// Returns true if the south button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool ButtonSouthPressed => Current?.buttonSouth.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the east button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool ButtonEastPressed => Current?.buttonEast.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the west button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool ButtonWestPressed => Current?.buttonWest.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the north button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool ButtonNorthPressed => Current?.buttonNorth.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the cross button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool CrossButtonPressed => Current?.crossButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the circle button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool CircleButtonPressed => Current?.circleButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the square button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool SquareButtonPressed => Current?.squareButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the triangle button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool TriangleButtonPressed => Current?.triangleButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the a button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool AButtonPressed => Current?.aButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the b button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool BButtonPressed => Current?.bButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the x button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool XButtonPressed => Current?.xButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the y button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool YButtonPressed => Current?.yButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the start button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool StartButtonPressed => Current?.startButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the select button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool SelectButtonPressed => Current?.selectButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the left shoulder button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool LShoulderButtonPressed => Current?.leftShoulder.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the right shoulder button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool RShoulderButtonPressed => Current?.rightShoulder.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the left stick button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool LStickButtonPressed => Current?.leftStickButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the right stick button was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool RStickButtonPressed => Current?.rightStickButton.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the left trigger was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool LTriggerPressed => Current?.leftTrigger.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the right trigger was pressed this frame on the current Gamepad.
		/// </summary>
		public static bool RTriggerPressed => Current?.rightTrigger.wasPressedThisFrame ?? false;

		/// <summary>
		/// Returns true if the D-pad Up button was pressed this frame on the current gamepad.
		/// </summary>
		public static bool DPadUpPressed => Current?.dpad.up.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the D-pad Down button was pressed this frame on the current gamepad.
		/// </summary>
		public static bool DPadDownPressed => Current?.dpad.down.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the D-pad Left button was pressed this frame on the current gamepad.
		/// </summary>
		public static bool DPadLeftPressed => Current?.dpad.left.wasPressedThisFrame ?? false;
		/// <summary>
		/// Returns true if the D-pad Right button was pressed this frame on the current gamepad.
		/// </summary>
		public static bool DPadRightPressed => Current?.dpad.right.wasPressedThisFrame ?? false;

		#endregion

		#region Down: Gamepad.current.button.isPressed

		/// <summary>
		/// Returns true if the south button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool ButtonSouthDown => Current?.buttonSouth.isPressed ?? false;
		/// <summary>
		/// Returns true if the east button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool ButtonEastDown => Current?.buttonEast.isPressed ?? false;
		/// <summary>
		/// Returns true if the west button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool ButtonWestDown => Current?.buttonWest.isPressed ?? false;
		/// <summary>
		/// Returns true if the north button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool ButtonNorthDown => Current?.buttonNorth.isPressed ?? false;
		/// <summary>
		/// Returns true if the cross button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool CrossButtonDown => Current?.crossButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the circle button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool CircleButtonDown => Current?.circleButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the square button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool SquareButtonDown => Current?.squareButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the triangle button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool TriangleButtonDown => Current?.triangleButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the a button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool AButtonDown => Current?.aButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the b button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool BButtonDown => Current?.bButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the x button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool XButtonDown => Current?.xButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the y button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool YButtonDown => Current?.yButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the start button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool StartButtonDown => Current?.startButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the select button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool SelectButtonDown => Current?.selectButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the left shoulder button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool LShoulderButtonDown => Current?.leftShoulder.isPressed ?? false;
		/// <summary>
		/// Returns true if the right shoulder button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool RShoulderButtonDown => Current?.rightShoulder.isPressed ?? false;
		/// <summary>
		/// Returns true if the left stick button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool LStickButtonDown => Current?.leftStickButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the right stick button is currently pressed on the current Gamepad.
		/// </summary>
		public static bool RStickButtonDown => Current?.rightStickButton.isPressed ?? false;
		/// <summary>
		/// Returns true if the left trigger is currently pressed on the current gamepad.
		/// </summary>
		public static bool LTriggerDown => Current?.leftTrigger.isPressed ?? false;
		/// <summary>
		/// Returns true if the right trigger is currently pressed on the current gamepad.
		/// </summary>
		public static bool RTriggerDown => Current?.rightTrigger.isPressed ?? false;

		/// <summary>
		/// Returns true if the D-pad Up button is currently pressed on the current gamepad.
		/// </summary>
		public static bool DPadUp => Current?.dpad.up.isPressed ?? false;
		/// <summary>
		/// Returns true if the D-pad Down button is currently pressed on the current gamepad.
		/// </summary>
		public static bool DPadDown => Current?.dpad.down.isPressed ?? false;
		/// <summary>
		/// Returns true if the D-pad Left button is currently pressed on the current gamepad.
		/// </summary>
		public static bool DPadLeft => Current?.dpad.left.isPressed ?? false;
		/// <summary>
		/// Returns true if the D-pad Right button is currently pressed on the current gamepad.
		/// </summary>
		public static bool DPadRight => Current?.dpad.right.isPressed ?? false;

		#endregion

		#region Released: Gamepad.current.button.wasReleasedThisFrame

		/// <summary>
		/// Returns true if the south button was released this frame on the current Gamepad.
		/// </summary>
		public static bool ButtonSouthReleased => Current?.buttonSouth.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the east button was released this frame on the current Gamepad.
		/// </summary>
		public static bool ButtonEastReleased => Current?.buttonEast.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the west button was released this frame on the current Gamepad.
		/// </summary>
		public static bool ButtonWestReleased => Current?.buttonWest.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the north button was released this frame on the current Gamepad.
		/// </summary>
		public static bool ButtonNorthReleased => Current?.buttonNorth.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the cross button was released this frame on the current Gamepad.
		/// </summary>
		public static bool CrossButtonReleased => Current?.crossButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the circle button was released this frame on the current Gamepad.
		/// </summary>
		public static bool CircleButtonReleased => Current?.circleButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the square button was released this frame on the current Gamepad.
		/// </summary>
		public static bool SquareButtonReleased => Current?.squareButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the triangle button was released this frame on the current Gamepad.
		/// </summary>
		public static bool TriangleButtonReleased => Current?.triangleButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the a button was released this frame on the current Gamepad.
		/// </summary>
		public static bool AButtonReleased => Current?.aButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the b button was released this frame on the current Gamepad.
		/// </summary>
		public static bool BButtonReleased => Current?.bButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the x button was released this frame on the current Gamepad.
		/// </summary>
		public static bool XButtonReleased => Current?.xButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the y button was released this frame on the current Gamepad.
		/// </summary>
		public static bool YButtonReleased => Current?.yButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the start button was released this frame on the current Gamepad.
		/// </summary>
		public static bool StartButtonReleased => Current?.startButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the select button was released this frame on the current Gamepad.
		/// </summary>
		public static bool SelectButtonReleased => Current?.selectButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the left shoulder button was released this frame on the current Gamepad.
		/// </summary>
		public static bool LShoulderButtonReleased => Current?.leftShoulder.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the right shoulder button was released this frame on the current Gamepad.
		/// </summary>
		public static bool RShoulderButtonReleased => Current?.rightShoulder.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the left stick button was released this frame on the current Gamepad.
		/// </summary>
		public static bool LStickButtonReleased => Current?.leftStickButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the right stick button was released this frame on the current Gamepad.
		/// </summary>
		public static bool RStickButtonReleased => Current?.rightStickButton.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the left trigger was released this frame on the current Gamepad.
		/// </summary>
		public static bool LTriggerReleased => Current?.leftTrigger.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the right trigger was released this frame on the current Gamepad.
		/// </summary>
		public static bool RTriggerReleased => Current?.rightTrigger.wasReleasedThisFrame ?? false;

		/// <summary>
		/// Returns true if the D-pad Up button was released this frame on the current gamepad.
		/// </summary>
		public static bool DPadUpReleased => Current?.dpad.up.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the D-pad Down button was released this frame on the current gamepad.
		/// </summary>
		public static bool DPadDownReleased => Current?.dpad.down.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the D-pad Left button was released this frame on the current gamepad.
		/// </summary>
		public static bool DPadLeftReleased => Current?.dpad.left.wasReleasedThisFrame ?? false;
		/// <summary>
		/// Returns true if the D-pad Right button was released this frame on the current gamepad.
		/// </summary>
		public static bool DPadRightReleased => Current?.dpad.right.wasReleasedThisFrame ?? false;

		#endregion

	}

}