using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Module : MonoBehaviour
{

    public Module core;
	public Module parent;
	public bool IsActive => gameObject.activeSelf && parent != null;
	public bool ActiveInParent => parent && parent.modules.Contains(this);

    [HideInInspector]
    public List<Module> modules = new List<Module>();

    public Vector2 centroid = Vector2.zero;
	[HideInInspector]
	public Rect centroidSpriteTextureRext = new Rect();

    public Type type;
    public Placement slot;
    public Placement slots;
	public Placement slotsCurrent;

    public bool effectCenterOfMass = false;
	public bool centerCentroidHorizontally = false;
	public bool centerCentroidVertically = false;

	public float mass = 1000f;
    public float massCurrent = 1000f;

    public int health = 10;
    public int healthCurrent = 10;
    
    public float thrust = 1000f;

    public float speed = 10f;
    public float speedCurrent = 0f;

    // Weapon related variables
    public GameObject projectile;
    public float projectileSpeed = 0f;
    public float projectileLifetime = 0f;
    public float projectileSize = 0f;
    public float projectileDamage = 0f;
    public float projectileRange = 0f;
    public float projectileRateOfFire = 0f;
    public float projectileRateOfFireCurrent = 0f;
    public float projectileMaxAmmo = 0f;
    public float projectileCurrentAmmo = 0f;
    public float projectileAmmoPerShot = 0f;
    public float projectileReloadTime = 0f;
    public float projectileReloadTimeCurrent = 0f;

	// Shield related variables
	public int shield = 0;
	public int shieldCurrent = 0;
    public float shieldRechargeRate = 0f;
	public float shieldRechargeRateCurrent = 0f;
	public float shieldRechargeDelay = 0f;
	public float shieldRechargeDelayCurrent = 0f;
	
    // Armor related variables
	public int armor = 0;
	public int armorCurrent = 0;
	public float armorRegenRate = 0f;
	public float armorRegenRateCurrent = 0f;
	public float armorRegenDelay = 0f;
	public float armorRegenDelayCurrent = 0f;
	public float armorRegenAmount = 0f;
	public float armorRegenAmountCurrent = 0f;
	public float armorRegenInterval = 0f;
	public float armorRegenIntervalCurrent = 0f;

    // Power related variables
    public float power = 0f;
    public float powerCurrent = 0f;
    public float powerConsumption = 0f;
    public float powerConsumptionCurrent = 0f;
    public float powerRechargeRate = 0f;
    public float powerRechargeRateCurrent = 0f;
    public float powerRechargeDelay = 0f;
    public float powerRechargeDelayCurrent = 0f;


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public float SumMass(Module m, float sum)
	{
		for (int i = 0; i < m.transform.childCount; i++) {
			if (m.transform.GetChild(i).TryGetComponent(out Module mod) && mod.core == this && mod.parent != null && mod.gameObject.activeSelf) {
				sum += mod.mass; sum = m.SumMass(mod, sum);
			}
		}
		return sum;
	}

	public void AddModules(bool addInactive = false)
	{
		modules.Clear();
		slotsCurrent = slots;
		for (int i = 0; i < transform.childCount; i++) {
			if (transform.GetChild(i).TryGetComponent(out Module m)) {
				if (!m.gameObject.activeSelf) {
					if (addInactive && slotsCurrent.HasFlag(m.slot)) { m.gameObject.SetActive(true); }
					else { continue; }
				}
				if (slotsCurrent.HasFlag(m.slot)) {
					modules.Add(m); m.parent = this; m.core = core; // update modules
					slotsCurrent &= ~m.slot; // remove slot flag from slots as the slot is now occupied
					CalculateCentroid();
                    m.AddModules(addInactive);
				}
				else { m.gameObject.SetActive(false); }
			}
		}
	}

	public void RemoveModules(bool deactivateGameObjects)
	{
		foreach (Module m in modules) {
			if (deactivateGameObjects) { m.gameObject.SetActive(false); }
			slots |= m.slot; m.RemoveModules(deactivateGameObjects);
		}
		modules.Clear();
	}

	[Flags]
    public enum Type
    {

        None = 0,
        Core = 1,
        Extension = 2,
        Wing = 4,
        Weapon = 8,
        Thruster = 16,
        Shield = 32,
        Armor = 64

    }

    [Flags]
    public enum Placement
    {

        None = 0,
        Front = 1,
        Back = 2,
        Left = 4,
        Right = 8,
        FrontLeft = 16,
        FrontRight = 32,
        BackLeft = 64,
        BackRight = 128

    }


	[ContextMenu("Calculate Centroid")]
	protected void CalculateCentroid()
	{

		SpriteRenderer sr = GetComponent<SpriteRenderer>();

		if (!sr || !effectCenterOfMass) { centroid = Vector2.zero; return; }

		int minX = int.MaxValue;
		int maxX = -1;
		int minY = int.MaxValue;
		int maxY = -1;

		Rect rect = sr.sprite.rect;
		Color[] pixels = sr.sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

		for (int y = 0; y < rect.height; y++) {
			for (int x = 0; x < rect.width; x++) {
				Color pixel = pixels[y * (int)rect.width + x];
				if (pixel.a > 0) // Non-transparent pixel
				{
					minX = Mathf.Min(minX, x);
					maxX = Mathf.Max(maxX, x);
					minY = Mathf.Min(minY, y);
					maxY = Mathf.Max(maxY, y);
				}
			}
		}


		Vector2 squarePos;
		int height = maxY - minY;
		int width = maxX - minX;
		int size = height > width ? height : width;
		int diff = Mathf.Abs(height - width);
		if (height > width) { squarePos = new Vector2(minX - (diff / 2), minY); }
		else { squarePos = new Vector2(minX, minY - (diff / 2)); }
		Vector2 pixelCoordinate = squarePos + new Vector2(size / 2f, size / 2f);

		centroidSpriteTextureRext = new Rect(squarePos.x, squarePos.y, size, size);

		// Normalize pixel coordinate to be in range [0, 1]
		Vector2 normalizedCoordinate = new Vector2(
			(pixelCoordinate.x - rect.x) / rect.width,
			(pixelCoordinate.y - rect.y) / rect.height
		);

		// Convert normalized coordinate to local space //Vector2 localSpaceCoordinate =
		centroid = Vector2.Scale((normalizedCoordinate - sr.sprite.pivot / rect.size), sr.sprite.bounds.size);

		//// Convert local space coordinate to world space
		//Vector2 worldSpaceCoordinate = sr.transform.TransformPoint(localSpaceCoordinate);

		//GameObject centroid = new GameObject("Centroid");
		//centroid.transform.parent = transform;
		//centroid.transform.position = worldSpaceCoordinate;

		if (centerCentroidHorizontally) { CenterCentroidHorizontally(); }
		if (centerCentroidVertically) { CenterCentroidVertically(); }

	}
	[ContextMenu("Center Centroid Horizontally")]
	private void CenterCentroidHorizontally() { centroid = new Vector2(0f, centroid.y); }
	[ContextMenu("Center Centroid Vertically")]
	private void CenterCentroidVertically() { centroid = new Vector2(centroid.x, 0f); }


}
