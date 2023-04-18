using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMissileTesting : MonoBehaviour
{

    public GameObject player;
    public GameObject missilePrefab;
    private List<Rigidbody2D> missiles = new List<Rigidbody2D>();

    // Start is called before the first frame update
    void Start() { if (!missilePrefab || !player) { Debug.LogError("Destroying Invalid DebugMissileTesting", gameObject); Destroy(this); } }

    // Update is called once per frame
    void Update()
    {
        
        // You must hold down Left Shift to spawn and launch missiles        
        if (!Input.GetKeyDown(KeyCode.LeftShift)) { return; }
        
        if (Mouse.LeftClick) {
            missiles.Add((Instantiate(missilePrefab, Mouse.WorldPosition, Quaternion.identity) as GameObject).GetComponent<Rigidbody2D>());
        }

        if (Mouse.RightClick) {
            missiles.RemoveAll(x => x == null);
            foreach (Rigidbody2D body in missiles) { body.GetComponent<Missile>().Fire(player.transform, missiles); }
        }

    }
}