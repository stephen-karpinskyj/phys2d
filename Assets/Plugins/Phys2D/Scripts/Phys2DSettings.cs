using UnityEngine;

[CreateAssetMenu(fileName = "Phys2D Settings", menuName = "Phys2D/Settings")]
public class Phys2DSettings : ScriptableObject
{
    [SerializeField, Range(1, 64)]
    private int solverIterations = 5;

    public int SolverIterations
    {
        get { return this.solverIterations; }
    }
}
