using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<Transform> enemyList = new List<Transform>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.tag == "Hitable")
            {
                enemyList.Add(child);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Transform> GetEnemyList()
    {
        return enemyList;
    }
}
