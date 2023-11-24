using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3.0f;

    private Rigidbody2D rigidbody2d;
    //轴向控制
    public bool vertical;
    //方向控制
    private int direction = 1;
    //方向改变时间间隔的常量
    public float changeTime = 3.0f;
    //计时器
    private float timer;

    private Animator animator;//声明组件

    private bool broken;//当前机器人是否故障

    public ParticleSystem smokeEffect;

    private AudioSource audioSource;

    public AudioClip fixedSound;

    public AudioClip[] hitSounds;

    public GameObject hitEffectParticle;//击打特效

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();//获取组件
        //animator.SetFloat("MoveX", direction);
        //animator.SetBool("Vertical", vertical);
        PlayMoveAnimation();
        broken = true;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!broken)
        {
            //已修好，那么不再移动
            return;//跳出当前Update方法，后面的代码全都不运行
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            //animator.SetFloat("MoveX", direction);
            PlayMoveAnimation();
            timer = changeTime;
        }

        Vector2 position = rigidbody2d.position;//通过刚体组件获得物体的位置

        if (vertical)//垂直轴向
        {
            position.y = position.y + Time.deltaTime * speed * direction;
        }
        else//水平轴向
        {
            position.x = position.x + Time.deltaTime * speed * direction;
        }
        rigidbody2d.MovePosition(position);
    }

    //触发检测
    private void OnCollisionEnter2D(Collision2D collision)//碰撞检测
    {
        RubyController rubyController = collision.gameObject.GetComponent<RubyController>();//查找到物体本身，再查找物体身上的组件
        if (rubyController != null)
        {
            rubyController.ChangeHealth(-1);
        }
    }

    //控制移动动画的方法
    private void PlayMoveAnimation()
    {
        if (vertical)//垂直轴向动画的控制
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", direction);
        }
        else//水平轴向动画的控制
        {
            animator.SetFloat("MoveX", direction);
            animator.SetFloat("MoveY", 0);
        }
    }

    //修复机器人
    public void Fix()
    {
        //在机器人所在位置生成，Quaternion.identity是没有旋转角度
        Instantiate(hitEffectParticle, transform.position, Quaternion.identity);
        broken = false;
        rigidbody2d.simulated = false;//不会再发生碰撞检测
        animator.SetTrigger("Fixed");
        smokeEffect.Stop();//烟雾慢慢消失
        int randomNum = Random.Range(0, 2);
        audioSource.Stop();
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(hitSounds[randomNum]);
        Invoke("PlayFixedSound", 1f);
        //UIHealthBar.instance.fixedNum = UIHealthBar.instance.fixedNum + 1;
        UIHealthBar.instance.fixedNum++;
        //Destroy(smokeEffect);//这种做法烟雾会直接消失
    }

    private void PlayFixedSound()
    {
        audioSource.PlayOneShot(fixedSound);
    }
}
