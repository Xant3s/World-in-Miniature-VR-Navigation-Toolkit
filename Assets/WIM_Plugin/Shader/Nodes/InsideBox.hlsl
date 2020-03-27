#ifndef BOXINSIDE_INCLUDED
#define BOXINSIDE_INCLUDED

#include "./PlaneCutoutUtil.hlsl"

void IsInsideBox_float(float3 BoxPosition, float4x4 BoxRotationMatrix, float3 BoxScale, float3 Position, out bool Out){
	float3 localRight = {1, 0, 0};
	float3 localUp =  {0, 1, 0};
	float3 localFront = {0, 0, -1};
	localRight = mul(BoxRotationMatrix, float4(localRight, 0)).xyz;
	localUp = mul(BoxRotationMatrix, float4(localUp, 0)).xyz;
	localFront = mul(BoxRotationMatrix, float4(localFront, 0)).xyz;

	float3 rightPlanePos = BoxPosition + localRight * BoxScale[0] / 2.0;
	float3 leftPlanePos = BoxPosition - localRight * BoxScale[0] / 2.0;
	float3 upPlanePos = BoxPosition + localUp * BoxScale[0] / 2.0;
	float3 downPlanePos = BoxPosition - localUp * BoxScale[0] / 2.0;
	float3 frontPlanePos = BoxPosition + localFront * BoxScale[0] / 2.0;
	float3 backPlanePos = BoxPosition - localFront * BoxScale[0] / 2.0;

	bool isInFrontOfRightPlane;
	bool isInFrontOfLeftPlane;
	bool isInFrontOfUpPlane;
	bool isInFrontOfDownPlane;
	bool isInFrontOfFrontPlane;
	bool isInFrontOfBackPlane;

	IsInFront_float(rightPlanePos, localRight, Position, isInFrontOfRightPlane);
	IsInFront_float(leftPlanePos, -localRight, Position, isInFrontOfLeftPlane);
	IsInFront_float(upPlanePos, localUp, Position, isInFrontOfUpPlane);
	IsInFront_float(downPlanePos, -localUp, Position, isInFrontOfDownPlane);
	IsInFront_float(frontPlanePos, localFront, Position, isInFrontOfFrontPlane);
	IsInFront_float(backPlanePos, -localFront, Position, isInFrontOfBackPlane);

	Out = !isInFrontOfRightPlane && !isInFrontOfLeftPlane &&
			!isInFrontOfUpPlane && !isInFrontOfDownPlane &&
			!isInFrontOfFrontPlane && !isInFrontOfBackPlane;
}

