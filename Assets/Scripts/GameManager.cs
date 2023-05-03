using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// The GameManager class is responsible for managing the game state, handling UI updates,
/// player respawning, bullet time effects, and tracking structures on screen.
/// </summary>
public class GameManager : MonoBehaviour
{

	#region Fields and Properties

	private static GameManager instance;

	public static readonly LayerMask RaycastLayersExcludingProjectiles;
	static GameManager() { RaycastLayersExcludingProjectiles = Physics2D.DefaultRaycastLayers & ~(1 << 8); }

	/// <summary>
	/// Active instance of the player. If this is null the player is currently dead.
	/// </summary>
	public static PlayerController Player;
    /// <summary>
    /// All entities on screen that are structures.
    /// </summary>
    public static List<Entity> Structures;        
    /// <summary>
    /// All colliders on screen.
    /// </summary>
    public static List<Collider2D> CollidersOnScreen = new List<Collider2D>();

	[Header("Player")]
	// Reference to the player prefab
	[Tooltip("Reference to the player prefab")]
	[SerializeField] private GameObject playerPrefab;

	// Spawn point for the player
	[Tooltip("Spawn point for the player")]
	[SerializeField] private Vector2 spawnPoint;

	// Player's score
	[Tooltip("Player's score")]
	[SerializeField] private float playerScore = 0f;

	// Number of player lives
	[Tooltip("Number of player lives")]
	[SerializeField] private int playerLives = 4;

	// Time it takes for the player to respawn
	[Tooltip("Time it takes for the player to respawn")]
	[SerializeField] private float playerRespawnTime = 2f;

	// Internal timer for player respawn
	private float playerRespawnTimer = 2f;

	// Gets the slow time factor
	/// <summary>
	/// Gets the slow time factor
	/// </summary>
	public static float SlowTimeFactor => instance.slowTimeFactor;

	[Header("Bullet Time")]
	// Factor by which the time slows down during bullet time
	[Tooltip("Factor by which the time slows down during bullet time")]
	[SerializeField] private float slowTimeFactor = 0.2f;

	// Returns true if bullet time is active
	/// <summary>
	/// Returns true if bullet time is active
	/// </summary>
	public static bool BulletTime => instance.slowTime;

	// Internal flag for slow time status
	[Tooltip("Internal flag for slow time status")]
	[SerializeField] private bool slowTime = false;

	[Header("UI")]
	// Text UI element for displaying player lives
	[Tooltip("Text UI element for displaying player lives")]
	[SerializeField] private Text livesText;

	// Text UI element for displaying player health
	[Tooltip("Text UI element for displaying player health")]
	[SerializeField] private Text healthText; //■□

	// Text UI element for displaying player score
	[Tooltip("Text UI element for displaying player score")]
	[SerializeField] private Text scoreText;

	// Text UI element for displaying game over message
	[Tooltip("Text UI element for displaying game over message")]
	[SerializeField] private Text gameOverText;

	// Button UI element for retrying the game
	[Tooltip("Button UI element for retrying the game")]
	[SerializeField] private Button retryButton;

	/// <summary>
	/// Gets the player's score
	/// </summary>
	public static float Score => instance.playerScore;
	/// <summary>
	/// Adds to the player's score
	/// </summary>
    public static void AddScore(float value) { 
        if (!Player) { return; }
        instance.playerScore += value; 
    }

	#endregion

	private void Awake() { instance = this; SpawnPlayer(); }

	private void Start() { if (SceneManager.loadedSceneCount == 1) { SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive); } }
	
	void Update() { 
		UpdateUI(); 
		HandleRespawn(); 
		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); } 
	}
	
	void FixedUpdate() { TrackStructuresOnScreen(); }

	/// <summary>
	/// Updates the UI elements including the score, lives, and health texts.
	/// </summary>
	private void UpdateUI()
    {
		// update UI score text
		scoreText.text = ((int)playerScore).ToString();

		// update UI lives text
		livesText.text = playerLives.ToString();
		if (playerLives == 0 && !gameOverText.isActiveAndEnabled) { gameOverText.enabled = true; retryButton.gameObject.SetActive(true); }
		else if (playerLives > 0 && gameOverText.isActiveAndEnabled) { gameOverText.enabled = false; retryButton.gameObject.SetActive(false); }

		// update UI player health text
		if (Player) {
			healthText.text = "";
			for (float i = 0.1f; i < 1.1f; i += 0.1f) {
				healthText.text += (i <= Player.HealthPercent || Mathf.Approximately(i, Player.HealthPercent)) ? "■" : "□";
			}
		}
		else { healthText.text = "□□□□□□□□□□"; }
	}

	/// <summary>
	/// Handles player respawn logic.
	/// </summary>
	private void HandleRespawn()
    {
		if (!Player) {
			if (slowTime) { Time.timeScale = 1f; slowTime = false; }
			if (playerLives == 0) { return; }
			playerRespawnTimer -= Time.deltaTime;
			if (playerRespawnTimer <= 0f) {
				playerRespawnTimer = playerRespawnTime;
				SpawnPlayer();
			}
		}
	}

	/// <summary>
	/// Spawns the player at the spawn point and sets initial properties.
	/// </summary>
	private void SpawnPlayer()
    {
		playerLives--; if (playerLives == 0) { return; }
		// instantiate player prefab and flash green for 1 second
		Player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity).GetComponent<PlayerController>();
        Player.FlashColor(Color.green, 1f);
		// give player invulnerability for 1 second
		Player.Invulnerable = true; 
        Runtime.DoInSeconds(() => { Player.Invulnerable = false; }, 1f);
	}

	/// <summary>
	/// Resets the game by setting player lives and score to their initial values.
	/// </summary>
	public void ResetGame() { playerLives = 4; playerScore = 0; }
	
	/// <summary>
	/// Tracks the structures on the screen.
	/// </summary>
	private void TrackStructuresOnScreen()
	{
		ScreenTrigger.Collider.Overlap(CollidersOnScreen);
		Structures = CollidersOnScreen
		.Where(c => c.GetComponent<Entity>() && c.GetComponent<Entity>().IsStructure)
		.Select(c => c.GetComponent<Entity>()).Distinct().ToList();
	}

	#region BulletTime

	public static void BulletTime_Start(float duration = 5f) { instance.M_BulletTime_Start(duration); }
    private void M_BulletTime_Start(float duration = 5f) { 
        if (bulletTime_End_Coroutine != null) { StopCoroutine(bulletTime_End_Coroutine); }
        Time.timeScale = slowTimeFactor; slowTime = true;
        bulletTime_End_Coroutine = StartCoroutine(BulletTime_End_Coroutine(duration));
        if (Player) { Player.SetGlowColor(Color.cyan); }
    }
    public static void BulletTime_End() { instance.M_BulletTime_End(); }
	private void M_BulletTime_End() { 
        Time.timeScale = 1f; slowTime = false; 
        if (Player) { Player.SetGlowColor(Color.white); }
    }
    private Coroutine bulletTime_End_Coroutine;
    private IEnumerator BulletTime_End_Coroutine(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        M_BulletTime_End();
    }

	#endregion

    // draw spawn point indicator ring
	private void OnDrawGizmosSelected() { Draw.Thickness = 0.3f; Draw.Ring(spawnPoint, 3f, Color.green); }

}