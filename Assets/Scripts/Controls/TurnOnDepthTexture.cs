using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class TurnOnDepthTexture : MonoBehaviour {

	// Use this for initialization
	void Start ()    {
        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
    }
}
