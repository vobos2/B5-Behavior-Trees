using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using RootMotion.FinalIK;
using System;

public class NonIK : MonoBehaviour
{
    private GameObject leader;
    private GameObject[] goodGuys;
    private GameObject[] badGuys;
    private GameObject bridge;
    public GameObject meeting1, meeting2, meeting3, meeting4, meeting5;
    //IK related interface
    public FullBodyBipedEffector leftHand;
    public FullBodyBipedEffector rightHand;
    public GameObject button, button2;
    public InteractionObject ikButton;
    public InteractionObject ikButton2;
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

    #region Ball Affordance
    /*    protected Node PutDown(GameObject p)
{
    return new Sequence(p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, ikBall),
                        new LeafWait(300),
                        this.Node_BallMotion(),
                        new LeafWait(500), p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand));
}*/

    /*   protected Node PickUp(GameObject p)
       {
           return new Sequence(this.Node_BallStop(),
                               p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, ikBall),
                               new LeafWait(1000),
                               p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand));
       }*/

    /*    public Node Node_BallStop()
        {
            return new LeafInvoke(() => this.BallStop());
        }
        public virtual RunStatus BallStop()
        {
            Rigidbody rb = button.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            return RunStatus.Success;
        }
        public Node Node_BallThrow()
        {
            return new LeafInvoke(() => this.BallThrow());
        }
        public Node Node_BallMotion()
        {
            return new LeafInvoke(() => this.BallMotion());
        }
        public virtual RunStatus BallThrow()
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            ball.transform.parent = null;
            rb.velocity = Vector3.forward;
            return RunStatus.Success;
        }
        public virtual RunStatus BallMotion()
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.isKinematic = false;
            ball.transform.parent = null;
            return RunStatus.Success;
        }*/
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
                         Node_TriggerBridge(),
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
                        SendGoodGuysToTarget(meeting1.transform),
                        new LeafWait(1000),
                        PressButton(goodGuys[0], ikButton),
                        SendGoodGuysToTarget(meeting2.transform),
                        new LeafWait(1000),
                        SendGoodGuysToTarget(meeting3.transform),
                        new LeafWait(1000),
                        SendGoodGuysToTarget(meeting4.transform),
                        new LeafWait(1000),
                        SendGoodGuysToTarget(meeting5.transform)
                         );
        /* new DecoratorLoop(
             new Sequence(this.PickUp(participant), this.Throw(participant, participant2)))
         );*/
        /*    new DecoratorLoop(
                 new Sequence(this.Kick(participant, participant2), new LeafWait(2000))));*/
        //new Sequence(this.PickUp(participant), this.Throw(participant, participant2)));
        return roaming;
    }
    private void OnTriggerEnter(Collider other)
    {
        //make path over water appear
    }
}