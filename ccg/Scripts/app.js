var app = angular.module('ccgApp', ['ngMap', 'angular-loading-bar', 'ngMaterial', 'angular.filter', 'googlechart', 'slick']);

app.factory('ccgFactory', ['$http', function ($http) {
    return {
        getNews: function () {
            return $http.get('/Data/GetNews');
        },
        getCaseStudies: function () {
            return $http.get('/Data/GetCaseStudies');
        },
        getLocations: function () {
            return $http.get('/Data/GetLocations');
        }
    };
}]);

app.directive('newsCard', newsCardDirective);
function newsCardDirective() {
    return {
        restrict: 'E',
        templateUrl: '/templates/newscard'
    };
}

app.directive('caseStudiesCard', caseStudiesCardDirective);
function caseStudiesCardDirective() {
    return {
        restrict: 'E',
        templateUrl: '/templates/casestudiescard'
    };
}

app.directive('locationsCard', locationsCardDirective);
function locationsCardDirective() {
    return {
        restrict: 'E',
        templateUrl: '/templates/locationscard'
    };
}

app.directive('slideToggle', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var target, content;
            attrs.expanded = attrs.expanded === "true" ? true : false;

            element.bind('click', function (e) {
                if (!attrs.slideToggle) {
                    if (!target) target = $(this).parent().find(".slideable")[0];
                } else {
                    if (!target) target = document.querySelector(attrs.slideToggle);
                }
                if (!content) content = target.querySelector('.slideable_content');
                if (!attrs.expanded) {
                    content.style.border = '1px solid rgba(0,0,0,0)';
                    var y = content.clientHeight;
                    content.style.border = 0;
                    target.style.height = y + 'px';
                } else {
                    target.style.height = '100px';
                }
                attrs.expanded = !attrs.expanded;
            });
        }
    };
});

app.directive('slideable', function () {
    return {
        restrict: 'C',
        compile: function (element, attr) {
            // wrap tag
            var contents = element.html();
            element.html('<div class="slideable_content" style="margin:0 !important; padding:0 !important" >' + contents + '</div>');

            return function postLink(scope, element, attrs) {
                // default properties
                attrs.duration = !attrs.duration ? '1s' : attrs.duration;
                attrs.easing = !attrs.easing ? 'ease-in-out' : attrs.easing;
                var startHeight = "100px";
                //if (!attrs.showonload) {

                //    var y = content.clientHeight;
                //    startHeight = y + 'px';
                //}
                element.css({
                    'overflow': 'hidden',
                    'height': startHeight,
                    'transitionProperty': 'height',
                    'transitionDuration': attrs.duration,
                    'transitionTimingFunction': attrs.easing,
                    'background': 'rgba(0, 0, 0, 0)'
                });
            };
        }
    };
});

app.filter("trust", ['$sce', function ($sce) {
    return function (htmlCode) {
        return $sce.trustAsHtml(htmlCode);
    };
}]);

app.controller('ccgController', ccgController);

ccgController.$inject = ['$scope', '$window', '$interval', 'ccgFactory', 'NgMap'];

