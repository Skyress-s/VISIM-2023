#include <conio.h>
#include <fstream>
#include <iostream>
#include <vector>

#include "DataStructures/Triangulator.h"
#include "DataStructures/Vector3.h"

void ReadInData(std::vector<Vector3>& outPoints) {
    std::ifstream inputFile;
    inputFile.open("merged.txt");
    int lines{};
    int skip = 0;
    if (!inputFile.is_open()) {
        std::cout << "File could not be opened!\n";
        return;
    }
    float x, y, z;
    Vector3 v;
    // while (inputFile >> x >> y >> z) {
    while (inputFile >> v) {
        skip++;
        // if (skip < 15000)
        //     continue;
        skip = 0.f;
        lines++;
        // outPoints.emplace_back(Vector3(x,z,y));
        outPoints.emplace_back(v);
    }
    inputFile.close();
}

void WriteToPointCloud(const std::vector<Vector3>& points) {
    std::ofstream outFile("output.txt");
    if (!outFile.is_open()) {
        std::cout << "File could not be opened or created!\n";
        return;
    }
    outFile << points.size();
    for (auto point : points) {
        outFile << "\n" << point;
    }
    outFile.close();
}

void TriangulateAndWriteToFile(std::vector<Vector3>& points) {
    Triangulator triangulator = Triangulator(points, 5);
    triangulator.Triangulate();
    
}



int main(int argc, char* argv[]) {
    // Write to file
    std::vector<Vector3> points{};
    ReadInData(points);
    std::cout << "Finished Reading File!" << std::endl;

    WriteToPointCloud(points);

    std::cout << "Finished Writing File!" << std::endl;

    TriangulateAndWriteToFile(points);
    
    return 0;
}
