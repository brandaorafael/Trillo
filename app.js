//var lostas = [{"nome": "testando1", "cards": [{"nome": "Card1", "id": "l1c1"}, {"nome": "Card2", "id": "l1c2"}]}, {"nome": "testando2", "cards": [{"nome": "Card1", "id": "l2c1"}, {"nome": "Card2", "id": "l2c2"}, {"nome": "Card3", "id": "l2c3"}, {"nome": "Card4", "id": "l2c4"}]}];
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

	app.directive("sortable", function(){
		return {
			restrict: "A",
			link: function(){
				
			}
		}
	});

	app.controller("ListaController",["$http", function($http){
		var listaCtrl = this;

		listaCtrl.novaLista = {"nome": "Nova Lista"}; 

		//listaCtrl.listas = listas;

		$http.get("http://localhost:60684/listas").success(function(data){
			listaCtrl.listas = data;
		});

		listaCtrl.addLista = function(){
			//listaCtrl.listas.push({"nome": "Nova Lista", "cards": []});
			
			$http.post("http://localhost:60684/listas", listaCtrl.novaLista).success(function(data){
				listaCtrl.listas.push(data[0]);
			}).error(function(){
				alert("Deu Erro");
			});
		}

		listaCtrl.removeLista = function(id){
			
			$http.delete("http://localhost:60684/listas/"+id).success(function(data){

				var index = listaCtrl.listas.map(function(e) { return e.id; }).indexOf(id);

				listaCtrl.listas.splice(index, 1);
				
			}).error(function(){
				alert("Deu Erro");
			});
		}

		listaCtrl.addCard = function(listaAtual){
			listaAtual.cards.push({"nome": "Novo Card", "id": listaAtual.cards.length + 1});
		}

	}]);

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

// homes.sort(function(a, b) {
//     return parseFloat(a.price) - parseFloat(b.price);
// });