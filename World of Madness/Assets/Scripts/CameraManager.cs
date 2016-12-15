using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
  // Credits to https://gist.github.com/ftvs/5822103
  // Transform of the camera to shake. Grabs the gameObject's transform
  // if null.
  public Transform camTransform;

  // How long the object should shake for.
  public float shakeDuration = 0.0f;

  // Amplitude of the shake. A larger value shakes the camera harder.
  public float shakeAmount = 0.8f;
  public float decreaseFactor = 1.0f;

  Vector3 originalPos;

  public void setDuration(float duration) {
    this.shakeDuration = duration;
  }

  void Awake()
  {
    if (camTransform == null)
    {
      camTransform = GetComponent(typeof(Transform)) as Transform;
    }
  }

  void OnEnable()
  {
    originalPos = camTransform.localPosition;
  }

  void Update()
  {
    if (shakeDuration > 0)
    {
      camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

      shakeDuration -= Time.deltaTime * decreaseFactor;
    }
    else
    {
      shakeDuration = 0f;
      camTransform.localPosition = originalPos;
    }
  }

}
