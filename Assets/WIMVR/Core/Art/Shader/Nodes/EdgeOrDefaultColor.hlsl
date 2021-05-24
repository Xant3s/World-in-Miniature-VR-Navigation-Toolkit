#ifndef EDGE_OR_DEFAULT_COLOR_DEFINED
#define EDGE_OR_DEFAULT_COLOR_DEFINED

void GetDistanceToPlane_float(bool boxEnabled, bool boxIsEdge,
								bool capsuleEnabled, bool capsuleIsEdge, 
								bool coneEnabled, bool coneIsEdge, 
								float4 edgeAlbedo, float4 defaultAlbedo, out float4 albedo) {
	bool isEdge = boxEnabled && boxIsEdge || capsuleEnabled && capsuleIsEdge || coneEnabled && coneIsEdge;
	albedo = isEdge? edgeAlbedo : coneIsEdge;
}

#endif