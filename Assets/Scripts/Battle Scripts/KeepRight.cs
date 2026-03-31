using UnityEngine;

public class KeepRightSideUp : MonoBehaviour
{
    void Update()
    {

        Vector3 newScale = transform.localScale;


        if (transform.parent != null && transform.parent.localScale.x < 0)
        {
            newScale.x = -Mathf.Abs(newScale.x);
        }
        else
        {
            newScale.x = Mathf.Abs(newScale.x);
        }

        transform.localScale = newScale;
    }
}