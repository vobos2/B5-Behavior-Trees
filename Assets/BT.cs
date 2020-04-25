using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using RootMotion.FinalIK;
using System;
using System.Linq;

public class BT : MonoBehaviour
{
    private GameObject leader;
    private GameObject[] goodGuys;
    private GameObject[] badGuys;
    private GameObject bridge;
    public GameObject meeting1, meeting2, meeting3, meeting4, meeting5;
    //IK related interface
    public FullBodyBipedEffector leftHand;
    public FullBodyBipedEffector rightHand;
    public GameObject button, button2, loot;
    public InteractionObject ikButton;
    public InteractionObject ikButton2;
    public InteractionObject ikLoot;
    private BehaviorAgent behaviorAgent;
    // Use this for initialization
    void Start()
    {
        goodGuys = GameObject.FindGameObjectsWithTag("goodGuy");
        Debug.Log(goodGuys);
        badGuys = GameObject.FindGameObjectsWithTag("badGuy");
        //leader = FindLeader();
        bridge = GameObject.FindGameObjectWithTag("bridge");
        bridge.SetActive(false);


        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }
    /*    private GameObject FindLeader()
        {
            *//*
             *  NEED TO ATTACH CONTROLS TO THIS CHARACTER SO THE USER CAN PICK.
             *//*

            //Randomly selects leader for the group 
            var len = goodGuys.Length;
            var choose = Mathf.RoundToInt(UnityEngine.Random.value * len);
            print(choose);
            return goodGuys[choose];
        }*/

    #region Loot Affordance
    /*    protected Node PutDown(GameObject p)
{
    return new Sequence(p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, ikBall),
                        new LeafWait(300),
                        this.Node_BallMotion(),
                        new LeafWait(500), p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand));
}*/

    protected Node PickUpPhone(GameObject p, InteractionObject obj)
    {
        return new Sequence(
                            this.Node_StopMoving(loot),
                            this.Node_RotatePhone(),
                            p.GetComponent<BehaviorMecanim>().Node_StartInteraction(rightHand, obj),
                            p.GetComponent<BehaviorMecanim>().ST_PlayBodyGesture("TALKING ON PHONE", 5000),
                            p.GetComponent<BehaviorMecanim>().Node_StopInteraction(rightHand));
    }
    protected Node Node_RotatePhone()
    {
        return new LeafInvoke(() => this.RotatePhone());
    }
    public virtual RunStatus RotatePhone()
    {
        var obj = GameObject.FindGameObjectWithTag("loot");
        var pos = new Vector3(-0.129f, 0f, -0.057f);

        obj.transform.localPosition = pos;

        return RunStatus.Success;
    }

    public Node Node_StopMoving(GameObject obj)
    {
        return new LeafInvoke(() => this.StopMoving(obj));
    }
    public virtual RunStatus StopMoving(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        return RunStatus.Success;
    }

    public Node Node_ObjMotion(GameObject obj)
    {
        return new LeafInvoke(() => this.ObjMotion(obj));
    }
    public virtual RunStatus ObjMotion(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
        //obj.transform.parent = null;
        return RunStatus.Success;
    }
    #endregion
    #region Kick affordance
    public Node Kick(GameObject p, GameObject p2)
    {

        return new Sequence(
                                p2.GetComponent<BehaviorMecanim>().Node_BodyAnimation("DUCK", true),
                                p.GetComponent<BehaviorMecanim>().Node_BodyAnimation("FIGHT", true),
                                new LeafWait(1000),
                                new Sequence(
                                p2.GetComponent<BehaviorMecanim>().Node_BodyAnimation("DUCK", false),
                                 p2.GetComponent<BehaviorMecanim>().Node_BodyAnimation("DYING", true)
                                ),
                                p.GetComponent<BehaviorMecanim>().Node_BodyAnimation("FIGHT", false)
                            );
    }
    public Node Node_KickTarget(GameObject p)
    {
        return new LeafInvoke(() => this.KickTarget(p));
    }
    public virtual RunStatus KickTarget(GameObject p)
    {

        return RunStatus.Success;
    }
    #endregion
    #region Button Affordance

