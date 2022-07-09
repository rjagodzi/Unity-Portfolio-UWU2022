using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisibility : MonoBehaviour
{
    [SerializeField] public GameObject m_UpMarker;
    [SerializeField] public GameObject m_DownMarker;

    public void SetMarkerVisibility(bool up, bool value)
    {
        if (up)
        {
            m_UpMarker.gameObject.SetActive(value);
        }
        else
        {
            m_DownMarker.gameObject.SetActive(value);
        }
    }

    public void OnBecameVisible()
    {
        SetMarkerVisibility(true, false);
        SetMarkerVisibility(false, false);
    }

    public void OnBecameInvisible()
    {
       // if (m_Ball.transform.position.y < transform.position.y)
      //  {
     //       SetMarkerVisibility(true, true);
      //  }
      //  else
        {
            SetMarkerVisibility(false, true);
        }
    }
}
