using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public AudioClip audioClip;

    public GameObject effectParticle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("与我们发生碰撞的是:" + collision);//挂在草莓上，打印输出Ruby
        RubyController rubyController = collision.GetComponent<RubyController>();//获取触发对象身上的RubyController组件
        //判断当前发生触发检测的游戏物体身上是否有RubyController脚本
        if( rubyController != null )
        {
            //有RubyController脚本
            //判断Ruby是否满血
            if (rubyController.Health < rubyController.maxHealth)
            {
                //Ruby是不满血状态
                rubyController.ChangeHealth(1);//调用Ruby身上的RubyController脚本中的ChangeHealth方法，生命值增加1
                rubyController.PlaySound(audioClip);
                Instantiate(effectParticle, transform.position, Quaternion.identity);
                Destroy(gameObject);//销毁挂载该脚本的物体本身
            }
        }
    }
}
