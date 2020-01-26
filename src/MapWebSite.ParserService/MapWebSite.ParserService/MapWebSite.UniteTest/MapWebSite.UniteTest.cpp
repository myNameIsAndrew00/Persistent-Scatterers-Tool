#include "pch.h"
#include "CppUnitTest.h"
#include <PointsDataSet.h>
#include <TxtDataPointsSource.h>
using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace MapWebSite::PointsSource;

namespace MapWebSiteUniteTest
{
	TEST_CLASS(MapWebSiteUniteTest)
	{
	public:
		
		TEST_METHOD(TestMethod1)
		{
			TxtDataPointsSource source;
			source.SetDisplacementsFile("rada");
			 
		}
	};
}
