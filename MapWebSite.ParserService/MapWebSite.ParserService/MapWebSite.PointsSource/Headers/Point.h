#pragma once
#include <string>
#include <vector>
#include "Displacement.h"

namespace MapWebSite {
	namespace PointsSource {
		class BasicPoint {
		public:

		protected:
			int number;
			float longitude;
			float latitude;
		};
	
	
		class Point : public BasicPoint {
		public:

		private:
			float referenceImageX;
			float referenceImageY;
			float height;
			float deformationRate;
			float standardDeviation;
			float estimatedHeight;
			float estimatedDeformationRate;
			std::string observations;
			std::vector<Displacement> displacements;
		};
	
	
	}
}