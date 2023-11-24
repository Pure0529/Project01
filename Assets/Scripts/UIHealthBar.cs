using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Image mask;
    private float originalSize;//初始值

    //单例，全局都可以引用，尽量一个游戏内只有一个单例
    public static UIHealthBar instance { get; private set; }

    public bool hasTask;//是否有任务
    //public bool ifCompleteTask;//是否完成任务
    public int fixedNum;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        originalSize = mask.rectTransform.rect.width;//获得mask中rectTransform组件中的width值
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //设置当前UI血条的显示值
    public void SetValue(float fillPercent)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * fillPercent);//通过水平轴设置mask的width值
    }
}
