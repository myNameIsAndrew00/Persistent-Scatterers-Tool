#pragma once 
#include <memory>
#include <string>
#include "../Headers/PointsDataSet.h"

namespace MapWebSite{ 
	namespace PointsSource {
		 
		class IDataPointsSource abstract {
		public:
			virtual std::unique_ptr<PointsDataSet> CreateDataSet(const std::string& datasetName) const = 0;
		};

	}
}