using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class IKBT : MonoBehaviour
{
    public Transform wander1;
    /*  public Transform wander2;
      public Transform wander3;*/
    public GameObject participant;
    public GameObject participant2;

    //IK related interface
    public GameObject ball;
    public InteractionObject ikBall;
    public FullBodyBipedEffector hand;


    private BehaviorAgent behaviorAgent;
    // Use this for initialization
    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

    #region IK related function
    protected Node PickUp(GameObject p)
    {
        return new Sequence(this.Node_BallStop(),
                            p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, ikBall),
                            new LeafWait(1000),
                            p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand));
    }

    public Node Node_BallStop()
    {
        return new LeafInvoke(() => this.BallStop());
    }
    public virtual RunStatus BallStop()
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        return RunStatus.Success;
    }

    /*    protected Node PutDown(GameObject p)
        {
            return new Sequence(p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, ikBall),
                                new LeafWait(300),
                                this.Node_BallMotion(),
                                new LeafWait(500), p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand));
        }*/
    protected Node ST_TurnTo(GameObject agent1, GameObject agent2)
    {

        return new Sequence(agent1.GetComponent<BehaviorMecanim>().ST_TurnToFace(new Val<Vector3>(() => agent2.transform.position)));
    }
    protected Node Throw(GameObject p1, GameObject p2)
    {
        return new Sequence(ST_TurnTo(p1, p2),
                           p1.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, ikBall),
                           p1.GetComponent<BehaviorMecanim>().Node_HandAnimation("PISTOLAIM", true),
                          new LeafWait(1000),
                          this.Node_BallThrow(),
                          p1.GetComponent<BehaviorMecanim>().Node_HandAnimation("PISTOLAIM", false),
                     new LeafWait(500), p1.GetComponent<BehaviorMecanim>().Node_OrientTowards(p2.transform.position));
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
    }
    #endregion

    protected Node ST_ApproachAndWait(Transform target)
    {
        Val<Vector3> position = Val.V(() => target.position);
        return new Sequence(participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
    }

    protected Node BuildTreeRoot()
    {
        Node roaming = new Sequence(
                        this.ST_ApproachAndWait(this.wander1),
                          /* new DecoratorLoop(
                               new Sequence(this.PickUp(participant), this.Throw(participant, participant2)))
                           );*/
                          new Sequence(this.PickUp(participant), this.Throw(participant, participant2)));
        return roaming;
    }
}
