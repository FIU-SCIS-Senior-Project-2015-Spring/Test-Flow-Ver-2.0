'use strict';

/**
 * @ngdoc overview
 * @name initProjApp
 * @description
 * # initProjApp
 *
 * Main module of the application.
 */
angular
  .module('initProjApp', [
    'ngAnimate',
    'ngCookies',
    'ngResource',
    'ngRoute',
    'ngSanitize',
    'ngTouch',
    'angularLocalStorage',
    'textAngular',
  ])
  .config(function ($routeProvider) {
    $routeProvider
      .when('/', {
          templateUrl: '/AngularViews/Workspace/workspace.html',
        controller: 'WorkspaceCtrl'
      })
    .when('/workspace', {
        templateUrl: '/AngularViews/Workspace/workspace.html',
        controller: 'WorkspaceCtrl'
    })
      .when('/alternative', {
        templateUrl: 'views/alternative.html',
        controller: 'AlternativeCtrl'
      })
      .when('/analytics', {
        templateUrl: 'views/analytics.html',
        controller: 'AnalyticsCtrl'
      })
	  .when('/testCases2', {
        templateUrl: 'views/testCases2.html',
       
      })
      .otherwise({
        redirectTo: '/'
      });
  });
