using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialog : MonoBehaviour
{
    public GameObject dialogBox;//实例化对话框
    public float displayTime = 4.0f;
    public float timerDisplay;

    public Text dialogText;

    public AudioSource audioSource;
    public AudioClip completeTaskClip;

    private bool hasPlayed;

    // Start is called before the first frame update
    void Start()
    {
        dialogBox.SetActive(false);
        timerDisplay = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                dialogBox.SetActive(false);
            }
        }
    }

    //显示对话框
    public void DisplayDialog()
    {
        timerDisplay = displayTime;
        dialogBox.SetActive(true);
        UIHealthBar.instance.hasTask = true;
        if (UIHealthBar.instance.fixedNum >= 5)
        {
            //已经完成任务，需要修改对话框内容
            dialogText.text = "哦，伟大的Ruby，谢谢你，你真的太棒了！";
            if (!hasPlayed)
            {
                audioSource.PlayOneShot(completeTaskClip);
                hasPlayed = true;
            }
        }
    }
}
