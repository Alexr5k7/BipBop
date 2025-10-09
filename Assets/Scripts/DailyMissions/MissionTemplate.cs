using UnityEngine;

[CreateAssetMenu(fileName = "NewMissionTemplate", menuName = "DailyMissions/MissionTemplate")]
public class MissionTemplate : ScriptableObject
{
    public string id;                 // Identificador �nico de la misi�n
    public string description;        // Texto de la misi�n (ej: "Juega 3 partidas al modo Geom�trico")
    public int goal = 1;              // Objetivo de la misi�n (ej: 3 partidas)
    public int reward = 10;           // Recompensa (ej: monedas)
    public int xpReward = 10;
    public Sprite icon;               // Icono que se muestra en UI
}
