Implementação final da apresentação [Gerando testes de unidade mais valiosos](https://docs.google.com/presentation/d/e/2PACX-1vQbN1tf-wxS40mZaPzJgwHHN6A7Mt4bq7DmKkucjQzmzFQ7WWN5iKAyHCvg6ZbuMD6uMrgAGGxM3qHd/pub?start=false&loop=false&delayms=3000&slide=id.gb9789b2cf1_0_77).

Fizemos a refatoração do método ChangeEmailAsync do UserService. A versão inicial desse método era considerada complexa pois assumia duas responsabilidades ao mesmo tempo: orquestração e lógica de domínio. 

Com a refatoração conseguimos mover a lógica de domínio para as Models de User e Company, o que permitiu validar as regras de domínio através de testes de unidade muito mais simples e valiosos.

Exploramos também o uso do padrão CanExecute/Execute e de Domain Events.



