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
    while (inputFile >> x >> y >> z) {
        lines++;

        // Emplace vs Push_back
        // BAD -> outPoints.emplace_back(Vector3(x,z,y));
        outPoints.emplace_back(x,z,y); // It calls the constructor for you in place. You only have to give the constructor args.
        // std::cout << outPoints[outPoints.size() - 1] <<std::endl;
    }
    inputFile.close();
}

void WriteToPointCloud(const std::vector<Vector3>& points, const std::string& filename) {
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

void TriangulateAndWriteToFile(std::vector<Vector3>& points) {
    Triangulator triangulator = Triangulator(points, 25);
    triangulator.CenterPointCloud();
    // triangulator.WriteToFile("PointCloud.txt", triangulator._pointCloud);
    triangulator.Triangulate();
    
}



int main(int argc, char* argv[]) {
    // Write to file
    std::vector<Vector3> points{};
    ReadInData(points);
    std::cout << "Finished Reading File!" << std::endl;
    
    // std::cout << "Finished Writing File!" << std::endl;

    TriangulateAndWriteToFile(points);
    
    return 0;
}
