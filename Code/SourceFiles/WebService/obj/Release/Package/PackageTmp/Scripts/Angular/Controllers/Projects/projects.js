
angular.module('initProjApp').controller('ProjectsCtrl', function ($scope, $http) {
    $http.get('/api/projects').success(function(data) {
        $scope.projects = data;
    });
});