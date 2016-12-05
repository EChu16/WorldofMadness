using UnityEngine;
using System.Collections;

public class Active : MonoBehaviour {
  public int activeID;

  void OnCollisionEnter(Collision col) {
    if (col.gameObject.tag == "player1" || col.gameObject.tag == "player2")
    {
      col.gameObject.GetComponent<Player>().setActive(activeID);
      Destroy(gameObject);
    }
  }
}
