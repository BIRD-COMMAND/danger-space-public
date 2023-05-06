using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The HealthBar class is responsible for updating the player's health bar UI.
/// </summary>
public class HealthBar : MonoBehaviour
{

	/// <summary>
	/// The squares that make up the health bar.
	/// </summary>
    public Image[] squares = new Image[10];

    /// <summary>
	/// Update individual health bar squares to reflect player health.
	/// </summary>
    void Update()
    {
		if (GameManager.Player) {
			float i = 0.1f;
			for (int j = 0; j < 10; j++, i += 0.1f) {
				squares[j].enabled = i <= GameManager.Player.HealthPercent || Mathf.Approximately(i, GameManager.Player.HealthPercent);
			}
		}
		else { for (int i = 0; i < 10; i++) { squares[i].enabled = false; } }
	}

}
