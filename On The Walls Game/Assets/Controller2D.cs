using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CapsuleCollider))]
public class Controller2D : MonoBehaviour {

    public LayerMask collisionMask;

    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    CapsuleCollider collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    void Start()
    {
        collider = GetComponent<CapsuleCollider>();
        CalculateRaySpacing();
        collisions.Reset();
    }


    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();

        if (velocity.x !=0)
        HorizontalCollisions (ref velocity);

        if (velocity.y !=0)
        VerticalCollisions (ref velocity);

        transform.Translate(velocity);
        Debug.Log(collisions.below);
    }

    void VerticalCollisions(ref Vector3 velocity)
    {

        float directionY = Mathf.Sign(velocity.y);
        float rayLenght = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector3 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector3.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit hit;
            Ray ray = new Ray(rayOrigin, Vector3.up * directionY);
            bool ishit = Physics.Raycast(ray, out hit, rayLenght, collisionMask);

            Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLenght, Color.red);

            if (ishit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLenght = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {

        float directionX = Mathf.Sign(velocity.x);
        float rayLenght = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector3 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector3.up * (horizontalRaySpacing * i);
            RaycastHit hit;
            Ray ray = new Ray(rayOrigin, Vector3.right * directionX);
            bool ishit = Physics.Raycast(ray, out hit, rayLenght, collisionMask);

            Debug.DrawRay(rayOrigin, Vector3.right * directionX * rayLenght, Color.red);

            if (ishit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLenght = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector3(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector3(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector3(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector3(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector3 topLeft, topRight;
        public Vector3 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }


}
