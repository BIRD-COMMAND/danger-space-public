using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

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
    
    private static GameManager instance;

	public static readonly LayerMask RaycastLayersExcludingProjectiles;
    static GameManager() {
	    RaycastLayersExcludingProjectiles = Physics2D.DefaultRaycastLayers & ~(1 << 8);
    }

    [Header("Player")]
	[SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector2 spawnPoint;
    public bool debugKeepPlayerInvulnerable = false;
    [SerializeField] private float playerScore = 0f;
    [SerializeField] private int playerLives = 4;
    [SerializeField] private float playerRespawnTime = 2f;
    private float playerRespawnTimer = 2f;

	public static float SlowTimeFactor => instance.slowTimeFactor;
	[Header("Bullet Time"), SerializeField] private float slowTimeFactor = 0.2f;
    public static bool SlowTime => instance.slowTime;
	[SerializeField] private bool slowTime = false;

	[Header("UI")]
    [SerializeField] private Text livesText;
    [SerializeField] private Text healthText; //■□
	[SerializeField] private Text scoreText;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Button retryButton;

    public static float Score => instance.playerScore;
    public static void AddScore(float value) { 
        if (!Player) { return; }
        instance.playerScore += value; 
    }

	private void Awake() { instance = this; SpawnPlayer(); }

	private void Start() {
        if (SceneManager.loadedSceneCount == 1) {
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        }
	}

	// Update is called once per frame
	void Update()
    {
        // update UI score text
        scoreText.text = ((int)playerScore).ToString();

        // update UI lives text
        livesText.text = playerLives.ToString();
        if (playerLives == 0 && !gameOverText.isActiveAndEnabled)    { gameOverText.enabled = true;  retryButton.gameObject.SetActive(true);  }
        else if (playerLives > 0 && gameOverText.isActiveAndEnabled) { gameOverText.enabled = false; retryButton.gameObject.SetActive(false); }

        // update UI player health text
        if (Player) {
            healthText.text = "";
            for (float i = 0.1f; i < 1.1f; i += 0.1f) {
                healthText.text += (i <= Player.HealthPercent || Mathf.Approximately(i, Player.HealthPercent)) ? "■" : "□";
            }
        }
        else { healthText.text = "□□□□□□□□□□"; }

        // handle respawn
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

    public void ResetGame()
    {
        playerLives = 4;
        playerScore = 0;
    }

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

	private void FixedUpdate()
	{
        TrackStructuresOnScreen();
	}

	private void SpawnPlayer()
    {
		playerLives--; if (playerLives == 0) { return; }
		// instantiate player prefab and flash green for 1 second
		Player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity).GetComponent<PlayerController>();
        Player.FlashColor(Color.green, 1f);
		// give player invulnerability for 1 second
		Player.invulnerable = true; 
        Runtime.DoInSeconds(() => { Player.invulnerable = debugKeepPlayerInvulnerable; }, 1f);
	}

	private void TrackStructuresOnScreen()
	{
		ScreenTrigger.Collider.Overlap(CollidersOnScreen);
		Structures = CollidersOnScreen
		.Where(c => c.GetComponent<Entity>() && c.GetComponent<Entity>().IsStructure)
		.Select(c => c.GetComponent<Entity>()).Distinct().ToList();
	}

	private void OnDrawGizmosSelected() { 
        // draw spawn point indicator ring
        Draw.Thickness = 0.3f; Draw.Ring(spawnPoint, 3f, Color.green); 
    }

}
