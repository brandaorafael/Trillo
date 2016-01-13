var lostas = [
	{"nome": "testando1",
	"cards": [
			{"nome": "Card1",
			"id": "1"},
			{"nome": "Card2",
			"id": "2"}
		]
	},
	{"nome": "testando2",
	"cards": [
			{"nome": "Card1",
			"id": "1"},
			{"nome": "Card2",
			"id": "2"}, 
			{"nome": "Card3", 
			"id": "3"}, 
			{"nome": "Card4", 
			"id": "4"}
		]
	}
];

(function(){
	var app = angular.module("app", []);

	var listas = lostas;

	app.controller("ListaController",["$http", function($http){

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

		listaCtrl.updateCard = function(card, lista){
			card.nome = listaCtrl.nomeEditado;

			$http.put("http://localhost:60684/cards/"+card.id, card).success(function(data){

				var indexLista = listaCtrl.listas.map(function(e) { return e.id; }).indexOf(lista.id);
				var indexCard = listaCtrl.listas[indexLista].cards.map(function(e) { return e.id; }).indexOf(card.id);

				listaCtrl.listas[indexLista].cards[indexCard] = card;
				
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

})();

$(document).ready(function() {

	var findCard = function(arr, cardId) {
    	for (var i = 0, len = arr.length; i < len; i++) {
        	if (arr[i].id === cardId)
            	return arr[i];
    	}
    	return null;
	}

	var ordenar = function(array){

		var arr = $(this).sortable('toArray');
		
		for(var i = 0; i < arr.length; i++){
			findCard(array, arr[i]).id = i + 1;
		}
	};

    $(".lista").sortable({
        connectWith: ".lista",
        items:".card",
        // update: function (event, ui) {
        // 	if(ui.sender === null){
        // 		console.log($(this));
        // 		console.log(event);
        // 		console.log(ui.sender);
        // 	}
        // },
        receive: function (event, ui) {
        	console.log(ui.item.data);
        },
        remove: function (event, ui) {
        	console.log(event);
        }
    });
    
});