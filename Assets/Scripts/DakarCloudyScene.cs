using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DakarCloudyScene : MonoBehaviour
{
    private readonly Dictionary<string, Material> materials = new Dictionary<string, Material>();

    private void Awake()
    {
        if (GameObject.Find("Dakar Environment") != null)
            return;

        BuildEnvironment();
        PositionCamera();
        SetCloudyLighting();
    }

    private void BuildEnvironment()
    {
        GameObject environment = new GameObject("Dakar Environment");
        Transform root = environment.transform;

        CreateBox(root, "Sand ground", new Vector3(0, -0.2f, 4), new Vector3(40, 0.3f, 48), "sand");
        CreateBox(root, "Wet road", new Vector3(0, 0, 3), new Vector3(8, 0.12f, 38), "asphalt");
        CreateBox(root, "Left pavement", new Vector3(-4.8f, 0.08f, 3), new Vector3(1.6f, 0.25f, 38), "pavement");
        CreateBox(root, "Right pavement", new Vector3(4.8f, 0.08f, 3), new Vector3(1.6f, 0.25f, 38), "pavement");

        for (int z = -12; z <= 18; z += 6)
            CreateBox(root, "Road marking", new Vector3(0, 0.07f, z), new Vector3(0.13f, 0.02f, 2.4f), "roadMarking");

        Transform taxi = BuildTaxi(root);
        BuildBuildings(root);
        BuildPalms(root);
        BuildClouds(root);
        BuildStreetLights(root);

        CreateText(root, "Dakar sign", "DAKAR", new Vector3(-5.3f, 4.8f, 7.5f), 1.15f, "signText", 180);
        CreateText(taxi, "Taxi roof sign", "TAXI", new Vector3(0, 1.8f, -0.12f), 0.28f, "taxiBlack", 180);
    }

    private Transform BuildTaxi(Transform root)
    {
        Transform taxi = new GameObject("Yellow and black Dakar taxi").transform;
        taxi.SetParent(root, false);
        taxi.position = new Vector3(0, 0.05f, 0.7f);
        taxi.rotation = Quaternion.Euler(0, -12, 0);

        CreateBox(taxi, "Yellow body", new Vector3(0, 0.78f, 0), new Vector3(1.9f, 0.62f, 3.35f), "taxiYellow");
        CreateBox(taxi, "Black lower trim", new Vector3(0, 0.48f, 0), new Vector3(1.94f, 0.13f, 3.3f), "taxiBlack");
        CreateBox(taxi, "Roof", new Vector3(0, 1.28f, -0.12f), new Vector3(1.55f, 0.55f, 1.78f), "taxiYellow");
        CreateBox(taxi, "Front windscreen", new Vector3(0, 1.31f, 0.79f), new Vector3(1.37f, 0.4f, 0.055f), "glass");
        CreateBox(taxi, "Rear windscreen", new Vector3(0, 1.31f, -1.02f), new Vector3(1.37f, 0.4f, 0.055f), "glass");

        for (int side = -1; side <= 1; side += 2)
        {
            float x = side * 0.81f;
            CreateBox(taxi, "Side windows", new Vector3(x, 1.32f, -0.1f), new Vector3(0.045f, 0.36f, 1.45f), "glass");
            CreateBox(taxi, "Black taxi stripe", new Vector3(side * 0.965f, 0.82f, -0.05f), new Vector3(0.035f, 0.18f, 2.15f), "taxiBlack");
            CreateBox(taxi, "Front bumper", new Vector3(side * 0.48f, 0.57f, 1.7f), new Vector3(0.62f, 0.12f, 0.12f), "chrome");

            for (int axle = -1; axle <= 1; axle += 2)
            {
                GameObject wheel = CreateCylinder(taxi, "Black tyre", new Vector3(side * 0.98f, 0.25f, axle * 1.02f), new Vector3(0.24f, 0.15f, 0.24f), "taxiBlack");
                wheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
                GameObject hub = CreateCylinder(taxi, "Wheel hub", new Vector3(side * 1.01f, 0.25f, axle * 1.02f), new Vector3(0.12f, 0.16f, 0.12f), "chrome");
                hub.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
        }

        CreateBox(taxi, "Taxi roof lamp", new Vector3(0, 1.62f, -0.12f), new Vector3(0.72f, 0.12f, 0.36f), "taxiBlack");
        CreateBox(taxi, "Left headlight", new Vector3(-0.57f, 0.86f, 1.69f), new Vector3(0.34f, 0.16f, 0.06f), "headlight");
        CreateBox(taxi, "Right headlight", new Vector3(0.57f, 0.86f, 1.69f), new Vector3(0.34f, 0.16f, 0.06f), "headlight");
        CreateBox(taxi, "Front grille", new Vector3(0, 0.69f, 1.7f), new Vector3(0.58f, 0.19f, 0.07f), "taxiBlack");
        return taxi;
    }

    private void BuildBuildings(Transform root)
    {
        string[] facades = { "ochre", "terracotta", "cream" };
        for (int i = 0; i < 5; i++)
        {
            float z = -3 + i * 6.2f;
            string facade = facades[i % facades.Length];
            CreateBox(root, "Dakar building left " + i, new Vector3(-8.2f, 2.6f, z), new Vector3(5.2f, 5.2f + (i % 2), 5.6f), facade);
            CreateBox(root, "Dakar building right " + i, new Vector3(8.2f, 2.4f, z + 2.5f), new Vector3(5.2f, 4.8f + ((i + 1) % 2), 5.6f), facades[(i + 1) % facades.Length]);

            for (int floor = 0; floor < 2; floor++)
            {
                float y = 1.8f + floor * 2.05f;
                CreateBox(root, "Shuttered window left", new Vector3(-5.55f, y, z + 0.3f), new Vector3(0.08f, 0.95f, 1.05f), "window");
                CreateBox(root, "Shuttered window right", new Vector3(5.55f, y, z + 2.8f), new Vector3(0.08f, 0.95f, 1.05f), "window");
            }
        }
    }

    private void BuildPalms(Transform root)
    {
        float[] positions = { -9.5f, 9.5f };
        foreach (float x in positions)
        {
            for (int i = 0; i < 3; i++)
            {
                float z = -5 + i * 11;
                CreateCylinder(root, "Palm trunk", new Vector3(x, 2.1f, z), new Vector3(0.22f, 2.1f, 0.22f), "palmTrunk");
                for (int frond = 0; frond < 6; frond++)
                {
                    float angle = frond * 60;
                    GameObject leaf = CreateBox(root, "Palm frond", new Vector3(x + Mathf.Sin(angle * Mathf.Deg2Rad) * 0.85f, 4.1f, z + Mathf.Cos(angle * Mathf.Deg2Rad) * 0.85f), new Vector3(0.2f, 0.12f, 1.85f), "palmLeaf");
                    leaf.transform.rotation = Quaternion.Euler(-18, angle, 0);
                }
            }
        }
    }

    private void BuildClouds(Transform root)
    {
        Random.State previousRandomState = Random.state;
        Random.InitState(41);
        for (int i = 0; i < 9; i++)
        {
            float x = -24 + i * 6;
            float y = 9.5f + Random.Range(-0.7f, 1.4f);
            float z = 22 + Random.Range(-2f, 3f);
            string cloudColor = i % 3 == 0 ? "cloudBright" : "cloudGray";
            CreateSphere(root, "Overcast cloud", new Vector3(x, y, z), new Vector3(6.5f, 2.1f, 2.3f), cloudColor);
            CreateSphere(root, "Cloud puff", new Vector3(x - 1.6f, y + 0.8f, z), new Vector3(3.2f, 2f, 2.2f), cloudColor);
            CreateSphere(root, "Cloud puff", new Vector3(x + 1.7f, y + 0.45f, z + 0.2f), new Vector3(3.5f, 2.1f, 2.3f), cloudColor);
        }
        Random.state = previousRandomState;
    }

    private void BuildStreetLights(Transform root)
    {
        foreach (float x in new[] { -5.7f, 5.7f })
        {
            for (int i = 0; i < 3; i++)
            {
                float z = -6 + i * 12;
                CreateCylinder(root, "Street light pole", new Vector3(x, 2.7f, z), new Vector3(0.07f, 2.7f, 0.07f), "lampPost");
                CreateBox(root, "Street light arm", new Vector3(x + (x < 0 ? 0.45f : -0.45f), 5.3f, z), new Vector3(0.95f, 0.08f, 0.08f), "lampPost");
                CreateSphere(root, "Street light", new Vector3(x + (x < 0 ? 0.85f : -0.85f), 5.18f, z), new Vector3(0.32f, 0.13f, 0.2f), "headlight");
            }
        }
    }

    private void PositionCamera()
    {
        Camera sceneCamera = GetComponent<Camera>();
        if (sceneCamera == null)
            return;

        sceneCamera.transform.position = new Vector3(8.4f, 5.4f, -12.8f);
        sceneCamera.transform.LookAt(new Vector3(0, 2.5f, 3.5f));
        sceneCamera.fieldOfView = 52;
        sceneCamera.farClipPlane = 180;
    }

    private static void SetCloudyLighting()
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.48f, 0.51f, 0.55f);
        RenderSettings.ambientIntensity = 1.15f;
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.57f, 0.62f, 0.68f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.009f;

        Light sun = FindObjectOfType<Light>();
        if (sun != null)
        {
            sun.color = new Color(0.78f, 0.82f, 0.9f);
            sun.intensity = 0.85f;
        }
    }

    private GameObject CreateBox(Transform parent, string name, Vector3 position, Vector3 scale, string materialName)
    {
        return CreatePrimitive(parent, PrimitiveType.Cube, name, position, scale, materialName);
    }

    private GameObject CreateSphere(Transform parent, string name, Vector3 position, Vector3 scale, string materialName)
    {
        return CreatePrimitive(parent, PrimitiveType.Sphere, name, position, scale, materialName);
    }

    private GameObject CreateCylinder(Transform parent, string name, Vector3 position, Vector3 scale, string materialName)
    {
        return CreatePrimitive(parent, PrimitiveType.Cylinder, name, position, scale, materialName);
    }

    private GameObject CreatePrimitive(Transform parent, PrimitiveType type, string name, Vector3 position, Vector3 scale, string materialName)
    {
        GameObject item = GameObject.CreatePrimitive(type);
        item.name = name;
        item.transform.SetParent(parent, false);
        item.transform.localPosition = position;
        item.transform.localScale = scale;

        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null)
            Destroy(itemCollider);

        item.GetComponent<Renderer>().sharedMaterial = GetMaterial(materialName);
        return item;
    }

    private Material GetMaterial(string name)
    {
        Material material;
        if (materials.TryGetValue(name, out material))
            return material;

        Color color;
        switch (name)
        {
            case "sand": color = new Color(0.68f, 0.55f, 0.38f); break;
            case "asphalt": color = new Color(0.12f, 0.15f, 0.18f); break;
            case "pavement": color = new Color(0.53f, 0.48f, 0.4f); break;
            case "roadMarking": color = new Color(0.88f, 0.83f, 0.67f); break;
            case "taxiYellow": color = new Color(1f, 0.66f, 0.03f); break;
            case "taxiBlack": color = new Color(0.055f, 0.065f, 0.075f); break;
            case "glass": color = new Color(0.1f, 0.22f, 0.29f); break;
            case "chrome": color = new Color(0.58f, 0.62f, 0.64f); break;
            case "headlight": color = new Color(0.98f, 0.91f, 0.68f); break;
            case "ochre": color = new Color(0.66f, 0.39f, 0.21f); break;
            case "terracotta": color = new Color(0.58f, 0.28f, 0.19f); break;
            case "cream": color = new Color(0.79f, 0.67f, 0.49f); break;
            case "window": color = new Color(0.15f, 0.2f, 0.22f); break;
            case "palmTrunk": color = new Color(0.36f, 0.23f, 0.12f); break;
            case "palmLeaf": color = new Color(0.16f, 0.31f, 0.2f); break;
            case "cloudBright": color = new Color(0.83f, 0.86f, 0.89f); break;
            case "cloudGray": color = new Color(0.65f, 0.7f, 0.75f); break;
            case "lampPost": color = new Color(0.16f, 0.18f, 0.19f); break;
            case "signText": color = new Color(0.89f, 0.19f, 0.1f); break;
            default: color = Color.white; break;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        material = new Material(shader);
        material.color = color;
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", name == "asphalt" ? 0.32f : 0.12f);
        materials.Add(name, material);
        return material;
    }

    private void CreateText(Transform parent, string name, string text, Vector3 position, float size, string materialName, float rotationY)
    {
        GameObject label = new GameObject(name);
        label.transform.SetParent(parent, false);
        label.transform.localPosition = position;
        label.transform.localRotation = Quaternion.Euler(0, rotationY, 0);

        TextMesh textMesh = label.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 96;
        textMesh.characterSize = size / 4f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = GetMaterial(materialName).color;
    }
}
