using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    private List<Transform> enemyList = new List<Transform>();

    [SerializeField] private string nextLevel;

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

    public void RemoveEnemy(Transform enemy)
    {
        enemyList.Remove(enemy);

        if (enemyList.Count == 0)
        {
            SceneManager.LoadScene(nextLevel);
        }
    }


	public List<Transform> GetEnemyList()
    {
        return enemyList;
    }
}
