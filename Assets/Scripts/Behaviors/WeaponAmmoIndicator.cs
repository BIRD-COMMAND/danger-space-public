using Shapes;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Module))]
public class WeaponAmmoIndicator : MonoBehaviour
{

	public Color activeColor = Color.green;
	public Color inactiveColor = Color.gray;
    public ShapeRenderer[] indicators;

	private Module module;
	private int max = 0;

	private void Update()
	{
		for (int i = max; i > -1; i--) {
			if (module.projectileAmmoCurrent > i) {
				if (indicators[i].Color != activeColor) {
					indicators[i].Color = activeColor;
					indicators[i].transform.GetChild(0).gameObject.SetActive(true);
				}
			}
			else if (indicators[i].Color != inactiveColor) {
				indicators[i].Color = inactiveColor;
				indicators[i].transform.GetChild(0).gameObject.SetActive(false);
			}
		}
	}

	void Start() { module = GetComponent<Module>(); max = indicators.Length - 1; }
	void OnValidate() { Start(); }

}