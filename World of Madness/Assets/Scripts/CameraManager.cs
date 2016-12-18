using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
  // Modification of https://gist.github.com/ftvs/5822103
  // Transform of the camera to shake. Grabs the gameObject's transform
  // if null.
  public Transform camTransform;

  // How long the object should shake for.
  public float shakeDuration = 0.0f;

  // Amplitude of the shake. A larger value shakes the camera harder.
  public float shakeAmount = 0.8f;
  public float decreaseFactor = 1.0f;
  public Vector3 shakeMod;

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

  void Update()
  {
    if (shakeDuration > 0)
    {
      shakeMod = Random.insideUnitSphere * shakeAmount;

      shakeDuration -= Time.deltaTime * decreaseFactor;
    }
    else
    {
      shakeDuration = 0f;
      shakeMod = new Vector3(0,0,0);
    }
  }

}
