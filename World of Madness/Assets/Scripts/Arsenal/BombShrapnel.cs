using UnityEngine;
using System.Collections;

public class BombShrapnel : MonoBehaviour {
  public Vector3 direction;
  private float speed;
  private float expireTime;

	// Use this for initialization
	void Start () {
    this.speed = 10.0f;
    this.expireTime = 0.1f;
	}


  void OnCollisionEnter(Collision col) {
    if (col.gameObject.tag == "wall") {
      Destroy(col.gameObject);
      Destroy(gameObject);
    } 
    else if (col.gameObject.tag == "player1" || col.gameObject.tag == "player2") {
      col.gameObject.GetComponent<Player>().loseLife();
      Destroy(gameObject);
    }
  }

	
	// Update is called once per frame
	void Update () {
    expireTime -= Time.deltaTime;
    if (expireTime <= 0) { Destroy (gameObject); }
	}
}
