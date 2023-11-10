#pragma once
#include <iostream>
#include <vector>

#include "Vector3.h"

class Triangulator {
public:
    std::vector<Vector3> _pointCloud;
    std::vector<std::vector<Vector3>> fieldPoints{};

private:
    bool hasCalculatedBounds = false;
    float xMin = std::numeric_limits<float>::max(), yMin = std::numeric_limits<float>::max();
    float xMax = std::numeric_limits<float>::lowest(), yMax = std::numeric_limits<float>::lowest();
    float lengthX;
    float lengthY;
    float stepLengthX;
    float stepLengthy;
    int numClouds;
    int partitions;
public:
    Triangulator(std::vector<Vector3>& points, int partitions) {
        // Calculate some starting values
        // std::cout << "yMax " << yMax << " yMin " << yMin << " xMax " << xMax << " xMin " << xMin << std::endl;
        _pointCloud = points;
        CalculateBounds();

        lengthX = xMax - xMin;
        lengthY = yMax - yMin;
        stepLengthX = lengthX / static_cast<float>(partitions);
        stepLengthy = lengthY / static_cast<float>(partitions);
        numClouds = partitions * partitions;
        this->partitions = partitions;
    }

    // 7 8 9
    // 4 5 6
    // 1 2 3    ^y ->x
    // pointClouds are written [x][y] index
    void Triangulate() {
        // Seperate elements into lesser clouds to be combined
        std::vector<std::vector<std::vector<Vector3>>> pointClouds = SplitIntoSmallerClouds();
        return;
        // for (auto pointCloud : pointClouds) {
        //     Vector3 average;
        //     for (auto point : pointCloud) {
        //         average += point / (float)_pointCloud.size();
        //     }
        // }
        //
        // std::cout << "Points in each file" << std::endl;
        // for (auto pointCloud : pointClouds) {
        //     std::cout << pointCloud.size() << " ";
        // }
    }

private:
    void CalculateBounds() {
        if (hasCalculatedBounds) {
            return;
        }
        hasCalculatedBounds = true;
        for (auto point : _pointCloud) {
            
            yMax = std::max(yMax, point.y());
            xMax = std::max(xMax, point.x());

            yMin = std::min(yMin, point.y());
            xMin = std::min(xMin, point.x());
        // std::cout << "yMax " << yMax << " yMin " << yMin << " xMax " << xMax << " xMin " << xMin << std::endl;
        }
    }

    std::vector<std::vector<std::vector<Vector3>>>& SplitIntoSmallerClouds() {
        std::vector<std::vector<std::vector<Vector3>>> pointClouds(partitions, std::vector<std::vector<Vector3>>(partitions, std::vector<Vector3>(2*2)));

        for (auto point : _pointCloud) {
            float relavtiveX = point.x() - xMin;
            float relavtiveY = point.y() - yMin;
            float fIndexX = (relavtiveX / lengthX) * static_cast<float>(partitions);
            float fIndexY = (relavtiveY / lengthY) * static_cast<float>(partitions);
            
            int indexX = (int)fIndexX;
            indexX = std::max(0, indexX);
            indexX = std::min(partitions-1, indexX);
            int indexY = (int)fIndexY;
            indexY = std::max(0, indexY);
            indexY = std::min(partitions-1, indexY);
            
            
            // std::cout << fIndexX << " " << fIndexY << std::endl;
            pointClouds[indexX][indexY].push_back(point);
            
            
        }

        return pointClouds;
    }

public:
};
