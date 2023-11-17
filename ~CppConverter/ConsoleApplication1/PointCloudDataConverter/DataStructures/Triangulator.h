#pragma once
#include <iostream>
#include <vector>

#include "Range3.h"
#include "Vector3.h"

class Triangulator {
public:
    std::vector<Vector3> _pointCloud;
    std::vector<std::vector<Vector3>> fieldPoints{};

private:
    bool hasCalculatedBounds = false;
    Range3 bounds;
    // float lengthX;
    // float lengthY;
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

        // lengthX = xMax - xMin;
        // lengthX = bounds.LengthX()
        // lengthY = yMax - yMin;
        // lengthY = 
        stepLengthX = bounds.LengthX() / static_cast<float>(partitions);
        stepLengthy = bounds.LengthY() / static_cast<float>(partitions);
        numClouds = partitions * partitions;
        this->partitions = partitions;
    }

    void CenterPointCloud() {
        // Getting offset vector
        // const Vector3 offset = Vector3(bounds.LengthX(), bounds.LengthY(), bounds.LengthZ()) / 2.f;
        const Vector3 offset = _pointCloud[0];

        for (Vector3& point : _pointCloud) {
            point -= offset;
            // point *= 0.05f;
        }
    }

    // 7 8 9
    // 4 5 6
    // 1 2 3    ^y ->x
    // pointClouds are written [x][y] index

    void Triangulate() {
        /*
        std::vector<std::vector<std::vector<Vector3>>> pointClouds = SplitIntoSmallerClouds();
#pragma omp parallel for
        for (int i = 0; i < pointClouds.size(); ++i) {
            std::cout << "Im : " << i << std::endl;
            for (int j = 0; j < pointClouds[i].size(); ++j) {
                std::cout << pointClouds[i][j].size() << std::endl;
            }
        }
    */

        CalculateBounds();
        std::vector<std::vector<Vector3>> pointClouds;
        std::vector<float> heights = std::vector<float>(partitions * partitions);
        SplitIntoSmallerClouds(pointClouds);
#pragma omp parallel for collapse(2) // Dont reallocate ANYTHING, dont emplace back, dont push to shered memory
        for (int i = 0; i < pointClouds.size(); ++i) {
            
            const float size = pointClouds[i].size();
            float average;
            
            for (int j = 0; j < pointClouds[i].size(); ++j) {
                average = pointClouds[i][j].y() / size;
            }
            heights[i] = average;
        }

        for (auto height : heights) {
            std::cout << height << std::endl;
        }
    }

private:
    void CalculateBounds() {
        // if (hasCalculatedBounds)
        // return;

        float xMin = std::numeric_limits<float>::max(),
              yMin = std::numeric_limits<float>::max(),
              zMin = std::numeric_limits<float>::max();

        float xMax = std::numeric_limits<float>::lowest(),
              yMax = std::numeric_limits<float>::lowest(),
              zMax = std::numeric_limits<float>::lowest();
        hasCalculatedBounds = true;
        for (auto point : _pointCloud) {
            xMax = std::max(xMax, point.x());
            yMax = std::max(yMax, point.y());
            zMax = std::max(zMax, point.z());

            xMin = std::min(xMin, point.x());
            yMin = std::min(yMin, point.y());
            zMin = std::min(zMin, point.z());
            // std::cout << "yMax " << yMax << " yMin " << yMin << " xMax " << xMax << " xMin " << xMin << std::endl;
        }

        bounds = Range3(xMin, yMin, zMin, xMax, yMax, zMax);
    }

    void SplitIntoSmallerClouds(std::vector<std::vector<Vector3>>& pointClouds) {
        // void SplitIntoSmallerClouds(std::vector<std::vector<std::vector<Vector3>>>& pointClouds) {
        // pointClouds = std::vector<std::vector<std::vector<Vector3>>>(
        //     partitions, std::vector<std::vector<Vector3>>(partitions, std::vector<Vector3>(2 * 2)));

        pointClouds = std::vector<std::vector<Vector3>>(partitions * partitions, std::vector<Vector3>(2));
        for (auto point : _pointCloud) {
            float relavtiveX = point.x() - bounds._min.x();
            float relavtiveY = point.y() - bounds._min.y();
            float fIndexX = (relavtiveX / bounds.LengthX()) * static_cast<float>(partitions);
            float fIndexY = (relavtiveY / bounds.LengthY()) * static_cast<float>(partitions);

            int indexX = (int)fIndexX;
            indexX = std::max(0, indexX);
            indexX = std::min(partitions - 1, indexX);
            int indexY = (int)fIndexY;
            indexY = std::max(0, indexY);
            indexY = std::min(partitions - 1, indexY);

            // std::cout << indexX << " | " << indexY << std::endl;
            pointClouds[indexX + indexY * partitions].push_back(point);
            // pointClouds[indexX][indexY].push_back(point);
        }
    }

public:
};
