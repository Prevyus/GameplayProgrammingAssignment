using UnityEngine;
using System.Collections.Generic;
using Custom;

public class AIManager : MonoBehaviour
{// THIS CLASS IS A CONTROLLER FOR ALL ENEMIES IN THE GAME
    public List<AIController> enemies = new List<AIController>();
    public Spawner[] spawners;
    AIController closestEnemy;

    public bool running = true;
    public bool disabledByButton = false;

    private void Start()
    {
        spawners = GetComponentsInChildren<Spawner>();
    }

    private void Update()
    {// THIS REMOVES ENEMIES THAT HAVE DIED/ARE NULL FROM THE LIST, AND ALSO GETS THE CLOSEST ENEMY TO THE PLAYER AND TURNS ITS CAMERA ON

        float closestDist = 99999;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null) { enemies.RemoveAt(i); continue; }

            float dist = (enemies[i].transform.position - GameRoot.Instance.playerController.transform.position).magnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closestEnemy = enemies[i];
            }
        }
        foreach(AIController enemy in enemies)
        {
            if (enemy == null) continue;
            if (closestEnemy != enemy) enemy.cam.gameObject.SetActive(false);
            else enemy.cam.gameObject.SetActive(true);
        }

    }

    public void EnableAI(bool active)
    {// ENABLES OR DISABLES ALL ENEMY FUNCTIONALITY

        if (disabledByButton) return;

        running = active;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].swordDealsDamage && active) enemies[i].sword.enabled = true;
            else if (!enemies[i].swordDealsDamage && active) enemies[i].sword.enabled = false;
            else if (enemies[i].swordDealsDamage && !active) enemies[i].sword.enabled = false;
            else if (!enemies[i].swordDealsDamage && !active) enemies[i].sword.enabled = false;

            enemies[i].anim.animator.enabled = active;
            enemies[i].anim.enabled = active;
            enemies[i].model.enabled = active;
            enemies[i].healthComponent.enabled = active;
            enemies[i].agent.enabled = active;
            enemies[i].enabled = active;
        }

        for (int i = 0; i < spawners.Length; i++)
        {
            if (active) spawners[i].ResumeSpawning();
            else spawners[i].PauseSpawning();
        }
    }
}
