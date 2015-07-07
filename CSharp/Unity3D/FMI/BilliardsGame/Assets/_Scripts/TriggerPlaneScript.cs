using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets._Scripts;

public class TriggerPlaneScript : MonoBehaviour {

    public Text textUI;
    private int smallBallsIn = 0;
    private int bigBallsIn = 0;
    private bool isGameOver = false;
    private readonly Dictionary<string, Action<Collider>> triggerActions;

    public TriggerPlaneScript()
    {
        this.triggerActions = new Dictionary<string, Action<Collider>>();
        this.triggerActions[ObjectTags.SmallBall] = this.HandleSmallBallTrigger;
        this.triggerActions[ObjectTags.BigBall] = this.HandleBigBallTrigger;
        this.triggerActions[ObjectTags.BlackBall] = this.HandleBlackBallTrigger;
    }

	// Use this for initialization
	void Start () {
        this.ShowScore();
	}

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        Action<Collider> triggerAction;

        if (this.triggerActions.TryGetValue(tag, out triggerAction))
        {
            triggerAction(other);
            this.ShowScore();
        }
    }

    private void HandleSmallBallTrigger(Collider ball)
    {
        this.smallBallsIn++;
        ball.gameObject.SetActive(false);
    }

    private void HandleBigBallTrigger(Collider ball)
    {
        this.bigBallsIn++;
        ball.gameObject.SetActive(false);
    }

    private void HandleBlackBallTrigger(Collider ball)
    {
        this.isGameOver = true;
        ball.gameObject.SetActive(false);
    }

    private void ShowScore()
    {
        string text = isGameOver ? 
@"The black ball is in.
Game is over!" :
               string.Format(
@"Balls in:
{0} small balls
{1} big balls", this.smallBallsIn, this.bigBallsIn);

        if (this.textUI)
        {
            this.textUI.text = text;
        }
    }
}
