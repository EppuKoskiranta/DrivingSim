using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animation myAnimation;

    void Awake()
    {
        myAnimation = gameObject.GetComponent<Animation>();
    }

    void update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("works");
            myAnimation.Play("selected");
        }
    }
}
