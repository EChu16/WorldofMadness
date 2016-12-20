using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

  private Transform myTransform;
  private float moveSpeed;
  private bool isFrozen;
  private float freezeExpireTime;

  // Use this for initialization
  void Start () {
    myTransform = GetComponent<Transform>();
    this.isFrozen = false;
  }


  // Toggle whether or not a player is frozen
  public void toggleFrozen() {
    this.isFrozen = !this.isFrozen;
  }


  // Set freeze token expire time
  public void setFreezeExpireTime(float expireTime) {
    this.freezeExpireTime = expireTime;
  }


  // Update is called once per frame
  void Update () {
    // Check move speed per frame
    moveSpeed = GetComponent<Player>().getMoveSpeed ();
    // Check if player freeze token is active
    if (!this.isFrozen) {
      if (myTransform.tag == "player1") {
        float p1MoveX = Input.GetAxisRaw ("Horizontal");
        float p2MoveY = Input.GetAxisRaw ("Vertical");
        Vector3 movement = new Vector3 (p1MoveX, 0.0f, p2MoveY);
        movement = Quaternion.Euler (0, 90, 0) * movement;
        var rotation = Quaternion.LookRotation (movement);
        if (p1MoveX != 0 || p2MoveY != 0) {
          myTransform.rotation = Quaternion.Slerp (myTransform.rotation, rotation, Time.deltaTime * moveSpeed);
        }
        myTransform.Translate (movement * moveSpeed * Time.deltaTime, Space.World);
      }
      else if (myTransform.tag == "player2") {
        float p2MoveX = Input.GetAxisRaw ("Horizontal2");
        float p2MoveY = Input.GetAxisRaw ("Vertical2");
        Vector3 movement = new Vector3 (p2MoveX, 0.0f, p2MoveY);
        movement = Quaternion.Euler (0, 90, 0) * movement;
        var rotation = Quaternion.LookRotation (movement);
        if (p2MoveX != 0 || p2MoveY != 0) {
          myTransform.rotation = Quaternion.Slerp (myTransform.rotation, rotation, Time.deltaTime * moveSpeed);
        }
        myTransform.Translate (movement * moveSpeed * Time.deltaTime, Space.World);
      }
    }
    else {
      this.freezeExpireTime -= Time.deltaTime;
      if (this.freezeExpireTime <= 0) {
        this.toggleFrozen();
      }
    }
  }
}
