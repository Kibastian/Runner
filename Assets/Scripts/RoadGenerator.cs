using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject RoadPref;
    private List<GameObject> roads = new List<GameObject>();
    public float maxSpeed = 10;
    private float speed = 0;
    public int maxC = 5;
    void Start()
    {
        ResetLevel();
        speed = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (speed == 0) return;
        foreach (var road in roads)
        {
            road.transform.position -= new Vector3(0, 0, Time.deltaTime * speed);
        }
        if (roads[0].transform.position.z < -300)
        {
            Destroy(roads[0]);
            roads.RemoveAt(0);
            CreateRoad();
        }

    }
    void CreateRoad()
    {
        Vector3 pos = Vector3.zero;
        if (roads.Count > 0) pos = roads[roads.Count - 1].transform.position + new Vector3(0, 0, 300);
        GameObject go = Instantiate(RoadPref, pos, Quaternion.identity);
        go.transform.SetParent(transform);
        roads.Add(go);
    }
    void ResetLevel()
    {
        speed = 0;
        if (roads.Count>0)
        {
            Destroy(roads[0]);
            roads.RemoveAt(0);
        }
        for (int i = 0; i<maxC; i++)
        {
            CreateRoad();
        }
    }
}
