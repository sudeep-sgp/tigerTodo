'use strict';

/****
* Author: Sudeep
***/

//sub module to hold todo controllers
angular.module('todoTigerApp.todoHome', ['ngRoute', 'ui.bootstrap', 'restangular'])
//routing to load todo view html
.config(['$routeProvider', function ($routeProvider, $q) {
    $routeProvider.when('/todos', {
        templateUrl: 'home/todos',
        controller: 'todoHomeCtrl',
        resolve: {
            allTodoList: function (Restangular, $q) {
                var $deferred = $q.defer();
                var allTodos;
                Restangular.one('home', 'alltodos').get().then(function (allTodos) {
                    allTodos = allTodos;
                    $deferred.resolve(allTodos);
                });

                return $deferred.promise;
            }

        }
    });//defining todoHomeCtrl to hold all client side business logic
}]).controller('todoHomeCtrl', ['$scope', '$rootScope', '$timeout', '$document', '$filter', '$q', 'ngTableParams', 'Restangular', 'allTodoList', function ($scope, $rootScope, $timeout, $document, $filter, $q, ngTableParams, Restangular, allTodoList) {

    $scope.allTodos;
    var todosList = allTodoList || [{ id: 1, isSelected: false, todoName: 'Sudeep\'s todo Test', todoNote: 'Testing todo', created: '8-12-2014 15:13', lastModified: '8-12-2014 15:13' }];;

    $scope.todos = allTodoList;

    $scope.$watch('todos', function (newVal) {
        $rootScope.totalTodos = todosList.length;
    });
    //defining model to hold and process all data, better to create a factory for handling this, ng-table used for populating data table
    $scope.todoTblParams = new ngTableParams({
        page: 1,            // show first page
        count: 5           // count per pagetodo
    }, {
        total: todosList.length, // length of data
        getData: function ($defer, params) {

            Restangular.one('home', 'alltodos').get().then(function (allTodos) {

                var sortedTodos = params.sorting() ?
                   $filter('orderBy')(allTodos, params.orderBy()) :
                   todosList;
                var filteredTodos = params.filter() ?
                        $filter('filter')(sortedTodos, params.filter()) :
                        allTodos;

                params.total(filteredTodos.length); // set total for recalc pagination
                $scope.todos = filteredTodos.slice((params.page() - 1) * params.count(), params.page() * params.count());
                if (filteredTodos) {
                    $timeout(function () { $scope.todos = filteredTodos.slice((params.page() - 1) * params.count(), params.page() * params.count()) }, 1000);
                } else {
                    $scope.todos = filteredTodos.slice((params.page() - 1) * params.count(), params.page() * params.count());
                }
                $defer.resolve($scope.todos);

                params.total($scope.todos.totalElements);
                if (_.isEmpty(allTodos)) {
                    $scope.noDataMsg = $scope.noResultFoundMsg;
                }
                $scope.isLoading = false;


            }, function (error) {
                $scope.isLoading = false;
                console.log(error);
            });




        }
    });

    $scope.controlFilter = function (todoName) {
        var defered = $q.defer(),
        valueToFilter = [];
        if (todoName && todoName.length > 2)
            valueToFilter.push(todoName);
        defered.resolve(valueToFilter);
        return defered;
    };

    //To0l tip setting based on the todo status
    $scope.setToolTip = function (isDone) {
        isDone ? $scope.toolTipContent = 'Double click here to mark as undone' : $scope.toolTipContent = 'Double click here to mark as done';
    }

    $scope.isTodoError = true;
    //formating date dd mm yyyy HH:mm
    Date.prototype.format = function () {
        var year = this.getFullYear().toString();
        var month = (this.getMonth() + 1).toString();
        var date = this.getDate().toString();
        var time = this.getHours().toString() + ':' + this.getMinutes().toString();
        return date + '-' + month + '-' + year + ' ' + time;
    };

    //saving newly entered todo
    $scope.saveNewTodo = function (form) {
        if ($scope.tigerTodo.todoNote && $scope.tigerTodo.todoName) {
            var postParam = {};
            var createdDate = new Date();
            var modifiedDate = new Date();
            var todoNote = $scope.tigerTodo.todoNote.replace(/[\W_]+/g, ' ').trim().slice(0, 14);
            if ($scope.tigerTodo.todoNote.search(/[\W_]+/g) > 0) {
                $scope.todoWarning = 'We have truncated unwanted symbols and space in note field!!';
                $scope.todoSuccess = undefined;
            } else {
                $scope.todoSuccess = 'Success, New todo created!!';
                $scope.todoWarning = undefined;
            }
            $scope.todoError = undefined;
            postParam = {
                isSelected: false, todoName: $scope.tigerTodo.todoName, todoNote: todoNote,
                created: createdDate, lastModified: modifiedDate
            };
            Restangular.one('home').post('save', postParam).then(function (todo) {
                todosList.push(todo);
                console.log(todo);
                $scope.todoTblParams.reload();
                $scope.remainingTodos();
            });

        } else {
            $scope.todoError = 'Sorry, You have missed to key in Name and Note fields!!';
            form['todoName'].$setValidity('', false);
            form['todoNote'].$setValidity('', false);
            form['todoName'].$dirty = true;
            form['todoNote'].$dirty = true;
            $timeout(function () {
                form['todoName'].$setValidity('', true);
                form['todoNote'].$setValidity('', true);
                form['todoName'].$dirty = false;
                form['todoNote'].$dirty = false;
            }, 3000)
            $scope.todoWarning = undefined;
            $scope.todoSuccess = undefined;

        }
    }

    //saving edited todo
    $scope.saveEditedTodo = function (todo) {
        if (todo) {
            //var todoToBeEdited = _.find(todosList, { id: todo.id });
            //todoToBeEdited.todoName = todo.todoName;
            //todoToBeEdited.todoNote = todo.todoNote;
            //todoToBeEdited.lastModified = new Date().format();

           var postParam = {
                id: todo.id, isSelected: todo.isSelected, created: todo.created,
                todoName: todo.todoName, todoNote: todo.todoNote
            };
            Restangular.one('home').post('update', postParam).then(function (todo) {
                //todosList.push(todo);
                console.log(todo);
                $scope.todoTblParams.reload();
                $scope.remainingTodos();
            });
          
            $scope.todoError = undefined;
            $scope.todoWarning = undefined;
            if (todo.todoNote.search(/[\W_]+/g) > 0) {
                $scope.todoWarning = 'We have truncated unwanted symbols and space in note field!!';
                $scope.todoSuccess = undefined;
            } else {
                $scope.todoSuccess = todoToBeEdited.todoName + ' updated!!';
                $scope.todoWarning = undefined;
            }
           
        } else {
            $scope.todoError = 'Error occured in editing!!';
            $scope.todoWarning = undefined;
            $scope.todoSuccess = undefined;
        }

    }

    //deleting single or multiple todos
    $scope.clearDoneTodos = function (todo, clearType) {
        if (todo && clearType == 'one' && todo.isSelected) {
            Restangular.one('home').post('delete', { id: todo.id }).then(function (todo) {
                $scope.isSuccessDelete = todo.success;
                console.log(todo);
                if ($scope.isSuccessDelete)
                    todosList = _.reject(todosList, { id: todo.id });
                $scope.remainingTodos();
                $scope.todoTblParams.reload()
            });
        } else if (clearType == 'all') {

            Restangular.one('home').post('deleteall').then(function (todo) {
                $scope.isSuccessDelete = todo.success;
                console.log(todo);
                if ($scope.isSuccessDelete)
                    todosList = _.reject(todosList, { id: todo.id });
                $scope.remainingTodos();
                $scope.todoTblParams.reload()
            });
            //todosList = _.reject(todosList, { isSelected: true });

        }
    }

    //switching the status
    $scope.switchDoneStatus = function (todo, type) {
        if (todo && type == 'one') {
            Restangular.one('home').post('switchstat', { id: todo.id }).then(function (todo) {
                $scope.isSuccessSwitch = todo.success;
                console.log(todo);
                var todoToBeMarked = _.find(todosList, { id: todo.id });
                $scope.isTaskDone = todo.isSelected;
                $scope.todoTblParams.reload();
            });

        } else if (type == 'allDone') {
            Restangular.one('home').post('doneall').then(function (todo) {
                $scope.isSuccessSwitch = todo.success;
                $scope.todoTblParams.reload();
            });

            //angular.forEach($scope.remainingTodos(), function (eachTodo) {
            //    eachTodo.isSelected = true;
            //});

        } else if (type == 'allUnDone') {

            Restangular.one('home').post('undoneall').then(function (todo) {
                $scope.isSuccessSwitch = todo.success;
                $scope.todoTblParams.reload();
            });


        }
    }

    //remaining todos
    $scope.remainingTodos = function () {
        return _.filter(todosList, { isSelected: false });
    }

    //done todos
    $scope.doneTodos = function () {
        return _.filter(todosList, { isSelected: true });
    }

    $timeout(function () {
        $document.find('.errorNotify').css("display", "none");
    }, 3000);
}]);