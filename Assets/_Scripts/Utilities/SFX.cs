using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

//name of audio assets that reside in Resources/CLIPS_DIRECTORY for frequent usage
public enum CommonSFX {attack,click,damage,dance,disabled,flyby,hurl,magic,strange,swing,warp};

public class SFX : MonoBehaviour {
	private static readonly int CHANNELS = 32;
	private static readonly string CLIPS_DIRECTORY = "SFX";
	private static GameObject _obj;
    private static SFX _instance;
    private static CircleQueue<AudioSource> _sources;
	private static Dictionary<string,AudioClip> _dictClips;

	static SFX()
	{
        _sources = new CircleQueue<AudioSource>(CHANNELS);
        _obj = new GameObject("SFX");
        DontDestroyOnLoad(_obj);
        _instance=_obj.AddComponent<SFX>();

        for (var i=0; i< CHANNELS; i++) {
            var audio = (AudioSource)_obj.AddComponent(typeof(AudioSource));
            audio.rolloffMode = AudioRolloffMode.Custom;
            _sources.Queue(audio);  
		}
				
		_dictClips = new Dictionary<string,AudioClip>();
		foreach(CommonSFX c in System.Enum.GetValues(typeof(CommonSFX)))
			_dictClips.Add (c.ToString(),(AudioClip)Resources.Load(CLIPS_DIRECTORY + "/" + c.ToString(), typeof(AudioClip)));

	}
	
	public static AudioSource Play(AudioClip c, System.Action callback=null,bool randomPitch=false,bool loop=false)
	{
        var s = _sources.Dequeue();
        if(s)
        { 
            s.clip = c;
            s.loop = loop;
            if(randomPitch) s.pitch = Random.Range(0.7f, 1.3f);
            else s.pitch = 1f;
            s.Play();
            if (loop != true)
                _instance.StartCoroutine(_instance.Callback(s,c.length, callback));
            s.volume = 1;
            return s;
		}
        return s;
	}

    IEnumerator Callback(AudioSource source,float wait,System.Action callback)
    {
        yield return new WaitForSeconds(wait);
        _sources.Queue(source);
        if (callback!=null) callback();
    }

	public static AudioSource Play(CommonSFX clip,System.Action callback=null,bool randomPitch=false,bool loop=false)
	{
		return Play(_dictClips[clip.ToString()],callback,randomPitch,loop);
	}
	
	public static AudioSource PlayRandom(AudioClip[] c, System.Action callback=null,bool randomPitch=false,bool loop=false)
	{ 
		return Play(c[Random.Range(0, c.Length)],callback,randomPitch,loop);
	}

    //for stopping a looping sound & releasing - you need to retain the AudioSource returned by Play
    public void Stop(AudioSource source) 
    {
        source.Stop();
        _sources.Queue(source);
    }

	private IEnumerator CallbackOnCompletion(float wait,System.Action callback)
	{
		yield return new WaitForSeconds(wait);
		if(callback!=null) callback();
	}
}
