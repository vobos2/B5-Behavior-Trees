/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
public class demoBT : MonoBehaviour
{
    private BehaviorAgent behaviorAgent;
    public int userInput;
    private enum StoryArc
    {
        MENU = 0,
        ADD = 1,
        MULTIPLY = 2,
        FREEFORM = 3
    }
    private StoryArc currArc = StoryArc.MENU;

    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();

    }
    private Node BuildTreeRoot()
    {
        return new DecoratorLoop(
            new SelectorParallel(
                MenuArc(),
                AddArc(),
                FreeFormArc()),
            new LeafInvoke(() => print(userInput))
            );
    }
    #region assertions
    private Node CheckMenuArc()
    {
        return new Sequence
        {
                    return new LeafAssert(() => (StoryArc)userInput == StoryArc.MENU);
        return LeafInvoke(() => currArc = StoryArc.MENU);
    }
}
private Node CheckAddArc()
{
    return new LeafAssert(() => (StoryArc)userInput == StoryArc.ADD)
    }
private Node CheckMultiplyArc()
{
    return new LeafAssert(() => (StoryArc)userInput == StoryArc.MULTIPLY)
    }
private Node CheckFreeFormArc()
{
    return new LeafAssert(() => (StoryArc)userInput == StoryArc.FREEFORM)
    }
#endregion
private Node MenuArc()
{
    return new Sequence(
        CheckMenuArc(),
        new LeafInvoke(() => print(currArc))
        );
}
private Node AddArc()
{
    return new Sequence(
        CheckMenuArc(),
        new LeafInvoke(() => print(currArc))
        );
}
#region affordances
private Node RetrieveUserInput()
{
    return new LeafInvoke(
        () =>
        {
            var input = 0;
            if (Input.GetKey("0"))
                input = 0;
            if (Input.GetKey("1"))
                input = 1;
            if (Input.GetKey("2"))
                input = 2;
            if (input =>0 && input < 3)
            {

            }
            userInput = input;
            return RunStatus.Success;
        }
        );
}
#endregion
// Update is called once per frame
void Update()
{

}
}
*/