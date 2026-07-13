using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private int stepsNeeded;

    public int steps = 0;

    private bool puzzleSolved;

    public bool ballGrabbed;

    [SerializeField] GameObject arrows;

    [SerializeField] private UnityEvent eventAfterCompletion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToSteps(PuzzleButton button)
    {
        if (!puzzleSolved)
        {
            if (button.index == steps + 1 && !button.correct)
            {
                steps++;
                button.Pressed();
            }
            else
            {
                button.SelfReset();
            }

            CheckCompletion();
        }
    }

    public void CheckCompletion()
    {
        if (stepsNeeded == steps)
        {
            eventAfterCompletion?.Invoke();
            puzzleSolved = true;
        }
    }

    public void FirstBallGrabbed()
    {
        if (!ballGrabbed)
        {
            arrows.SetActive(true);
            ballGrabbed = true;
        }
    }
}
