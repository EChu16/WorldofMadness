﻿using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
  public int powerUpID;

  void OnTriggerEnter(Collider col) {
    if (col.gameObject.tag == "player1" || col.gameObject.tag == "player2")
    {
      col.gameObject.GetComponent<Player>().setPowerUp(powerUpID);
      Destroy(gameObject);
    }
  }
}
