using UnityEngine;

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject {
    public int trys;
    public bool isTry;
    public int default_trys;
    public float time_left;
    public float default_time_left;
    public bool[] pipes_n_wires;
}