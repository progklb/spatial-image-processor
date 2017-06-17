using UnityEngine;
using System.Collections;

/// <summary>
/// Loads images into memory and allows basic controls for cycling between them.
/// </summary>
public class ImageLoader : MonoBehaviour 
{
	#region VARIABLES
	/// The images that we can separate and view.
	public Texture2D[] m_Images;

	/// The separator controller used to separate and represent images.
	private ImageProcessor m_Processor;

	/// The index in the array of the current image we are looking at.
	private int m_ImgIdx;
	/// The current image texture that has been selected for separation
	private Texture2D m_ImageTexture;
	#endregion


	#region FUNCTIONS
	void Start () 
	{
		m_Processor = FindObjectOfType<ImageProcessor>();

		if (m_Images.Length != 0)
		{
			m_ImgIdx = 0;
		}
		else
		{
			Debug.LogWarning("No images assigned to display");
		}
	}

	void Update () 
	{
		// Left click to cycle to next image
		if (Input.GetMouseButtonDown(0))
		{
			Debug.Log("Cycling images: " + m_ImgIdx + "->" + (m_ImgIdx + 1));

			m_ImageTexture = m_Images[m_ImgIdx];
			m_ImgIdx = m_ImgIdx >= m_Images.Length - 1 ? 0 : m_ImgIdx + 1;

			SetCurrentImage();
		}
	}

	/// <summary>
	/// Clears the previous image and its representation from memory and loads the specified image.
	/// </summary>
	/// <param name="image">Image to represent in 3D space.</param>
	void SetCurrentImage()
	{
		m_Processor.CleanUpWorld();
		ImageProcessor.onDestroyingComplete += ProcessImage;
	}

	void ProcessImage()
	{
		ImageProcessor.onDestroyingComplete -= ProcessImage;
		m_Processor.ProcessImage(m_ImageTexture);
	}
	#endregion
}
