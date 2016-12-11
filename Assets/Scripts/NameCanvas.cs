using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameCanvas : MonoBehaviour {

    [SerializeField]
    private Vector3 Offset;
    [SerializeField]
    private Text TextField;
    public string PlayerName;
    public GameObject Target;

	void Start ()
    {
        TextField.text = PlayerName;
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if (Target != null)
        {
            var forwrd = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            transform.forward = forwrd.direction;
            transform.position = Target.transform.position + Offset;
        }
        else
        {
            Destroy(gameObject);
        }
	}

}
