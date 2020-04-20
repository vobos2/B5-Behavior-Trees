using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class MyBehaviorTree2 : MonoBehaviour
{
    public Transform meetingPoint;

    public GameObject participant;
    public GameObject participant2;
    public GameObject police;

    private BehaviorAgent behaviorAgent;


    private GameObject[] badGuys;
    private GameObject[] goodGuys;
    // Use this for initialization
    void Start()
    {
        findAgents();
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected Node ST_ApproachWait(GameObject agent, Transform target)
    {
        Val<Vector3> position = Val.V(() => target.position);
        var seq = new Sequence();
        seq.Children.Add(agent.GetComponent<BehaviorMecanim>().Node_GoTo(position));
        //seq.Children.Add(participant2.GetComponent<BehaviorMecanim>().Node_GoTo(position));
        seq.Children.Add(new LeafWait(1000));
        return seq;
        //return new Sequence(, participant2.GetComponent<BehaviorMecanim>().Node_GoTo(position),);
    }
    protected Node ST_Converse(GameObject agent1, GameObject agent2)
    {
        var seq = new Sequence();

        seq.Children.Add(
            agent1.GetComponent<BehaviorMecanim>().ST_PlayFaceGesture("HEADNOD", 1000));
        seq.Children.Add(new LeafWait(1000));
        seq.Children.Add(
            agent2.GetComponent<BehaviorMecanim>().ST_PlayFaceGesture("ACKNOWLEDGE", 1000));
        seq.Children.Add(new LeafWait(2000));

        return new DecoratorLoop(seq);
    }
    protected Node ST_TurnTo(GameObject agent1, GameObject agent2)
    {

        return new Sequence(agent1.GetComponent<BehaviorMecanim>().ST_TurnToFace(new Val<Vector3>(() => agent2.transform.position)));
    }
    public void findAgents()
    {
        badGuys = GameObject.FindGameObjectsWithTag("badGuy");
        goodGuys = GameObject.FindGameObjectsWithTag("goodGuy");
    }
    protected Node BuildTreeRoot()
    {
        /* Val<float> pp = Val.V(() => police.transform.position.z);
         Func<bool> act = () => (police.transform.position.z > 10);*/
        Node roaming = new DecoratorLoop(
            new Sequence(
                    this.ST_ApproachWait(badGuys[0], this.meetingPoint),
                    this.ST_ApproachWait(badGuys[1], this.meetingPoint),
                    ST_TurnTo(badGuys[0], badGuys[1]),
                    ST_Converse(badGuys[0], badGuys[1])
                    )
            );

        /*    Node trigger = new DecoratorLoop(new LeafAssert(act));
            Node root = new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new SequenceParallel(trigger, roaming)));*/
        return roaming;
    }
}
