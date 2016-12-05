using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
  // Player attributes
  private int lives;
  private float moveSpeed = 20.0f;
  private PowerUps currentPowerup;
  private Actives currentActive;

  // Active/Power up attributes
  private int activeExpireTime;
  private int amountOfPowerUpItem;

  // Power up prefabs
  public NinjaStar ninjaStar;

  // Mostly for readability
  // Power Ups that a Player can collect in game
  private enum PowerUps {NONE, NINJA_STAR};
  // Actives that a Player can collect in game
  private enum Actives {NONE, SUSHI, BOOST};

  // Set default attributes
  void Start() {
    this.lives = 3;
    this.activeExpireTime = -1;
    this.amountOfPowerUpItem = -1;
    this.currentPowerup = PowerUps.NONE;
    this.currentActive = Actives.NONE;
  }

  // Have player gain life when collecting sushi tokens
  public void gainLife() {
    this.lives += 1;
  }

  // Have player lose life when hurt
  public void loseLife() {
    this.lives -= 1;
  }

  // Set player's move speed
  public void setMoveSpeed(float newMoveSpeed) {
    this.moveSpeed = newMoveSpeed;
  }

  // Get player's move speed
  public float getMoveSpeed() {
    return this.moveSpeed;
  }

  // Set power up to player
  public void setPowerUp(int powerUp) {
    this.currentPowerup = (PowerUps)powerUp;
    switch (this.currentPowerup) {
    case PowerUps.NINJA_STAR:
      this.amountOfPowerUpItem = 3;
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
    }
  }

  // Check if player is dead
  public bool isDead() {
    return this.lives == 0;
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
      Instantiate (ninjaStar, transform.position + transform.forward*2, transform.rotation);
      this.amountOfPowerUpItem--;
      break;
    }
  }

  // See if player wants to use their powerup or item
  public void checkForPlayerInput() {
    if (transform.tag == "player1") {
      if (Input.GetKeyDown (KeyCode.C)) {
        usePowerUp(currentPowerup);
      }
    }
    else if (transform.tag == "player2") {
      if (Input.GetKeyDown (KeyCode.Slash)) {
        usePowerUp(currentPowerup);
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
        checkForPlayerInput ();
        if (this.amountOfPowerUpItem == 0) {
          this.currentPowerup = PowerUps.NONE;
        }
      }
    }
    else {
      // Player is dead
      Destroy(gameObject);
    }
  }
}