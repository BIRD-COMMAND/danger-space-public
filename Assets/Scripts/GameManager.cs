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

	/// <summary>
	/// Whether the application is paused
	/// </summary>
	[Tooltip("Whether the application is paused")]
	[SerializeField] private bool isPaused = false;
	/// <summary>
	/// Whether the application is paused
	/// </summary>
	public static bool IsPaused { get => instance.isPaused; set => instance.isPaused = value; }

	/// <summary>
	/// Whether the application is in edit mode
	/// </summary>
	[Tooltip("Whether the application is in edit mode")]
	[SerializeField] private bool editMode = false;
	/// <summary>
	/// Whether the application is in edit mode
	/// </summary>
	public static bool EditMode { get => instance.editMode; set => instance.editMode = value; }

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

	// Whether the player is invulnerable
	[Tooltip("Whether the player is invulnerable")]
	[SerializeField] private bool playerInvulnerable = false;
	/// <summary>
	/// Whether the player is invulnerable
	/// </summary>
	public static bool PlayerInvulnerable { get => instance.playerInvulnerable; set => instance.playerInvulnerable = value; }

	// Whether the player has infinite energy
	[Tooltip("Whether the player has infinite energy")]
	[SerializeField] private bool infiniteEnergy = false;
	/// <summary>
	/// Whether the player has infinite energy
	/// </summary>
	public bool InfiniteEnergy { get => infiniteEnergy; set => infiniteEnergy = value; }

	// Gets the slow time factor
	/// <summary>
	/// Gets the slow time factor
	/// </summary>
	public static float BulletTimeFactor => instance.bulletTimeFactor;

	[Header("Bullet Time")]
	// Factor by which the time slows down during bullet time
	[Tooltip("Factor by which the time slows down during bullet time")]
	[SerializeField] private float bulletTimeFactor = 0.2f;

	// Returns true if bullet time is active
	/// <summary>
	/// Returns true if bullet time is active
	/// </summary>
	public static bool BulletTime { 
		get => instance.bulletTime;
		set {
			if (value) { Time.timeScale = BulletTimeFactor; }
			else { Time.timeScale = 1f; }
			instance.bulletTime = value; 
		}
	}

	// Internal flag for slow time status
	[Tooltip("Internal flag for slow time status")]
	[SerializeField] private bool bulletTime = false;


	[Header("UI")]
	// Text UI element for displaying player lives
	[Tooltip("Text UI element for displaying player lives")]
	[SerializeField] private Text livesText;

	// Text UI element for displaying player score
	[Tooltip("Text UI element for displaying player score")]
	[SerializeField] private Text scoreText;

	// GameObject containing the Player Stats UI Elements
	[Tooltip("GameObject containing the Player Stats UI Elements")]
	[SerializeField] private GameObject playerStats;

	// GameObject containing the Game Over screen UI Elements
	[Tooltip("GameObject containing the Game Over screen UI Elements")]
	[SerializeField] private GameObject gameOverScreen;

	// GameObject containing the Pause Menu UI Elements
	[Tooltip("GameObject containing the Pause Menu UI Elements")]
	[SerializeField] private GameObject pauseMenu;

	// GameObject containing the Edit Mode Overlay UI Elements
	[Tooltip("GameObject containing the Edit Mode Overlay UI Elements")]
	[SerializeField] private GameObject editModeOverlay;

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

	private void Start() { 
		if (SceneManager.loadedSceneCount == 1) { SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive); }
		TogglePause();
	}
	
	void Update() { 
		if (Input.GetKeyDown(KeyCode.Tab)) { TogglePause(); }
		if (EditMode) { return; }
		UpdateUI(); 
		HandleRespawn();
	}
	
	void FixedUpdate() { TrackStructuresOnScreen(); }

	/// <summary>
	/// Pause / Unpause the game
	/// </summary>
	public void TogglePause()
	{
		if (editMode) { ToggleEditMode(); return; }
		if (isPaused) {
			if (BulletTime) { Time.timeScale = bulletTimeFactor; }
			else { Time.timeScale = 1f; }
		}
		else { Time.timeScale = 0f; }
		isPaused = !isPaused;
		pauseMenu.SetActive(isPaused);
	}
	/// <summary>
	/// Toggle Edit Mode
	/// </summary>
	public void ToggleEditMode()
	{
		if (editMode) {
			GetComponent<EditModeManager>().StopEditMode();
			Destroy(GetComponent<CameraController>());
			Time.timeScale = 0f;
		}
		else {
			GetComponent<EditModeManager>().StartEditMode();			
			gameObject.AddComponent<CameraController>();
			Time.timeScale = 1f;
		}
		editMode = !editMode;
		pauseMenu.SetActive(!editMode);
		playerStats.SetActive(!editMode);
		editModeOverlay.SetActive(editMode);
	}
	/// <summary>
	/// Quits the game
	/// </summary>
	public void QuitApplication() { Application.Quit(); }

	/// <summary>
	/// Updates the UI elements including the score, lives, and health texts.
	/// </summary>
	private void UpdateUI()
    {

		// update UI score text
		scoreText.text = ((int)playerScore).ToString();

		// update UI lives text
		livesText.text = playerLives.ToString();
		if (playerLives == 0 && !gameOverScreen.activeSelf) { gameOverScreen.SetActive(true); }
		else if (playerLives > 0 && gameOverScreen.activeSelf) { gameOverScreen.SetActive(false); }

	}

	/// <summary>
	/// Handles player respawn logic.
	/// </summary>
	private void HandleRespawn()
    {
		if (!Player) {
			if (bulletTime) { Time.timeScale = 1f; bulletTime = false; }
			if (playerLives == 0) { return; }
			playerRespawnTimer -= Time.deltaTime;
			if (playerRespawnTimer <= 0f) {
				playerRespawnTimer = playerRespawnTime;
				SpawnPlayer();
			}
		}
		else {
			if (Player.Invulnerable != playerInvulnerable) { Player.Invulnerable = playerInvulnerable; }
			if (infiniteEnergy) { Player.CurrentEnergy = Player.MaxEnergy; }
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

    // draw spawn point indicator ring
	private void OnDrawGizmosSelected() { Draw.Thickness = 0.3f; Draw.Ring(spawnPoint, 3f, Color.green); }

}