'use strict';

/**
 * @ngdoc function
 * @name initProjApp.controller:AboutCtrl
 * @description
 * # AboutCtrl
 * Controller of the initProjApp
 */
angular.module('initProjApp')
  .controller('AlternativeCtrl', function ($scope) {
    $scope.awesomeThings = [
      'HTML5 Boilerplate',
      'AngularJS',
      'Karma'
    ];
  });
