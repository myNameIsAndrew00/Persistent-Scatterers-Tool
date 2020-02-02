//use this class to handle which points regions are displayed on map
class PointsRegionsManager {

    constructor() {
        //constants related to server points cache manager 
        this.pointsRegions = {};
    }

    ResetCache() {
        this.pointsRegions = {};
        //todo: create a policy to handle points
    }

    AddRegion(cacheKey, points) {
        this.pointsRegions[cacheKey] = points;
    }

    /**
     * Get the regions keys available in cache
     * @param {any} regionsKeys If the parameter is set, get the cached regions keys from a range of keys
     */
    GetRegionsKeys(regionsKeys) {
        var result = [];

        if (regionsKeys == null)
            Object.keys(this.pointsRegions)
                .forEach(key => result.push(key));
        else
            for (var i = 0; i < regionsKeys.length; i++)
                if (Object.prototype.hasOwnProperty.call(this.pointsRegions, regionsKeys[i]))
                    result.push(regionsKeys[i]);

        return result;
    }

    /**
     * Get the regions which are cached
     * @param {any} regionsKeys If the parameter is set, get the cached regions with the keys from a range of keys 
     */
    GetRegions(regionsKeys) {
        var result = [];           
        //todo: return all if need
        for (var i = 0; i < regionsKeys.length; i++)
            result.push(...this.pointsRegions[regionsKeys[i]]);

        return result;
    }

   
}


export { PointsRegionsManager };