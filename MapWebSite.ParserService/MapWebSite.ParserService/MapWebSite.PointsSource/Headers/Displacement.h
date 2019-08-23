#pragma once
#include <string>

namespace MapWebSite {
	namespace PointsSource {
		class Displacement {
		public:

		private:
			time_t date;
			float jd;
			float daysFromReference;
			float value;
		};

	}
}