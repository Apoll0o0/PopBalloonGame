using UnityEngine;

[CreateAssetMenu(fileName = "BalloonType", menuName = "BalloonGame/Balloon Type")]
public class BalloonType : ScriptableObject
{
    [Header("Temel Özellikler")]
    public Color color = Color.white;   
    public int score = 1;               
    [Header("Opsiyonel")]
    public AudioClip popSfx;            
    public GameObject popVfx;      
}
