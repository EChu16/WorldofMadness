using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour {
  void OnCollisionEnter(Collision col) {
    if (col.gameObject.tag == "player1" || col.gameObject.tag == "player2")
    {
      col.gameObject.GetComponent<Player>().loseLife();
    }
  }
}
