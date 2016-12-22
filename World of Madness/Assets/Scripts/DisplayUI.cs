using UnityEngine;
using System.Collections.Generic;

public class DisplayUI : MonoBehaviour {
  // Icon Prefabs
  public GameObject heartPrefab;
  public GameObject nullPrefab;
  public GameObject bombIconPrefab;
  public GameObject ninjaStarIconPrefab;
  public GameObject p1DisplayPrefab;
  public GameObject p2DisplayPrefab;
  public GameObject freezePrefab;

  // UI attributes
  private GameObject p1Header;
  private GameObject p2Header;
  private List<GameObject> playerOneLives;
  private List<GameObject> playerTwoLives;
  private GameObject p1WepDisplay;
  private GameObject p2WepDisplay;
  private int initialPlayerLives;
  private bool isFrozen;
  private GameObject freezeEffect;
  private float screenDuration;
  private Vector3 yShift;


  public void loadDisplayUI() {
    // Instantiate display UI
    this.initialPlayerLives = 5;
    this.screenDuration = -1.0f;
    this.playerOneLives = new List<GameObject>();
    this.playerTwoLives = new List<GameObject>();
    instantiatePlayerHeaders();
    instantiatePlayerLifeIcons();
    instantiatePlayerWeaponIcons();
  }


  // Instantiate player header titles
  private void instantiatePlayerHeaders() {
    p1Header = Instantiate(p1DisplayPrefab, new Vector3(27.0f, 35.0f, 21.5f), p1DisplayPrefab.transform.rotation) as GameObject;
    p2Header = Instantiate(p2DisplayPrefab, new Vector3(27.0f, 35.0f, -16.2f), p2DisplayPrefab.transform.rotation) as GameObject;
  }


  // Instantiate heart icons for both players
  private void instantiatePlayerLifeIcons() {
    for (int i = 0; i < this.initialPlayerLives; i++) {
      this.playerOneLives.Add(Instantiate(heartPrefab, new Vector3(25.5f, 35.0f, i + 17.0f), heartPrefab.transform.rotation) as GameObject);
      this.playerTwoLives.Add(Instantiate(heartPrefab, new Vector3(25.5f, 35.0f, i - 21.2f), heartPrefab.transform.rotation) as GameObject);
    }
  }


  // Instantiate weapon icons
  private void instantiatePlayerWeaponIcons() {
    p1WepDisplay = Instantiate(nullPrefab, new Vector3(24.2f, 35.0f, 20.9f), nullPrefab.transform.rotation) as GameObject;
    p2WepDisplay = Instantiate(nullPrefab, new Vector3(24.2f, 35.0f, -21.0f), nullPrefab.transform.rotation) as GameObject;
  }


  // Change weapon display for player
  public void changeWeaponDisplay(string player, string weapon) {
    GameObject temp = nullPrefab;
    if (weapon == "NINJASTAR") {
      temp = ninjaStarIconPrefab; 
    }
    else if(weapon == "BOMB") {
      temp = bombIconPrefab;
    }
    if (player == "player1") {
      GameObject wepDisplayTemp = this.p1WepDisplay;
      Destroy(p1WepDisplay);
      this.p1WepDisplay = Instantiate(temp, wepDisplayTemp.transform.position, temp.transform.rotation) as GameObject;
    }
    else {
      GameObject wepDisplayTemp = this.p2WepDisplay;
      Destroy(p2WepDisplay);
      this.p2WepDisplay = Instantiate(temp, wepDisplayTemp.transform.position, temp.transform.rotation) as GameObject;
    }
  }


  // Update position of all UI elements
  public void updateUIPositions(float yVal) {
    yShift = new Vector3 (0, yVal, 0);
    p1Header.transform.Translate(yShift);
    p2Header.transform.Translate(yShift);
    p1WepDisplay.transform.Translate(yShift);
    p2WepDisplay.transform.Translate(yShift);
    for (int i = 0; i < this.playerOneLives.Count; i++) {
      this.playerOneLives[i].transform.Translate(yShift);
    }
    for (int i = 0; i < this.playerTwoLives.Count; i++) {
      this.playerTwoLives[i].transform.Translate(yShift);
    }
  }


  // Update display when player loses life
  public void losePlayerLife(string player) {
    if (player == "player1") {
      Destroy(playerOneLives[0]);
      playerOneLives.Remove(playerOneLives[0]);
    }
    else if (player == "player2"){
      Destroy(playerTwoLives[playerTwoLives.Count - 1]);
      playerTwoLives.Remove(playerTwoLives[playerTwoLives.Count-1]);
    }
  }


  // Update display when player gains life
  public void gainPlayerLife(string player) {
    if (player == "player1") {
      this.playerOneLives.Insert(0, Instantiate(heartPrefab, new Vector3(this.playerOneLives[0].transform.position.x, 35.0f, this.playerOneLives[0].transform.position.z - 1.0f), heartPrefab.transform.rotation) as GameObject);
    }
    else if (player == "player2"){
      int p2LifeSize = playerTwoLives.Count;
      this.playerTwoLives.Add(Instantiate(heartPrefab, new Vector3(this.playerTwoLives[0].transform.position.x, 35.0f, p2LifeSize - 21.2f), heartPrefab.transform.rotation) as GameObject);
    }
  }


  // Apply freeze effect
  public void applyFreezeEffect(float duration) {
    this.freezeEffect = Instantiate(freezePrefab, new Vector3(0,0,0), freezePrefab.transform.rotation) as GameObject;
    this.screenDuration = duration;
    this.isFrozen = true;
  }

  void Update() {
    if(this.isFrozen) {
      freezeEffect.transform.position = Camera.main.transform.position;
      freezeEffect.transform.position -= new Vector3 (0, 3.0f, 0);
      screenDuration -= Time.deltaTime;
      if (screenDuration <= 0) {
        this.isFrozen = false;
        Destroy (this.freezeEffect);
      }
    }
  }
}
