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


    // Filename "test.txt" will result in file : test.txt 
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

    std::vector<Vector3> HeightIntoRegularGrid(std::vector<float> heights, const float& stepX, const float& stepZ)
    {
        std::vector<Vector3> trigPoints = std::vector<Vector3>();
        
        for (int i = 0; i < partitions * partitions; i++) {
            Vector3 point = Vector3(stepX * static_cast<float>(i % partitions), heights[i],
                                    stepZ * (float)static_cast<int>((float)i / (float)partitions));
            trigPoints.push_back(point);
        }
        return trigPoints;
    }

    void Triangulate()
    {
        CalculateBounds();
        std::cout << bounds << std::endl;

        std::vector<float> heights = std::vector<float>(partitions * partitions);

        std::vector<std::vector<Vector3>> pointClouds;
        SplitIntoSmallerClouds(pointClouds);

        // #pragma omp parallel for collapse(2) // Dont reallocate ANYTHING, dont emplace back, dont push to shered memory
        for (int i = 0; i < pointClouds.size(); ++i) {
            const float size = pointClouds[i].size();
            float average = 0;

            for (int j = 0; j < pointClouds[i].size(); ++j) {
                average += pointClouds[i][j].y() / size;
            }
            heights[i] = average;
        }

        // for (auto height : heights) {
        //     std::cout << height << std::endl;
        // }

        // Create the points, with correct xyz coordinates
        const float stepX = (bounds.LengthX() - stepLengthX) / (partitions - 1);
        const float stepZ = (bounds.LengthZ() - stepLengthZ) / (partitions - 1);
        std::vector<Vector3> trigPoints = HeightIntoRegularGrid(heights, stepX, stepZ);

        // center this regular grid
        const float xMin = 0;
        const float xMax = stepX * static_cast<float>(partitions - 1);
        const float zMin = 0;
        const float zMax = stepZ * static_cast<float>(partitions - 1);

        const Vector3 regularGridOffsetVector = Vector3(xMax + xMin, 0, zMax + zMin) / 2.0f;

        for (Vector3& point : trigPoints) {
            point -= regularGridOffsetVector;
        }

        WriteToFile("Vertices.txt", trigPoints);
        // Calculate Triangle and neighbour info

        const int pointPerSide = partitions;
        const int quadsPerSide = partitions-1;
        const int totalNumQuads = quadsPerSide * quadsPerSide;


        std::vector<FIndicesAndNeighbour> indicesAndNeighbours{};
        for (int i = 0; i < quadsPerSide - 1; ++i) {
        // int i = 0;
            for (int j = 0; j < quadsPerSide - 1; ++j) {
                int SW = i * pointPerSide + j;
                int SE = i * pointPerSide + j + 1;
                int NW = (i+1) * pointPerSide + j;
                int NE = (i+1) * pointPerSide + j + 1;

                FIndicesAndNeighbour TriOne;
                TriOne.indices[0] = SW;
                TriOne.indices[1] = NE;
                TriOne.indices[2] = SE;
                FIndicesAndNeighbour TriTwo;
                TriTwo.indices[0] = SW;
                TriTwo.indices[1] = NW;
                TriTwo.indices[2] = NE;
                indicesAndNeighbours.push_back(TriOne);
                indicesAndNeighbours.push_back(TriTwo);
                std::cout << TriOne << std::endl;
                std::cout << TriTwo << std::endl;
                
            }
        }

        WriteToFile("indicesAndNeighbours.txt", indicesAndNeighbours);

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
