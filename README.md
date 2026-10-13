# Help The Fox - 2026.2

Organização: Insper Code 

Evento: Semana Transforme

correção e upgrade de sua versão anterior, o antigo README ainda está disponível em antiquo.md, será útil em nossos trabalhos.



## Descrição

Help the Fox é uma experiência única de Escape Room híbrido (físico e digital), desenvolvida por alunos do Insper em parceria com o Escape 60. A proposta mistura lógica, enigmas físicos, interações com ambiente, e um jogo virtual feito com Unity Engine.

Durante a Semana Transforme, equipes com 3 a 4 jogadores têm 25 minutos para resolver uma sequência de enigmas que envolvem objetos escondidos fisicamente na sala 411 do prédio P2 e interações no mundo digital para libertar a raposa Lola, a protagonista da história.



## Objetivo

Ajudar a raposa Lola a recuperar seu notebook e resumos trancados por seu amigo, Fabrício Farias (Fafa), antes que ela perca a prova final e fique de DP passando por seus desafios!



## Conteúdo Disponível no Beta

verifique o antiquo.md



### Fluxo principal do jogo

1. Tela inicial
2. Tela de instruções *(opcional)*
3. Spawn na sala principal
4. Ida ao corredor
5. Resolução do slide puzzle no corredor
6. conclui o puzzle de reorganizar a sala
7. conclui o puzzle de robo e morse
8. acha tabela de morse na sala física e decodifica o morse do robo
9. acha folha “em branco” na sala física (mapa da luz negra)
10. acha caneta de luz negra na sala física
11. conclui o puzzle da luz negra
12. volta pra sala principal
13. coloca a senha no notebook do fafa
14. tela de vitória




## Puzzles Planejados

## Reorganizar Sala (1 Letra)

O jogador deve arrumar os objetos da sala até que, pela perspectiva correta, eles formem uma letra semelhante a organização da sala 411. 



### Notebook e Código Morse (1 Letra)

No computador, Fafa costuma esquecer sua senha e deixa uma música em Morse para lembrar sua senha



### Puzzle da Luz Negra e Lab Invisível (2 Letras)

O jogador encontra na sala física:

* uma folha aparentemente em branco;
* uma caneta de luz negra.

Ao usar a luz negra:

* um mapa oculto aparece;
* a sala digital está escura e o mapa revela o labirinto que é a sala (ela está muito desorganizada);
* seguindo o mapa corretamente, o jogador encontra um post-it com 2 letras. 

### Organizar as letras nos livros (2 Letras)

* Fafa manda a Lola organizar os livros numa ordem definida
* Os números dos livros revelam um código
* O código entrega 2 letras



## Objetivo Final

Os puzzles liberam letras que devem ser organizadas para formar a senha final:

I N S P ER → **INSPER**

Ao inserir a senha no notebook do Fafa, o jogador conclui o jogo e acessa a tela de vitória. 


# Andamento (para devs)

> Marque um item como concluído somente quando ele estiver integrado ao projeto e puder ser testado no Unity sem erros no Console.

## Base do jogo

- [x] Lola se movimenta e possui animações
- [x] Câmera acompanha a personagem
- [x] Paredes e objetos possuem colisão
- [x] Sistema de interação com `E` funciona
- [ ] Progresso dos puzzles é mantido entre as cenas

## Menu

- [x] Botão Jogar inicia uma nova partida
- [ ] Botão Sair funciona
- [ ] Nova partida limpa o progresso anterior

## Sala Inicial

- [x] Lola e Fafa aparecem corretamente
- [x] Computador pode ser acessado
- [x] Código Morse `. - .` é reproduzido
- [x] Campo de senha funciona
- [ ] Senha `REPSNI` leva à vitória
- [ ] Porta leva ao corredor

## Corredor

- [ ] Lola consegue circular pelo corredor
- [ ] Paredes possuem colisão
- [ ] Portas levam às salas corretas
- [ ] É possível retornar das salas

## Sala de Reorganização

- [ ] Mesas podem ser empurradas
- [ ] Mesas não atravessam paredes ou outros objetos
- [ ] Organização correta forma a letra `E`
- [ ] Puzzle pode ser reiniciado
- [ ] Pensamento da Lola aparece ao concluir
- [ ] Porta retorna ao corredor

## Sala Escura

- [ ] Labirinto está montado
- [ ] Paredes invisíveis possuem colisão
- [ ] Mapa físico corresponde ao labirinto
- [ ] Papel com `SP` pode ser encontrado
- [ ] Pensamento da Lola aparece ao concluir
- [ ] Porta retorna ao corredor

## Sala dos Livros

- [ ] Livros podem ser selecionados ou organizados
- [ ] Puzzle utiliza a pista física
- [ ] Escolhas erradas podem ser corrigidas
- [ ] Resposta correta revela `IN`
- [ ] Pensamento da Lola aparece ao concluir
- [ ] Porta retorna ao corredor

## Computador e final

- [ ] Computador reconhece os puzzles concluídos
- [ ] Dica final do Fafa aparece
- [ ] Senhas erradas mostram uma mensagem
- [ ] `REPSNI` abre a tela de vitória
- [ ] É possível começar uma nova partida

## Integração final

- [ ] Todas as cenas estão adicionadas à build
- [ ] Puzzles podem ser concluídos em qualquer ordem
- [ ] Progresso não é perdido ao trocar de cena
- [ ] Nenhum puzzle é concluído duas vezes
- [ ] Fluxo completo pode ser jogado do menu à vitória
- [ ] Não existem erros vermelhos no Console
- [ ] Build final foi gerada e testada

# Equipe

Lucas Domingues, Thiago Araújo, Gabriel Moreira, Guilherme Vieira e Heitor Donato
