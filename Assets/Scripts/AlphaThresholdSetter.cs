using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlphaThresholdSetter : MonoBehaviour
{
    [Range(0,1)] public float threshold = 0.1f;

    void Awake()
    {
        var img = GetComponent<Image>();
        img.useSpriteMesh = true;
        img.alphaHitTestMinimumThreshold = threshold;
    }
}