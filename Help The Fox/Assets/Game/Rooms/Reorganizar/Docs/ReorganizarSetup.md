# Sala de Reorganização — guia de uso e integração

## Para abrir e testar, mesmo sem experiência no Unity

1. No **Unity Hub**, escolha **Projects → Add → Add project from disk**.
2. Selecione a pasta `C:\Users\thiag\OneDrive\Documentos\GitHub\help-the-fox2026\Help The Fox`.
   Não selecione `HelpTheFox2026`: essa é outra pasta, com um projeto-base.
3. A versão registrada pela equipe é **6000.4.3f1**. Use essa versão; se necessário,
   instale-a pelo Hub com o Unity Download Archive. Esta alteração não migra a versão do projeto.
4. Aguarde a importação e a resolução dos pacotes do projeto.
5. Na janela **Project**, abra `Assets/Game/Rooms/Reorganizar/Scenes/SalaReorganizacao.unity`.
6. Aperte **Play**, no alto do editor. Clique na janela **Game** para dar foco ao teclado.
7. Antes do Play, a cena contém um objeto `SalaReorganizacao_AutoSetup`. Durante o Play,
   ele cria o tabuleiro, as mesas, os alvos, o ponto de entrada, a personagem e a interface.
   Isso é esperado. Não é preciso montar os objetos manualmente para este teste.
8. Para sair do teste, aperte **Play** outra vez. Mudanças em objetos gerados durante o Play
   não são salvas: altere o componente de montagem ou o layout para mudanças permanentes.

Não é preciso editar scripts para jogar esta cena. O trabalho manual principal é conferir
visualmente o comportamento, o tamanho das artes e a legibilidade da letra E.

## Teste guiado sem resolver todo o puzzle manualmente

O menu superior do Unity agora contém **Help The Fox → Reorganizar**.
Ele é uma ferramenta de desenvolvimento e não aparece no jogo publicado.

1. Com o Play parado, escolha **Help The Fox → Reorganizar → Abrir sala de teste**.
   Isso evita abrir uma cena diferente por engano.
2. Aperte **Play** e aguarde aparecerem o tabuleiro e a Lola.
3. Escolha **Help The Fox → Reorganizar → Testar solução (reinicia sala)**.
4. Aguarde aproximadamente 40 segundos sem controlar a personagem. A ferramenta reinicia
   somente esta sala e percorre a solução usando os movimentos normais, respeitando o intervalo.
   Ela não força uma conclusão nem pula a verificação dos alvos.
5. Ao terminar, confira se as mesas formam E e se o pensamento permanece aberto.
6. Aperte Enter para confirmar. Aperte C para consultar a pista outra vez.
7. Tente R e empurrar uma mesa depois da conclusão: a formação deve permanecer intacta.

Para interromper a demonstração, use **Help The Fox → Reorganizar → Parar teste automático**.
Antes da conclusão, R permite recomeçar. Depois da conclusão, inicie outra sessão de Play
para testar novamente do zero. Os controles manuais são devolvidos ao terminar, interromper
ou sair do Play. Se o menu estiver desabilitado, confira se o Play está ativo, se não está
pausado e se o puzzle ainda não foi concluído. O teste usa o layout padrão desta sala.

## Controles

| Ação | Controle |
| --- | --- |
| Andar e empurrar | WASD ou setas |
| Reiniciar mesas e personagem, antes de resolver | R ou botão Reiniciar sala |
| Confirmar pensamento | Enter, Enter numérico, Espaço ou botão Confirmar |
| Consultar a pista depois de resolver | C ou botão Consultar pista |
| Pedir retorno ao corredor, perto da área roxa | E ou botão Retornar ao corredor |

A saída está em modo de integração: informa que o retorno foi solicitado e emite eventos.
Ela não troca de cena. O corredor será conectado pela pessoa responsável por essa parte.

## Configuração que já acompanha a cena

Selecione `SalaReorganizacao_AutoSetup`. No Inspector, o componente
`ReorganizarRoomBootstrap` já recebe:

