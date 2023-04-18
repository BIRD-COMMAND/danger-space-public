using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{

	// Weapon related variables
	public GameObject projectile;
	public Transform projectileSpawn;
	public float projectileSpeed = 20f;
	public float projectileLifetime = 10f;
	public float projectileSize = 0f;
	public float projectileDamage = 2f;
	public float projectileRange = 0f;
	public float projectileRateOfFire = 0.2f;
	public float projectileRateOfFireCurrent = 0f;
	public float projectileAmmo = 100f;
	public float projectileAmmoCurrent = 100f;
	public float projectileAmmoPerShot = 1f;
	public float projectileReloadTime = 3f;
	public float projectileReloadTimeCurrent = 0f;
	public float ProjectileAmmoCurrentPercent => projectileAmmoCurrent / projectileAmmo;

	private void Start()
	{
		// Destroy invalid weapon on spawn
		if (!projectile || !projectileSpawn) { 
			Debug.LogError("Destroying Invalid Weapon: Missing projectile or projectileSpawn", gameObject); 
			Destroy(this); 
		}
	}

	private void Update()
	{
		projectileRateOfFireCurrent = Mathf.Max(projectileRateOfFireCurrent - Time.deltaTime, 0f);
		projectileReloadTimeCurrent = Mathf.Max(projectileReloadTimeCurrent - Time.deltaTime, 0f);
		if (projectileAmmoCurrent == 0f && projectileReloadTimeCurrent == 0f) { projectileAmmoCurrent = projectileAmmo; }
	}

	public void Fire()
	{
		if (projectileRateOfFireCurrent == 0f && projectileAmmoCurrent > 0f) {
			projectileRateOfFireCurrent = projectileRateOfFire;
			projectileAmmoCurrent -= projectileAmmoPerShot;
			Projectiles.Get(projectile).Launch(this);
		}
	}

}
