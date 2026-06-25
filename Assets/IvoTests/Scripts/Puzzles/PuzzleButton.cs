using UnityEngine;

public class PuzzleButton : MonoBehaviour
{

    public int index;
    public bool correct;

    [SerializeField] private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelfReset()
    {
        animator.SetTrigger("Wrong");
    }

    public void Pressed()
    {
        correct = true;
        animator.SetTrigger("Correct");
    }
}
