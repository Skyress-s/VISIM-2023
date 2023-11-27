#pragma once
#include <fstream>
#include <iostream>
#include <string>
#include <vector>

#include "FIndicesAndNeighbour.h"
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
    float stepLengthZ;
    int numClouds;
    int partitions;

    bool boundsDirty = true;

public:
    Triangulator(std::vector<Vector3>& points, int partitions)
    {
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
        stepLengthZ = bounds.LengthZ() / static_cast<float>(partitions);
        numClouds = partitions * partitions;
        this->partitions = partitions;
    }

    Vector3 GetOffsetVector() const
    {
        return Vector3(bounds._max.x() + bounds._min.x(), bounds._max.y() + bounds._min.y(),
                       bounds._max.z() + bounds._min.z()) / 2.0f;
    }

    void CenterPointCloud()
    {
        CalculateBounds();
        // Getting offset vector
        const Vector3 offset = GetOffsetVector();
        // const Vector3 offset = _pointCloud[0];

        for (Vector3& point : _pointCloud) {
            point -= offset;
            // point *= 0.05f;
        }
        boundsDirty = true;
    }

    template <typename T>
    void WriteToFile(const std::string filename, std::vector<T> items)
    {
        std::ofstream outFile(filename);
        if (!outFile.is_open()) {
            std::cout << "File could not be opened or created!\n";
            return;
        }
        outFile << items.size();
        for (auto item : items) {
            // std::cout << point << std::endl;
            outFile << "\n" << item;
        }
        outFile.close();
    }

    // 7 8 9
    // 4 5 6
    // 1 2 3    ^y ->x
    // pointClouds are written [x][y] index

    void Triangulate()
    {
        CalculateBounds();
        std::cout << bounds << std::endl;

        std::vector<float> heights = std::vector<float>(partitions * partitions);

        std::vector<std::vector<Vector3>> pointClouds;
        SplitIntoSmallerClouds(pointClouds);

        /*
        // Print Data of SplitClouds
        for (int i = 0; i < pointClouds.size(); ++i) {
            std::vector<Vector3> test = std::vector<Vector3>(0);
            for (int j = 0; j < pointClouds[i].size(); ++j) {
                test.push_back(pointClouds[i][j]);
            }
            WriteToFile("Test" + std::to_string(i) + ".txt", test);
        }
        return;
        */

        // #pragma omp parallel for collapse(2) // Dont reallocate ANYTHING, dont emplace back, dont push to shered memory
        for (int i = 0; i < pointClouds.size(); ++i) {
            const float size = pointClouds[i].size();
            float average = 0;

            for (int j = 0; j < pointClouds[i].size(); ++j) {
                average += pointClouds[i][j].y() / size;
            }
            heights[i] = average;
            // std::cout << heights[i] << std::endl;
        }

        // for (auto height : heights) {
        //     std::cout << height << std::endl;
        // }

        // Create the points, with correct xyz coordinates
        std::vector<Vector3> trigPoints = std::vector<Vector3>(partitions * partitions);
        const float stepX = (bounds.LengthX() - stepLengthX) / (partitions - 1);
        const float stepZ = (bounds.LengthZ() - stepLengthZ) / (partitions - 1);
        for (int i = 0; i < partitions * partitions; i++) {
            Vector3 point = Vector3(stepX * static_cast<float>(i % partitions), heights[i],
                                    stepZ * (float)static_cast<int>((float)i / (float)partitions));
            trigPoints.push_back(point);
        }

        // center this regular grid
        const float xMin = 0;
        const float xMax = stepX * static_cast<float>(partitions - 1);
        const float zMin = 0;
        const float zMax = stepZ * static_cast<float>(partitions - 1);

        const Vector3 regularGridOffsetVector = Vector3(xMax + xMin, 0, zMax + zMin) / 2.0f;

        for (Vector3& point : trigPoints) {
            point -= regularGridOffsetVector;
        }

        // for (int i = 0; i < partitions; ++i) {
        //     for (int j = 0; j < partitions; ++j) {
        //             std::cout << heights[j + partitions * i] << " | ";
        //     }
        //     std::cout << std::endl;
        // }
        WriteToFile("Vertices.txt", trigPoints);


        // Write to structure
    }

private:
    void CalculateBounds()
    {
        if (!boundsDirty)
            return;

        boundsDirty = false;
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
        // std::cout << bounds << std::endl;
    }

    void SplitIntoSmallerClouds(std::vector<std::vector<Vector3>>& pointClouds)
    {
        // void SplitIntoSmallerClouds(std::vector<std::vector<std::vector<Vector3>>>& pointClouds) {
        // pointClouds = std::vector<std::vector<std::vector<Vector3>>>(
        //     partitions, std::vector<std::vector<Vector3>>(partitions, std::vector<Vector3>(2 * 2)));

        pointClouds = std::vector<std::vector<Vector3>>(partitions * partitions, std::vector<Vector3>(2));
        for (auto point : _pointCloud) {
            float relavtiveX = point.x() - bounds._min.x();
            float relavtiveZ = point.z() - bounds._min.z();
            float fIndexX = (relavtiveX / bounds.LengthX()) * static_cast<float>(partitions);
            float fIndexZ = (relavtiveZ / bounds.LengthZ()) * static_cast<float>(partitions);

            int indexX = (int)fIndexX;
            indexX = std::max(0, indexX);
            indexX = std::min(partitions - 1, indexX);
            int indexZ = (int)fIndexZ;
            indexZ = std::max(0, indexZ);
            indexZ = std::min(partitions - 1, indexZ);

            // std::cout << indexX << " | " << indexY << std::endl;
            pointClouds[indexX + indexZ * partitions].push_back(point);
            // pointClouds[indexX][indexY].push_back(point);
        }
    }

public:
};
