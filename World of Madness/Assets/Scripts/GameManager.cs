using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour {
  // Default GameObject prefab initializations
  // Core objects
  public Camera camera;
  public GameObject plane;
  public GameObject wall;
  public GameObject player1prefab;
  public GameObject player2prefab;

  // Power ups
  public GameObject ninjaStarPrefab;
  // Actives
  public GameObject sushiPrefab;
  public GameObject boostPrefab;
  //Traps
  public GameObject sandpitPrefab;

  // GameObjects after instantiating prefabs
  private GameObject player1;
  private GameObject player2;
  private List<GameObject> allPlatformRows = new List<GameObject>();
  private Dictionary<int, List<GameObject>> allObjects = new Dictionary<int, List<GameObject>>();

  // Camera attributes
  private float cameraMoveSpeed = 15.0f;
  private float cameraBottomFOV; // Field of View

  // Map attributes
  private Dictionary<string, int[,]> gameMaps = new Dictionary<string, int[,]>();
  private int MAP_CHUNK_SIZE = 10;
  private int MAX_PLATFORMS_PREMADE = 2;
  private int MAP_NUM_COLS = 10;
  private int currentPlatform;
  private int currentWorldRow;
  private int lastWorldRowPosition;

  // Mainly for readability - enum wrappers for different items
  private enum Objects{PLANE, WALL, PLAYER_1, PLAYER_2, POWER_UP, ACTIVE, PASSABLE_TRAP};
  private enum PowerUps{NINJA_STAR};
  private enum Actives{SUSHI};
  private enum PassableTraps{SANDPIT};

  // At the beginning of initialization of game
	void Start () {
    currentWorldRow = 0;
    lastWorldRowPosition = 0;
    loadMapPlatformsFromFile("Maps/map_layout.txt");
    generatePlatform(0, 1); // Initial platform begins at 0
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
      allObjects[xVal].Add(Instantiate(sandpitPrefab, new Vector3(xVal * 10, 0.0f, (zVal * 10) - 45), sandpitPrefab.transform.rotation) as GameObject);
      break;
    default:
      // Should never hit here
      Debug.Log ("Trap object not found");
      break;
    }
  }

  // Create type of object from its type of value
  private void instantiateObject(Objects obj, int xVal, int zVal) {
    switch (obj) {
    case Objects.WALL:
      allObjects[xVal].Add(Instantiate(wall, new Vector3(xVal * 10, 5, (zVal * 10) - 45), Quaternion.identity) as GameObject);
      break;
    case Objects.PLAYER_1:
      player1 = Instantiate(player1prefab, new Vector3(xVal * 10, 5, (zVal * 10) - 45), player1prefab.transform.rotation) as GameObject;
      break;
    case Objects.PLAYER_2:
      player2 = Instantiate(player2prefab, new Vector3(xVal * 10, 5, (zVal * 10) - 45), player2prefab.transform.rotation) as GameObject;
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
    float newXVal = camera.transform.position.x;
    if (Player1isAhead()) {
      if (camera.transform.position.x != player1.transform.position.x) {
        newXVal = (player1.transform.position + transform.forward * Time.deltaTime * cameraMoveSpeed).x;
      }
    }
    else if (camera.transform.position.x != player2.transform.position.x) {
      newXVal = (player2.transform.position + transform.forward * Time.deltaTime * cameraMoveSpeed).x;
    }
    camera.transform.position = new Vector3(newXVal, camera.transform.position.y, camera.transform.position.z);
  }

  // Create or destroy platform as needed
  private void alterMapAsNeeded() {
    cameraBottomFOV = camera.transform.position.x - (camera.fieldOfView * 0.5f);

    // Create next platform if player nears the top of current generated platform
    if((currentWorldRow * 10) < player1.transform.position.x + 50 || (currentWorldRow * 10) < player2.transform.position.x + 50) {
      if (currentPlatform + 1 > MAX_PLATFORMS_PREMADE) { generatePlatform (1); }
      else { generatePlatform (currentPlatform + 1); }
    }

    // Destroy platform rows if player doesn't see that platform anymore
    if(cameraBottomFOV > ((lastWorldRowPosition * 10) + 14.5)) {
      // Prevent removing a plane more than once
      if (allPlatformRows [0].transform.position.x == lastWorldRowPosition * 10.0f) {
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

  // Check if game is over
  public bool isGameOver() {
    return player1.gameObject == null || player2.gameObject == null;
  }

  // Update game per frame
	void Update () {
    if (!isGameOver ()) {
      adjustCameraView ();
      alterMapAsNeeded ();
    }
  }
}
