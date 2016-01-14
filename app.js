(function(){
	var app = angular.module("app", []);

	var listas = [];

	app.controller("ListaController",["$http", "$scope", function($http, $scope){

		var listaCtrl = this;

		listaCtrl.nomeEditado = "";

		listaCtrl.novaLista = {"nome": "Nova Lista", "editando": false};

		listaCtrl.novoCard = {"nome": "Novo Card", "editando": false}; 

		$http.get("http://localhost:60684/listas").success(function(data){
			listaCtrl.listas = data;
			angular.forEach(listaCtrl.listas, function(lista, key){
				lista.editando = false;
				angular.forEach(lista.cards, function(card, key){
					card.editando = false;
				});
			});
			$scope.listas = listaCtrl.listas;
		});

		listaCtrl.addLista = function(){
			$http.post("http://localhost:60684/listas", listaCtrl.novaLista).success(function(data){
				listaCtrl.listas.push(data[0]);
			}).error(function(){
				alert("Deu Erro");
			});
		}

		listaCtrl.removeLista = function(lista){
			$http.delete("http://localhost:60684/listas/"+lista.id).success(function(data){

				var index = listaCtrl.listas.map(function(e) { return e.id; }).indexOf(lista.id);

				listaCtrl.listas.splice(index, 1);
				
			}).error(function(){
				alert("Deu Erro");
			});
		}

		listaCtrl.updateLista = function(lista){
			
			lista.nome = listaCtrl.nomeEditado;

			$http.put("http://localhost:60684/listas/"+lista.id, lista).success(function(data){

				var index = listaCtrl.listas.map(function(e) { return e.id; }).indexOf(lista.id);

				listaCtrl.listas[index] = lista;
				
			}).error(function(){
				alert("Deu Erro");
			});
		}

		listaCtrl.addCard = function(lista){
			listaCtrl.novoCard.id_lista = lista.id;

			$http.post("http://localhost:60684/cards", listaCtrl.novoCard).success(function(data){
				lista.cards.push(data[0]);
			}).error(function(){
				alert("Deu Erro");
			});
		}

		listaCtrl.removeCard = function(card, lista){
			$http.delete("http://localhost:60684/cards/"+card.id).success(function(data){

				var indexLista = listaCtrl.listas.map(function(e) { return e.id; }).indexOf(lista.id);
				var indexCard = listaCtrl.listas[indexLista].cards.map(function(e) { return e.id; }).indexOf(card.id);

				listaCtrl.listas[indexLista].cards.splice(indexCard, 1);
				
			}).error(function(){
				alert("Deu Erro");
			});
		}

		listaCtrl.updateCard = function(card, listaInicial){
			card.nome = listaCtrl.nomeEditado;

			$http.put("http://localhost:60684/cards/"+card.id, card).success(function(data){

				var indexLista = listaCtrl.listas.map(function(e) { return e.id; }).indexOf(listaInicial.id);
				var indexCard = listaCtrl.listas[indexLista].cards.map(function(e) { return e.id; }).indexOf(card.id);

				listaCtrl.listas[indexLista].cards[indexCard] = card;
				
			}).error(function(){
				alert("Deu Erro");
			});	
		}

		$scope.updateCard = function(card, listaInicial, listaFinal){
			
			card.id_lista = listaFinal.id;

			$http.put("http://localhost:60684/cards/"+card.id, card).success(function(data){
				
				var indexListaInicial = listaCtrl.listas.map(function(e) { return e.id; }).indexOf(listaInicial.id);
				var indexListaFinal = listaCtrl.listas.map(function(e) { return e.id; }).indexOf(listaFinal.id);
				var indexCard = listaCtrl.listas[indexListaInicial].cards.map(function(e) { return e.id; }).indexOf(card.id);

				listaCtrl.listas[indexListaInicial].cards.splice(indexCard, 1);
				listaCtrl.listas[indexListaFinal].cards.push(card);
			}).error(function(){
				alert("Deu Erro");
			});
		}
	}]);

	app.directive('focusOn',function($timeout) {
	    return {
	        restrict: 'A',
	        link: function($scope, $element, $attr) {
	            if ($attr.ngShow){
	                $scope.$watch($attr.ngShow, function(newValue){
	                    if(newValue){
	                        $timeout(function(){
	                            $element.focus();
	                        }, 0);
	                    }
	                })      
	            }
	            if ($attr.ngHide){
	                $scope.$watch($attr.ngHide, function(newValue){
	                    if(!newValue){
	                        $timeout(function(){
	                            $element.focus();
	                        }, 0);
	                    }
	                })      
	            }

	        }
	    };
	});

	app.directive('ngEnter', function() {
        return function(scope, element, attrs) {
            element.bind("keydown keypress", function(event) {
                if(event.which === 13) {
                    scope.$apply(function(){
                        scope.$eval(attrs.ngEnter, {'event': event});
                    });

                    event.preventDefault();
                }
            });
        };
    });

    app.directive('ngSortable', function($timeout) {
	    return {
	        restrict: 'A',
	        scope: false,
	        link: function($scope, element, attrs) {
	            $(element).sortable({
			        connectWith: ".cards",
			        items:"li:not(.placeholder)",
			        receive: function (event, ui) {

			        	var indexListaInicial = $scope.listas.map(function(e) { return e.id; }).indexOf(+ui.sender.attr("id"));
						var indexListaFinal = $scope.listas.map(function(e) { return e.id; }).indexOf(+this.id);
						var indexCard = $scope.listas[indexListaInicial].cards.map(function(e) { return e.id; }).indexOf(+ui.item.attr("id"));

			        	var card = $scope.listas[indexListaInicial].cards[indexCard];
			        	var listaInicial = $scope.listas[indexListaInicial];
			        	var listaFinal = $scope.listas[indexListaFinal];

			        	$scope.updateCard(card, listaInicial, listaFinal);
			        	
			        	$scope.$apply();
			       	}
			    });

	        }

	    }
	});

	app.directive('ngSubmitable', function($timeout) {
	    return {
	        restrict: 'A',
	        scope: false,
	        link: function($scope, element, attrs) {
				$(element).on('change', function() {
				    $(this).submit(); 
				 });
			}
		}
	});

})();