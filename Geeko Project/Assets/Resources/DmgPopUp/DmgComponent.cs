using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 0.5f);
    }

    private void Update()
    {
        var rect = transform.GetChild(0).GetComponent<RectTransform>();
        rect.transform.position += new Vector3(0, 50f*Time.deltaTime, 0);
    }
}
