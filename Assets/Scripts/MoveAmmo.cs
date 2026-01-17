using DG.Tweening;
using System;
using UnityEngine;

public class MoveAmmo : MonoBehaviour
{
    private WeaponController weaponController;
    void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Restart)
                 .SetEase(Ease.Linear);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           weaponController = other.GetComponent<WeaponController>();
            if (weaponController!=null)
            {
                Debug.Log("Funciono");
            }
            else
            {
                Debug.Log("No Funciono :V");
            }
            if (weaponController.AmmoReserve < weaponController.MaxAmmo)
            {
                int bulletsToAdd = Mathf.Min(30, weaponController.MaxAmmo - weaponController.AmmoReserve);

                weaponController.AmmoReserve += bulletsToAdd;
                gameObject.SetActive(false);
            }
        }
    }

}
