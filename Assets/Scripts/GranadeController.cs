using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GranadeController : MonoBehaviour
{
    [SerializeField] private GameObject granadeInHand;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float throwForce = 8f;
    private bool hasGrenadeInHand;
    private bool isPreparingThrow;
    Transform weapontransform;

    public bool HasGrenadeInHand { get => hasGrenadeInHand; set => hasGrenadeInHand = value; }

    public void putGranade()
    {
        if (!hasGrenadeInHand)
        {
            hasGrenadeInHand = true;
            granadeInHand.SetActive(true);
            weapontransform = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.gameObject.layer == LayerMask.NameToLayer("Weapon"));
            weapontransform.gameObject.SetActive(false);
        }
        else
        {
            isPreparingThrow = true;
        }
    }
    /*public void PositionThrow()
    {
        UpdateThrowIndicator();
    }*/

    public void ThrowGranade()
    {
        if (!isPreparingThrow || granadeInHand == null) return;

        isPreparingThrow = false;
        hasGrenadeInHand = false;

        Vector3 spawnPos = granadeInHand.transform.position;
        Quaternion spawnRot = granadeInHand.transform.rotation;

        GameObject thrownGrenade = Instantiate(grenadePrefab, spawnPos, spawnRot);

        Rigidbody rb = thrownGrenade.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);

        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

        thrownGrenade.GetComponent<Granade>().Activate();

        // ocultar la visual, NO destruir
        granadeInHand.SetActive(false);

        if (weapontransform != null)
            weapontransform.gameObject.SetActive(true);

    }

    /*private void UpdateThrowIndicator()
    {
        Vector3 startPos = granade.transform.position + Vector3.up * 1.5f; // altura de la mano
        Vector3 velocity = Camera.main.transform.forward * throwForce;
        Vector3 gravity = Physics.gravity;

        Vector3 predictedPos = startPos;
        float timestep = 0.05f;

        for (float t = 0; t < 3f; t += timestep)
        {
            Vector3 nextPos = startPos + velocity * t + 0.5f * gravity * t * t;
            if (Physics.Raycast(predictedPos, nextPos - predictedPos, out RaycastHit hit, (nextPos - predictedPos).magnitude))
            {
                predictedPos = hit.point;
                break;
            }
            predictedPos = nextPos;
        }

        // finalmente coloca el indicador
        throwIndicator.transform.position = predictedPos;
    }*/
}
