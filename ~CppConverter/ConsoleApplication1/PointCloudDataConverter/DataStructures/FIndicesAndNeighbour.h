#pragma once
#include "Vector3.h"

struct FIndicesAndNeighbour {
public:
    int indices[3];
    int neighboursTriangles[3];

    FIndicesAndNeighbour()
    {
        indices[0] = indices[1] = indices[2] = 0;
        neighboursTriangles[0] = neighboursTriangles[1] = neighboursTriangles[2] = -1;
    }

private:
    friend std::ostream& operator<<(std::ostream&, const FIndicesAndNeighbour&);
    friend std::istream& operator>>(std::istream&, FIndicesAndNeighbour&);
};
