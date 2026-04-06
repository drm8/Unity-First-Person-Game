using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
		//foreach (Transform child in GetComponentsInChildren<Transform>())
		//{
  //          for (int i = enemyList.Count - 1; i >= 0; i--) {
  //              if (enemyList[i].ShouldBeDestroyed())
  //              {
  //                  enemyList.RemoveAt(i);
		//		}
		//	}
		//}
	}

    public void RemoveEnemy(Transform enemy)
    {
        enemyList.Remove(enemy);
    }


	public List<Transform> GetEnemyList()
    {
        return enemyList;
    }
}
