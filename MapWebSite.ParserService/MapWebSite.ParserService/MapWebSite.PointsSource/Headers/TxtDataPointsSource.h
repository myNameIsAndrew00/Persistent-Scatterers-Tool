#pragma once
#include "..//Interfaces/IDataPointsSource.h"
#include <tuple>

namespace MapWebSite {
	namespace PointsSource {
		using HeaderData = std::tuple<time_t, float, float>;

		class TxtDataPointsSource : public IDataPointsSource
		{
			static const int headerUnusedLinesCount = 10;

		public:
			std::unique_ptr<PointsDataSet> CreateDataSet(const std::string& datasetName) const override;

			void SetHeaderFile(const std::string& HeaderFile);
			void SetDisplacementsFile(const std::string& DisplacementsFile);

		private:
			std::string headerFile;
			std::string displacementsFile;

			std::vector<HeaderData> parseHeaderData() const;

		};

	}
}