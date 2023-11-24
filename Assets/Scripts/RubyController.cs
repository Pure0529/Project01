using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    //由于主角身上的刚体组件移动和代码中主角的移动产生冲突，使主角碰到物体时发生抖动，想要解决这个问题，就将主角代码中的移动改为刚体组件的移动，就不会产生冲突
    private Rigidbody2D rigidbody2d;//声明刚体组件
    public float speed;//Ruby的速度
    
    //Ruby生命值
    public int maxHealth = 5;//最大生命值
    private int currentHealth;//Ruby的当前生命值

    //Ruby的无敌时间
    public float timeInvincible = 2.0f;//无敌时间常量
    public bool isInvincible;
    public float invincibleTimer;//计时器

    private Vector2 lookDirection = new Vector2(1, 0);//定义变量，存储Ruby的朝向
    private Animator animator;

    public GameObject projectilePrefab;

    public AudioSource audioSource;

    public AudioSource walkAudioSource;

    public AudioClip playerHit;

    public AudioClip attackSoundClip;

    public AudioClip walkSound;

    private Vector3 respawnPosition;

    public int Health { get { return currentHealth; } }

    // Start is called before the first frame update
    void Start()//只在刚开始运行的时候执行一次
    {
        //Application.targetFrameRate = 10;//帧率变成每秒调用10次，人眼看到连续的帧率为每秒30次或者每秒60次，最好是60次，默认也是60次
        rigidbody2d = GetComponent<Rigidbody2D>();//获取组件
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        //currentHealth = 3;
        //int a = GetRubyHealthValue();
        //Debug.Log("Ruby当前的生命值：" + a);
        //在声明变量的时候speed为3，inspector里面speed为5，start方法里面speed为4，最终运行之后speed为4，所以运行顺序是先运行声明变量，在运行inspector，最后运行start方法
        //speed = 4;
        //audioSource = GetComponent<AudioSource>();
        respawnPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()//如果没有加修饰符，默认是private
    {
        //玩家输入监听
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);//move是有方向的向量
        //当前玩家输入的某个轴向值不为0
        //Mathf.Approximately(move.x, 0)这是判断输入的x值和0是否近似相等
        if (!Mathf.Approximately(move.x, 0) || !Mathf.Approximately(move.y, 0))
        {
            lookDirection.Set(move.x, move.y);
            //lookDirection = move;
            lookDirection.Normalize();//单位格式化,只会赋值1或者-1等
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.clip = walkSound;
                walkAudioSource.Play();
            }
        }
        else
        {
            walkAudioSource.Stop();
        }

        //动画的控制
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);//直接取move的模长

        //变量类型：告诉计算机要存储哪种数据，以便可以在内存中获得适当的空间量
        //Vector2变量类型：一种存储两个数字的数据类型，存贮在Inspector面板里关于transform组件中position的x,y值
        //移动
        Vector2 position = transform.position;
        ////Ruby的水平方向移动
        //position.x = position.x + speed * horizontal * Time.deltaTime;//每秒0.1m的移动
        ////Ruby的垂直方向移动
        //position.y = position.y + speed * vertical * Time.deltaTime;

        //Ruby位置的移动
        position = position + speed * move * Time.deltaTime;

        //transform.position = position;
        rigidbody2d.MovePosition(position);//通过刚体移动位置

        //无敌时间计算
        if (isInvincible)
        {
            invincibleTimer = invincibleTimer - Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincible = false;
            }
        }

        //修理机器人的方法（攻击）
        if (Input.GetKeyDown(KeyCode.H))
        {
            Launch();
        }

        //检测是否与NPC对话
        if (Input.GetKeyDown(KeyCode.T))
        {
            //发射射线，从Ruby的锚点位置加上向上0.2个单位向量的位置，向看着的方向，1.5m之内检测到的NPC层级的物体存放到hit中
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NPCDialog npcDialog = hit.collider.GetComponent<NPCDialog>();
                if(npcDialog != null)
                {
                    npcDialog.DisplayDialog();
                }
            }
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }

            //受到伤害
            isInvincible = true;
            invincibleTimer = timeInvincible;
            animator.SetTrigger("Hit");
            PlaySound(playerHit);
        }

        //如果currentHealth + amount的值大于maxHealth，则返回maxHealth，如果currentHealth + amount的值小于0，则返回0
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);//限制当前值的大小，第一个参数是当前生命值，第二个参数是下限，第三个参数是上限
        //Debug.Log(currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
        {
            Respawn();
        }

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    //public int GetRubyHealthValue()
    //{
    //    return currentHealth;
    //}

    private void Launch()
    {
        if (!UIHealthBar.instance.hasTask)
        {
            return;
        }
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        //获取projectileObject身上的Projectile脚本
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);//调用该脚本中的方法
        animator.SetTrigger("Launch");//调用Launch的动画
        PlaySound(attackSoundClip);
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    private void Respawn()
    {
        ChangeHealth(maxHealth);
        transform.position = respawnPosition;
    }
}
