using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class QuestionMark : MonoBehaviour
{
    private EnemyController enemyController;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        enemyController.onStateChange += StateChange;
    }
    
    private void StateChange()
    {
        spriteRenderer.enabled = enemyController.enemyState == EnemyState.RespondingToSound ||
                                 enemyController.enemyState == EnemyState.HearingSound;
    }
}
