using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControls : MonoBehaviour
{
    //External components
    public GameObject player;
    public Sprite[] crosshairs;
    public GameObject projectile;
    public GameObject bomb;
    public GameObject bombTrigger;

    //Public variables for the primary weapon
    public string primaryWeapon = "sixgun"; //Player starts with sixgun
    public int startingBullets = 6;
    public int bulletSpeed = 500;
    public float firingRate = 0.5f;
    public float reloadTime = 2f;
    //Private variables for the primary weapon cooldown
    private int projectileCount;
    private float reloadTimeStamp;
    private float projectileTimeStamp;

    //Public variables for the primary weapon
    public int startingBombs = 2;
    public int bombThrowSpeed = 250;
    public float bombThrowRate = 1f;
    public float bombCooldown = 2.5f;
    //Private variables for the primary weapon cooldown
    private int bombCount;
    private float bombCoodownTimeStamp;
    private float bombThrowTimeStamp;

    //Private variables for aiming
    private Vector3 mousePosition;
    private float rotationSpeed;
     
    void Start()
    {
        //Get the rotation speed of the player
        rotationSpeed = player.GetComponent<PlayerController>().rotationSpeed;
        projectileCount = startingBullets;
    }

    void FixedUpdate()
    {
        //Crosshair follows the mouse in fixed update
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = Vector2.Lerp(transform.position, mousePosition, rotationSpeed);
    }

    void Update()
    {
        //Making sure that the crosshairs stay on top
        gameObject.transform.Translate(Vector3.back);

        //Weapon Select
        if (Input.GetMouseButtonDown(1) || Input.GetButtonDown("Jump"))
        {
            if (primaryWeapon == "sixgun")
            {
                primaryWeapon = "firebomb";
                bombCount = startingBombs;
                GetComponent<SpriteRenderer>().sprite = crosshairs[1];
            } else
            {
                primaryWeapon = "sixgun";
                reloadTimeStamp = 0;
                projectileCount = startingBullets;
                GetComponent<SpriteRenderer>().sprite = crosshairs[0];
            }
        }

        //Primary Firing
        if (primaryWeapon == "sixgun")
        {
            if (projectileCount > 0 && projectileTimeStamp <= Time.time && reloadTimeStamp <= Time.time)
            {
                if (Input.GetMouseButton(0))
                {
                    ShootProjectile();
                    projectileCount--;
                    projectileTimeStamp = Time.time + firingRate;
                }
            }
            if (projectileCount <= 0)
            {
                reloadTimeStamp = Time.time + reloadTime;
                projectileCount = startingBullets;
            }
        }

        //Secondary Firing        
        if (primaryWeapon == "firebomb")
        {
            if (bombCount > 0 && bombThrowTimeStamp <= Time.time && bombCoodownTimeStamp <= Time.time)
            {
                if (Input.GetMouseButton(0))
                {
                    ThrowBomb();
                    bombCount--;
                    bombThrowTimeStamp = Time.time + bombThrowRate;
                }
            }
            if (bombCount <= 0)
            {
                bombCoodownTimeStamp = Time.time + reloadTime;
                bombCount = startingBombs;
            }
        }
    }

    void ShootProjectile()
    {
        Vector3 bulletPos = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        GameObject bullet = Instantiate(projectile, bulletPos, player.transform.rotation) as GameObject;
        bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.up * bulletSpeed);
    }

    void ThrowBomb()
    {
        //Create an invisible trigger collider to catch the thrown bomb.
        Vector3 currentPos = transform.position; //Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPos.z = 0;
        GameObject trigger = Instantiate(bombTrigger, currentPos, player.transform.rotation) as GameObject;

        //Get the correct look rotation beween the crosshair and the player for the bombthrow - it's super accurate
        Quaternion currentRot = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.forward);
        currentRot.x = 0;
        currentRot.y = 0;

        //Create the bomb as the child of the trigger collider.
        Vector3 bombPos = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        GameObject firebomb = Instantiate(bomb, bombPos, currentRot) as GameObject;
        firebomb.GetComponent<Rigidbody2D>().AddForce(firebomb.transform.up * bombThrowSpeed);
        firebomb.transform.parent = trigger.gameObject.transform;
    }
}