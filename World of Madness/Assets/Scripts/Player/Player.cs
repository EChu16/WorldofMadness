using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
  // Player attributes
  private int lives;
  private PowerUps currentPowerup;

  // Power up attributes
  private int powerUpExpireTime;
  private int amountOfPowerUpItem;

  // Power up prefabs
  public NinjaStar ninjaStar;

  // Readability - Power Ups that a Player can collect in game
  private enum PowerUps {NONE, NINJA_STAR, ONIGIRI, BOOST};

  // Set default attributes
  void Start() {
    this.lives = 3;
    this.powerUpExpireTime = -1;
    this.amountOfPowerUpItem = -1;
    this.currentPowerup = PowerUps.NONE;
  }

  // Have player lose life when hurt
  public void loseLife() {
    this.lives -= 1;
  }

  // Set power up to player
  public void setPowerUp(int powerUp, int expireTime = -1, int amountOfItem = -1) {
    this.currentPowerup = (PowerUps)powerUp;
    this.powerUpExpireTime = expireTime;
    this.amountOfPowerUpItem = amountOfItem;
  }

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

  private void die() {
    this.lives = 0;
  }

  private bool playerFellOff() {
    return transform.position.y < -5.0f;
  }

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
        if (this.amountOfPowerUpItem == 0 || this.powerUpExpireTime == 0) {
          this.currentPowerup = PowerUps.NONE;
        }
      }
    } else {
      Destroy(gameObject);
    }
  }
}