- **Table Sprite:** `desk_0`, de `Assets/Sprites/objects/desk.png`.
- **Lola Sprite:** `front_idle_0`, de `Assets/Sprites/lola/front_idle.png`.
- **Show Targets During Play:** desmarcado. Marque antes do Play para revelar os contornos
  dos alvos durante o desenvolvimento. Os alvos também têm Gizmos ao selecionar seus objetos.

Se um sprite estiver ausente, o módulo usa um quadrado colorido como placeholder.
A personagem é `PlayerPlaceholder_Lola_Local`: usa a arte existente, mas seu controle é
provisório e pertence apenas à sala. Não é o jogador definitivo nem usa o Animator global.

O tabuleiro tem **14 × 10 células**, incluindo a borda de paredes; cada célula mede **1 × 1**.
Há **11 mesas e 11 alvos**. O intervalo entre ações é **0,14 segundo**. Não há forças,
velocidades físicas, puxões ou empurrões em cadeia. Só direções cardinais unitárias são aceitas.
Uma ação válida move a personagem e, quando necessário, uma única mesa; uma ação bloqueada
não move nenhum deles. A rotação das mesas permanece fixa.

As posições finais são:

```text
          x=4  x=5  x=6
    y=6    E    E    E
    y=5    E
    y=4    E    E    E
    y=3    E
    y=2    E    E    E
```

Posições iniciais das mesas:
`(2,2), (2,3), (2,4), (5,5), (3,6), (4,4), (7,3), (5,4), (9,4), (6,7), (5,7)`.
O nascimento e a área de saída ficam em **(1,4)**. Paredes ocupam as bordas da grade.

O layout inicial foi construído por movimentos reversíveis a partir da formação final.
`ReorganizarLayout.VerifiedSolution` guarda uma solução de referência, verificada por teste.
As letras U/D/L/R representam cima/baixo/esquerda/direita; a solução não é exibida no jogo.

## Conclusão, reinício e retorno

- A conclusão exige que os 11 alvos distintos estejam ocupados, com uma mesa por célula.
- A letra **E** aparece no painel e na interface; os contornos da formação final ficam visíveis.
- O pensamento é: “Agora consigo enxergar! As mesas formam a letra E. É melhor anotarmos.”
- O painel aguarda confirmação e bloqueia as ações da própria sala. Ele não altera
  `Time.timeScale` e não interfere com o progresso de outros puzzles.
- Depois de resolver, as mesas e o reset ficam bloqueados, mas a personagem pode continuar
  andando para sair. Consultar a pista novamente não registra outra conclusão.
- Antes de resolver, R restaura também o nascimento da personagem para evitar que uma mesa
  seja recolocada em cima dela. Não existem animações de movimento pendentes para cancelar.
- Ao recarregar a sala na mesma sessão do jogo, o adaptador recompõe as mesas na formação
  resolvida e não repete a notificação. O registro é apenas em memória e é limpo ao iniciar
  uma nova sessão de Play. Não há salvamento permanente.
- Se sair antes de resolver e voltar, a sala recomeça no layout inicial.

## Integração com os sistemas da equipe

Não foram criados GameProgress, inventário, sistema de senha, corredor ou gerenciador global.
Não foram alterados os scripts compartilhados, as outras cenas, Packages ou ProjectSettings.
A cena de teste não foi adicionada à lista de build: ela abre diretamente no editor.
Ao integrar, a pessoa responsável deverá adicionar a cena sem reordenar as existentes,
pois outros módulos ainda usam índices de cena.

### Progresso e pensamento

`PuzzleCompletionAdapter.PuzzleCompleted` é um evento C# com `PuzzleNotification`:

```text
PuzzleId: Reorganizar
DiscoveredLetters: E
Message: Agora consigo enxergar! As mesas formam a letra E. É melhor anotarmos.
```