void GetDistanceToBox_float(float3 BoxPosition, float4x4 BoxRotationMatrix, float3 BoxScale, float3 Position, float EdgeThickness, bool isInverted, out float Distance){
	float3 localRight = {1, 0, 0};
	float3 localUp =  {0, 1, 0};
	float3 localFront = {0, 0, -1};
	localRight = mul(BoxRotationMatrix, float4(localRight, 0)).xyz;
	localUp = mul(BoxRotationMatrix, float4(localUp, 0)).xyz;
	localFront = mul(BoxRotationMatrix, float4(localFront, 0)).xyz;

	float3 rightPlanePos = BoxPosition + localRight * BoxScale[0] / 2.0;
	float3 leftPlanePos = BoxPosition - localRight * BoxScale[0] / 2.0;
	float3 upPlanePos = BoxPosition + localUp * BoxScale[0] / 2.0;
	float3 downPlanePos = BoxPosition - localUp * BoxScale[0] / 2.0;
	float3 frontPlanePos = BoxPosition + localFront * BoxScale[0] / 2.0;
	float3 backPlanePos = BoxPosition - localFront * BoxScale[0] / 2.0;

	float distanceToRightPlane;
	float distanceToLeftPlane;
	float distanceToUpPlane;
	float distanceToDownPlane;
	float distanceToFrontPlane;
	float distanceToBackPlane;

	GetDistanceToPlane_float(rightPlanePos, localRight, Position, distanceToRightPlane);
	GetDistanceToPlane_float(leftPlanePos, -localRight, Position, distanceToLeftPlane);
	GetDistanceToPlane_float(upPlanePos, localUp, Position, distanceToUpPlane);
	GetDistanceToPlane_float(downPlanePos, -localUp, Position, distanceToDownPlane);
	GetDistanceToPlane_float(frontPlanePos, localFront, Position, distanceToFrontPlane);
	GetDistanceToPlane_float(backPlanePos, -localFront, Position, distanceToBackPlane);

	float distances [6] = {distanceToRightPlane, distanceToLeftPlane, distanceToUpPlane, 
							distanceToDownPlane, distanceToFrontPlane, distanceToBackPlane};
	float infinity = 10000000;
	Distance = infinity;
	for(int i = 0; i < 6; i++){
		Distance = min(Distance, distances[i]);
	}

	if(isInverted) {
		Distance = infinity;
		bool isInFrontOfRightPlane;
		bool isInFrontOfLeftPlane;
		bool isInFrontOfUpPlane;
		bool isInFrontOfDownPlane;
		bool isInFrontOfFrontPlane;
		bool isInFrontOfBackPlane;

		IsInFront_float(rightPlanePos, localRight, Position, isInFrontOfRightPlane);
		IsInFront_float(leftPlanePos, -localRight, Position, isInFrontOfLeftPlane);
		IsInFront_float(upPlanePos, localUp, Position, isInFrontOfUpPlane);
		IsInFront_float(downPlanePos, -localUp, Position, isInFrontOfDownPlane);
		IsInFront_float(frontPlanePos, localFront, Position, isInFrontOfFrontPlane);
		IsInFront_float(backPlanePos, -localFront, Position, isInFrontOfBackPlane);

		bool rightOK = distanceToRightPlane <= EdgeThickness;
		bool leftOK = distanceToLeftPlane <= EdgeThickness;
		bool frontOK = distanceToFrontPlane <= EdgeThickness;
		bool backOK = distanceToBackPlane <= EdgeThickness;
		bool upOK = distanceToUpPlane <= EdgeThickness;
		bool downOK = distanceToDownPlane <= EdgeThickness;

		if((!isInFrontOfRightPlane || rightOK) 
			&& (!isInFrontOfLeftPlane || leftOK)
			&& (!isInFrontOfUpPlane || upOK)
			&& (!isInFrontOfDownPlane || downOK )) {
			Distance = min(Distance, distanceToFrontPlane);
		}
		if((!isInFrontOfRightPlane || rightOK) 
			&& (!isInFrontOfLeftPlane || leftOK)
			&& (!isInFrontOfUpPlane || upOK)
			&& (!isInFrontOfDownPlane || downOK )) {
			Distance = min(Distance, distanceToBackPlane);
		}
		if((!isInFrontOfFrontPlane || frontOK) 
			&& (!isInFrontOfBackPlane || backOK)
			&& (!isInFrontOfUpPlane || upOK)
			&& (!isInFrontOfDownPlane || downOK )) {
			Distance = min(Distance, distanceToRightPlane);
		}
		if((!isInFrontOfFrontPlane || frontOK) 
			&& (!isInFrontOfBackPlane || backOK)
			&& (!isInFrontOfUpPlane || upOK)
			&& (!isInFrontOfDownPlane || downOK )) {
			Distance = min(Distance, distanceToLeftPlane);
		}
		if((!isInFrontOfRightPlane || rightOK) 
			&& (!isInFrontOfLeftPlane || leftOK)
			&& (!isInFrontOfFrontPlane || frontOK)
			&& (!isInFrontOfBackPlane || backOK )) {
			Distance = min(Distance, distanceToUpPlane);
		}
		if((!isInFrontOfRightPlane || rightOK) 
			&& (!isInFrontOfLeftPlane || leftOK)
			&& (!isInFrontOfFrontPlane || frontOK)
			&& (!isInFrontOfBackPlane || backOK )) {
			Distance = min(Distance, distanceToDownPlane);
		}
	}
}

#endif