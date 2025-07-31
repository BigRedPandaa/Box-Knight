using UnityEngine;

[CreateAssetMenu(menuName = "2D Sprite Painter/Prop Layer")]
public class SpritePropLayer : ScriptableObject
{
    public string layerName = "Default";
    public GameObject[] propPrefabs;

    [Range(0f, 1f)]
    public float spawnChance = 1f;

    public Vector2 scaleRange = new Vector2(1f, 1f);
    public Gradient colorVariation = new Gradient();
}
