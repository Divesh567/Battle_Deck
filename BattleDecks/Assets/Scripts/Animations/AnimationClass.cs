using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationClass : ScriptableObject
{
    public class PlayAnimationEvent : UnityEvent { }

    public PlayAnimationEvent OnPlayAnimationEvent = new PlayAnimationEvent();


    public class StopAnimationEvemy : UnityEvent { }

    public StopAnimationEvemy OnStopAnimationEvent = new StopAnimationEvemy();

    protected string animID;


}
