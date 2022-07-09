using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [SerializeField] private Transform m_BallTransform;
    [SerializeField] private List<Transform> m_Layers;
    private Transform m_Transform;

    [SerializeField] private float m_TopLayerHeight;
    [SerializeField] private float m_BottomLayerHeight;

    public void Start()
    {
        m_Transform = GetComponent<Transform>();
     /*   m_Layers = new List<Transform>();
        for(int i=0;i<m_Transform.childCount;i++)
        {
            m_Layers.Add(m_Transform.GetChild(i).GetComponent<Transform>());
        }

        m_Layers.Sort(delegate (Transform x, Transform y) {
            if (x.position.z > y.position.z) return -1;
            else if (x.position.z < y.position.z) return 1;
            else return 0;
        });*/
    }

    public void Update()
    {
        if (m_BallTransform.position.y >= 1f)
        {
            for (int i = 0; i < m_Layers.Count; i++)
            {
                Transform layer = m_Layers[i];
                float ypos = Mathf.Lerp(m_BallTransform.position.y-1 + m_TopLayerHeight, m_BottomLayerHeight, (float)i / (float)m_Layers.Count);
                layer.position = new Vector3(layer.position.x, ypos, layer.position.z);
            }
        }
        else
        {
            for(int i=0;i<m_Layers.Count;i++)
            {
                Transform layer = m_Layers[i];
                float ypos = Mathf.Lerp(m_TopLayerHeight, m_BottomLayerHeight, (float)i / (float)m_Layers.Count);
                layer.position = new Vector3(layer.position.x, ypos, layer.position.z);
            }
        }
    }
}
