angular.module('app.personal').service('estimateService', ['http', function (http) {
    this.getEmergencyModel = function() {
        var url = '/estimation/emergency';
        return http.get(url);
    };

    this.estimateEmergency = function(model) {
        var url = '/estimation/estimate/emergency';
        return http.post(url, model);
    }

    this.finishEmergency = function (model) {
        var url = '/estimation/emergency';
        return http.post(url, model);
    }


    this.getRetirementModel = function () {
        var url = '/estimation/retirement';
        return http.get(url);
    };

    this.estimateRetirement = function (model) {
        var url = '/estimation/estimate/retirement';
        return http.post(url, model);
    }

    this.finishRetirement = function (model) {
        var url = '/estimation/retirement';
        return http.post(url, model);
    }

    this.getHomeModel = function () {
        var url = '/estimation/home';
        return http.get(url);
    };

    this.estimateHome = function (model) {
        var url = '/estimation/estimate/home';
        return http.post(url, model);
    }

    this.finishHome = function (model) {
        var url = '/estimation/home';
        return http.post(url, model);
    }

    this.getCarModel = function () {
        var url = '/estimation/car';
        return http.get(url);
    };

    this.estimateCar = function (model) {
        var url = '/estimation/estimate/car';
        return http.post(url, model);
    }

    this.finishCar = function (model) {
        var url = '/estimation/car';
        return http.post(url, model);
    }

    this.getCollegeModel = function () {
        var url = '/estimation/college';
        return http.get(url);
    };

    this.estimateCollege = function (model) {
        var url = '/estimation/estimate/college';
        return http.post(url, model);
    }

    this.finishCollege = function (model) {
        var url = '/estimation/college';
        return http.post(url, model);
    }

    this.getTripModel = function () {
        var url = '/estimation/trip';
        return http.get(url);
    };

    this.estimateTrip = function (model) {
        var url = '/estimation/estimate/trip';
        return http.post(url, model);
    }

    this.finishTrip = function (model) {
        var url = '/estimation/trip';
        return http.post(url, model);
    }

    this.getImprovehomeModel = function () {
        var url = '/estimation/improvehome';
        return http.get(url);
    };

    this.estimateImprovehome = function (model) {
        var url = '/estimation/estimate/improvehome';
        return http.post(url, model);
    }

    this.finishImprovehome = function (model) {
        var url = '/estimation/improvehome';
        return http.post(url, model);
    }

    this.getCustomModel = function () {
        var url = '/estimation/custom';
        return http.get(url);
    };

    this.estimateCustom = function (model) {
        var url = '/estimation/estimate/custom';
        return http.post(url, model);
    }

    this.finishCustom = function (model) {
        var url = '/estimation/custom';
        return http.post(url, model);
    }

    this.estimateByAmmountAndDate = function(model) {
        var url = '/estimation/estimatebydateamount';
        return http.post(url, model);
    };

    this.estimateByMonthlyPayment = function (model) {
        var url = '/estimation/estimatebypayment';
        return http.post(url, model);
    };
}]);