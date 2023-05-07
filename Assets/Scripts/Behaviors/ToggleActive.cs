using UnityEngine;

/// <summary>
/// Toggles the active state of the game object.
/// </summary>
public class ToggleActive : MonoBehaviour
{
    
	/// <summary>
	/// Toggles the active state of the game object.
	/// </summary>
    public void Toggle() {
		gameObject.SetActive(!gameObject.activeSelf);
	}

}
