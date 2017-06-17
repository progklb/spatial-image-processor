using UnityEngine;
using System.Collections;

/// <summary>
/// Displays an image as a texture on a meshrenderer.
/// </summary>
public class ImageDisplayer : MonoBehaviour 
{
	#region VARIABLES
	private MeshRenderer m_Renderer;
	#endregion


	#region FUNCTIONS
	void Start()
	{
		m_Renderer = GetComponent<MeshRenderer>();

		if (!m_Renderer)
		{
			Debug.LogWarning("No MeshRenderer assigned to display the loaded image.");
		}
		else
		{
			ImageProcessor.onLoadedImage += SetImage;
		}
	}

	void SetImage(Texture2D texture)
	{
		m_Renderer.material.mainTexture = texture;
	}
	#endregion
}
