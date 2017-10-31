using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]

public class PlayerController2D : MonoBehaviour
{
	public LayerMask collisionMask;

	//depth of where the rays cast from
	const float skinwidth = .015f;

	//# of horizontal and vertical rays fired
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	//distance between each ray
	float horizontalRaySpacing;
	float verticalRaySpacing;

	public float maxClimbAngle = 45f;
	public float maxDescendAngle = 75f;



	//defines the corners of the collider to be used for raycasting calculations
    struct RaycastCorners
    {
		public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }
	RaycastCorners raycastCorners;
	//contains flags for checking if something is colliding and what direction
	public struct CollisionInfo 
	{
		public bool above, below, left, right;
		public bool climbingSlope;
		public bool descendingSlope;
		public Vector3 velocityOld;

		public float slopeAngle, slopeAngleOld;
		//clears the collision info
		public void resetInfo() {
			above = below = left = right = false;
			climbingSlope = false;
			descendingSlope = false;


			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
	public CollisionInfo collisions;

    //Collider used for collision detection
    BoxCollider2D collider;


	// Use this for initialization
	void Start ()
    {
        collider = GetComponent<BoxCollider2D>();
		CalculateRaySpacing();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

	//updates the positions of the corners

	void UpdateRayCastCorners () 
	{
		Bounds bounds = collider.bounds;
		bounds.Expand (skinwidth * -2);

		raycastCorners.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastCorners.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastCorners.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastCorners.topRight = new Vector2(bounds.max.x, bounds.max.y);

	}
	//calculates the distance between each ray that is cast

	void CalculateRaySpacing () 
	{
		Bounds bounds = collider.bounds;
		bounds.Expand (skinwidth * -2);

		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
	//moves the character based on gravity and input
	public void Move(Vector3 velocity, float localscalex) 
	{
		
		UpdateRayCastCorners();
		collisions.resetInfo();
		collisions.velocityOld = velocity;

		if(velocity.y < 0) {
			DescendSlope(ref velocity);
		}
		if (velocity.x != 0) {
			if(localscalex > 0) {
				HorizontalCollisions(ref velocity, localscalex);
			}
			else if(localscalex < 0) {
				velocity.x *= -1;
				HorizontalCollisions(ref velocity, localscalex);
			}

		}
		if (velocity.y != 0) {
			VerticalCollisions(ref velocity);
		}

		//move using the character's transform component 
		transform.Translate(velocity);  
	}
	//detects vertical collisions and adjusts the velocity accordingly
	void VerticalCollisions(ref Vector3 velocity) 
	{

		float directionY = Mathf.Sign(velocity.y);
		float raylength = Mathf.Abs(velocity.y) + skinwidth;

		Vector2 RayOrigin;
		for (int i = 0; i < verticalRayCount; i++) {
			
			if(directionY == -1) 
			{
				RayOrigin = raycastCorners.bottomLeft;
			}
			else 
			{
				RayOrigin = raycastCorners.topLeft;
			}
			RayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(RayOrigin, Vector2.up * directionY, raylength, collisionMask);

			if (hit)
			{
				velocity.y = (hit.distance - skinwidth) * directionY;
				raylength = hit.distance;

				if(collisions.climbingSlope) 
				{
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}

		}

		if(collisions.climbingSlope) {
			float directionX = Mathf.Sign(velocity.x);
			raylength = Mathf.Abs(velocity.x) + skinwidth;

			if(directionX == -1) 
			{
				RayOrigin = raycastCorners.bottomLeft;
			}
			else 
			{
				RayOrigin = raycastCorners.bottomRight;
			}
			RayOrigin += Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(RayOrigin, Vector2.right * directionX, raylength, collisionMask);

			if (hit) 
			{
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

				if(slopeAngle != collisions.slopeAngle) 
				{
					velocity.x = (hit.distance - skinwidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void DescendSlope (ref Vector3 velocity) {
		float directionX = Mathf.Sign(velocity.x);
		Vector2 rayOrigin;

		if(directionX == -1) 
		{
			rayOrigin = raycastCorners.bottomRight;

		}
		else 
		{
			rayOrigin = raycastCorners.bottomLeft;
		}

		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if(hit)
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{
				if (Mathf.Sign(hit.normal.x) == directionX) 
				{
					if(hit.distance - skinwidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) 
					{
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}
	//detects horizontal collisions and adjusts the velocity accordingly
	void HorizontalCollisions(ref Vector3 velocity, float localscalex) 
	{
		//Debug.Log (localscalex);
		//float directionX = Mathf.Sign(velocity.x);
		float directionX=0;
		if ((int)localscalex < 0)
			directionX = -1;
		else if ((int)localscalex > 0)
			directionX = 1;
		//float directionX = localscalex;
		float raylength = Mathf.Abs(velocity.x) + skinwidth; 
		//Debug.Log (directionX);

		Vector2 RayOrigin;
		for (int i = 0; i < horizontalRayCount; i++) {
			//Debug.Log (directionX);
			if(directionX == -1) 
			{
				RayOrigin = raycastCorners.bottomLeft;
				//Debug.Log ("left");
			}
			else 
			{
				RayOrigin = raycastCorners.bottomRight;
				//Debug.Log ("right");
			}
			RayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit;
			if((int)localscalex > 0) {

				hit = Physics2D.Raycast(RayOrigin, Vector2.right * directionX, raylength, collisionMask);

			}
			else {
				directionX *= Mathf.Sign(localscalex);
				hit = Physics2D.Raycast(RayOrigin, Vector2.left, raylength, collisionMask);

			}
				
			if (hit)
			{
				//slope detection
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up); // get the angle of the slope
				
				if (i == 0 && slopeAngle <= maxClimbAngle) 
				{
					if(collisions.descendingSlope) 
					{
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld)
					{
						distanceToSlopeStart = hit.distance - skinwidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if(!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					velocity.x = (hit.distance - skinwidth) * directionX;
					raylength = hit.distance;

					if(collisions.climbingSlope) 
					{
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);

					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}

			}



		}
	}

	void ClimbSlope (ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs(velocity.x);

		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY) 
		{
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sin(velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}

	}
}
