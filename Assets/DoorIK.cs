using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class DoorIK : MonoBehaviour
{
    public Transform wander1;
    public GameObject participant;
    public GameObject participant2;

    //IK related interface
    public GameObject button;
    public InteractionObject ikButton;
    public FullBodyBipedEffector hand;

    public GameObject door;
    public bool isUp;
    private BehaviorAgent behaviorAgent;
    // Use this for initialization
    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
        isUp = false;
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
    #region Door Affordance
    public Node PressButton(GameObject p)
    {
        return new Sequence(
                            p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, ikButton),
                            Node_TriggerDoor(),
                            new LeafWait(1000),
                            p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand));
    }
    public Node Node_TriggerDoor()
    {

        return new LeafInvoke(() => this.triggerDoor());
    }
    public virtual RunStatus triggerDoor()
    {
        DynamicWallAnimation animate = door.GetComponent<DynamicWallAnimation>();

        animate.triggerDoor();

        return RunStatus.Success;
    }
    /*    public virtual RunStatus DoorDown()
        {
            DynamicWallAnimation animate = door.GetComponent<DynamicWallAnimation>();
            animate.goDown();
            Val.V(() => isUp = false);
            return RunStatus.Success;
        }*/
    #endregion

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
                           new DecoratorLoop(
                                new Sequence(this.PressButton(participant), new LeafWait(2000))));
        //new Sequence(this.PickUp(participant), this.Throw(participant, participant2)));
        return roaming;
    }
}
