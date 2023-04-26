using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class Weapon : MonoBehaviour
{

	public enum Emanation
	{
		Point,
		Random,
		Arc,
		Pulse
	}

	private static Projectile shot;
	private static List<Collider2D> overlaps = new List<Collider2D>();
	private static List<Projectile> shots = new List<Projectile>();

	// Weapon related variables
	public Emanation emanation;
	public GameObject projectile;
	public GameObject impactEffect;
	public ParticleSystem[] fireEffects;
	public Transform projectileSpawn;
	public float projectileSpawnRadius = 0f;
	public int projectilesPerShot = 1;
	public float projectileSpeed = 20f;
	public float projectileLifetime = 10f;
	public float projectileSize = 0f;
	public float projectileDamage = 2f;
	public float projectileRange = 0f;
	public float projectileSpread = 1f;
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

	public void Fire(Agent shooter)
	{
		if (projectileRateOfFireCurrent == 0f && projectileAmmoCurrent > 0f) {
			projectileRateOfFireCurrent = projectileRateOfFire;
			projectileAmmoCurrent -= projectileAmmoPerShot;
			shots.Clear();
			Vector3 spawnPoint = projectileSpawn.position; 
			Quaternion spawnRotation = projectileSpawn.rotation;
			for (int i = 0; i < projectilesPerShot; i++) {
				shot = PoolManager.Get(projectile) as Projectile;
				shot.shooter = shooter;
				shot.weapon = this;
				if (projectileSpawnRadius > 0f) {
					bool overlap = true;
					for (float j = 1f; j < 2.5f; j += 0.05f) {
						switch (emanation) {
							case Emanation.Point:
								spawnPoint = (Vector2)projectileSpawn.position + (UnityEngine.Random.insideUnitCircle * projectileSpawnRadius * j);
								spawnRotation = projectileSpawn.rotation;
								//Debug.DrawLine(transform.position, shot.transform.position, Color.red, 0.5f);
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
						//else { foreach (Collider2D item in overlaps) { Debug.Log(item.name); } }
					}
					if (overlap) { /*Debug.Log("overlapped");*/ shot.Return(); Debug.Log("Projectile Return()ed by Weapon.overlap"); }
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
				shot.timer = projectileLifetime;
				shots.Add(shot);
			}
			//foreach (ParticleSystem item in fireEffects) { if (item != null && !item.isPlaying) { item.Play(); break; } }
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (projectilesPerShot > 1 && projectileSpawnRadius > 0f) {
			Shapes.Draw.UseDashes = true; Shapes.Draw.DashStyle = Shapes.DashStyle.defaultDashStyle;
			Shapes.Draw.DashSizeUniform *= 8f; Shapes.Draw.Thickness = 0.04f;
			Shapes.Draw.Ring(projectileSpawn.position, projectileSpawnRadius);
		}
	}

}