Há também **On Puzzle Id Completed**, um `UnityEvent<string>` no Inspector, emitido uma
única vez para a conclusão. Conecte a API real de progresso da equipe nesse ponto.
Na cena de montagem automática, conecte permanentemente o evento **Puzzle Completed** do componente `ReorganizarRoomBootstrap`, no objeto `SalaReorganizacao_AutoSetup`, antes de entrar em Play. Ele encaminha o mesmo identificador. Os objetos criados durante o Play são temporários.

O painel `LolaThoughtPanel` é uma apresentação local temporária. A interface usa IMGUI
para dispensar montagem manual de Canvas, fontes e EventSystem; pode ser substituída pelo
sistema compartilhado de pensamentos quando ele existir.

### Porta

`RoomExitPortal.OnReturnRequested` é o UnityEvent de retorno; `ReturnRequested` é a versão
C#. Ambos só disparam com a personagem próxima e sem o pensamento aberto.
Conecte-os ao sistema compartilhado de porta/transição e ao ponto de entrada do corredor.
Para salvar a conexão na cena automática, use **Return Requested** do `ReorganizarRoomBootstrap` em `SalaReorganizacao_AutoSetup`, antes de entrar em Play. Ele encaminha o pedido da porta criada em execução. Não há carregamento de cenas concorrente no módulo.

### Jogador definitivo

O projeto atual não fornece Player.prefab funcional. Para integrar o jogador real:

1. Substitua a criação do `PlayerPlaceholder_Lola_Local` em `ReorganizarRoomBootstrap` por
   uma instância do prefab compartilhado. Posicione-a em `PlayerSpawn_Safe`.
2. Mantenha **um único controlador de deslocamento ativo nessa sala**. A regra de movimentação
   local deve receber direções cardinais em `RoomOrganizationPuzzle.TryMovePlayer`.
3. Se for reutilizar `RoomPlayerPlaceholder` como adaptador de teclado, desative o componente
   de movimento contínuo apenas na instância local. Não modifique o script ou prefab global.
4. Faça o sistema de animação acompanhar a direção das ações aceitas. A lógica atual não
   cria animações definitivas nem conserta o Animator compartilhado.
5. Caso a equipe exija movimento contínuo, implemente e teste um adaptador específico que
   mantenha jogador e tabuleiro sincronizados; não ative os dois movimentos simultaneamente.

## Função de cada script

| Arquivo | Responsabilidade |
| --- | --- |
| TableBoardState | Regras puras, ocupação, movimento, reset, validação e estado resolvido. |
| ReorganizarLayout | Dimensões, posições iniciais, alvos em E e solução de referência. |
| TableGrid | Conversão de coordenadas, lista de paredes, registro das mesas e Gizmos. |
| PushableTable | Posição visual da mesa e encaminhamento de pedidos validados de movimento. |
| TableTarget | Posição-alvo e visualização de desenvolvimento. |
| RoomOrganizationPuzzle | Conecta regras e objetos Unity, intervalo entre ações e eventos da sala. |
| RoomPlayerPlaceholder | Entrada de teclado local, provisória e removível. |
| RoomResetController | Tecla R e entrada pública para botão de reinício. |
| RoomSpawnPoint | Posicionamento inicial e marcador de desenvolvimento. |
| RoomExitPortal | Proximidade, interação e evento de retorno ao corredor. |
| PuzzleCompletionAdapter | Notificação única, registro da sessão e consulta da pista. |
| PuzzleNotification | Dados da conclusão; não é um componente. |
| LolaThoughtPanel | Pensamento com confirmação e bloqueio das ações locais. |
| ReorganizarRoomBootstrap | Montagem da cena isolada, visuais provisórios, câmera e interface. |
| ReorganizarRoomTools (Editor) | Abre a cena correta e percorre a solução pelo menu durante o desenvolvimento. |

## Verificação e testes

Os testes de regras ficam em `Tests/Editor/TableBoardStateTests.cs`. Para executá-los no Unity:
abra **Window → General → Test Runner**, escolha **EditMode → Run All**.

