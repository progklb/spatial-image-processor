using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a single colour as an object in 3D space.
/// </summary>
public class ColorRepresentor : MonoBehaviour 
{
	#region VARIABLES/PROPERTIES
	/// The colour that this object represents
	public Color color { get; set; }
	/// The target scale of this object
	public Vector3 scale { get; set; }

	/// The mesh renderer attached to this object
	public MeshRenderer m_Renderer;
	/// Speed adjustment when scaling the object over time
	public float m_ScaleSpeed = 1f;
	/// The min scale at which we apply the oversized material.
	public float m_OversizedScaleMin = 10f;

	/// The material applied to all objects by default
	public Material m_StandardMaterial;
	/// The material applied to objects above a certain size
	public Material m_OversizedMaterial;
	#endregion


	#region FUNCTIONALITY
	void Start()
	{
		if (!m_Renderer)
		{
			GetComponent<MeshRenderer>();
		}

		if (!m_StandardMaterial || !m_OversizedMaterial)
		{
			Debug.LogError("Materials not assigned. Disabling object.");
			enabled = false;
			return;
		}

		// Apply standard material and set its colour to the colour that this object represents
		m_Renderer.material = m_StandardMaterial;
	} 

	/// <summary>
	/// Sets this instance up according to what has been assigned as its color, and
	/// scales this object to its target scale represented by the 'scale' public property.
	/// Note that if the object is currently disabled it will be enabled automatically.
	/// </summary>
	public void SetAndScale()
	{
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}

		// Position this object in 3D space based on its RGB colour value
		transform.position = new Vector3(color.r * 255, color.g * 255, color.b * 255);
		// Color this object according to the colour it represents
		m_Renderer.material.color = color;

		StartCoroutine(ScaleIn());
	}

	/// <summary>
	/// Instructs this instance to scale to zero. This stops all active coroutines,
	/// scales itself down to (0,0,0) and calls if destroySelf is true will destroy on the gameobject.
	/// </summary>
	/// <param name="destroyGameObject">Whether the game object will be destroyed once hiding is completed.</param>
	public void HideSelf(bool destroyGameObject)
	{
		StopAllCoroutines();
		StartCoroutine(ScaleOut(destroyGameObject));
	}

	/// <summary>
	/// Scales the object from its current scale to the scale represented by the 'scale' public property.
	/// This also checks the size of the object, swapping the material for the oversized material if required.
	/// </summary>
	IEnumerator ScaleIn()
	{
		float time = 0f;

		// Note that we are scaling uniformly, so we only check X in order to reduce processing overhead.

		while (transform.localScale.x != scale.x)
		{
			time = time > 1f ? 1f : time + Time.deltaTime;

			transform.localScale = Vector3.Lerp(transform.localScale, scale, time);
			CheckMaterialSwap();

			yield return new WaitForEndOfFrame();
		}
	}

	/// <summary>
	/// Scales this object from its current size to zero, and thereafter calls destroy on the gameobject.
	/// </summary>
	/// <returns>The out.</returns>
	IEnumerator ScaleOut(bool destroyGameObject)
	{
		float time = 0f;;

		// Note that we are scaling uniformly, so we only check X in order to reduce processing overhead.

		while (transform.localScale.x != 0)
		{
			time += Time.deltaTime;

			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, time);

			yield return new WaitForEndOfFrame();
		}

		if (destroyGameObject)
		{
			Destroy(this.gameObject);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Checks the scale of this object and swaps the material to oversized if the scale is above threshold.
	/// </summary>
	void CheckMaterialSwap()
	{
		if (transform.localScale.x > m_OversizedScaleMin)
		{
			m_Renderer.material = m_OversizedMaterial;

			float a = m_Renderer.material.color.a;
			m_Renderer.material.color = new Color(color.r, color.g, color.b, a);
		}
	}
	#endregion
}
