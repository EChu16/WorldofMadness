using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
  // Player attributes
  private int lives;
  private float moveSpeed = 25.0f;
  private float hitMarkerExpireTime;
  private Color originalPlayerColor;
  private PowerUps currentPowerup;
  private Actives currentActive;

  // Active/Power up attributes
  private float activeExpireTime;
  private float powerUpDelayTime;
  private float currentDelayTime;

  // Power up prefabs
  public NinjaStar ninjaStar;
  public Bomb bomb;

  // Mostly for readability
  // Power Ups that a Player can collect in game
  private enum PowerUps {NONE, NINJA_STAR, BOMB};
  // Actives that a Player can collect in game
  private enum Actives {NONE, SUSHI, BOOST, GAME_FREEZE};

  // Set default attributes
  void Start() {
    this.lives = 3;
    this.activeExpireTime = -1;
    this.powerUpDelayTime = -1;
    this.currentDelayTime = -1;
    this.hitMarkerExpireTime = -1;
    this.currentPowerup = PowerUps.NONE;
    this.currentActive = Actives.NONE;
    this.originalPlayerColor = transform.Find("pCube1").GetComponent<Renderer>().material.color;
    Debug.Log (this.originalPlayerColor);
  }

  // Have player gain life when collecting sushi tokens
  public void gainLife() {
    this.lives += 1;
  }

  // Have player lose life when hurt
  public void loseLife() {
    this.lives -= 1;
    this.hitMarkerExpireTime = 0.1f;
    showPlayerHitMarker();
  }

  // Set player's move speed
  public void setMoveSpeed(float newMoveSpeed) {
    this.moveSpeed = newMoveSpeed;
  }

  // Get player's move speed
  public float getMoveSpeed() {
    return this.moveSpeed;
  }

  // Decreases a current power up delay time so that it can hit <= 0
  private void decreasePowerUpDelayTime() {
    this.currentDelayTime -= Time.deltaTime;
  }

  // Set power up to player
  public void setPowerUp(int powerUp) {
    this.currentPowerup = (PowerUps)powerUp;
    switch (this.currentPowerup) {
    case PowerUps.NINJA_STAR:
      this.powerUpDelayTime = 1.0f;
      break;
    case PowerUps.BOMB:
      this.powerUpDelayTime = 2.5f;
      break;
    }
  }

  // Do active effect on player
  public void setActive(int active) {
    this.currentActive = (Actives)active;
    switch (this.currentActive) {
    case Actives.SUSHI:
      gainLife ();
      this.currentActive = Actives.NONE;
      break;
    case Actives.BOOST:
      this.activeExpireTime = 3.0f;
      this.setMoveSpeed (this.getMoveSpeed () + 5.0f);
      break;
    case Actives.GAME_FREEZE:
      this.activeExpireTime = 10.0f;
      GameObject.FindWithTag("GameController").GetComponent<GameManager>().toggleCameraMovement();
      break;
    }
  }

  // Check if player is dead
  public bool isDead() {
    return this.lives == 0;
  }

  // Check if player has a current active
  private bool hasActive() {
    return this.currentActive != Actives.NONE;
  }

  // Decreases a current active time so that it can hit <= 0
  private void decreaseActiveTime() {
    this.activeExpireTime -= Time.deltaTime;
  }

  // Resets player to their original state and removes the active
  private void stopActive() {
    switch (this.currentActive) {
    case Actives.BOOST:
      this.setMoveSpeed (this.getMoveSpeed () - 5.0f);
      break;
    case Actives.GAME_FREEZE:
      GameObject.FindWithTag("GameController").GetComponent<GameManager>().toggleCameraMovement();
      break;
    };
    this.currentActive = Actives.NONE;
  }

  // Check if player has an existing powerup active
  private bool hasPowerUp() {
    return this.currentPowerup != PowerUps.NONE;
  }

  // Mostly for debugging
  public int getPowerUp() {
    return (int)this.currentPowerup;
  }

  // Use power up
  private void usePowerUp(PowerUps powerUp) {
    switch (powerUp) {
    case PowerUps.NINJA_STAR:
      // Create Ninja Star and shoot it
      ninjaStar.direction = transform.forward;
      Instantiate (ninjaStar, transform.position + transform.forward*5, ninjaStar.transform.rotation);
      break;
    case PowerUps.BOMB:
      bomb.direction = transform.forward;
      Instantiate (bomb, transform.position + transform.forward*5, bomb.transform.rotation);
      break;
    }
    this.currentDelayTime = this.powerUpDelayTime;
  }

  // See if player wants to use their powerup or item
  public void checkForPlayerInput() {
    if (transform.tag == "player1") {
      if (Input.GetKeyDown (KeyCode.C)) {
        if (this.currentDelayTime <= 0) {
          usePowerUp (this.currentPowerup);
        }
      }
    }
    else if (transform.tag == "player2") {
      if (Input.GetKeyDown (KeyCode.Slash)) {
        if (this.currentDelayTime <= 0) {
          usePowerUp (this.currentPowerup);
        }
      }
    }
  }

  // Kill the player instantly
  private void die() {
    this.lives = 0;
  }

  // Check if player fell off the map
  private bool playerFellOff() {
    return transform.position.y < -5.0f;
  }

  // Flash player color to show it was hit
  private void showPlayerHitMarker() {
    transform.Find("pCube1").GetComponent<Renderer>().material.color = Color.white;
    decreaseHitMarkerExpireTime();
  }

  // Decrease hit marker expire time
  private void decreaseHitMarkerExpireTime() {
    this.hitMarkerExpireTime -= Time.deltaTime;
  }

  // Update game per frame while checking for conditions
  void Update() {
    // Check if player is dead
    if (!isDead()) {
      // Check if player happened to fall off the map
      if (playerFellOff ()) {
        die();
      }

      // Check for player input only if player has a powerup
      if (hasPowerUp ()) {
        if (this.currentDelayTime >= 0) {
          decreasePowerUpDelayTime();
        }
        else {
          checkForPlayerInput();
        }
      }

      // Check if player has an active
      if (hasActive ()) {
        if (this.activeExpireTime >= 0) {
          decreaseActiveTime ();
        }
        // Stop active when it expires
        else {
          stopActive();
        }
      }

      // Check if player was hit
      if (this.hitMarkerExpireTime >= 0) {
        Debug.Log ("hit color");
        showPlayerHitMarker();
      }
      else {
        transform.Find("pCube1").GetComponent<Renderer>().material.color = this.originalPlayerColor;
        Debug.Log ("original color");
      }
    }
    else {
      // Player is dead
      Destroy(gameObject);
    }
  }
}