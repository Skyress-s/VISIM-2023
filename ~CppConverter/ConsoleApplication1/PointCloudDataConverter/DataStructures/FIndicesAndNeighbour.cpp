#include "FIndicesAndNeighbour.h"

std::ostream& operator<<(std::ostream& os, const FIndicesAndNeighbour& v) {
    os << std::fixed; // 0.00001 not 1 * e^-7

    os << v.indices[0] << " " << v.indices[1] << " "<< v.indices[2] << " "<< v.neighboursTriangles[0] << " "<< v.neighboursTriangles[1] << " "<< v.neighboursTriangles[2];
    
    // os << v.xyz[0] << " " << v.xyz[1] << " " << v.xyz[2];
    // os << "(" << v.m_xyz[0] << ", " << v.m_xyz[1] << ", " << v.m_xyz[2] << ") ";
    // os << "(" << v.m_normal[0] << ", " << v.m_normal[1] << ", " << v.m_normal[2] << ") ";
    // os << "(" << v.m_st[0] << ", " << v.m_st[1] << ") ";
    return os;
}

std::istream& operator>>(std::istream& is, FIndicesAndNeighbour& v) {
    // Trenger fire temporære variabler som kun skal lese inn parenteser og komma
    // char dum, dum2, dum3, dum4;

    // smart rading, both 0.00001 and 0.01 work
    // TODO implement
    // is >> dum >> v.xyz[0] >> dum2 >> v.xyz[1] >> dum3 >> v.xyz[2] >> dum4;
    // is >> dum >> v.m_normal[0] >> dum2 >> v.m_normal[1] >> dum3 >> v.m_normal[2] >> dum4;
    // is >> dum >> v.m_st[0] >> dum2 >> v.m_st[1] >> dum3;
    return is;
}
