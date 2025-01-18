#ifndef PLANE_CUTOUT_UTIL_DEFINED
#define PLANE_CUTOUT_UTIL_DEFINED

void IsInFront_float(float3 PlanePosition, float3 PlaneNormal, float3 Position, out bool IsInFront){
	float3 dir = Position - PlanePosition;
	IsInFront = dot(dir, PlaneNormal) > 0;
}

void GetDistanceToPlane_float(float3 PlanePosition, float3 PlaneNormal, float3 Position, out float Distance) {
	float3 dir = Position - PlanePosition;
	int isInFront = dot(dir, PlaneNormal) > 0;
	float dist;
	float sn = -dot(PlaneNormal, (Position - PlanePosition));
	float sd = dot(PlaneNormal, PlaneNormal);
	float sb = sn / sd;
	float3 closestPointOnPlane = Position + sb * PlaneNormal;
	dist = distance(Position, closestPointOnPlane);
	Distance = abs(dist);
}


#endif