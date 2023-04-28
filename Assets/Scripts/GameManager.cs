using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static PlayerController Player;
    private static GameManager instance;

	public static readonly LayerMask RaycastLayersExcludingProjectiles;
    static GameManager() {
	    RaycastLayersExcludingProjectiles = Physics2D.DefaultRaycastLayers & ~(1 << 8);
    }

    [Header("Player")]
	[SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector2 spawnPoint;
    public bool debugKeepPlayerInvulnerable = false;
    [SerializeField] private int playerLives = 3;
    [SerializeField] private float playerRespawnTime = 2f;
    private float playerRespawnTimer = 2f;

    [Header("Score")]
	[SerializeField] private Text scoreText;
    [SerializeField] private float score = 0f;
    public static float Score => instance.score;
    public static void AddScore(float value) { instance.score += value; }

	private void Awake() { instance = this; SpawnPlayer(); }

	// Update is called once per frame
	void Update()
    {
        // update UI score text
        scoreText.text = ((int)score).ToString();

        // handle respawn
        if (!Player) {
            if (playerLives == 0) { return; }
            playerRespawnTimer -= Time.deltaTime;
            if (playerRespawnTimer <= 0f) {
                playerRespawnTimer = playerRespawnTime;
				playerLives--; 
                SpawnPlayer();
			}
		}
    }

    private void SpawnPlayer()
    {
        // instantiate player prefab and flash green for 1 second
        Player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity).GetComponent<PlayerController>();
        Player.FlashColor(Color.green, 1f);
		// give player invulnerability for 1 second
		Player.invulnerable = true; 
        Runtime.DoInSeconds(() => { Player.invulnerable = debugKeepPlayerInvulnerable; }, 1f);
	}

	private void OnDrawGizmosSelected() { 
        // draw spawn point indicator ring
        Draw.Thickness = 0.3f; Draw.Ring(spawnPoint, 3f, Color.green); 
    }

}
