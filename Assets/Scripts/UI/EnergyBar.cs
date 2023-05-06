using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The EnergyBar class is responsible for updating the player's energy bar UI.
/// </summary>
public class EnergyBar : MonoBehaviour
{

    /// <summary>
    /// Reference to the energy bar rect transform.
    /// </summary>
    public RectTransform energyBar;

	/// <summary>
	/// Set the size of the energy bar based on the player's energy percent
	/// </summary>
	void Update()
    {
		energyBar.SetSizeWithCurrentAnchors(
			RectTransform.Axis.Horizontal,
            Mathf.Lerp(26f, 256f, GameManager.Player ? GameManager.Player.EnergyPercent : 0f)
		);
    }

}
