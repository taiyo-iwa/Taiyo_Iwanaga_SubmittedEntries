using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private const string CHARACTER_TAG = "Character";

    [SerializeField] private Transform _goalLineObject = default;

    private RaceProgressTracker _tracker = default;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(CHARACTER_TAG))
        {
            return;
        }

        Plane plane = new Plane(_goalLineObject.up, _goalLineObject.position);
        bool isFront = plane.GetSide(other.transform.position);
        if (!isFront)
        {
            return;
        }

        _tracker = other.gameObject.GetComponent<RaceProgressTracker>();
        _tracker.OnGoalLineTouched();
    }
}
