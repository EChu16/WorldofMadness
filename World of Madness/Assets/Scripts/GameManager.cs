using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour {
  // Default GameObject prefab initializations

  //audio clips
  public AudioClip menubgm;
  public AudioClip gamebgm;
  public AudioClip losebgm;
  public AudioClip star;
  public AudioClip bomb;

  // Core objects
  public Camera displayCam;
  public GameObject plane;
  public GameObject wall;
  public GameObject player1prefab;
  public GameObject player2prefab;
  public GameObject titleScreenPrefab;
  public GameObject p1losePrefab;
  public GameObject p2losePrefab;

  // Power ups
  public GameObject ninjaStarPrefab;
  public GameObject bombPrefab;
  // Actives
  public GameObject sushiPrefab;
  public GameObject boostPrefab;
  public GameObject gameFreezePrefab;
  //Traps
  public GameObject sandpitPrefab;
  public GameObject spikePrefab;
  public GameObject playerFreezePrefab;
  public GameObject lavaPrefab;

  // GameObjects after instantiating prefabs
  private GameObject player1;
  private GameObject player2;
  private List<GameObject> allPlatformRows;
  private Dictionary<int, List<GameObject>> allObjects = new Dictionary<int, List<GameObject>>();

  // Camera attributes
  public bool cameraCanMove = true;
  private float cameraMoveSpeed = 15.0f;
  private float cameraBottomFOV; // Field of View
  private float originalYPos;
  private Vector3 lastCameraPosition;
  private Vector3 beforeShakeCameraPosition;
  private Vector3 initialStartOfGameCamPos;

  // Map attributes
  private Dictionary<string, int[,]> gameMaps = new Dictionary<string, int[,]>();
  private int MAP_CHUNK_SIZE = 10;
  private int MAX_PLATFORMS_PREMADE = 2;
  private int MAP_NUM_COLS = 10;
  private int currentPlatform;
  private int currentWorldRow;
  private int lastWorldRowPosition;

  // Mainly for readability - enum wrappers for different items
  private enum State{TITLESCREEN, GAME, LOSE};
  private enum Objects{PLANE, WALL, PLAYER_1, PLAYER_2, POWER_UP, ACTIVE, PASSABLE_TRAP, DEATH_TRAP};
  private enum PowerUps{NINJA_STAR, BOMB};
  private enum Actives{SUSHI, BOOST, GAME_FREEZE};
  private enum PassableTraps{SANDPIT, PLAYER_FREEZE};
  private enum DeathTraps{SPIKES, LAVA};

  private State currentState;
  private bool titleScreenLoaded;
  private bool loseScreenLoaded;
  private GameObject titleScreenDisplay;
  private GameObject loseScreenP1;
  private GameObject loseScreenP2;
  private string deadPlayer;


  // At the beginning of initialization of game
  void Start () {
    this.currentState = State.TITLESCREEN;
    this.titleScreenLoaded = false;
    this.loseScreenLoaded = false;
    this.initialStartOfGameCamPos = Camera.main.transform.position;
    this.deadPlayer = "";
  }


  // Fast way to convert a type 'char' to type 'int'
  public static int convertCharToIntFast(char val) {
    return (val - 48);
  }


  // Load chunk of platform from text file
  private int[,] loadMapChunk(StreamReader mapReader) {
    int[,] mapChunk = new int[MAP_CHUNK_SIZE,10];
    for (int row = 0; row < MAP_CHUNK_SIZE; ++row) {
      string line = mapReader.ReadLine ();
      for (int column = 0; column < line.Length; ++column) {
        mapChunk[row,column] = convertCharToIntFast(line[column]);
      }
    }
    return mapChunk;
  }


  // Load all platforms from text file
  private void loadMapPlatformsFromFile(string fileName) {
    string row;
    StreamReader mapReader = new StreamReader(Application.dataPath + "/" + fileName, Encoding.Default);
    while ((row = mapReader.ReadLine()) != null) {
      gameMaps[row] = loadMapChunk(mapReader);
    }
  }


  // Create a random power up
  private void instantiateRandomPowerUp(int xVal, int zVal) {
    PowerUps generatePowerUp = (PowerUps)(Random.Range(0, System.Enum.GetValues (typeof(PowerUps)).Length));
    switch (generatePowerUp) {
    case PowerUps.NINJA_STAR:
      allObjects[xVal].Add(Instantiate(ninjaStarPrefab, new Vector3(xVal * 10, 5, (zVal * 10) - 45), ninjaStarPrefab.transform.rotation) as GameObject);
      break;
    case PowerUps.BOMB:
      allObjects [xVal].Add (Instantiate (bombPrefab, new Vector3 (xVal * 10, 5, (zVal * 10) - 45), bombPrefab.transform.rotation) as GameObject);
      break;
    default:
      // Should never hit here
      Debug.Log ("Power up not found");
      break;
    }
  }


  // Create a random active
  private void instantiateRandomActive(int xVal, int zVal) {
    Actives generateActive = (Actives)(Random.Range(0, System.Enum.GetValues (typeof(Actives)).Length));
    switch (generateActive) {
    case Actives.SUSHI:
      allObjects[xVal].Add(Instantiate(sushiPrefab, new Vector3(xVal * 10, 5, (zVal * 10) - 45), sushiPrefab.transform.rotation) as GameObject);
      break;
    case Actives.BOOST:
      allObjects [xVal].Add (Instantiate (boostPrefab, new Vector3 (xVal * 10, 5, (zVal * 10) - 45), boostPrefab.transform.rotation) as GameObject);
      break;
    case Actives.GAME_FREEZE:
      allObjects [xVal].Add (Instantiate (gameFreezePrefab, new Vector3 (xVal * 10, 5, (zVal * 10) - 45), gameFreezePrefab.transform.rotation) as GameObject);
      break;
    default:
      // Should never hit here
      Debug.Log ("Active token not found");
      break;
    }
  }


  // Create a random Trap
  private void instantiateRandomPassableTrap(int xVal, int zVal) {
    PassableTraps generatePassableTrap = (PassableTraps)(Random.Range(0, System.Enum.GetValues (typeof(PassableTraps)).Length));
    switch (generatePassableTrap) {
    case PassableTraps.SANDPIT:
      allObjects[xVal].Add(Instantiate(sandpitPrefab, new Vector3(xVal * 10, 0.5f, (zVal * 10) - 45), sandpitPrefab.transform.rotation) as GameObject);
      break;
    case PassableTraps.PLAYER_FREEZE:
      allObjects[xVal].Add(Instantiate(playerFreezePrefab, new Vector3(xVal * 10, 0.5f, (zVal * 10) - 45), playerFreezePrefab.transform.rotation) as GameObject);
      break;
    default:
      // Should never hit here
      Debug.Log ("Passable trap object not found");
      break;
    }
  }


  // Create a random Trap
  private void instantiateRandomDeathTrap(int xVal, int zVal) {
    DeathTraps generateDeathTrap = (DeathTraps)(Random.Range(0, System.Enum.GetValues (typeof(DeathTraps)).Length));
    switch (generateDeathTrap) {
    case DeathTraps.SPIKES:
      allObjects[xVal].Add(Instantiate(spikePrefab, new Vector3(xVal * 10, 0.5f, (zVal * 10) - 45), spikePrefab.transform.rotation) as GameObject);
      break;
    case DeathTraps.LAVA:
      allObjects[xVal].Add(Instantiate(lavaPrefab, new Vector3(xVal * 10, 0.5f, (zVal * 10) - 45), lavaPrefab.transform.rotation) as GameObject);
      break;
    default:
      // Should never hit here
      Debug.Log ("Death trap object not found");
      break;
    }
  }


  // Create type of object from its type of value
  private void instantiateObject(Objects obj, int xVal, int zVal) {
    switch (obj) {
    case Objects.WALL:
      //allObjects[xVal].Add(Instantiate(wall, new Vector3((xVal * 10) - 4.0f, 0.0f, (zVal * 10) - 48.8f), Quaternion.identity) as GameObject);
      allObjects[xVal].Add(Instantiate(wall, new Vector3(xVal * 10, 5, (zVal * 10) - 45), Quaternion.identity) as GameObject);
      break;
    case Objects.PLAYER_1:
      player1 = Instantiate(player1prefab, new Vector3(xVal * 10, 0, (zVal * 10) - 45), player1prefab.transform.rotation) as GameObject;
      break;
    case Objects.PLAYER_2:
      player2 = Instantiate(player2prefab, new Vector3(xVal * 10, 0, (zVal * 10) - 45), player2prefab.transform.rotation) as GameObject;
      break;
    case Objects.POWER_UP:
      instantiateRandomPowerUp(xVal, zVal);
      break;
    case Objects.ACTIVE:
      instantiateRandomActive(xVal, zVal);
      break;
    case Objects.PASSABLE_TRAP:
      instantiateRandomPassableTrap(xVal, zVal);
      break;
    case Objects.DEATH_TRAP:
      instantiateRandomDeathTrap(xVal, zVal);
      break;
    }
  }


  // Generate platform
  private void generatePlatform(int platform, int specificPlatform=-1) {
    string key = (platform).ToString () + "-";
    if (specificPlatform == -1) { key += (Random.Range (1, MAX_PLATFORMS_PREMADE + 1).ToString ()); }
    else { key += (specificPlatform).ToString (); }
    currentPlatform = platform;

    int[,] genPlatform = gameMaps[key];
    for (int rowIdx = 0; rowIdx < genPlatform.GetLength(0); rowIdx++) {
      allObjects [currentWorldRow] = new List<GameObject>();
      allPlatformRows.Add(Instantiate(plane, new Vector3(currentWorldRow*10.0f, 0,0), Quaternion.identity) as GameObject);
      for (int colIdx = 0; colIdx < MAP_NUM_COLS; colIdx++) {
        instantiateObject((Objects)genPlatform[rowIdx, colIdx],currentWorldRow, colIdx);
      }
      currentWorldRow++;
    }
  }


  // Determine if player 1's x position is greater than player 2
  private bool Player1isAhead() {
    return player1.transform.position.x > player2.transform.position.x;
  }


  // Adjust camera to center on player furthest ahead
  private void adjustCameraView() {
    float newXVal = Camera.main.transform.position.x;
    if (Player1isAhead()) {
      if (Camera.main.transform.position.x != player1.transform.position.x) {
        newXVal = (player1.transform.position + transform.forward * Time.deltaTime * cameraMoveSpeed).x;
      }
    }
    else if (Camera.main.transform.position.x != player2.transform.position.x) {
      newXVal = (player2.transform.position + transform.forward * Time.deltaTime * cameraMoveSpeed).x;
    }
    Camera.main.transform.position = new Vector3(newXVal, Camera.main.transform.position.y, Camera.main.transform.position.z);
    displayCam.transform.position = new Vector3(newXVal, displayCam.transform.position.y, displayCam.transform.position.z);

    displayCam.GetComponent<DisplayUI>().updateUIPositions(newXVal - lastCameraPosition.x);
  }


  // Create or destroy platform as needed
  private void alterMapAsNeeded() {
    cameraBottomFOV = Camera.main.transform.position.x - (Camera.main.fieldOfView * 0.5f);

    // Create next platform if player nears the top of current generated platform
    if((currentWorldRow * 10) < player1.transform.position.x + 50 || (currentWorldRow * 10) < player2.transform.position.x + 50) {
      if (currentPlatform + 1 > MAX_PLATFORMS_PREMADE) { generatePlatform (1); }
      else { generatePlatform (currentPlatform + 1); }
    }

    // Destroy platform rows if player doesn't see that platform anymore
    if(cameraBottomFOV > ((lastWorldRowPosition * 10))) {
      // Prevent removing a plane more than once
      if (allPlatformRows[0].transform.position.x == (lastWorldRowPosition * 10.0f)) {
        Destroy (allPlatformRows [0]);
        // Remove from list
        allPlatformRows.RemoveAt (0);
        // Destroy all objects in that platform row
        foreach(var game_object in allObjects[lastWorldRowPosition]) {
          Destroy(game_object);
        }
        // Clean up Dictionary
        allObjects.Remove(lastWorldRowPosition);
        lastWorldRowPosition++;
      }
    }
  }


  // Don't allow camera to readjust (Freeze effect)
  public void toggleCameraMovement() {
    this.cameraCanMove = !this.cameraCanMove;
  }


  // Check if game is over
  public bool isGameOver() {
    return player1.gameObject == null || player2.gameObject == null;
  }


  private void loadTitleScreen() {
    titleScreenDisplay = Instantiate(titleScreenPrefab, new Vector3(15, 0, 0), titleScreenPrefab.transform.rotation) as GameObject;
    this.titleScreenLoaded = true;

    GetComponent<AudioSource>().clip = menubgm;
    GetComponent<AudioSource>().Play();
    }


  private void startGameWhenPlayerReady() {
    if (Input.GetKeyDown (KeyCode.Space)) {
      this.lastWorldRowPosition = 0;
      this.currentWorldRow = 0;
      this.originalYPos = Camera.main.transform.position.y;
      allPlatformRows = new List<GameObject>();
      loadMapPlatformsFromFile("Maps/map_layout.txt");
      generatePlatform(0, 1); // Initial platform begins at 0
      displayCam.GetComponent<DisplayUI>().loadDisplayUI();
      Destroy (titleScreenDisplay);
      this.currentState = State.GAME;

      GetComponent<AudioSource>().clip = gamebgm;
      GetComponent<AudioSource>().Play();
    }
  }


  private void displayLoseScreen(string deadPlayer) {
    if (deadPlayer == "PLAYER1") {
      loseScreenP1 = Instantiate(p1losePrefab, new Vector3(15, 0, 0), p1losePrefab.transform.rotation) as GameObject;
    }
    else {
      loseScreenP2 = Instantiate(p2losePrefab, new Vector3(15, 0, 0), p2losePrefab.transform.rotation) as GameObject;
    }
    this.loseScreenLoaded = true;

    GetComponent<AudioSource>().clip = losebgm;
    GetComponent<AudioSource>().Play();
    }


  private void waitForPlayerChoice() {
    // Load Title Screen
    if (Input.GetKeyDown (KeyCode.T)) {
      this.titleScreenLoaded = false;
      if (loseScreenP1 != null) { Destroy(loseScreenP1); }
      else { Destroy(loseScreenP2); }
      this.loseScreenLoaded = false;
      this.deadPlayer = "";
      this.currentState = State.TITLESCREEN;
    }
    else if (Input.GetKeyDown (KeyCode.R)) {
      // Restart game
      this.lastWorldRowPosition = 0;
      this.currentWorldRow = 0;
      this.originalYPos = Camera.main.transform.position.y;
      allPlatformRows = new List<GameObject>();
      generatePlatform(0, 1); // Initial platform begins at 0
      displayCam.GetComponent<DisplayUI>().loadDisplayUI();
      if (loseScreenP1 != null) { Destroy(loseScreenP1); }
      else { Destroy(loseScreenP2); }
      this.loseScreenLoaded = false;
      this.deadPlayer = "";
      this.currentState = State.GAME;

      GetComponent<AudioSource>().clip = gamebgm;
      GetComponent<AudioSource>().Play();
    }
  }


  private void clearGame() {
    this.lastWorldRowPosition = 0;
    this.cameraCanMove = true;
    GameObject[] allGameObjects = (FindObjectsOfType<GameObject>() as GameObject[]);
    foreach (GameObject gameObj in allGameObjects) {
      if (gameObj.activeInHierarchy) {
        if (gameObj.transform.tag == "MainCamera" || gameObj.transform.tag == "displaycam" || gameObj.transform.tag == "light" || gameObj.transform.tag == "GameController" || gameObj.transform.tag == "titlescreen" || gameObj.transform.tag == "losescreen") {
          // Keep Alive
        }
        else {
          Destroy (gameObj);
        }
      }
    }
    // Reset Camera positions
    Camera.main.transform.position = this.initialStartOfGameCamPos;
    displayCam.transform.position = this.initialStartOfGameCamPos;
  }


  // Update game per frame
  void Update () {
    if (currentState == State.TITLESCREEN) {
      if (!this.titleScreenLoaded) {
        loadTitleScreen();
      }
      startGameWhenPlayerReady();
    }
    else if (currentState == State.GAME) {
      if (!isGameOver ()) {
        if (this.cameraCanMove) {
          lastCameraPosition = Camera.main.transform.position;
          adjustCameraView ();
        }
        else {
          float xShift = lastCameraPosition.x - Camera.main.transform.position.x;
          displayCam.GetComponent<DisplayUI> ().updateUIPositions (xShift);
          Camera.main.transform.position = lastCameraPosition;
          displayCam.transform.position = lastCameraPosition;
        }
        alterMapAsNeeded ();
      }
      else {
        currentState = State.LOSE;
        if (player1.gameObject == null) {
          this.deadPlayer = "PLAYER1";
        }
        else {
          this.deadPlayer = "PLAYER2";
        }
        clearGame ();
      }
      beforeShakeCameraPosition = Camera.main.transform.position;
      Camera.main.transform.position += Camera.main.GetComponent<CameraManager> ().shakeMod;
      float x_shift = Camera.main.transform.position.x - beforeShakeCameraPosition.x;
      displayCam.GetComponent<DisplayUI> ().updateUIPositions (x_shift);
      Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, this.originalYPos, Camera.main.transform.position.z);
    } 
    else if (currentState == State.LOSE) {
      if (!this.loseScreenLoaded) {
        displayLoseScreen(this.deadPlayer);
      }
      waitForPlayerChoice ();
    }
  }
}
