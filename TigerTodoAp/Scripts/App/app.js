'use strict';

/****
 * Author: Sudeep
 ***/

// Declaring root module
angular.module('todoTigerApp', [
  'ngRoute',
  'ngTable',
  'ui.bootstrap',
  'todoTigerApp.todoHome'
]).config(['$routeProvider',
    function ($routeProvider) {
        $routeProvider.otherwise({
            redirectTo: '/todos'
        });
    }]).controller('todoConfirmErrorModalCtrl', ['$scope', '$timeout', '$modalInstance', 'params',
    function ($scope, $timeout, $modalInstance, params) {
        $scope.params = params;
        //After clicking ok button
        $scope.ok = function () {
            $modalInstance.close('OK');
        };
        //cancel Modal
        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        }
        $timeout(function () {
            if ($scope.params.isDone == false)
                $modalInstance.dismiss('cancel');
        }, 3000);
    }]).directive('confirmErrorClick', ['$modal', '$window',
    function ($modal, $window) {
        //Directive for handling popup alert on error / confirmation
        return {
            restrict: 'EA',
            piority: 1,
            scope: {
                isDone: '=',
                clickAction: '&confirmAction'
            },
            link: function (scope, iElement, iAttrs) {
                /*var clickAction = iAttrs.confirmAction || function(){alert("U clicked")};*/
                var msg = iAttrs.confirmMsg || "Are you sure?";
                var errorTitle = iAttrs.errorTitle;
                var errorMsg = iAttrs.errorMsg;
                iElement.bind('click', function (event) {
                    $window.scrollTo(0, 0);
                    var modalInstance = $modal.open({
                        controller: 'todoConfirmErrorModalCtrl',
                        templateUrl: function (params) { return '/home/confirm'; },
                        scope: scope,
                        resolve: {
                            params: function () {
                                return {
                                    title: 'Are you confirmed to clear?',
                                    msg: msg,
                                    isDone: scope.isDone,
                                    errMsg: errorMsg,
                                    errTitle: errorTitle
                                };
                            }
                        }
                    }).result.then(function (returnValue) {
                        if (scope.isDone)
                            scope.$eval(scope.clickAction);
                        console.log(' ok');
                    }, function () {
                        console.log(' cancel');
                    });
                });
            }
        }
    }]);