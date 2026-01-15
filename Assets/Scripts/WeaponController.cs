using DG.Tweening;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform barrel;

    [Header("Ammo")]
    [SerializeField] private int currentAmmo;
    [SerializeField] private int maxAmmo;
    [SerializeField] private bool infiniteAmmo;

    [Header("Performance")]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootRate;
    [SerializeField] private int damage;

    [Header("Reload")]
    [SerializeField] private bool canRecharge;
    [SerializeField] private float durationReload = 1f;

    [Header("Croshair")]
    [SerializeField] private GameObject pointWeapon;

    public float ShootRate { get => shootRate; set => shootRate = value; }
    public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }
    public int CurrentAmmo { get => currentAmmo; set => currentAmmo = value; }

    private ObjectPool objectPool;
    private Transform weaponTransform;
    private float lastShootTime;

    private bool isPlayer;
    private bool isAim;
    private bool isSprint;
    private bool isRecharge;

    //position weapon
    private Vector3 originalPosition;
    private Vector3 AimPosition;

    private PlayerMovement playerMovement;
    private Camera camera;
    private GranadeController granadeController;

    private void Awake()
    {
        //Check if Ia am a Player
        isPlayer = gameObject.CompareTag("Player");
        weaponTransform = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.gameObject.layer == LayerMask.NameToLayer("Weapon"));
        //get objectPool
        objectPool = GetComponent<ObjectPool>();
        playerMovement = GetComponent<PlayerMovement>();
        originalPosition = weaponTransform.localPosition;
        AimPosition = new Vector3(0, -0.1f, 0.074f);
        camera = Camera.main;

    }
    private void Start()
    {
        granadeController = GetComponent<GranadeController>();
    }
    private void Update()
    {
        ControlAim();
    }

    /// <summary>
    /// Handle Weapon Shoot
    /// </summary>
    public void Shoot()
    {
        //update last shoot time
        lastShootTime = Time.time;

        if (!infiniteAmmo) currentAmmo--;

        //Get a new active bullet
        GameObject bullet = objectPool.GetGameObject();

        //position and rotation
        bullet.transform.position = barrel.position;
        bullet.transform.rotation = barrel.rotation;

        //assign damage
        bullet.GetComponent<BulletController>().Damage = damage;

        //if is Player
        if (isPlayer)
        {
            //Create a Ray from Camera to the middle of the screen
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            RaycastHit hit;
            Vector3 targetPoint;

            //Check if the ray hit with something and adjust direction
            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(5); //Get a point at 5m
            }

            if (!isAim)
            {
            weaponTransform.DOLocalMoveZ(originalPosition.z - 0.2f, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
               weaponTransform.DOLocalMoveZ(originalPosition.z, 0.1f).SetEase(Ease.InQuad);
            });
            }
            
            bullet.GetComponent<Rigidbody>().linearVelocity = (targetPoint - barrel.position).normalized * bulletSpeed;
        }
        //If Enemy
        else
        {
            //TODO random directions near the player
            //Give velocity to Bullet
            bullet.GetComponent<Rigidbody>().linearVelocity = barrel.forward * bulletSpeed;
        }
    }

    /// <summary>
    /// check if it is posible to shoot
    /// </summary>
    /// <returns></returns>
    public bool CanShoot()
    {
        //Check shootRate
        if (isPlayer)
        {
            if ((Time.time - lastShootTime >= shootRate) && !granadeController.HasGrenadeInHand && !isRecharge)
            {
                //Check Ammo
                if (currentAmmo > 0 || infiniteAmmo)
                {
                    return true;
                }

            }
        }
        else
        {
            if ((Time.time - lastShootTime >= shootRate))
            {
                //Check Ammo
                if (currentAmmo > 0 || infiniteAmmo)
                {
                    return true;
                }

            }
        }

            return false;
    }
    private void ControlAim()
    {
        if (isPlayer)
        {        
        isSprint = playerMovement.IsSprinting;
        if (isSprint && isAim || granadeController.HasGrenadeInHand)
        {
            StopAim();
        }
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, isAim ? 40 : 60, Time.deltaTime * 10f);
        weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, isAim ? AimPosition : originalPosition, Time.deltaTime * 10f);
        }
    }
    public void AimShoot()
    {
        if (!granadeController.HasGrenadeInHand)
        {
        isAim = true;
        pointWeapon.SetActive(false);
        }
        
    }
    public void StopAim()
    {
        isAim = false;
        pointWeapon.SetActive(true);
        
    }
    public void Recharge()
    {
        if (canRecharge)
        {
            isRecharge = true;
            weaponTransform.DOLocalMoveY(originalPosition.y - 0.3f, durationReload)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    weaponTransform.DOLocalMoveY(originalPosition.y, durationReload)
                        .SetEase(Ease.InOutQuad).OnComplete(() =>
                        {
                            isRecharge=false;
                        });
                });
            });
            
        }
    }
}