    public Node PressButton(GameObject p, InteractionObject button)
    {


        if (button == ikButton)
        {
            return new Sequence(
                         p.GetComponent<BehaviorMecanim>().Node_StartInteraction(leftHand, ikButton),
                         new LeafWait(1000),
                         Node_TriggerBridge(),
                         p.GetComponent<BehaviorMecanim>().Node_StopInteraction(leftHand));
        }
        else
        {
            return new Sequence(
                         p.GetComponent<BehaviorMecanim>().Node_StartInteraction(rightHand, ikButton2),
                         new LeafWait(1000),
                         Node_KillAll(),
                         p.GetComponent<BehaviorMecanim>().Node_StopInteraction(rightHand));
        }


    }
    public Node Node_TriggerBridge()
    {

        return new LeafInvoke(() => this.triggerBridge());
    }
    public virtual RunStatus triggerBridge()
    {
        // DynamicWallAnimation animate = door.GetComponent<DynamicWallAnimation>();

        bridge.SetActive(true);

        return RunStatus.Success;
    }
    public Node Node_KillAll()
    {
        var seq = new SequenceParallel();
        foreach (var agent in goodGuys)
        {
            seq.Children.Add(agent.GetComponent<BehaviorMecanim>().Node_BodyAnimation("DYING", true));
        }

        return seq;
    }


    #endregion

    #region Normal Fight
    protected Node Fight()
    {
        var seq = new Sequence();
        var zip = goodGuys.Zip(badGuys, (g, b) => (g, b));
        foreach (var (g, b) in zip)
        {
            seq.Children.Add(Kick(g, b));
        }

        return new SequenceShuffle(seq);
    }
    protected Node WalkToEnemy()
    {
        var seqWalkToEnemy = new SequenceParallel();
        var zip = goodGuys.Zip(badGuys, (g, b) => (g, b));
        foreach (var (g, b) in zip)
        {
            Val<Vector3> pos = Val.V(() => b.transform.position);
            seqWalkToEnemy.Children.Add(g.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(pos, 1.5f));
        }
        return seqWalkToEnemy;
    }
    #endregion
    protected Node ST_TurnTo(GameObject agent1, GameObject agent2)
    {

        return new Sequence(agent1.GetComponent<BehaviorMecanim>().ST_TurnToFace(new Val<Vector3>(() => agent2.transform.position)));
    }



    protected Node ST_ApproachAndWait(GameObject p, Transform target)
    {
        Val<Vector3> positionGoal = Val.V(() => target.position);
        return new Sequence(p.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(positionGoal, 1f), new LeafWait(1000), p.GetComponent<BehaviorMecanim>().ST_TurnToFace(positionGoal));
    }
    protected Node SendGoodGuysToTarget(Transform target)
    {
        var seq = new SequenceParallel();
        foreach (var agent in goodGuys)
        {
            seq.Children.Add(ST_ApproachAndWait(agent, target.transform));
        }

        return new SequenceShuffle(seq);
    }

    protected Node BuildTreeRoot()
    {
        Node roaming = new Sequence(
                        WalkToEnemy(),
                        Fight(),
                        /* SendGoodGuysToTarget(meeting1.transform),*/
                        new LeafWait(1000),
                        SendGoodGuysToTarget(meeting4.transform)
                         /*PressButton(goodGuys[0], ikButton2)*/
                         /* SendGoodGuysToTarget(meeting2.transform),
                          new LeafWait(1000),
                          SendGoodGuysToTarget(meeting3.transform),
                          new LeafWait(1000),
                          SendGoodGuysToTarget(meeting4.transform),
                          new LeafWait(1000),
                          SendGoodGuysToTarget(meeting5.transform)*/
                         );
        /*      Node roaming = new DecoratorLoop(new Sequence(
                  PickUpPhone(goodGuys[0], ikLoot)));*/
        /* new DecoratorLoop(
             new Sequence(this.PickUp(participant), this.Throw(participant, participant2)))
         );*/
        /*    new DecoratorLoop(
                 new Sequence(this.Kick(participant, participant2), new LeafWait(2000))));*/
        //new Sequence(this.PickUp(participant), this.Throw(participant, participant2)));
        return roaming;
    }

}