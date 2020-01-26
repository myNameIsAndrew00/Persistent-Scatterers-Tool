#pragma once
#include <vector>
#include <string>
#include "../Headers/PointsDataSet.h"

namespace MapWebSite {

	namespace PointsSource {
		 
		
		class IDataPointsZoomLevelsGenerator abstract {
		public:
			virtual std::vector<PointsDataSet> CreateDataSet(const PointsDataSet& originalDataSet, const int minZoomLevel, const int maxZoomLevel) = 0;
		};

	}
}