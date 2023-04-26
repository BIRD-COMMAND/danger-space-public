using Shapes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject playerPrefab;
    public int playerLives = 3;
    public float playerRespawnTime = 2f;
    private float playerRespawnTimer = 2f;

    // Update is called once per frame
    void Update()
    {        
        // handle respawn
        if (!PlayerController.player) {             
            if (playerLives == 0) { return; }
            playerRespawnTimer -= Time.deltaTime;
            if (playerRespawnTimer <= 0f && playerLives > 0) {
				playerLives--; playerRespawnTimer = playerRespawnTime;
                PlayerController.player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>().GetComponent<PlayerController>();
                PlayerController.player.invulnerable = true;
                PlayerController.player.FlashColor(Color.green, 1f);
				StartCoroutine(RemovePlayerInvulnerability());
			}
		}
    }

    private System.Collections.IEnumerator RemovePlayerInvulnerability()
    {
        yield return new WaitForSeconds(1f);
        PlayerController.player.invulnerable = false;
    }

}
