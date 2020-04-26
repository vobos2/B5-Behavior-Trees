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
    private GameObject boss;
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
    private int userInput;
    private StoryArc currArc;
    // Use this for initialization
    void Start()
    {
        currArc = StoryArc.MENU;
        goodGuys = GameObject.FindGameObjectsWithTag("goodGuy");
        Debug.Log(goodGuys);
        badGuys = GameObject.FindGameObjectsWithTag("badGuy");
        boss = GameObject.FindGameObjectWithTag("boss");
        //leader = FindLeader();
        bridge = GameObject.FindGameObjectWithTag("bridge");
        bridge.SetActive(false);


        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }
    private enum StoryArc
    {
        MENU = 0,
        BRIDGE = 1,
        GAMEOVER = 2

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
    #region Fight Boss
    protected Node FightBoss(GameObject p, InteractionObject obj)
    {
        return new Sequence(
            PickUpPhone(p, obj),
            Node_ExplodeBoss(),
             new LeafWait(4000)
                            );
    }
    protected Node Node_ExplodeBoss()
    {
        return new LeafInvoke(() => this.ExplodeBoss());
    }
    public virtual RunStatus ExplodeBoss()
    {
        Val<Vector3> pos = Val.V(() => boss.transform.localPosition);

        for (int i = 1; i <= 10; i++)
        {
            Debug.Log(boss.transform.localPosition.y);
            boss.transform.position += new Vector3(0, i, 0);
            /*boss.transform.localScale += boss.transform.localScale * Time.deltaTime * i;
            boss.transform.position += new Vector3(0, pos.Value.y * i * Time.deltaTime, 0);*/
        }
        return RunStatus.Success;
    }
    #endregion
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
                            ST_ApproachAndWait(p, loot.transform),
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
    #region User Input

    private Node MenuArc()
    {
        return new Sequence(
            CheckMenuArc(),
            new LeafInvoke(() => print("Please Choose button 1 or button 2")),
            RetrieveUserInput());
    }
    private Node Button1Arc(GameObject p)
    {
        return new Sequence(
            CheckButton1Arc(),
            new LeafInvoke(() => print(currArc)),
            RetrieveUserInput());
            //PressButton(p, ikButton));
    }
    private Node Button2Arc(GameObject p)
    {
        return new Sequence(
           CheckButton2Arc(),
           new LeafInvoke(() => print(currArc)),
           RetrieveUserInput());
           //PressButton(p, ikButton2));
    }
    private Node CheckButton1Arc()
    {
        return new LeafAssert(() => (StoryArc)userInput == StoryArc.BRIDGE);
    }
    private Node CheckButton2Arc()
    {
        return new LeafAssert(() => (StoryArc)userInput == StoryArc.GAMEOVER);
    }
    private Node CheckMenuArc()
    {
        return new LeafAssert(() => (StoryArc)userInput == StoryArc.MENU);
    }
    private Node RetrieveUserInput()
    {
        return new DecoratorInvert(
                        new DecoratorLoop(
                            new Sequence(
                                new LeafInvoke(
                                    () =>
                                    {
                                        var input = -1;
/*                                        if (Input.GetKey("0"))
                                            input = 0;*/
                                        if (Input.GetKey("1"))
                                            input = 1;
                                        if (Input.GetKey("2"))
                                            input = 2;

                                        if (input > 0 && input < 3)
                                        {
                                            userInput = input;
                                            currArc = (StoryArc)userInput;
                                            return RunStatus.Failure;
                                        }
                                        else
                                        {
                                            return RunStatus.Running;
                                        }


                                    }
                                 ),
                                new LeafInvoke(() => print("Waiting.."))
                              )
                          )
                    );
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
        Node roaming = new DecoratorLoop(
            new SequenceParallel(
                new SelectorParallel(
                    MenuArc(),
                    Button1Arc(goodGuys[0]),
                    Button2Arc(goodGuys[0])
                ),
                RetrieveUserInput()
            ));
        /* Node roaming = new Sequence(
                         *//*WalkToEnemy(),
                         Fight(),*//*
                         FightBoss(goodGuys[0], ikLoot),
                         *//* SendGoodGuysToTarget(meeting1.transform),*//*
                         new LeafWait(1000)
                          //SendGoodGuysToTarget(meeting5.transform)
                          *//*PressButton(goodGuys[0], ikButton2)*/
        /* SendGoodGuysToTarget(meeting2.transform),
         new LeafWait(1000),
         SendGoodGuysToTarget(meeting3.transform),
         new LeafWait(1000),
         SendGoodGuysToTarget(meeting4.transform),
         new LeafWait(1000),
         SendGoodGuysToTarget(meeting5.transform)*//*
        );*/
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