using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
  public int powerUpID;

  void OnCollisionEnter(Collision col) {
    if (col.gameObject.tag == "player1" || col.gameObject.tag == "player2")
    {
      col.gameObject.GetComponent<Player>().setPowerUp(powerUpID, -1, 3);
      Destroy(gameObject);
    }
  }
}
