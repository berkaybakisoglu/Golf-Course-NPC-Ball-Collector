using UnityEngine;

[CreateAssetMenu(fileName = "NewGolfBallData", menuName = "GolfBall Data", order = 1)]
public class GolfBallData : ScriptableObject
{
    public int PointValue;
    public int Level;
    public Material Material;
}