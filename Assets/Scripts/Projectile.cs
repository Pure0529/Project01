using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;//声明刚体组件

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();//获取挂载该脚本物体的刚体组件
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude > 100)//如果位置的模长大于1000
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction,float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("当前子弹碰撞到的游戏物体:" + collision.gameObject);

        EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.Fix();
        }
        Destroy(gameObject);
    }
}
