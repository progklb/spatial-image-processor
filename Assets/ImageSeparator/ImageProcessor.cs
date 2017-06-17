using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class can be provided an image texture and create a 3D-space representation of the image
/// using colour-coded and positioned objects.
/// </summary>
public class ImageProcessor : MonoBehaviour 
{
	#region EVENTS
	/// Provided the image that has been provided for separation when the initial call is made.
	public static event Action<Texture2D> onLoadedImage = delegate {};
	/// Indicates when the routine has officially begun separating the image.
	public static event Action onLoadingStarted = delegate {};
	/// Indicates each percentile incrememnt of progress while separating the image.
	public static event Action<short> onLoadingProgressUpdate = delegate {};
	/// Indicates when the separation process has ended.
	public static event Action onLoadingComplete = delegate {};

	/// Indicates when destroying of all pixel representors has been completed.
	public static event Action onDestroyingComplete = delegate {};
	#endregion


	#region CONSTANTS
	/// The max length of a frame that we want to maintain when processing images.
	private const float FRAME_TIME = 1 / 30f;
	/// Amount to increase the scale of the existing object by when encountering a pixel of the same colour.
	private const float SCALE_INCR = 0.1f;
	/// The fallback object in-case not prefab has been assigned
	private const PrimitiveType OBJECT = PrimitiveType.Cube;
	/// The name of the parent container for all pixel representative objects for the loaded image
	private const string PARENT_CONTAINER_NAME = "Image Container";
	#endregion


	#region VARIABLES
	/// Prefab to use to represent a single pixel
	public GameObject m_RepresentationObj;

	/// Stores all assigned pixel represenations
	private Dictionary<string, ColorRepresentor> m_ObjectsSet = new Dictionary<string, ColorRepresentor>();
	/// Stores all free and assigned pixel representations
	private List<ColorRepresentor> m_ObjectsAll = new List<ColorRepresentor>();
	/// The next free pixel representor in the list of all available
	private int m_ObjectsAllIdx;

	/// The parent for all pixel representation objects
	private Transform m_ParentContainer;
	#endregion


	#region INIT
	void OnEnable() 
	{
		// Fallback object incase no prefab was applied
		if (!m_RepresentationObj)
		{
			m_RepresentationObj = GameObject.CreatePrimitive(OBJECT);
			Destroy(m_RepresentationObj.GetComponent<Collider>());
			Debug.LogWarning("No object to represent pixels has been assigned. Creating dummy object.");
		}

		if (!GameObject.Find(PARENT_CONTAINER_NAME))
		{
			m_ParentContainer = new GameObject(PARENT_CONTAINER_NAME).transform;
		}

		Initialise();
	}

	void OnDisable()
	{
		CleanUpWorldHelper();
	}
	#endregion


	#region PUBLIC FUNCTIONS
	/// <summary>
	/// Creates a 3D-space representation of the provided texture.
	/// </summary>
	/// <param name="texture">Texture.</param>
	public void ProcessImage(Texture2D texture)
	{
		onLoadedImage(texture);

		StartCoroutine(CreateRGBSpace(texture));
	}

	public void CleanUpWorld()
	{
		StopAllCoroutines();
		StartCoroutine(CleanUpWorldHelper());
	}
	#endregion


	#region IMAGE REPRESENTATION
	/// <summary>
	/// Sets up this instance by creating all necessary representor objects for representing image pixels
	/// and caching them for future use.
	/// </summary>
	void Initialise()
	{
		for (int x = 0 ; x <= 256; x++)
		{
			for (int y = 0 ; y <= 256 ; y++)
			{
				GameObject obj = Instantiate(m_RepresentationObj);
				obj.transform.parent = m_ParentContainer;

				ColorRepresentor cr = obj.GetComponent<ColorRepresentor>();

				m_ObjectsAll.Add(cr);

				obj.SetActive(false);
			}
		}

		Debug.Log("Initialised. Pixel representors available = " + m_ObjectsAll.Count);
	}

	/// <summary>
	/// Iterates through a provided textures pixels and creates pixel representative objects for each colour in the image.
	/// Anlysis of the image is broken into 100 chunks for progress indication and performance reasons.
	/// </summary>
	/// <param name="texture">Texture which we are separating into a 3D space representation</param>
	IEnumerator CreateRGBSpace(Texture2D texture)
	{
		Debug.Log("Processing image. Dimensions: w=" + texture.width + " h=" + texture.height);

		onLoadingStarted();

		Color[] pixels = texture.GetPixels();

		Debug.Log("Totals: pixels=" + pixels.Length + " representors=" + m_ObjectsAll.Count);

		// loading based on 100%
		short loadProgress = 0;
		// note the current time for monitoring frame length
		float realtime = Time.realtimeSinceStartup;

		for (int i = 0; i < pixels.Length; ++i)
		{
			RepresentPixel(pixels[i]);

			// monitor the duration of this frame, and yield when max time elapses.
			if ((Time.realtimeSinceStartup - realtime) >= FRAME_TIME)
			{
				loadProgress = (short)((i / (float)pixels.Length) * 100f);
				onLoadingProgressUpdate(loadProgress);

				realtime = Time.realtimeSinceStartup;

				yield return null;
			}

			// We have run out of representors
			if (m_ObjectsAllIdx >= m_ObjectsAll.Count)
			{
				Debug.LogWarning("No more representors are available. Image not fully processed.");
				break;
			}
		}

		onLoadingComplete();
	}

	/// <summary>
	/// Represents the provided colour as an object in 3D space, adding it to the objects dictionary.
	/// If a new colour is found, it will instantiate a new object in the scene and assign it the necessary values.
	/// If an existing colour is found, it will scale up the existing object.
	/// </summary>
	/// <param name="pixel">The colour value that will be represented.</param>
	void RepresentPixel(Color pixel)
	{
		string rgbKey = string.Format("{0}:{1}:{2}", (int)(pixel.r * 255f), (int)(pixel.b * 255f), (int)(pixel.g * 255f));

		ColorRepresentor representor;
		m_ObjectsSet.TryGetValue(rgbKey, out representor);

		if (representor != null)
		{
			representor.scale = representor.scale + new Vector3(SCALE_INCR, SCALE_INCR, SCALE_INCR);
		}
		else
		{
			representor = m_ObjectsAll[m_ObjectsAllIdx++];
			representor.color = pixel;
			representor.scale = Vector3.one;

			m_ObjectsSet.Add(rgbKey, representor);
		}

		m_ObjectsSet[rgbKey].SetAndScale();
	}

	/// <summary>
	/// Scales all existing objects in the scene to zero, instructing them all to destroy themselves and clearing the objects dictionary.
	/// Any alread-running coroutines will be stopped.
	/// </summary>
	IEnumerator CleanUpWorldHelper()
	{
		int count = 0; 
		int chunkSize = m_ObjectsAllIdx / 50;

		Debug.Log(string.Format("Cleaning the world of {0} objects. Chunk size = {1}", m_ObjectsAllIdx, m_ObjectsAllIdx / 100));

		foreach (ColorRepresentor obj in m_ObjectsSet.Values)
		{
			if (obj != null)
			{
				obj.HideSelf(false);

				++count;
				if (count > chunkSize)
				{
					count = 0; 
					yield return null;
				}
			}
		}

		m_ObjectsSet.Clear();
		m_ObjectsAllIdx = 0;

		onDestroyingComplete();
	}
	#endregion
}
