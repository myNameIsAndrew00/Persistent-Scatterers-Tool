#pragma once
#include <string>
#include <vector>
#include "Point.h"

namespace MapWebSite {
	namespace PointsSource {
		class PointsDataSet {
		public:


		private:
			int id;
			std::string name;
			long zoomLevel;
			std::vector<Point> points;
		};
	}
}