using UnityEngine;
using System.Collections.Generic;

public class DisplayUI : MonoBehaviour {
  // Icon Prefabs
  public GameObject heartPrefab;
  public GameObject p1DisplayPrefab;
  public GameObject p2DisplayPrefab;

  // UI attributes
  private GameObject p1Header;
  private GameObject p2Header;
  private List<GameObject> playerOneLives;
  private List<GameObject> playerTwoLives;
  private int initialPlayerLives;
  private Vector3 yShift;

  // Use this for initialization
  void Start () {
    // Initial setting of attributes
    this.initialPlayerLives = 5;
    this.playerOneLives = new List<GameObject>();
    this.playerTwoLives = new List<GameObject>();

    // Instantiate display UI
    instantiatePlayerHeaders();
    instantiatePlayerLifeIcons();
  }


  // Instantiate player header titles
  private void instantiatePlayerHeaders() {
    p1Header = Instantiate(p1DisplayPrefab, new Vector3(21.0f, 35.0f, 21.5f), p1DisplayPrefab.transform.rotation) as GameObject;
    p2Header = Instantiate(p2DisplayPrefab, new Vector3(21.0f, 35, -16.2f), p2DisplayPrefab.transform.rotation) as GameObject;
  }


  // Instantiate heart icons for both players
  private void instantiatePlayerLifeIcons() {
    for (int i = 0; i < this.initialPlayerLives; i++) {
      this.playerOneLives.Add(Instantiate(heartPrefab, new Vector3(19.5f, 35.0f, i + 17.0f), heartPrefab.transform.rotation) as GameObject);
      this.playerTwoLives.Add(Instantiate(heartPrefab, new Vector3(19.5f, 35.0f, i - 21.2f), heartPrefab.transform.rotation) as GameObject);
    }
  }


  // Update position of all UI elements
  public void updateUIPositions(float yVal) {
    yShift = new Vector3 (0, yVal, 0);
    p1Header.transform.Translate(yShift);
    p2Header.transform.Translate(yShift);
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
}
