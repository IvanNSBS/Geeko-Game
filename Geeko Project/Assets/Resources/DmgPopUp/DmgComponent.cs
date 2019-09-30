using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Destroy(this.gameObject, 0.6f);
    }

    private void Update()
    {
        var rect = GetComponent<RectTransform>();
        rect.transform.position += new Vector3(0, 1.2f*Time.deltaTime, 0);
    }
}
