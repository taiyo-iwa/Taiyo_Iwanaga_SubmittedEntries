using UnityEngine;

public class GoalDecision : MonoBehaviour
{
    
    private const string PLAYERTAG = "Player";

    [SerializeField] private string HOMESCENE = default;
    [SerializeField] SceneChangeController _sceneChangeController = default;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYERTAG))
        {
            _sceneChangeController.ChangeScene(HOMESCENE);
        }
    }
}
