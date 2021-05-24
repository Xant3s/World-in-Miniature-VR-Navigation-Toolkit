#ifndef INSIDE_CAPSULE_DEFINED
#define INSIDE_CAPSULE_DEFINED

bool IsInsideSphere(float3 center, float radius, float3 position) {
    return distance(center, position) <= radius;
}

void IsInsideCapsule_float(float3 P1, float3 P2, float Radius, float3 Position, out bool Out){
    if(IsInsideSphere(P1, Radius, Position) || IsInsideSphere(P2, Radius, Position)){
        Out = true;
        return;
    }

    float3 pDir = Position - P1;
    float3 direction = P2 - P1;
    float d = dot(direction, pDir);
    float lengthsq = length(direction) * length(direction);

    if(d < 0 || d > lengthsq){
        Out = false;
        return;
    }

    float dsq = pDir[0] * pDir[0] + pDir[1] * pDir[1] + pDir[2] * pDir[2] - d * d / lengthsq;
    Out = !(dsq > Radius * Radius);
}

float GetDistanceToSphere(float3 center, float radius, float3 position) {
    return distance(center, position) - radius;
}

void GetDistanceToCapsule_float(float3 p1, float3 p2, float radius, float3 position, out float distanceToCapsule) {
    float3 dir = p2 - p1;
    float d = clamp(dot(position - p1, dir) / length(dir) * length(dir), 0, 1);
    float3 pointOnLine = p1 + d * dir;
    float3 dist = distance(pointOnLine, position);
    float3 closestPoint;
    int condition = dist * dist <= radius * radius;
    if(condition){
        closestPoint = position;
    } else {
        float3 delta = position - pointOnLine;
        delta = normalize(delta) * radius;
        closestPoint = pointOnLine + delta;
    }
    distanceToCapsule = distance(closestPoint, position);
}

#endif