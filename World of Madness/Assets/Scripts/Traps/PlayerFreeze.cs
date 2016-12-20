using UnityEngine;
using System.Collections;

public class PlayerFreeze : MonoBehaviour {
  void OnTriggerEnter(Collider col) {
    if (col.gameObject.tag == "player1" || col.gameObject.tag == "player2")
    {
      col.gameObject.GetComponent<PlayerMovement>().toggleFrozen();
      col.gameObject.GetComponent<PlayerMovement>().setFreezeExpireTime(0.8f);
      Destroy(gameObject);
    }
  }
}
