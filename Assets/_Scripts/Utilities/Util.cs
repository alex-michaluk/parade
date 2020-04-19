using System.Collections;
using UnityEngine;

public class Util : MonoBehaviour
{
    private static Util _instance;
    private static GameObject _obj;
    public static GameObject[] Kids { get; private set; }
    public static GameObject Elvis { get; private set; }
    public static GameObject Robot { get; private set; }
    public static GameObject Keypad { get; private set; }
    public static GameObject HealthDisplay { get; private set; }
    public static GameObject ParticlesHealth { get; private set; }
    public static GameObject[] ProjectilesStandard { get; private set; }
    public static GameObject[] ProjectilesFire { get; private set; }
    public static GameObject[] ProjectilesMagic { get; private set; }

    public static GameObject[][] Projectiles { get; private set; }

    private static string[] FirstNames,Adjectives;

    static Util()
    {
        var json = Resources.Load<TextAsset>("JSON/adjectives") as TextAsset;
        Adjectives = getJsonArray<string>(json.text);
        json = Resources.Load<TextAsset>("JSON/names") as TextAsset;
        FirstNames = getJsonArray<string>(json.text);

        Kids = new GameObject[8];
        for (var kid = 0; kid < 8; kid++)
            Kids[kid] = Resources.Load("KID" + kid) as GameObject;
        Elvis = Resources.Load("ELVIS") as GameObject;
        Robot = Resources.Load("ROBOT") as GameObject;
        Keypad = Resources.Load("KEYPAD") as GameObject;
        HealthDisplay = Resources.Load("HEALTHDISPLAY") as GameObject;
        ParticlesHealth = Resources.Load("CIRCLE-HEALTH") as GameObject;

        //load projectiles - see ProjectilePools.cs - align to enum ProjectilePoolName
        Projectiles = new GameObject[System.Enum.GetNames(typeof(ProjectilePoolName)).Length][];
        for (var i= 0;i < System.Enum.GetNames(typeof(ProjectilePoolName)).Length;i++)
        {
            var j = 0;
            while (Resources.Load("PROJECTILES/" + i + "-" + j) != null) j++;
            Projectiles[i] = new GameObject[j];
            for (var p = 0; p < j; p++) Projectiles[i][p] = Resources.Load("PROJECTILES/" + i + "-" + p) as GameObject;
        }
    }

    public static void PulseColor(Renderer r, Color startColor, Color endColor, bool changeBack = true, System.Action callback = null)
    {
        instance.StartCoroutine(instance.RendererColorLerp(r, startColor, endColor, changeBack, callback));
    }

    IEnumerator RendererColorLerp(Renderer r, Color startColor, Color endColor, bool changeBack = true, System.Action callback = null)
    {
        for (var t = 0f; t <= 1f; t += .1f)
        {
            Tint(r, Color.Lerp(startColor, endColor, Mathf.SmoothStep(0.0f, 1.0f, t)));
            yield return new WaitForSeconds(.01f);
        }

        if (changeBack)
        {
            for (var t = 0f; t <= 1f; t += .1f)
            { 
                Tint(r, Color.Lerp(endColor, startColor, Mathf.SmoothStep(0.0f, 1.0f, t)));
                yield return new WaitForSeconds(.01f);
            }
        }
        if (callback != null) callback();
    }

    public static void Tint(Renderer r, Color c) { foreach (Material m in r.materials) m.color = c; }

    public static string GetName() {return FirstNames[Random.Range(0, FirstNames.Length)];}

    public static string GetAdjective() {return (Adjectives[Random.Range(0, Adjectives.Length)]);}

    //YourObject[] objects = JsonHelper.getJsonArray<YourObject> (jsonString);
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }
    private class Wrapper<T> { public T[] array= { }; }

    /*--------------------------------------------------------------------------------------------*/

    // SINGLETON
    static public Util instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType(typeof(Util)) as Util;
                if (_instance == null)
                {
                    _obj = new GameObject("UTIL");
                    DontDestroyOnLoad(_obj);
                    _instance = _obj.AddComponent<Util>();
                }
            }
            return _instance;
        }
    }
}



