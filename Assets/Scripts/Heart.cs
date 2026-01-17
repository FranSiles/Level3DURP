using DG.Tweening;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public float beatScale = 1.2f;  // Cuánto crecerá
    public float beatDuration = 0.3f; // Duración de cada latido
    public int loops = -1; // -1 para infinito

    void Start()
    {
        // Escala original
        Vector3 originalScale = transform.localScale;

        // Hacer un "pulsing" infinito
        transform.DOScale(originalScale * beatScale, beatDuration)
                 .SetLoops(loops, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);
    }
}
