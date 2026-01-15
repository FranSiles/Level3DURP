using System.Collections;
using UnityEngine;

public class Granade : MonoBehaviour
{
    [SerializeField] private float delay = 5f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int damage = 2;
    [SerializeField] private GameObject explosionEffect;

    public void Activate()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    private void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        Debug.Log("Explote");

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Daño realizado");
                hit.GetComponent<EnemyController>().DamageEnemy(damage);
            }
        }

        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
