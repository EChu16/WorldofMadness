using UnityEngine;
using System.Collections;

public class Sandpit : MonoBehaviour {
  private float reductionSpeed = 10.0f;

  void OnTriggerEnter(Collider col) {
    if (col.gameObject.tag == "player1" || col.gameObject.tag == "player2")
    {
      float currentMoveSpeed = col.gameObject.GetComponent<Player>().getMoveSpeed();
      col.gameObject.GetComponent<Player>().setMoveSpeed(currentMoveSpeed - this.reductionSpeed);
    }
  }

  void OnTriggerExit(Collider col) {
    if (col.gameObject.tag == "player1" || col.gameObject.tag == "player2")
    {
      float currentMoveSpeed = col.gameObject.GetComponent<Player>().getMoveSpeed();
      col.gameObject.GetComponent<Player>().setMoveSpeed(currentMoveSpeed + this.reductionSpeed);
    }
  }
}
