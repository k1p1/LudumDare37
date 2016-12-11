using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private string GroundTag;
    [SerializeField]
    private float Height;
    [SerializeField]
    private int MaxIterations;

    public Vector3 GetSpawnPosition()
    {
        int iterations = MaxIterations;
        Vector3 spawnPosition = new Vector3();
        bool hitGround = false;
        RaycastHit hit;

        do
        {
            hitGround = false;
            spawnPosition = new Vector3(Random.Range(-transform.localScale.x, transform.localScale.x), Height, Random.Range(-transform.localScale.z, transform.localScale.z));
            if (Physics.Raycast(spawnPosition, Vector3.down, out hit) && hit.collider.CompareTag(GroundTag))
            {
                hitGround = true;
            }
        } while (--iterations > 0 && !hitGround);

        return spawnPosition;
    }
}
