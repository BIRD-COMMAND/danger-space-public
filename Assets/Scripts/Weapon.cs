using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class Weapon : MonoBehaviour
{

	/// <summary>
	/// Static reference to the current projectile being configured for firing.
	/// </summary>
	private static Projectile shot;
	/// <summary>
	/// Static Collider2D list used to check for projectile overlap when firing.
	/// </summary>
	private static List<Collider2D> overlaps = new List<Collider2D>();
	/// <summary>
	/// Static Projectile list used to return a list of projectiles fired by a weapon.
	/// </summary>
	private static List<Projectile> shots = new List<Projectile>();
	
	/// <summary>
	/// The type of projectile emanation used by the weapon.
	/// </summary>
	public Emanation emanation;
	/// <summary>
	/// Prefab for the projectile fired by the weapon.
	/// </summary>
	public GameObject projectilePrefab;
	/// <summary>
	/// Prefab for the impact effect spawned when a projectile hits something.
	/// </summary>
	public GameObject impactEffectPrefab;
	/// <summary>
	/// Transform used as the spawn point for projectiles fired by the weapon.
	/// </summary>
	public Transform projectileSpawn;
	/// <summary>
	/// The number of projectiles fired per shot.
	/// </summary>
	public int projectilesPerShot = 1;
	/// <summary>
	/// The radius around the projectile spawn point where projectiles can be spawned.
	/// </summary>
	public float projectileSpawnRadius = 0f;
	/// <summary>
	/// The speed at which projectiles fired by the weapon travel.
	/// </summary>
	public float projectileSpeed = 20f;
	/// <summary>
	/// The lifetime of projectiles fired by the weapon.
	/// </summary>
	public float projectileLifetime = 10f;
	/// <summary>
	/// The damage dealt by projectiles fired by the weapon.
	/// </summary>
	public float projectileDamage = 2f;
	/// <summary>
	/// The angle of spread for projectiles fired by the weapon.
	/// </summary>
	public float projectileSpread = 1f;
	/// <summary>
	/// The rate of fire for projectiles fired by the weapon.
	/// </summary>
	public float projectileRateOfFire = 0.2f;
	/// <summary>
	/// The cooldown timer for firing projectiles.
	/// </summary>
	public float projectileShotCooldown = 0f;
	/// <summary>
	/// The maximum amount of ammo the weapon can hold.
	/// </summary>
	public float projectileAmmo = 100f;
	/// <summary>
	/// The current amount of ammo the weapon has.
	/// </summary>
	public float projectileAmmoCurrent = 100f;
	/// <summary>
	/// The amount of ammo the weapon uses per shot.
	/// </summary>
	public float projectileAmmoPerShot = 1f;
	/// <summary>
	/// The reload time for the weapon.
	/// </summary>
	public float projectileReloadTime = 3f;
	/// <summary>
	/// The reload timer for the weapon.
	/// </summary>
	public float projectileReloadTimeCurrent = 0f;
	/// <summary>
	/// The amount of ammo remaining in the weapon as a normalized percentage [0.0f - 1.0f] of the maximum ammo.
	/// </summary>
	public float ProjectileAmmoCurrentPercent => projectileAmmoCurrent / projectileAmmo;

	/// <summary>
	/// Validate the weapon configuration and destroy the weapon component if it's invalid.
	/// </summary>
	private void Start()
	{
		// Destroy invalid weapon on spawn
		if (!projectilePrefab || !projectileSpawn) { 
			Debug.LogError("Destroying Invalid Weapon: Missing projectile or projectileSpawn", gameObject); 
			Destroy(this); 
		}
	}

	/// <summary>
	/// Update the projectile shot cooldown and reload time.
	/// </summary>
	private void Update()
	{
		if (GetComponent<BulletTimeScaler>()) {
			projectileShotCooldown = Mathf.Max(projectileShotCooldown - Time.unscaledDeltaTime, 0f);
			projectileReloadTimeCurrent = Mathf.Max(projectileReloadTimeCurrent - Time.unscaledDeltaTime, 0f);
		}
		else {
			projectileShotCooldown = Mathf.Max(projectileShotCooldown - Time.deltaTime, 0f);
			projectileReloadTimeCurrent = Mathf.Max(projectileReloadTimeCurrent - Time.deltaTime, 0f);
		}
		if (projectileAmmoCurrent == 0f && projectileReloadTimeCurrent == 0f) { projectileAmmoCurrent = projectileAmmo; }
	}

	/// <summary>
	/// Fires projectile(s) based on the weapon's configuration and returns a list of each individual projectile fired.<br/>
	/// Note that the returned List is static and reused by all weapons, so copy the list if you need it to persist between frames.
	/// </summary>
	public List<Projectile> Fire(Agent shooter)
	{
		shots.Clear();
		if (projectileShotCooldown == 0f && projectileAmmoCurrent > 0f) {
			projectileShotCooldown = projectileRateOfFire;
			projectileAmmoCurrent -= projectileAmmoPerShot;
			Vector3 spawnPoint = projectileSpawn.position; 
			Quaternion spawnRotation = projectileSpawn.rotation;
			for (int i = 0; i < projectilesPerShot; i++) {
				shot = PoolManager.Get(projectilePrefab) as Projectile;
				shot.shooter = shooter;
				shot.weapon = this;
				if (projectileSpawnRadius > 0f) {
					bool overlap = true;
					for (float j = 1f; j < 2.5f; j += 0.05f) {
						switch (emanation) {
							case Emanation.Point:
								spawnPoint = (Vector2)projectileSpawn.position + (UnityEngine.Random.insideUnitCircle * projectileSpawnRadius * j);
								spawnRotation = projectileSpawn.rotation;
								break;
							case Emanation.Random:
								spawnPoint = (Vector2)transform.position + (UnityEngine.Random.insideUnitCircle.normalized * Vector2.Distance(transform.position, projectileSpawn.position) * j);
								spawnRotation = projectileSpawn.rotation;
								shot.transform.LookUp(spawnPoint + (spawnPoint - transform.position));
								break;
							case Emanation.Arc:
								//TODO implement   arc emanation projectile spawning
								break;
							case Emanation.Pulse:
								//TODO implement pulse emanation projectile spawning
								break;
							default: break;
						}
						if (shot.collider.Overlap(spawnPoint, spawnRotation.Angle2D(), overlaps) == 0) { overlap = false; break; }
					}
					if (overlap) { shot.Return(); }
					else {
						if (projectileSpread > 0f) { shot.Activate(spawnPoint, spawnRotation * Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-projectileSpread, projectileSpread))); }
						else { shot.Activate(spawnPoint, spawnRotation); }						 
					}
				}
				else {
					if (projectileSpread > 0f) { shot.Activate(spawnPoint, spawnRotation * Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-projectileSpread, projectileSpread))); }
					else { shot.Activate(spawnPoint, spawnRotation); }
				}
				shot.body.velocity = shot.transform.up * projectileSpeed;
				if (shooter.inBulletTime) { shot.body.velocity /= Time.timeScale; }
				shot.timer = projectileLifetime;
				shots.Add(shot);
			}
		}
		return shots;
	}

	/// <summary>
	/// When selected draws a ring representing the projectileSpawnRadius if it's greater than 0.
	/// </summary>
	private void OnDrawGizmosSelected()
	{
		if (projectilesPerShot > 1 && projectileSpawnRadius > 0f) {
			Shapes.Draw.UseDashes = true; Shapes.Draw.DashStyle = Shapes.DashStyle.defaultDashStyle;
			Shapes.Draw.DashSizeUniform *= 8f; Shapes.Draw.Thickness = 0.04f;
			Shapes.Draw.Ring(projectileSpawn.position, projectileSpawnRadius);
		}
	}

	/// <summary>
	/// Enum for the different types of projectile emanation.
	/// </summary>
	public enum Emanation
	{
		Point,
		Random,
		Arc,
		Pulse
	}

}
