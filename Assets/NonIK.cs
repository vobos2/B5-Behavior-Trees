using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class NonIK : MonoBehaviour
{
    public GameObject participant;
    public GameObject participant2;

    //IK related interface
    public FullBodyBipedEffector foot;

    private BehaviorAgent behaviorAgent;
    // Use this for initialization
    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

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

    protected Node ST_TurnTo(GameObject agent1, GameObject agent2)
    {

        return new Sequence(agent1.GetComponent<BehaviorMecanim>().ST_TurnToFace(new Val<Vector3>(() => agent2.transform.position)));
    }



    protected Node ST_ApproachAndWait(GameObject p, GameObject target)
    {
        Val<Vector3> positionGoal = Val.V(() => target.transform.position);
        Val<Vector3> positionP = Val.V(() => p.transform.position);
        return new Sequence(p.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(positionGoal, 2f), new LeafWait(1000), target.GetComponent<BehaviorMecanim>().ST_TurnToFace(positionP));
    }

    protected Node BuildTreeRoot()
    {
        Node roaming = new Sequence(
                        this.ST_ApproachAndWait(participant, participant2),
                           /* new DecoratorLoop(
                                new Sequence(this.PickUp(participant), this.Throw(participant, participant2)))
                            );*/
                           new DecoratorLoop(
                                new Sequence(this.Kick(participant, participant2), new LeafWait(2000))));
        //new Sequence(this.PickUp(participant), this.Throw(participant, participant2)));
        return roaming;
    }
}