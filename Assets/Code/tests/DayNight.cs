using UnityEngine;
using System.Collections;

public class DayNight : MonoBehaviour {

	void Update () {
        this.transform.Rotate(new Vector3(0, 0.095f, 0));
	}
}
