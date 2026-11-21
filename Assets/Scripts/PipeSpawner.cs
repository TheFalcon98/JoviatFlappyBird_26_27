using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PipeSpawner : MonoBehaviour
{
    public float timeToSpawn = 3;
    public GameObject pipePrefab;
    public Transform spawnTop;
    public Transform spawnBottom;
    private bool spawning;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public void BeginSpawning()
    {
        if (!isActiveAndEnabled || spawning || !pipePrefab || !spawnTop || !spawnBottom) return;
        spawning = true;
        InvokeRepeating(nameof(CreateNewPipe), 0f, Mathf.Max(0.1f, timeToSpawn));
    }

    public void StopSpawning()
    {
        spawning = false;
        CancelInvoke(nameof(CreateNewPipe));
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateNewPipe()
    {
        if (!spawnTop || !spawnBottom || !pipePrefab) return;
        float randomY = Random.Range(spawnBottom.position.y, spawnTop.position.y);
        Vector3 newPos = new Vector3(spawnTop.position.x, randomY, spawnTop.position.z);
        GameObject o = Instantiate(pipePrefab, newPos, Quaternion.identity);
        o.transform.parent = transform;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PIPE")
        {
            Destroy(other.gameObject.transform.parent.gameObject);
        }
    }
}
