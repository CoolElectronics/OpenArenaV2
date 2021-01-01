using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scythe : ability
{
    [SerializeField]
    Binding binding;
    [SerializeField]
    GameObject bladeObject;
    [SerializeField]
    GameObject bladePivot;
    float strikeCooldownTimer = 0;
    [SerializeField]
    float strikeCooldownTimeMax = 1;
    float bladeActiveTimer = 0;
    [SerializeField]
    float bladeActiveTimeMax = 1;
    [SerializeField]
    Vector2 centerOffset;
    [SerializeField]
    LayerMask lmEnemy;
    [SerializeField]
    float knockback;
    public float bladeReachDistance;
    Vector2 bladeDir;
    Vector3 center;
    move controller;
    dash dashcontroller;



    void Start()
    {
        controller = GetComponent<move>();
        dashcontroller = GetComponent<dash>();
    }
    void Update()
    {
        strikeCooldownTimer -= Time.deltaTime;
        bladeActiveTimer -= Time.deltaTime;
        Vector3 center = transform.position + (Vector3)centerOffset;
        if (bladeActiveTimer > 0)
        {
            bladeObject.SetActive(true);
            Collider2D[] enemies = Physics2D.OverlapCircleAll(center, bladeReachDistance, lmEnemy);
            bool hit = false;
            foreach (Collider2D eCol in enemies)
            {
                GameObject enemy = eCol.gameObject;
                Vector2 enemyDir = Vector2.zero;
                if (enemy.transform.position.y > center.y + 0.5f)
                {
                    //Enemy is above player
                    enemyDir = Vector2.up;
                }
                else if (enemy.transform.position.y < center.y - 0.6f)
                {
                    //Enemy is below the player
                    enemyDir = -Vector2.up;
                }
                else if (enemy.transform.position.x > center.x)
                {
                    //Enemy is to the right of the player
                    enemyDir = Vector2.right;
                }
                else if (enemy.transform.position.x < center.x)
                {
                    //Enemy is to the left of the player
                    enemyDir = -Vector2.right;
                }
                else
                {
                    Debug.Log("Sigh...;",enemy);
                    //Enemy is... inside the player??
                }
                if (enemyDir == bladeDir)
                {
                    //Destroy(enemy);
                    hit = true;
                }
            }
            if (hit)
            {
                if (bladeDir.y == -1){
                    controller.canDblJump = true;
                    dashcontroller.dashCooldownTimer = 0;
                }
                if (bladeDir.y == 0)
                {
                    controller.SetForceX(bladeDir.x * knockback, 0);
                }
                else
                {
                    controller.SetForceY(bladeDir.y * knockback, 0);
                }
            }
        }
        else
        {
            bladeObject.SetActive(false);
        }
        if (Input.GetKeyDown(binding.attack))
        {
            if (strikeCooldownTimer < 0)
            {
                strikeCooldownTimer = strikeCooldownTimeMax;
                bladeActiveTimer = bladeActiveTimeMax;
                if (Input.GetKey(binding.up))
                {
                    bladeDir = Vector2.up;
                    bladePivot.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else if (Input.GetKey(binding.down))
                {
                    bladeDir = -Vector2.up;
                    bladePivot.transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else if (controller.dir == -1)
                {
                    bladeDir = -Vector2.right;
                    bladePivot.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                else if (controller.dir == 1)
                {
                    bladeDir = Vector2.right;
                    bladePivot.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    throw new System.Exception("Whaa");
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        DrawGizmoDisk(center, Quaternion.Euler(0, 90, 90), bladeReachDistance, 0.01f);
    }
    public static void DrawGizmoDisk(Vector3 position, Quaternion rotation, float radius, float thick)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 0.5f); //this is gray, could be anything
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, new Vector3(1, thick, 1));
        Gizmos.DrawSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
    }
}
