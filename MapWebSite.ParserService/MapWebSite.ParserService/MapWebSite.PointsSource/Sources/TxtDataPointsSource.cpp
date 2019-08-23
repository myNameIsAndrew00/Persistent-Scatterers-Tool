#include "../Headers/TxtDataPointsSource.h"
#include <fstream>

namespace MapWebSite {
	namespace PointsSource {

#pragma region Setters / Getters
		
		void TxtDataPointsSource::SetHeaderFile(const std::string& HeaderFile)
		{
			this->headerFile = HeaderFile;
		}

		void TxtDataPointsSource::SetDisplacementsFile(const std::string& DisplacementsFile)
		{
			this->displacementsFile = DisplacementsFile;
		}

		

#pragma endregion


#pragma region Public

		std::unique_ptr<PointsDataSet> MapWebSite::PointsSource::TxtDataPointsSource::CreateDataSet(const std::string& datasetName) const
		{
			std::unique_ptr<PointsDataSet> result(new PointsDataSet);

			if (this->headerFile.empty() || this->displacementsFile.empty()) return nullptr;

			return result;
		}

		

#pragma endregion



#pragma region Private

		std::vector<HeaderData> TxtDataPointsSource::parseHeaderData() const
		{
			std::vector<HeaderData> result;
			std::ifstream fileHandler(headerFile);
			std::string line;

			for (int index = 0; index < headerUnusedLinesCount; index++)
				std::getline(fileHandler, line);

			while (std::getline(fileHandler, line)) {
				 

			}

			return result;
		}

#pragma endregion
	}
}
