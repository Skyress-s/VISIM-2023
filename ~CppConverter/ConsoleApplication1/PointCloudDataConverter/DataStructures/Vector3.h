#pragma once

#include <ostream>

struct Vector3 {
public:
    float xyz[3]{};

    Vector3();
    Vector3(const float& x, const float& y, const float& z);
    float x() const { return xyz[0]; }
    float y() const { return xyz[1]; }
    float z() const { return xyz[2]; }

    
    void x(const float& newValue) {xyz[0] = newValue; }
    void y(const float& newValue) {xyz[1] = newValue; }
    void z(const float& newValue) {xyz[2] = newValue; }

    float Length()
    {
        return sqrt(xyz[0] * xyz[0] + xyz[1] * xyz[1] + xyz[2] * xyz[2]);
    }

    Vector3 operator+(const Vector3& rhs) const {
        return Vector3(xyz[0] + rhs.xyz[0], xyz[1] + rhs.xyz[1], xyz[2] + rhs.xyz[2]);
    }
    
    Vector3 operator-(const Vector3& rhs) const {
        return Vector3(xyz[0] - rhs.xyz[0], xyz[1] - rhs.xyz[1], xyz[2] - rhs.xyz[2]);
    }

    Vector3 operator/(const Vector3& rhs) const {
        return Vector3(xyz[0] / rhs.xyz[0], xyz[1] / rhs.xyz[1], xyz[2] / rhs.xyz[2]);
    }

    Vector3 operator/(const float& rhs) {
        return Vector3(x() / rhs, y() / rhs, z()/rhs);
    }
    
    Vector3 operator*(const Vector3& rhs) const {
        return Vector3(xyz[0] * rhs.xyz[0], xyz[1] * rhs.xyz[1], xyz[2] * rhs.xyz[2]);
    }

    Vector3 operator*(const float& rhs) {
        return Vector3(x() * rhs, y() * rhs, z()*rhs);
    }

    Vector3& operator +=(const Vector3& vec) {
        *this = *this + vec;
        return *this;
    }
    
    Vector3& operator +=(Vector3& vec) {
        *this = *this + vec;
        return *this;
    }
    Vector3& operator -=(const Vector3& vec) {
        *this = *this - vec;
        return *this;
    }
    Vector3& operator -=(Vector3& vec) {
        *this = *this - vec;
        return *this;
    }
    Vector3& operator *=(Vector3& vec) {
        *this = *this * vec;
        return *this;
    }
    Vector3& operator *=(const Vector3& vec) {
        *this = *this * vec;
        return *this;
    }
    Vector3& operator *=(const float& scalar) {
        *this = *this * scalar;
        return *this;
    }
    

private:
    friend std::ostream& operator<<(std::ostream&, const Vector3&);
    friend std::istream& operator>>(std::istream&, Vector3&);
};
