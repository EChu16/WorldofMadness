using UnityEngine;
using System.Collections;

public class NinjaStar : MonoBehaviour {
  public Vector3 direction;
  private Vector3 lastPosition;
  private float speed;
  private float expireTime;
  private bool continueTranslate;
  private bool hasAlreadyCollided;

	// Use this for initialization
	void Start () {
    this.speed = 35.0f;
    this.expireTime = 5.0f;
    this.continueTranslate = true;
    this.hasAlreadyCollided = false;
	}
	
  void OnCollisionEnter(Collision col) {
    // If ninja star collides into a wall, have it stay stuck there like a real ninja star
    if (col.gameObject.tag == "wall") {
      this.hasAlreadyCollided = true;
      this.continueTranslate = false;
    } 
    // If ninja star collides into a player, make sure it hasn't collided yet
    else if (col.gameObject.tag == "player1" || col.gameObject.tag == "player2" && !this.hasAlreadyCollided) {
      col.gameObject.GetComponent<Player>().loseLife();
      Destroy(gameObject);
    }
  }

	// Update is called once per frame
	void Update () {
    // Don't let ninja stars sit there forever
    expireTime -= Time.deltaTime;
    if (continueTranslate) {
      transform.position += this.direction * Time.deltaTime * this.speed;
      this.lastPosition = transform.position;
    } else {
      transform.position = this.lastPosition;
    }
    if (expireTime <= 0) { Destroy (gameObject); }
	}
}