Eles cobrem movimento de uma célula, parede, outra mesa, ausência de puxão, rejeição de
diagonal e salto, limites, alvos parciais, conclusão, bloqueio após resolver, reset,
posições duplicadas, configurações inválidas, retorno resolvido e a solução do layout completo.
Esses testes usam o mesmo código puro que controla a cena.

Há também **9 testes de integração** em `Tests/PlayMode/ReorganizarRoomFlowTests.cs`.
Com o Play parado, abra **Window → General → Test Runner → PlayMode → Run All**.
Eles verificam o intervalo entre empurrões, a notificação única, o pensamento com confirmação,
o reset dos objetos reais, as teclas R e Enter pelo Input System, a porta por proximidade,
a recriação da sala resolvida, o registro das mesas e a montagem da cena com os sprites originais.
O Test Runner gerencia sua própria entrada e saída do Play; estes testes não fazem parte do jogo.


Checklist manual no Play:

- [ ] A personagem aparece livre na área roxa e responde a WASD e setas.
- [ ] Um toque anda/empurra uma célula; segurar uma tecla respeita o intervalo.
- [ ] Parede e mesa vizinha bloqueiam o empurrão; andar para trás não puxa.
- [ ] R restaura todas as mesas e a personagem, sem reiniciar outras cenas.
- [ ] Com alvos incompletos, não há conclusão.
- [ ] Ao resolver, a disposição parece um E e o pensamento espera confirmação.
- [ ] Enquanto o pensamento está aberto, movimento, reset e saída ficam bloqueados.
- [ ] Confirmar devolve o controle; C reabre a pista sem outra notificação.
- [ ] Depois de resolver, as mesas permanecem fixas e o reset não desfaz a conclusão.
- [ ] E perto da saída mostra o aviso de integração, sem carregar cena indevida.
- [ ] Na integração, sair/reentrar preserva o resultado resolvido na mesma sessão.

Resultado da verificação externa: **16 testes passaram, sem falhas**. Os **14 scripts de execução compilaram sem erros ou avisos** contra as bibliotecas disponíveis na instalação Unity 6000.5.1f1 e seu Input System em cache. Também foram verificados os GUIDs, os identificadores dos sprites, os metadados, os nomes de classes e o isolamento das alterações. Essa compilação não usa necessariamente a mesma versão exata dos pacotes registrados no projeto; a importação na versão da equipe continua pendente.

A abertura da cena e sua apresentação inicial foram confirmadas pelo usuário.
As quatro assemblies (execução, ferramentas do editor, testes de regras e testes PlayMode)
foram compiladas externamente sem erros ou avisos, usando as referências da versão
**Unity 6000.4.3f1** e do **Input System 1.19.0** importado pelo projeto.
Os 9 testes de integração foram preparados e compilados, mas sua execução automática numa
cópia isolada foi bloqueada pela conexão com o cliente de licenciamento do Unity nessa sessão.
Não são apresentados como testes aprovados. O fluxo completo ainda precisa ser confirmado
no editor do usuário pelo teste guiado ou pelo Test Runner.
Compilação externa e testes das regras não substituem essa execução.

## Arquivos e escopo

Modificados: os oito scripts originais e `Docs/ReorganizarSetup.md`.
Criados: seis scripts de execução adicionais, uma ferramenta exclusiva do editor, a cena isolada,
quatro arquivos de definição de assembly, duas suítes de testes e os metadados `.meta` ausentes
do módulo e de suas pastas ancestrais novas.
Nenhum GUID já existente foi substituído. A arte existente é apenas referenciada.

Limitações: personagem e interface provisórias, retorno ao corredor aguardando integração,
sem conexão ao progresso global e sem salvamento entre execuções do jogo. O polimento visual
e a dificuldade para uma sessão real de jogadores ainda precisam de avaliação no Play.

Sugestão de commit: `fix(reorganizar): corrigir regras e preparar sala isolada testável`
