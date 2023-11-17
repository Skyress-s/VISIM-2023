#pragma once
#include "Vector3.h"

struct Range3 {
    Vector3 _min;
    Vector3 _max;

public:
    Range3() {
        const float inf = std::numeric_limits<float>::max();
        const float minusInf = std::numeric_limits<float>::lowest();
        _min = Vector3(minusInf, minusInf, minusInf);
        _max = Vector3(inf, inf, inf);
    }
    
    Range3(const Vector3& min, const Vector3& max) {
        _min = min;
        _max = max;
    }

    Range3(const float& xMin, const float& yMin, const float& zMin, const float& xMax, const float& yMax,
           const float& zMax)
    // :Range3(Vector3(xMin, yMin, zMin), Vector3(xMax, yMax, zMax))
    {
        _min = Vector3(xMin, yMin, zMin);
        _max = Vector3(xMax, yMax, zMax);
    }

    float LengthX() { return _max.x() - _min.x(); }
    float LengthY() { return _max.y() - _min.y(); }
    float LengthZ() { return _max.z() - _min.z(); }
};
