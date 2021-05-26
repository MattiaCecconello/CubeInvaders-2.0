using UnityEngine;
using redd096;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(MoveCameraTutorialManager))]
public class MoveCameraTutorialManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if(GUILayout.Button("Show Destination"))
        {
            //show destination and repaint scene
            ((MoveCameraTutorialManager)target).ShowDestination();
            SceneView.RepaintAll();
        }
    }
}

#endif

public class MoveCameraTutorialManager : BaseTutorialManagerState
{
    [Header("Use Destination or Quantity Movement?")]
    [SerializeField] bool useDestination = false;

    [Header("Destination to Reach")]
    [CanShow("useDestination")] [SerializeField] string nameObjectToReach = "";
    [CanShow("useDestination")] [SerializeField] Vector3 approxMin = -Vector3.one;
    [CanShow("useDestination")] [SerializeField] Vector3 approxMax = Vector3.one;

    [Header("How Much Moves Camera?")]
    [CanShow("useDestination", NOT = true)] [SerializeField] float howMuchMovesCamera = 100;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //set player state - using destination or movement camera
        if (useDestination)
            GameManager.instance.player.SetState(new MoveCameraPlayerTutorial(GameManager.instance.player, GameObject.Find(nameObjectToReach), approxMax, approxMin));
        else
            GameManager.instance.player.SetState(new MoveCameraPlayerTutorial(GameManager.instance.player, howMuchMovesCamera));
    }

    public void ShowDestination()
    {
        GameObject objectToReach = GameObject.Find(nameObjectToReach);

        //draw line to object
        if(objectToReach)
        {
            Vector3 position = objectToReach.transform.position;

            //from world to position
            Debug.DrawLine(FindObjectOfType<World>().transform.position, position, Color.red, 1);

            //face front
            Debug.DrawLine(position + new Vector3(approxMin.x, approxMin.y, approxMin.z), position + new Vector3(approxMin.x, approxMax.y, approxMin.z), Color.cyan, 1);    //left down - left up
            Debug.DrawLine(position + new Vector3(approxMin.x, approxMin.y, approxMin.z), position + new Vector3(approxMax.x, approxMin.y, approxMin.z), Color.cyan, 1);    //left down - right down
            Debug.DrawLine(position + new Vector3(approxMax.x, approxMax.y, approxMin.z), position + new Vector3(approxMin.x, approxMax.y, approxMin.z), Color.cyan, 1);    //right up - left up
            Debug.DrawLine(position + new Vector3(approxMax.x, approxMax.y, approxMin.z), position + new Vector3(approxMax.x, approxMin.y, approxMin.z), Color.cyan, 1);    //right up - right down
            //face right                                                                                            
            Debug.DrawLine(position + new Vector3(approxMax.x, approxMin.y, approxMin.z), position + new Vector3(approxMax.x, approxMin.y, approxMax.z), Color.cyan, 1);    //left down - right down
            Debug.DrawLine(position + new Vector3(approxMax.x, approxMax.y, approxMin.z), position + new Vector3(approxMax.x, approxMax.y, approxMax.z), Color.cyan, 1);    //left up - right up
            Debug.DrawLine(position + new Vector3(approxMax.x, approxMax.y, approxMax.z), position + new Vector3(approxMax.x, approxMin.y, approxMax.z), Color.cyan, 1);    //right up - right down
            //face back                                                                                                     
            Debug.DrawLine(position + new Vector3(approxMax.x, approxMin.y, approxMax.z), position + new Vector3(approxMin.x, approxMin.y, approxMax.z), Color.cyan, 1);    //left down - right down
            Debug.DrawLine(position + new Vector3(approxMax.x, approxMax.y, approxMax.z), position + new Vector3(approxMin.x, approxMax.y, approxMax.z), Color.cyan, 1);    //left up - right up
            Debug.DrawLine(position + new Vector3(approxMin.x, approxMax.y, approxMax.z), position + new Vector3(approxMin.x, approxMin.y, approxMax.z), Color.cyan, 1);    //right up - right down
            //face left                                                                                              
            Debug.DrawLine(position + new Vector3(approxMin.x, approxMin.y, approxMax.z), position + new Vector3(approxMin.x, approxMin.y, approxMin.z), Color.cyan, 1);    //left down - right down
            Debug.DrawLine(position + new Vector3(approxMin.x, approxMax.y, approxMax.z), position + new Vector3(approxMin.x, approxMax.y, approxMin.z), Color.cyan, 1);    //left up - right up
        }                                                         
    }
}
