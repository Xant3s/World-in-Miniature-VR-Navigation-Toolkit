#ifndef INSIDE_EDGE_DEFINED
#define INSIDE_EDGE_DEFINED

void IsInsideEdge_float(float distance, float edgeThickness, out bool isInsideEdge) {
    isInsideEdge = abs(distance) <= edgeThickness;
}

#endif