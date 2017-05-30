using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    #region Fields

    //numero massimo di stanze posizionate dal generatore
    public int maxRoomsCount;

    //contenitore per le stanze
    public Transform roomsPlaceholder;
    //corridoio che fa da connessione tra le stanze
    public Room corridor;
    //modelli prefatti delle stanze
    public Room[] rooms;
    //muro che serve a tappare le uscite non connesse
    public GameObject wall;

    //lista che contiene le stanze attualmente posizionate nella mappa
    private List <Room> spawnedRooms = new List<Room>();
    //lista delle uscite non ancora connesse alla stanza
    List<Exit> notConnectedExits = new List<Exit>();

    //riferimento al game manager che si occupa di gestire il gioco
    private PlayManager gm;

    #endregion


    #region Unity Methods

    void Start()
    {
        //viene creato il riferimento al game manager
        gm = GetComponent<PlayManager>();
    }

    /*void Update()
    {
      
    }

    private void OnDrawGizmos()
    {
        foreach (Room room in spawnedRooms)
        {
            Gizmos.DrawCube(room.transform.position, new Vector3(15, 10, 15));
        }
    }*/

    #endregion


    #region Methods

    public void GenerateMap()
    {
        //Debug.Log("Generating map...");

        for(int i=0; i<roomsPlaceholder.transform.childCount; i++)
        {
            Destroy(roomsPlaceholder.GetChild(i).gameObject);
        }

        //esegue il posizionamento di una nuova stanza per il numero di stanze da posizionare
        for (int i=0; i<maxRoomsCount; i++)
        {
            //(Debug.Log("i: " + i);

            //nuova stanza da posizionare
            Room newRoom = Instantiate(rooms[Random.Range(0, rooms.Length)]);
            Exit[] newExits = newRoom.GetComponentsInChildren<Exit>();

            for(int j=0; j<newExits.Length; j++)
            {
                notConnectedExits.Add(newExits[j]);
            }

            //se sono già state posizionate delle stanze
            if (i > 0)
            {
                //uscita di una stanza precedentemente posizionata viene estratta a caso tra quelle non connesse
                Exit previousRoomExit = notConnectedExits[Random.Range(0, notConnectedExits.Count)];
                Room newCorridor = Instantiate(corridor);
                PlaceRoom(previousRoomExit, newCorridor.GetComponent<Room>());
                notConnectedExits.Remove(previousRoomExit);

                //viene posizionata la stanza collegata all'uscita selezionata
                PlaceRoom(newCorridor.GetComponentInChildren<Exit>(), newRoom.GetComponent<Room>());
                //l'uscita viene rimossa da quelle non connesse
                notConnectedExits.Remove(previousRoomExit);
            }
            else // altrimenti posiziona la prima stanza
            {
                
                PlaceFirstRoom(newRoom.GetComponent<Room>(), Vector3.zero);
            }
                
        }

        //Debug.Log("Closing exits...");
        //chiude tutte le uscite non utilizzate per connetere le stanze
        foreach (Exit exit in notConnectedExits)
        {
            GameObject newWall = Instantiate(wall, exit.transform.position, Quaternion.identity, exit.transform);
        }
    }

    private void PlaceFirstRoom(Room room, Vector3 position)
    {
        room.transform.SetParent(roomsPlaceholder);

        for (int i = 0; i < room.spawnPoints.Length; i++)
        {
            gm.SpawnPoints.Add(room.spawnPoints[i]);
        }
    }

    private void PlaceRoom(Exit exit, Room room)
    {
        room.transform.SetParent(exit.transform);
        room.transform.localPosition = Vector3.zero;
        room.transform.localRotation = Quaternion.identity;
        exit.Connected = true;

        for (int i = 0; i < room.spawnPoints.Length; i++)
        {
            gm.SpawnPoints.Add(room.spawnPoints[i]);
        }

        //Debug.Log("Room placed in " + room.transform.position);
    }

    private void PlaceCorridor(Exit exit, Room corridor)
    {
        corridor.transform.SetParent(exit.transform);
        corridor.transform.localPosition = Vector3.zero;
        corridor.transform.localRotation = Quaternion.identity;
        exit.Connected = true;
    }

    #endregion

}
