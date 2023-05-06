using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SliderLabel updates a Text component with the value of a Slider existing on the parent GameObject.
/// </summary>
public class SliderLabel : MonoBehaviour
{

	private Slider slider; private Text text;
	private void Awake() { slider = GetComponentInParent<Slider>(); text = GetComponent<Text>(); }
	private void Update() { text.text = slider.value.ToString(); }

}
