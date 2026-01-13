using System.Collections;
using UnityEngine;

public class Granade : MonoBehaviour
{
    [SerializeField] private float delay = 5f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float damage = 100f;
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

        /*Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float finalDamage = damage * (1 - distance / explosionRadius);
                finalDamage = Mathf.Max(0, finalDamage);

                hit.GetComponent<EnemyHealth>()?.TakeDamage(finalDamage);
            }
        }*/

        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
