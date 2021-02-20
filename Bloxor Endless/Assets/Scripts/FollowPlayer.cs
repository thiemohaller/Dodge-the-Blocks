using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform objectToFollow;
    public Vector3 offset = new Vector3(0, 3, -5);

    private void Start() {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = objectToFollow.position + offset;
    }
}
