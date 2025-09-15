using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyMissionManager : MonoBehaviour
{
    public static DailyMissionManager Instance;

    [Header("Config")]
    [SerializeField] private List<MissionTemplate> missionTemplates;
    [SerializeField] private int missionsPerDay = 3;

    [Header("UI")]
    [SerializeField] private Transform missionsContainer;
    [SerializeField] private MissionUI missionUIPrefab;

    private List<DailyMission> activeMissions = new List<DailyMission>();
    private List<MissionUI> missionUIList = new List<MissionUI>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Genera misiones si hay templates v�lidas
        if (missionTemplates == null || missionTemplates.Count == 0)
        {
            Debug.LogError("No hay MissionTemplates asignadas en el inspector.");
            return;
        }

        GenerateAndDisplayMissions();
    }

    private void GenerateAndDisplayMissions()
    {
        activeMissions.Clear();
        missionUIList.Clear();

        // Limpiar children previos
        foreach (Transform child in missionsContainer)
        {
            Destroy(child.gameObject);
        }

        // Crear nuevas misiones
        List<MissionTemplate> pool = new List<MissionTemplate>();
        foreach (var template in missionTemplates)
        {
            if (template != null) pool.Add(template);
        }

        if (pool.Count == 0)
        {
            Debug.LogError("Todas las MissionTemplates son null.");
            return;
        }

        for (int i = 0; i < missionsPerDay; i++)
        {
            if (pool.Count == 0) break;

            int index = Random.Range(0, pool.Count);
            var chosenTemplate = pool[index];
            pool.RemoveAt(index);

            DailyMission newMission = new DailyMission(chosenTemplate);
            activeMissions.Add(newMission);

            // Instancia UI y asigna misi�n
            if (missionUIPrefab != null)
            {
                MissionUI ui = Instantiate(missionUIPrefab, missionsContainer);
                if (ui != null)
                {
                    ui.Setup(newMission, null); // aqu� podr�as pasar iconos si quieres
                    missionUIList.Add(ui);
                }
                else
                {
                    Debug.LogError("MissionUI prefab no se pudo instanciar.");
                }
            }
            else
            {
                Debug.LogError("missionUIPrefab no asignado en el inspector.");
            }
        }

        Debug.Log($"Generadas y mostradas {activeMissions.Count} misiones.");
    }

    public void AddProgress(string missionId, int amount = 1)
    {
        DailyMission mission = activeMissions.Find(m => m.template.id == missionId && !m.IsCompleted);
        if (mission != null)
        {
            mission.AddProgress(amount);
            RefreshUI();

            if (mission.IsCompleted)
            {
                Debug.Log($"Misi�n completada: {mission.template.description}. Recompensa: {mission.template.reward} monedas");
            }
        }
        else
        {
            Debug.Log($"Se intent� a�adir progreso a misi�n '{missionId}', pero no est� activa o ya fue completada.");
        }
    }

    private void RefreshUI()
    {
        foreach (var ui in missionUIList)
        {
            if (ui != null)
                ui.Refresh();
        }
    }
}
