using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UITexture))]
public class PartAnimation : MonoBehaviour {
    /// <summary>
    /// from 0~1
    /// </summary>
    public Vector2 center = Vector2.zero;
    private List<float> angles;
    public bool isPlaying = false;
    public bool isRotate;

    private Vector3 textureCenter = Vector3.zero;
    private int index;
    private int length;

    void Start()
    {
        List<float> temp = new List<float>();
        for (int i = 0; i < 360; i++)
        {
            temp.Add(i);
        }
        SetOriginalData(center, temp, false);
//        print(textureCenter);
        Play();
    }

    void Update()
    {
        if (isPlaying)
        {
            transform.RotateAround(textureCenter, Vector3.forward, angles[index % length]);
            
            index++;
        }
    }
    /// <summary>
    /// center.x [0~1] center.y [0~1] center.z depth
    /// </summary>
    /// <param name="center"></param>
    /// <param name="ang"></param>
    public void SetOriginalData(Vector3 center, List<float> ang, bool _isRotate)
    {
        isRotate = _isRotate;

        this.center = center;
        float c = center.x;
        center.x = center.y;
        center.y = c;
        center.y *= -1;

        if (isRotate)
        {
            this.angles = ang;
        }
        else
        {
            angles = new List<float>();
            for (int i = 0; i < ang.Count - 1; i++)
            {
                angles.Add(ang[i + 1] - ang[i]);
            }
        }

        index = 0;
        length = angles.Count;

        UITexture temp = GetComponent<UITexture>();
        textureCenter = temp.localCorners[1];
        textureCenter.x += temp.localSize.x * center.x;
        textureCenter.y += temp.localSize.y * center.y;
        Vector4 tempCenter = textureCenter;
        tempCenter.w = 1;
        textureCenter = transform.localToWorldMatrix * tempCenter;
        temp.depth = (int)center.z;
        
        
    }

    public void Play()
    {
        isPlaying = true;
    }
}
