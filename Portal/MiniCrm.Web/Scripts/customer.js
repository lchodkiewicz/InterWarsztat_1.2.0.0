angular.module('customerScope', [])
.controller('CustomerController', ['$scope', function ($scope) {


   
        $scope.friendz = [
            { Name: 'John', UserName: 'Jhn', PhoneNumber: '1', Email: 'a@a.pl' },
            { Name: 'Adam', UserName: 'Ada', PhoneNumber: '2', Email: 'b@b.pl' },
            { Name: 'Ewa', UserName: 'Ev', PhoneNumber: '3', Email: 'c@c.pl' },
            { Name: 'A1', UserName: 'A', PhoneNumber: '9', Email: 'd@d.pl' },
            { Name: 'B1', UserName: 'B', PhoneNumber: '5', Email: 'e@e.pl' },
            { Name: 'C44', UserName: 'C', PhoneNumber: '6', Email: 'f@f.pl' },
            { Name: 'SD', UserName: 'DS', PhoneNumber: '7', Email: 'g@g.pl' },
            { Name: '!@3', UserName: '123', PhoneNumber: '8', Email: 'h@h.pl' },
            { Name: 'TG', UserName: 'HB', PhoneNumber: '9', Email: 'i@i.pl' },
    ]

    $scope.checkQuery2 = function () {
        if ($scope.query2.length == 0) {
            $scope.query2 = undefined;
        }

    };


}]);

