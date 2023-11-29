#include <fstream>
#include <iostream>
#include <vector>

#include "DataStructures/Triangulator.h"
#include "DataStructures/Vector3.h"

// Reads on the data from merged.txt and stores it in the outPoints vector.
void ReadInData(std::vector<Vector3>& outPoints)
{
    std::ifstream inputFile;
    inputFile.open("merged.txt");
    int lines{};
    // int skip = 0;
    if (!inputFile.is_open()) {
        std::cout << "File could not be opened!\n";
        return;
    }
    
    float x, y, z;
    while (inputFile >> x >> y >> z) {
        lines++;

        // Emplace vs Push_back
        // Emplace is better because it calls the constructor in place.
        // It calls the constructor for you in place. You only have to give the constructor args.
        outPoints.emplace_back(x, z, y);
    }
    inputFile.close();
}

// Writes the points to a file in.
void WriteToPointCloud(const std::vector<Vector3>& points, const std::string& filename)
{
    std::ofstream outFile(filename);
    if (!outFile.is_open()) {
        std::cout << "File could not be opened or created!\n";
        return;
    }
    outFile << points.size();
    for (auto point : points) {
        // std::cout << point << std::endl;
        outFile << "\n" << point;
    }
    outFile.close();
}

void WritePointCloudToFile(const Triangulator& triangulator)
{
    // TASK 2.1
    triangulator.WriteToFile("PointCloud.txt", triangulator._pointCloud);
}

void TriangulateAndWriteToFile(Triangulator& triangulator)
{

    // TASK 2.3
    triangulator.Triangulate();
}


int main(int argc, char* argv[])
{
    // Write to file
    std::vector<Vector3> points{};
    // TASK 2.1
    ReadInData(points);
    std::cout << "Finished Reading File!" << std::endl;

    // Creating triangulator
    Triangulator triangulator = Triangulator(points, 25);
    triangulator.CenterPointCloud();
    
    WritePointCloudToFile(triangulator);
    std::cout << "Finished Writing Point Cloud File!" << std::endl;


    TriangulateAndWriteToFile(triangulator);
    std::cout << "Finished Writing Vertices and Indices Files!" << std::endl;


    return 0;
}
