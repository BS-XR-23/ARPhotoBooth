using UnityEngine;

public abstract class BaseSolution : MonoBehaviour
{
    protected void Start()
    {
        Debug.Log("BaseSolution Started");
        DemoSolution1 baseSolution = (DemoSolution1)this;
        Debug.Log($"{baseSolution} Started");
        
        baseSolution.Start();
    }
}
