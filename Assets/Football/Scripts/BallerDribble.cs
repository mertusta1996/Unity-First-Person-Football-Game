using UnityEngine;

public class BallerDribble : MonoBehaviour
{
	[Header("References")]
	public BallerComponents ballerComponents;

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Ball"))
		{
			if (ballerComponents.isDribbling)
			{
				ballerComponents.footballSound.pitch = 1.5f;
				ballerComponents.footballSound.volume = 0.4f;
				ballerComponents.footballSound.Play();
			}
		}
	}
}
