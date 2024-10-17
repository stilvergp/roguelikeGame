using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;  // Prefabs de habitaciones
    public int minRoomCount;           // Mínimo de habitaciones a generar
    public int maxRoomCount;           // Máximo de habitaciones a generar
    public Vector2 roomSize;           // Tamaño de cada habitación

    private List<RoomNode> rooms = new List<RoomNode>();  // Lista de habitaciones generadas
    private HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();  // Conjunto de posiciones ocupadas

    // Direcciones de movimiento (arriba, abajo, derecha, izquierda)
    private Vector2[] directions = new Vector2[] 
    {
        new Vector2(0, 1),  // Arriba
        new Vector2(0, -1), // Abajo
        new Vector2(1, 0),  // Derecha
        new Vector2(-1, 0)  // Izquierda
    };

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // Generar la cantidad de habitaciones entre minRoomCount y maxRoomCount
        int roomCount = Random.Range(minRoomCount, maxRoomCount + 1);

        // Generar la primera habitación en el centro
        Vector2 startPosition = Vector2.zero;
        occupiedPositions.Add(startPosition);
        GameObject firstRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)]);
        firstRoom.transform.position = startPosition * roomSize;
        rooms.Add(new RoomNode(startPosition, firstRoom));

        // Generar habitaciones adicionales
        while (rooms.Count < roomCount)
        {
            CreateConnectedRoom();
        }
    }

    void CreateConnectedRoom()
    {
        // Elegir aleatoriamente una habitación existente para conectarse
        RoomNode existingRoom = rooms[Random.Range(0, rooms.Count)];
        List<Vector2> availableDirections = GetAvailableDirections(existingRoom.room.GetComponent<Room>());

        // Si hay direcciones disponibles, proceder a crear una nueva habitación
        if (availableDirections.Count > 0)
        {
            Vector2 direction = availableDirections[Random.Range(0, availableDirections.Count)];
            Vector2 newRoomPosition = existingRoom.position + direction;

            // Verificar que la nueva posición no esté ocupada
            if (!occupiedPositions.Contains(newRoomPosition))
            {
                // Generar la nueva habitación
                GameObject newRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)]);
                newRoom.transform.position = newRoomPosition * roomSize;

                // Conectar las puertas de las habitaciones
                if (ConnectDoors(existingRoom.room.GetComponent<Room>(), newRoom.GetComponent<Room>(), direction))
                {
                    // Marcar la nueva habitación como ocupada y agregarla a la lista
                    occupiedPositions.Add(newRoomPosition);
                    rooms.Add(new RoomNode(newRoomPosition, newRoom));
                }
                else
                {
                    Destroy(newRoom);  // Destruir la habitación si no se conecta correctamente
                }
            }
        }
    }

    // Obtener direcciones abiertas basadas en las puertas de la habitación
    List<Vector2> GetAvailableDirections(Room roomScript)
    {
        List<Vector2> availableDirections = new List<Vector2>();

        if (roomScript.hasDoorTop) availableDirections.Add(Vector2.up);
        if (roomScript.hasDoorBottom) availableDirections.Add(Vector2.down);
        if (roomScript.hasDoorRight) availableDirections.Add(Vector2.right);
        if (roomScript.hasDoorLeft) availableDirections.Add(Vector2.left);

        return availableDirections;
    }

    // Método para conectar puertas de dos habitaciones
    bool ConnectDoors(Room existingRoomScript, Room newRoomScript, Vector2 direction)
    {
        bool connected = false;

        // Comprobar si las puertas se pueden conectar
        if (direction == Vector2.up && existingRoomScript.hasDoorTop && newRoomScript.hasDoorBottom)
        {
            connected = true;
        }
        else if (direction == Vector2.down && existingRoomScript.hasDoorBottom && newRoomScript.hasDoorTop)
        {
            connected = true;
        }
        else if (direction == Vector2.right && existingRoomScript.hasDoorRight && newRoomScript.hasDoorLeft)
        {
            connected = true;
        }
        else if (direction == Vector2.left && existingRoomScript.hasDoorLeft && newRoomScript.hasDoorRight)
        {
            connected = true;
        }

        // Conectar puertas si son compatibles
        if (connected)
        {
            if (direction == Vector2.up)
            {
                existingRoomScript.hasDoorTop = true;
                newRoomScript.hasDoorBottom = true;
            }
            else if (direction == Vector2.down)
            {
                existingRoomScript.hasDoorBottom = true;
                newRoomScript.hasDoorTop = true;
            }
            else if (direction == Vector2.right)
            {
                existingRoomScript.hasDoorRight = true;
                newRoomScript.hasDoorLeft = true;
            }
            else if (direction == Vector2.left)
            {
                existingRoomScript.hasDoorLeft = true;
                newRoomScript.hasDoorRight = true;
            }
        }

        return connected;
    }

    // Clase que mantiene la posición de la habitación y su objeto
    class RoomNode
    {
        public Vector2 position;
        public GameObject room;

        public RoomNode(Vector2 pos, GameObject roomObj)
        {
            position = pos;
            room = roomObj;
        }
    }
}