function ccgController($scope, $window, $interval, ccgFactory, NgMap) {
    console.log('home controller', Date());
    var heatmap, vm = this;
    var topicsPageNum = 1;
    var productsPageNum = 1;
    $scope.showcard = 3;
    $scope.searching = false;
    $scope.currentNavItem = 'page3';
    $scope.showMarkers = true;
    $scope.showHeatmap = false;
    $scope.topicsLimit = 12;
    var colorArray = ['#1baae5', '#195772', '#0378aa', '#3e7187'];

    function doInit() {
        getNews();
        getCaseStudies();
        getLocations();
    }

    function checkWidth() {

        if ($window.innerWidth < 767) {
            $scope.chartHeight = "190px";
            $scope.isMobile = true;
        } else {
            $scope.chartHeight = "200px";
            $scope.isMobile = false;
        }

    }
    checkWidth();

    function groupArray(arr, field) {
        var counts = arr.reduce((p, c) => {
            var name = c[field];
            if (!p.hasOwnProperty(name)) {
                p[name] = 0;
            }
            p[name]++;
            return p;
        }, {});
        return counts;
    }
    
    var formatChart = function (obj, chartType, category) {
        $scope[chartType].data.rows = [];
        // group array by field
        var counts = groupArray(obj, category);

        // map grouped array into usable object
        var countsExtended = Object.keys(counts).map(k => {
            return { name: k, count: counts[k] };
        });

        var i = 0;

        countsExtended.forEach(function (item) {
            var objToPush = {
                c: [
                    { v: item.name },
                    { v: item.count },
                    { v: colorArray[i % colorArray.length] },
                    { v: item.count }
                ]
            };
            i++;
            $scope[chartType].data.rows.push(objToPush);
        });
    };

    
    var getCaseStudies = function () {
        $scope.searching = true;
        ccgFactory.getCaseStudies().then(function (res) {
            console.log('got ccg case studies', res);
            $scope.searching = false;
            $scope.caseStudiesPretty = JSON.stringify(res, null, 2);
            $scope.caseStudies = res.data.casestudies;
            $scope.caseStudiesCategories = [...new Set($scope.caseStudies.map(item => item.Category))];
            
        }).catch(function (e) {
            console.log('error getting ccg topic', e);
        });
    };


    var getNews = function () {
        $scope.searching = true;
        ccgFactory.getNews().then(function (res) {
            console.log('got ccg news', res);
            $scope.searching = false;
            $scope.newsPretty = JSON.stringify(res, null, 2);
            $scope.topics = res.data.news;
            $scope.topicCategories = [...new Set($scope.topics.map(item => item.Date))];
        }).catch(function (e) {
            console.log('error getting ccg news', e);
        });
    };


    var getLocations = function () {
        $scope.searching = true;
        ccgFactory.getLocations().then(function (res) {
            console.log('got ccg locations', res);
            $scope.searching = false;
            $scope.locationsPretty = JSON.stringify(res, null, 2);
            $scope.locations = res.data.locations;
        }).catch(function (e) {
            console.log('error getting ccg');
        });
    };


    angular.element($window).bind("resize", function () {
        var iw = $window.innerWidth;
        console.log(iw);
        if (iw < 767) {
            $scope.isMobile = true;
        } else {
            $scope.isMobile = false;
        }
        $scope.$apply();
        $scope.onMapLoaded();
    });

    $scope.getMore = function () {
        switch ($scope.showcard) {
            case 1:
                if ($scope.topicsLimit < $scope.topics.length) {
                    $scope.topicsLimit = $scope.topicsLimit + 12;
                }
                break;
            case 2:
                productsPageNum++;
                getProducts(true);
                break;
        }
    };

    $scope.toggleExpanded = function (e, item) {
        e.stopPropagation();
        item.expanded = !item.expanded;
    };

    $scope.goToUrl = function (url) {
        $window.open(url);
    };

    $scope.catSelectionChanged = function () {
        //if All was selected deselect all others
        if (this.topicCategoryFilter.indexOf('All') !== -1) {
            this.topicCategoryFilter = undefined;
            $(".md-select-menu-container").hide();
        }
    };
    

    $scope.filterTopic = function (val, type) {
        //filter by first selected categories
        var groupBy = "Date";
        if (!$scope.topics) return;
        var returnData = $scope.topics.slice(0, $scope.topicsLimit);


        if (type && type.length > 0) {
            returnData = returnData.filter(function (item) {
                return type.indexOf(item[groupBy]) !== -1;
            });
        }

        //filter by user entered search string and look for it in Title and Description
        if (val) {
            returnData = returnData.filter(x => x.Title.toLowerCase().indexOf(val.toLowerCase()) > -1
                || x.Description.toLowerCase().indexOf(val.toLowerCase()) > -1);
        }

        $scope.showing = returnData.length;
        //if (!scope.showing) $scope.showing = 0;
        if ($scope.showing > $scope.topicsLimit) $scope.showing = $scope.topicsLimit;
        formatChart(returnData, 'myTopicChartObject', groupBy);
        return returnData;
    };

    $scope.filterCaseStudy = function (val, type) {
        //filter by first selected categories
        if (!$scope.caseStudies) return;
        var returnData = $scope.caseStudies;

        if (type && type.length > 0) {
            returnData = returnData.filter(function (item) {
                return type.indexOf(item.Category) !== -1;
            });
        }

        //filter by user entered search string and look for it in Title and Description
        if (val) {
            returnData = returnData.filter(x => x.Title.toLowerCase().indexOf(val.toLowerCase()) > -1
                || x.Description.toLowerCase().indexOf(val.toLowerCase()) > -1);
        }

        $scope.showing = returnData.length;
        formatChart(returnData, 'myCaseStudyChartObject', 'Category');
        return returnData;
    };

    
    $scope.showDetail = function (e, item) {
        vm.selectedEvent = item;
        vm.map.showInfoWindow('map-iw', this);
    };

    
    $scope.scrollTo = function (el) {
        $(el).goTo();
    };


    // chart section
    $scope.myTopicChartObject = {};

    $scope.myTopicChartObject.type = "ColumnChart";

    $scope.myTopicChartObject.data = {
        "cols": [
            { id: "t", label: "Date", type: "string" },
            { id: "s", label: "Articles", type: "number" },
            { role: "style", type: "string" },
            { role: "annotation", type: "string" }
        ], "rows": []            
    };

    $scope.myTopicChartObject.options = {
        'title': '# of Articles By Date',
        "vAxis": {
            showTextEvery: 1,
            textStyle: { fontSize: 9 },
            gridlines: { color: 'transparent' }
        },
        "hAxis": {
            showTextEvery: 1,
            textStyle: { fontSize: 10 }
        },
        "legend": { position: "none" },
        'chartArea': { 'width': '100%', 'height': '80%'},
        'backgroundColor': '#fafafa',
        'animation': {
            'duration': 1000,
            'easing': 'out',
            'startup': true
        }
    };


    $scope.myCaseStudyChartObject = {};

    $scope.myCaseStudyChartObject.type = "ColumnChart";

    $scope.myCaseStudyChartObject.data = {
        "cols": [
            { id: "t", label: "Category", type: "string" },
            { id: "s", label: "Case Studies", type: "number" },
            { role: "style", type: "string" },
            { role: "annotation", type: "string" }
        ], "rows": []
    };

    $scope.myCaseStudyChartObject.options = {
        'title': '# of Articles By Category',
        "vAxis": {
            showTextEvery: 1,
            textStyle: { fontSize: 9 },
            gridlines: { color: 'transparent' }
        },
        "hAxis": {
            showTextEvery: 1,
            textStyle: { fontSize: 10 }
        },
        "legend": { position: "none" },
        'chartArea': { 'width': '100%', 'height': '80%' },
        'backgroundColor': '#fafafa',
        'animation': {
            'duration': 1000,
            'easing': 'out',
            'startup': true
        }
    };


    //map section
    $scope.toggleMarkers = function () {
        $scope.showMarkers = !$scope.showMarkers;
    };

    $scope.onMapLoaded = function () {
        NgMap.getMap({ id: 'big-map' }).then(function (map) {
            vm.map = map;
            map.setCenter({ lat: 40.89792939468468, lng: -74.31821335000001 });
        });
    };

    $scope.goTo = function (i) {
        $scope.showcard = i;
    };

    doInit();
}

$(function () {
    $.fn.goTo = function () {
        $('html, body').animate({
            scrollTop: $(this).offset().top - 50 + 'px'
        }, 'fast');
        $(this).focus();
        return this; // for chaining...
    };
});