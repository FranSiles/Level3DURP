using DG.Tweening;
using UnityEngine;

public class MoveGranade : MonoBehaviour
{
    private GranadeController granadeController;
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
            granadeController = other.GetComponent<GranadeController>();
            if (granadeController.CurrentGranade < granadeController.MaxGranade)
            {
                granadeController.CurrentGranade += 1;
                gameObject.SetActive(false);
            }
        }
    }
}
