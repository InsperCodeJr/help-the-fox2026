# Sala de Reorganização das Mesas

## Objetivo

Implementar o módulo da sala isolado, sem alterar os outros módulos do projeto. A lógica da sala foi publicada em scripts separados dentro da pasta Assets/Game/Rooms/Reorganizar.

## Estrutura recomendada

- Assets/Game/Rooms/Reorganizar/Scripts
- Assets/Game/Rooms/Reorganizar/Docs

## Configuração no Unity

1. Crie uma nova cena chamada `SalaReorganizacao` ou o nome que a equipe preferir.
2. Coloque o fundo da sala com `fundo_sala.png` ou um placeholder simples.
3. Crie um GameObject vazio chamado `ReorganizarPuzzle`.
4. Adicione os componentes:
   - `TableGrid`
   - `RoomOrganizationPuzzle`
   - `RoomResetController`
   - `PuzzleCompletionAdapter`
5. Atribua o `TableGrid` ao campo `TableGrid` do `RoomOrganizationPuzzle`.
6. Adicione uma `Player` real ou um placeholder com `PlayerMovement`.
7. Posicione a player a uma distância segura de todas as mesas, por exemplo `(0, 0, 0)`.
8. Crie os objetos das mesas e atribua o componente `PushableTable` a cada um.
9. Configure a `gridPosition` de cada mesa no Inspector seguindo o mesmo sistema da sala.
10. Crie os objetos que representam as posições-correto da letra `E` e adicione `TableTarget` em cada um.
11. Ajuste os valores de `CellPosition` desses alvos para combinar com a grid da sala.
12. Arraste todas as mesas para a lista `Tables` e todos os alvos para a lista `Targets` dentro de `RoomOrganizationPuzzle`.
13. Crie um GameObject `LetterEVisual` e deixe-o desativado no início.
14. Arraste esse GameObject para o campo `LetterEVisual` de `RoomOrganizationPuzzle`.
15. Crie um canvas com um painel de texto e um botão `Confirmar`.
16. Adicione o componente `LolaThoughtPanel` ao painel.
17. Arraste o `CanvasGroup`, o `TextMeshProUGUI` e o botão para os campos do componente.
18. Arraste o painel para o campo `ThoughtPanel` do `PuzzleCompletionAdapter`.
19. No `PuzzleCompletionAdapter`, atribua o mesmo `LetterEVisual` e configure a string final da mensagem.
20. Crie um `RoomExitPortal` e posicione a saída na parede da sala.
21. Arraste a `Player` para o campo `Player` do portal.
22. Configure `SceneIndex` ou `SceneName` para retornar ao corredor ou à cena inicial.
23. Para testes isolados, o portal pode simplesmente carregar a cena atual ou deixar apenas o placeholder de retorno com `Debug.Log`.

## Regras de sala

- A tabela só se move quando o jogador está adjacente a ela e empurra na direção correta.
- A direção da movimentação é sempre uma célula.
- Se a célula seguinte estiver bloqueada por parede ou outra mesa, o movimento é cancelado.
- O reset usa `R` diretamente no teclado.
- O painel de pensamento da Lola bloqueia o movimento com `Time.timeScale = 0f` até o jogador confirmar.
- A conclusão do puzzle é registrada localmente em memória e evita duplicação no mesmo fluxo.
- O sistema ainda precisa ser integrado ao progresso global do time, se existir mais tarde.

## Observações

- Não existem `GameProgress`, `PuzzleId` ou sistema central de progresso no projeto atual.
- O comportamento de conclusão foi exposto por `PuzzleCompleted` e `PuzzleNotification` para permitir integração futura sem duplicar uma arquitetura global.
- O módulo foi desenhado para funcionar sozinho em uma cena isolada e manter a lógica da sala modular.
