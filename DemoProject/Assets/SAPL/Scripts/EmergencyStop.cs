using UnityEngine;

public class EmergencyStop : MonoBehaviour
{
    public void TurnOffMachine()
    {
        var root = gameObject.transform.root;
        Transform[] children = root.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
		{
            if (child.gameObject.name.Equals("GridLayout"))
			{
                child.gameObject.SetActive(false);
			}
		}
    }
}
