using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        RubyController rubyController = collision.GetComponent<RubyController>();
        if (rubyController != null)
        {
            rubyController.ChangeHealth(-1);
            Debug.Log("Ruby当前的生命值是:" + rubyController.Health);
        }
    }
}
