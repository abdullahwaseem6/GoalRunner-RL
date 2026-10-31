using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public static class ArenaBuilder
{
    const int Count = 16;
    const float Spacing = 16f;

    [MenuItem("GoalRunner/Build Arenas")]
    public static void Build()
    {
        AddTag("Goal"); AddTag("Obstacle"); AddTag("Wall");

        var old = GameObject.Find("Arenas");
        if (old != null) Object.DestroyImmediate(old);
        var root = new GameObject("Arenas");

        var floorMat = Mat("Floor", new Color(0.8f, 0.8f, 0.8f));
        var wallMat  = Mat("Wall", new Color(0.35f, 0.35f, 0.35f));
        var goalMat  = Mat("Goal", Color.green);
        var obsMat   = Mat("Obstacle", Color.red);
        var agentMat = Mat("Agent", Color.blue);

        for (int i = 0; i < Count; i++)
        {
            var arena = new GameObject("Arena" + i).transform;
            arena.SetParent(root.transform);
            arena.localPosition = new Vector3((i % 4) * Spacing, 0f, (i / 4) * Spacing);

            Box("Floor", PrimitiveType.Plane, arena, Vector3.zero, new Vector3(1.2f, 1f, 1.2f), floorMat);
            Box("WallN", PrimitiveType.Cube, arena, new Vector3(0, 0.5f, 6),  new Vector3(12, 1, 0.2f), wallMat, "Wall");
            Box("WallS", PrimitiveType.Cube, arena, new Vector3(0, 0.5f, -6), new Vector3(12, 1, 0.2f), wallMat, "Wall");
            Box("WallE", PrimitiveType.Cube, arena, new Vector3(6, 0.5f, 0),  new Vector3(0.2f, 1, 12), wallMat, "Wall");
            Box("WallW", PrimitiveType.Cube, arena, new Vector3(-6, 0.5f, 0), new Vector3(0.2f, 1, 12), wallMat, "Wall");

            var goal = Box("Goal", PrimitiveType.Cube, arena, new Vector3(0, 0.5f, 4.5f), Vector3.one * 0.8f, goalMat, "Goal");
            goal.GetComponent<BoxCollider>().isTrigger = true;

            var o1 = Obstacle(arena, -1.5f, obsMat);
            var o2 = Obstacle(arena, 1.5f, obsMat);

            var agentGo = Box("Agent", PrimitiveType.Cube, arena, new Vector3(0, 0.5f, -4.5f), Vector3.one * 0.8f, agentMat);
            var rb = agentGo.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            var nose = Box("Nose", PrimitiveType.Cube, agentGo.transform, new Vector3(0, 0, 0.6f), Vector3.one * 0.3f, wallMat);
            Object.DestroyImmediate(nose.GetComponent<BoxCollider>());

            var agent = agentGo.AddComponent<RunnerAgent>();
            agent.goal = goal.transform;
            agent.obstacles = new[] { o1, o2 };
            agent.MaxStep = 1500;

            var bp = agentGo.GetComponent<BehaviorParameters>();
            if (bp == null) bp = agentGo.AddComponent<BehaviorParameters>();
            bp.BehaviorName = "Runner";
            bp.BrainParameters.VectorObservationSize = 5;
            bp.BrainParameters.NumStackedVectorObservations = 1;
            bp.BrainParameters.ActionSpec = ActionSpec.MakeContinuous(3);

            var dr = agentGo.AddComponent<DecisionRequester>();
            dr.DecisionPeriod = 5;
            dr.TakeActionsBetweenDecisions = true;

            var rays = agentGo.AddComponent<RayPerceptionSensorComponent3D>();
            rays.SensorName = "Rays";
            rays.DetectableTags = new List<string> { "Goal", "Obstacle", "Wall" };
            rays.RaysPerDirection = 4;
            rays.MaxRayDegrees = 70f;
            rays.SphereCastRadius = 0.3f;
            rays.RayLength = 12f;
        }

        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(24f, 60f, 10f);
            Camera.main.transform.rotation = Quaternion.Euler(70f, 0f, 0f);
        }
        EditorSceneManager.MarkSceneDirty(root.scene);
    }

    static SlidingObstacle Obstacle(Transform arena, float z, Material mat)
    {
        var go = Box("Obstacle", PrimitiveType.Cube, arena, new Vector3(0, 0.5f, z), new Vector3(3, 1, 0.8f), mat, "Obstacle");
        go.AddComponent<Rigidbody>().isKinematic = true;
        return go.AddComponent<SlidingObstacle>();
    }

    static GameObject Box(string name, PrimitiveType type, Transform parent, Vector3 pos, Vector3 scale, Material mat, string tag = "Untagged")
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        go.tag = tag;
        return go;
    }

    static Material Mat(string name, Color color)
    {
        string path = "Assets/Materials/" + name + ".mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m != null) return m;
        if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets", "Materials");
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        m = new Material(shader) { color = color };
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    static void AddTag(string tag)
    {
        var tm = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var tags = tm.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tm.ApplyModifiedPropertiesWithoutUndo();
    }
}
