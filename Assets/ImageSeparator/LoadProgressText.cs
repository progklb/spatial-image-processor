using UnityEngine;
using System.Collections;

/// <summary>
/// Updates progress text on the game object.
/// </summary>
public class LoadProgressText : MonoBehaviour 
{
	#region VARIABLE
	/// Text to display on loading progress text element
	public string m_TextToDisplay = "Load progress ... {0}%";

	/// The mesh renderer used to display the text
	private MeshRenderer m_TextRenderer;
	/// The text mesh itself
	private TextMesh m_Text;
	#endregion


	#region FUNCTIONS
	void Start () 
	{
		m_TextRenderer = GetComponent<MeshRenderer>();
		m_Text = GetComponent<TextMesh>();

		if (!m_TextRenderer || !m_Text)
		{
			Debug.LogError("Cannot set up text. Please ensure that necessary components are present");
			enabled = false;
		}
		else
		{
			m_Text.text = string.Format(m_TextToDisplay, "0");
			m_TextRenderer.enabled = false;

			ImageProcessor.onLoadingStarted += ShowText;
			ImageProcessor.onLoadingProgressUpdate += ProgressUpdate;
			ImageProcessor.onLoadingComplete += HideText;
		}
	}

	void ShowText()
	{
		m_TextRenderer.enabled = true;
	}

	void HideText()
	{
		m_TextRenderer.enabled = false;
	}

	void ProgressUpdate(short percent)
	{
		m_Text.text = string.Format(m_TextToDisplay, percent);
	}
	#endregion

}
