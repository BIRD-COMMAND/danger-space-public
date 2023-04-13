using Shapes;
using System.Collections;
using System.Collections.Generic;
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
		if (module.projectileCurrentAmmo < indicators.Length) {
			for (int i = max; i >= 0; i--) {
				if (module.projectileCurrentAmmo > i) {
					indicators[i].Color = activeColor;
					indicators[i].transform.GetChild(0).gameObject.SetActive(true);
				}
				else {
					indicators[i].Color = inactiveColor;
					indicators[i].transform.GetChild(0).gameObject.SetActive(false);
				}
			}
		}
		else if (indicators[max].Color != activeColor) {
			indicators[max].Color = activeColor;
			indicators[max].transform.GetChild(0).gameObject.SetActive(true); 
		}
	}

	void Start() { module = GetComponent<Module>(); max = indicators.Length - 1; }
	void OnValidate() { Start(); }

}
