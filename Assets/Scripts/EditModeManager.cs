using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Shapes;
using System.Linq;

public class EditModeManager : MonoBehaviour
{

	/// <summary>
	/// Reference to the player
	/// </summary>
	private PlayerController player;

	/// <summary>
	/// All entities in the scene. Repopulated each FixedUpdate.
	/// </summary>
	private List<Entity> entities = new List<Entity>();

	/// <summary>
	/// The entity the mouse is currently hovering over or interacting with
	/// </summary>
	private Entity mouseEntity;
	/// <summary>
	/// Whether the mouse is currently dragging an entity
	/// </summary>
	private bool draggingEntity = false;
	/// <summary>
	/// The offset between the mouse's position and the entity's position when the mouse started dragging the entity
	/// </summary>
	private Vector2 dragOffset = Vector2.zero;

	public void StartEditMode()
	{
		// we do this so enemies can't target the player (and fire at them)
		if (GameManager.Player) { player = GameManager.Player; GameManager.Player = null; }

		// OnEditModeStarted Callback
		foreach (Entity entity in FindObjectsByType<Entity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) { entity.OnEditModeStarted(); }

		// zoom camera out
		GetComponent<Camera>().orthographicSize *= 1.75f;
	}

	public void StopEditMode()
	{		
		// OnEditModeStopped Callback
		foreach (Entity entity in FindObjectsByType<Entity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) { entity.OnEditModeStopped(); }

		// reassign player reference to the GameManager so enemies can target the player
		if (player) { GameManager.Player = player; player = null; }

		// zoom camera in
		gameObject.transform.position = new Vector3(0f, 0f, -10f);
		GetComponent<Camera>().orthographicSize = 80f;
	}

	private void FixedUpdate()
	{
		if (!GameManager.EditMode) { return; }
		entities.Clear(); entities.AddRange(FindObjectsByType<Entity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
	}

	private void Update()
	{
		
		if (!GameManager.EditMode) { return; }
		
		ScreenTrigger.DrawScreenBounds();

		// draw visualizations for all entities that have one
		foreach (Entity entity in entities) { if (entity) { entity.OnEditModeDisplay(); } }

		// try to get the entity the mouse is currently hovering over
		if (!draggingEntity) { mouseEntity = Mouse.HoveredItems<Entity>().OrderBy(x => x.Radius).FirstOrDefault(); }
		if (mouseEntity) { mouseEntity.DrawRadius(Color.white); } // draw entity highlight

		// try to start mouse drag
		if (Mouse.LeftClick && !draggingEntity && mouseEntity) {
			// duplicate entity if control is held down
			if (Input.GetKey(KeyCode.LeftControl)) { mouseEntity = mouseEntity.Duplicate(); mouseEntity.OnEditModeStarted(); }
			draggingEntity = true; dragOffset = mouseEntity.Position - Mouse.WorldPosition;
		}

		// stop mouse drag
		if (draggingEntity && !Mouse.LeftDown) { draggingEntity = false; }
		if (draggingEntity && !mouseEntity) { draggingEntity = false; }

		// do mouse drag
		if (draggingEntity && mouseEntity) { 
			Vector2 oldPosition = mouseEntity.transform.position;
			mouseEntity.transform.position = Mouse.WorldPosition + dragOffset;
			mouseEntity.OnEditModeMoved(oldPosition);
		}

		// try to delete entity (unless it's the player)
		if (Mouse.RightClick && mouseEntity && mouseEntity != player) { mouseEntity.OnWillBeDestroyed(); }

	}

	/// <summary>
	/// Spawns a prefab at the world origin
	/// </summary>
	public void SpawnPrefab(GameObject prefab) {
		if (!prefab) { return; } 
		Entity entity;
		if (PoolManager.CanGet(prefab)) { 
			entity = PoolManager.Get(prefab).Activate(Vector3.zero, Quaternion.identity).GetComponent<Entity>(); 
		} else { 
			entity = Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<Entity>(); 
		}
		if (entity) { entity.OnEditModeStarted(); }
	}

}
