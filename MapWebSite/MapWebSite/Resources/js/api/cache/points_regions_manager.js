//use this class to handle which points regions are displayed on map
class PointsRegionsManager {

    constructor() {
        //constants related to server points cache manager
        this.latitudeSide = 0.05;
        this.longitudeSide = 0.10;

        this.pointsRegions = {};
    }

    ResetCache() {
        this.pointsRegions = {};
    }

    AddRegion(cacheKey, pointsCount, filled) {
        if (this.pointsRegions.hasOwnProperty(cacheKey)) {
            if (this.pointsRegions[cacheKey].pointsCount < pointsCount)
                this.pointsRegions[cacheKey] = { pointsCount, filled };
        }
        else this.pointsRegions[cacheKey] = { pointsCount, filled };
    }

    GetRegions(from, to, datasetId) {
        var result = [];
        var resultIndex = 0;

        var cached = true;

        var limits = this.getCacheSearchingLimits(from, to);
        var cacheKey = null;

        for (var i = limits.from.lat; i <= limits.to.lat; i += this.latitudeSide)
            for (var j = limits.from.long; j <= limits.to.long; j += this.longitudeSide) {
                cacheKey = this.getCacheKey({
                        lat: i.toFixed(2),
                        long: j.toFixed(2)
                    },{
                        lat: (i + this.latitudeSide).toFixed(2),
                        long: (j + this.longitudeSide).toFixed(2)
                    },
                    datasetId);
                if (this
                    .pointsRegions
                    .hasOwnProperty(cacheKey)) {

                    if (!this.pointsRegions[cacheKey].filled) cached = false;

                    result[resultIndex++] =
                        {
                            RegionKey: cacheKey,
                            RegionPointsCount: this.pointsRegions[cacheKey].pointsCount
                        }
                }
                else cached = false;

            }

        if (cached) return 'cached';
        return result;
    }

    /**
     *     
    /*internal purpose functions bellow */
    /*
     **/

    getCacheSearchingLimits(from, to) {
        var latSideInt = Math.floor(this.latitudeSide * 100);
        var longSideInt = Math.floor(this.longitudeSide * 100);

        var fromLatInt = Math.floor(from.lat * 100);
        var fromLongInt = Math.floor(from.long * 100);
        var toLatInt = Math.floor(to.lat * 100);
        var toLongInt = Math.floor(to.long * 100);

        return {
            from: {
                lat: ((fromLatInt - fromLatInt % latSideInt)).toFixed(6) / 100,
                long: ((fromLongInt - fromLongInt % longSideInt)).toFixed(6) / 100
            },
            to: {
                lat: ((toLatInt + (latSideInt - toLatInt % latSideInt))).toFixed(6) / 100,
                long: ((toLongInt + (longSideInt - toLongInt % longSideInt))).toFixed(6) / 100
            }
        }
    }

    getCacheKey(from, to, datasetId) {
        return from.lat.toString() + '_' + from.long.toString() +
            '_' + to.lat.toString() + '_' + to.long.toString() +
            '_' + datasetId.toString();
    }

}


export { PointsRegionsManager };