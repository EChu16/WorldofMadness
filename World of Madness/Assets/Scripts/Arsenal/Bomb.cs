using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
  public Vector3 direction;
  public BombShrapnel shrapnel;
  private Vector3 lastPosition;
  private float speed;
  private float acceleration;
  private float expireTime;
  private bool exploded;
  private bool continueTranslate;

	// Use this for initialization
	void Start () {
    this.speed = 35.0f;
    this.expireTime = 3.0f;
    this.exploded = false;
    this.continueTranslate = true;
	}
	
  void OnTriggerEnter(Collider col) {
    // If bomb collides with a wall, explode and destroy wall blocks on impact
    if (col.gameObject.tag == "wall") {
      this.explode ();
      Camera.main.GetComponent<CameraManager> ().setDuration (0.3f);
      Destroy (col.gameObject);
      Destroy (gameObject);
    } 
    // Make player lose life and push back from impact
    else if (col.gameObject.tag == "player1" || col.gameObject.tag == "player2") {
      this.explode ();
      col.gameObject.GetComponent<Player> ().loseLife ();
      Camera.main.GetComponent<CameraManager> ().setDuration (0.3f);
      Destroy (gameObject);
    }
    else if (col.gameObject.tag == "ground") {
      //this.continueTranslate = false;
    }
    else {
    }
  }


  private void explode() {
    Instantiate (shrapnel, transform.position + transform.forward*8, shrapnel.transform.rotation);
    Instantiate (shrapnel, transform.position - transform.forward*8, shrapnel.transform.rotation);
    Instantiate (shrapnel, transform.position + transform.right*8, shrapnel.transform.rotation);
    Instantiate (shrapnel, transform.position - transform.right*8, shrapnel.transform.rotation);
  }

	// Update is called once per frame
	void Update () {
    expireTime -= Time.deltaTime;
    if (continueTranslate) {
      transform.position += this.direction * Time.deltaTime * this.speed;
      this.lastPosition = transform.position;
    } else {
      transform.position = this.lastPosition;
    }
    if (expireTime <= 0) {
      if (!this.exploded) {
        explode();
        this.exploded = true;
      }
      Destroy (gameObject);
    }
	}
}
