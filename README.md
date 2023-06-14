# JAILBREAKERS - escape the prison
In deze spannende VR-game speel jij de rol van een ontsnapte gevangene uit een gevangenis in een vulkaan die nog een paar uitdagende obstakels moet overwinnen om te ontsnappen. Gelukkig krijg je hulp van een andere gevangene die zich in een aparte kamer bevindt (de agent). Door slim samen te werken en elkaar te ondersteunen, kunnen jullie de verschillende poorten passeren en uiteindelijk de vrijheid bereiken. In deze tutorial zal uitgelegd worden hoe je deze game maakt.

## Samenvatting  voor deze tutorial
Wij zullen in deze tutorial bespreken wat wij allemaal hebben gebruikt, welke puzzels wij hebben gemaakt voor de speler en voor de AI. Hoe onze AI samenwerkt met de speler.

# Info van de one-pager
In deze spannende VR-game speel jij de rol van een ontsnapte gevangene uit een gevangenis in een vulkaan die nog een paar uitdagende obstakels moet overwinnen om te ontsnappen. Gelukkig krijg je hulp van een andere gevangene die zich in een aparte kamer bevindt (de agent). Door slim samen te werken en elkaar te ondersteunen, kunnen jullie de verschillende poorten passeren en uiteindelijk de vrijheid bereiken. Deze parkour-game biedt een coöperatieve ervaring waarbij je acties kunt uitvoeren om de agent te helpen de obstakels te overwinnen en de uitgang te bereiken. 

## Doel
Het doel van het spel is om gezamenlijk uit de vulkaan te geraken, zodat ze kunnen ontsnappen uit de gevangenis waarin ze vastzitten. Dit vereist teamwork, samenwerking en mogelijk het overwinnen van verschillende uitdagingen.

## Hoe gaat de wereld eruitzien
Jij en de Agent lopen over 2 grote bruggen, deze brug is echter geblokkeerd door verschillende poorten, alsook dat onder de brug lava stroomt. In iedere kamer wil je in feite de poort openen, in samenwerking met de ML agent, om zo uit de gevangenis te kunnen ontsnappen.

## Agent functies 
- Platformen springen voor knop of object te bereiken 
- Op knoppen drukken 
- Items oppakken en op de juiste plek leggen 
- Kleine andere puzzel elementen oplossen 
- Grote objecten verschuiven 

## Agent Rewards 

+ Juiste knop ingedrukt houden à reward 
+ Andere player door de deur lopen à grote reward 
+ Door de deur open à grote reward

- Eraf vallen = end episode en negatieve punten 
- Vanaf de agent zijn deur open à knop reward = 0 
- Als een object zijn Y waarde negatief is = negatieve reward 

## Aanpassingen
We hebben dit blijven volgen maar we hebben het wat kleiner moeten maken door tijdsgebrek. 

# Inhoud - effectieve implementatie

## Methoden
### Agent script
**Initialize**
Dit wordt maar éénmalig aangesproken en zal dienen als de absolute basis instellingen voor de agent.
**OnEpisodeBegin**
Deze methode wordt aangeroepen bij het begin van een episode en dus ook na EndEpisode. Dit zou enkele dingen moeten resetten als de agent zijn episode gedaan is, al dan niet gelukt of mislukt.
**CollectObservations**
Verzamelt observaties van de omgeving voor de agent. Aangezien wij RayPerceptionSensors gebruiken zal hier enkel lokale positie, snelheid en enkele omgeving variabelen in staan.
**OnActionReceived**
Past de ontvangen acties van de agent toe op de omgeving. Voornamelijk agent movement.
**OnCollisionEnter**
Wordt aangeroepen op botsingen met gameobjecten. Zeer belangrijk voor rewards aan agent te geven.
**OnTriggerEnter**
Zoals OnCollision maar wordt enkel aangeroepen als de agent een trigger object aanraakt (object waar agent door kan lopen). Zeer belangrijk voor rewards aan agent te geven.
**ApplyMovement**
Methode voor overzicht, wordt aangeroepen vanuit OnActionReceived met 2 ContinuousActions. Zorgt voor de horizontale movement van de agent.
**ApplyRotation**
Methode voor overzicht, wordt aangeroepen vanuit OnActionReceived met 1 ContinuousActions. Zorgt voor de horizontale rotatie van de agent.
**Jump**
Methode voor overzicht, wordt aangeroepen vanuit OnActionReceived. Zorgt dat de agent kan springen als hij al niet springt.
**Heuristic**
ML Agent default methode voor manuele controle van de agent. Wordt enkel gebruikt als heuristic mode aan staat.
**HasTool**
Kleine methode die bool terug geeft als agent een child object heeft of niet (kamer 4).
**GetTool**
Als agent een child object heeft en het is niet een camera geef het terug als een transform object.
**Update**
wordt enkel gebruikt dat de agent zijn spawnpoint gelijk is aan de spawnpoint set in de GameManager.

### GameManager
**Awake**
Wordt gebruikt om de GameManager te koppelen aan alle objecten die een reference naar dit type object hebben. (bij meerdere training scenarios zal je dit op een andere manier moeten doen)
**Start**
Zetten van de de begin state, alsook zet beginwaarden van variabelen.
**UpdateGameState**
Afhankelijk van de state parameter, verander gamestate hiernaar. Alsook worden verschillende items per state hierin aangepast.
**RandomSpawn**
Methode die locatie van agent zal randomisen voor training moeilijkheid.
**Reset**
Methode die zorgt voor resetten van bepaalde items wanneer bv item of agent valt of in het algemeen resetten van omgeving.
**moveBlockages**
Zorgt voor het verplaatsen van obstakels wanneer getriggerd, en volgens gamestate. 
**ResetTool**
Zorgt voor het randomisen en resetten van de locatie van de tool per episode.
**ResetBox**
Wordt momenteel niet meer gebruikt want dit zou dienen voor het resetten en randomisen van de locatie van de door voor een kamer die niet is afgewerkt.
**SetGateTrue**
Kleine methode die observatie bool true zal maken.
**randomButtonPos**
Zal per state de button in de kamer verplaatsen/randomisen voor training moeilijkheid te verhogen.
**enum GameState**
Niet een methode maar belangrijk, hierin staan de mogelijke states/levels van het spel.

### PuzzleManager
**CompletedPuzzlePiece**
Deze methode wordt gebruikt als er een juiste gem in de juiste socket komt. Deze maakt de lokale variabel +1 en doet ook een CheckForPuzzleComplete()
**CheckForPuzzleComplete**
Deze methode checkt alleen maar of alle juiste puzzelstukjes gelijk is aan de totale hoeveelheid puzzeltukken dat moet opgelost wordt.
**PuzzlePieceRemoved**
Als je een juist puzzelstuk verplaatst moet de lokale variabel -1 worden.
### SocketLogic
**Awake**
Wordt gebruikt om de socket component te krijgen.
**OnEnable**
Er worden koppelingen gemaakt tussen methode ObjectInSocket, ObjectOutSocket en de gebeurtenis.
**OnDisable**
Hierbij worden de koppelingen terug verwijderd van OnEnable.
**ObjectInSocket**
Deze methode werkt samen met de PuzzleManager. Als het juiste puzzelstuk in de socket ligt start het de method CompletedPuzzlePiece()
**ObjectOutSocket**
Deze methode werkt samen met de PuzzleManager. Deze start de methode PuzzlePieceRemoved() als je een juist puzzelstuk terug verwijderd.

## Installatie
Programma’s die wij hebben gebruikt:
* Unity 2020.3.47f1 met daarop:
    * MLagent 2.0.1
    * Oculus XR Plugin 1.13.1
    * OpenXR Plugin 1.7.0
    * XR Interaction Toolkit 1.0.0-pre.3
    * XR Plugin Management 4.2.1
    * TextMeshPro 3.0.6
    * Jewel Pack 3.1 (asset store)
* Anaconda versie 23.3.0

## VR Speler 
### Kamer 1
Kamer 1 is de simpelste kamer en dient daarom deels tutorial.
In deze kamer moet de speler op een rode knop drukken naast de deur. Hierdoor wordt de deur voor de agent geopend.
#### werking
De speler heeft een onzichtbare sphere verbonden aan zijn hand. Wanneer deze collide met de knop zorgt een OnTrigger script dat de  deur opent en de knop groen word.
We gebruiken ook een Tag zodat enkel de spelers handen de knop kunnen indrukken.
### Kamer 2
Kamer 2 is een sport oefening.
De kamer bevat een basket bal en een ring. Wanneer de speler de bal door de ring gooit opent de deur voor de agent. 
#### werking
Als we een platte onzichtbare kubus in de ring steken kunnen we opnieuw een OnTrigger en tag gebruiken wanneer de bal door de ring gaat.

### Kamer 3
Kamer 3 is een fysica puzzel.
Een wipplank hangt over de rand van het platform.
In de plank ligt een bal.
Enkel deze bal kan de knop naast de wip activeren en de deur openen. 

Door gewichten op de wip te leggen kantelt deze en rolt de bal op de knop.
#### Werking
**De plank**
De plank kantelt over een cilinder.
De plank schuift niet naar achter door een bol.
De plank werd vastgezet en kan niet kantelen behalve rond de cilinder.

**De gewichten**
De bal weegt meer dan de gewichten. Hierdoor moet de speler meerdere gewichten op de plank leggen.
De speler kan enkel de gewichten oppakken.
Hij kan de bal niet oppakken. Hij kan de wipplank niet duwen of de knop activeren.

### Kamer 4
Kamer 4 is een raadsel. 6 kristallen liggen op een tafel en het raadsel op de muur verteld welk kristal waar moet.
#### Werking
**Gems en sockets**
De gems zijn gewone objecten die je kan oppakken en in een socket steken maar alleen als je ze allemaal in de juiste socket steekt gaat de deur open. Dit gebeurt door 2 scripts:
* socket logic script (Dat aan elke socket wordt gegeven)
    * Daarin geef je dan het juiste puzzel object aan mee per socket
* PuzzleManager (Dit wordt aan het moeder object mee gegeven)

## AI Agent
### Kamer 1
In deze start kamer is enkel een knop aanwezig. De locatie van de knop kan echter ieder spel variëren. Alsook moet er een obstakel en deur aanwezig zijn voor de volgende kamer.
#### Werking
Wanneer de agent de knop ziet en deze aanraakt zal de eerste deur van de speler opengaan. Pas als de agent zijn deur open is gegaan door de speler kan de agent verder gaan.

### Kamer 2
In deze kamer is de knop geplaatst op een verhoogt object zodat de horizontale sensor van de agent dit niet direct detecteert. Alsook kan de locatie weeral variëren.
#### Werking
Als de agent de knop vindt en deze uiteindelijk benadert dan zal de 2de deur van de speler opengaan. Pas als de agent zijn deur open is gegaan door de speler kan de agent verder gaan.

### Kamer 3
De knop bevindt zich nu in het midden van de lucht dus zal de agent het ook hier niet meteen vinden en zal hij het al springend moeten aanraken. Alsook is de knop boven een put voor het licht moeilijker te maken. Pas als zijn deur open is gegaan door de speler kan de agent verder gaan.
#### Werking
Als de knop is aangeraakt zal de 3rde deur van de speler open gaan. Pas als de agent zijn deur open is gegaan door de speler kan de agent verder gaan.

### Kamer 4
Hierin zullen 2 object zijn, een tool en een basket. De bedoeling is dat tool in basket wordt gegooid/gelegd.
#### Werking
Eerst zal de agent de 'tool' moeten vinden en dit dan aanraken. Nu zal 'tool' een child object van de agent worden zodat het vastplakt aan de agent. 
Nu moet de agent de 'basket' vinden en dit aanraken. Hierdoor zal tool terug een child object worden van de kamer en zal de 4de deur van de speler opengaan. Pas als de agent zijn deur open is gegaan door de speler kan de agent verder gaan.

## Overzicht observaties, acties en rewards
### Agent observaties
Door gebruik te maken van RayPerceptionSensor worden vele observaties achter de schermen uitgevoerd. Echter hebben we toch nog ervoor gezorgt dat de agent enkele variabelen als observatie krijgt voor extra input van de opgeving.  
Alsook geven we de agent zijn lokale positie en velocity mee.

### Agent acties 
- springen, links/rechts draaien
- zien m.b.v. 3d RayPerceptionSensors
    - één horizontaal op oog hoogt voor de agent.
    - één ver boven de agent en zeer laag mikkend, zodat het kan dienen als een verticale sensor.
- objecten aanraken

### Agent Rewards
+ Knop éénmalig aanraken is matige positieve reward (add)
+ Tool aanraken in matige positieve reward (add)
+ Basket aanraken als hij tool als child heeft is grote positieve reward (add)
+ Door de deur lopen is grote positieve reward (set)

- Vanaf de agent zijn knop is ingedrukt is knop reward 0 
- Als beweegbare objecten zijn Y waarde negatief is geef agent negatieve reward (add) en respawn/reset object
- Als agent ongewenste objecten aanraakt geef negatieve punten (add) en mogelijks EndEpisode (dan moet reward set zijn)
- Eraf vallen is EndEpisode en negatieve punten (set)
- Als x tijd is verstreken trigger EndEpisode en negatieve punten (set)

# Resultaten ML Agent
Doorheen het trainen van de agent is meermaals gespeeld met min of meer iedere optie die te gebruiken was in het config bestand alsook de settings van de 3D RayPerceptionSensor en de waarden van de rewards.
We hebben echter het beste resultaat bereikt met gebruik te maken van Behavioral_cloning en gail voor immitation learning alsook curiosity voor exploratie meer aan te moedigen.

### Room1
![](https://hackmd.io/_uploads/H1wG4Uvvn.png)
Zoals u kunt zien, heeft de Agent op de eerste maar reeds simpele kamer vrij snel begrepen wat er moest gebeuren. Dit volgens mij met dank aan het gebruik van een demonstratie en veel rewards van de omgeving. Het is te vermelden dat we na het succes  van de AI kamer4 ook de andere 3 kamers hebben hertrained aangezien we nu pas echt gebruik maakte van curiosity en zagen hoe veel beter dit was.

### Room2
![](https://hackmd.io/_uploads/Syn-SUvD2.png)
Hier ziet u dat na een 150k steps begon hij te begrijpen wat er moest gebeuren, dit is tevens ook wanneer dat behavioral_cloning stopt met gebruikt te worden. We denken dat deze kamer ook zo goed verliep omdat we 2 rayPerception componenten gebruikten.

### Room3
![](https://hackmd.io/_uploads/S1oxwLDvn.png)
Bij deze kamer begon de agent al beter te begrijpen wat er moest gebeuren alvorens dat de behavioral_cloning stopte van toepassing te zijn. 

### Room4
![](https://hackmd.io/_uploads/Syh-vUPDh.png)
Deze kamer had weliswaar veel meer tijd nodig voor tot een functioneel einde te krijgen maar dankzij een combinatie te vormen van behavioral cloning en gail, extrinsic en curiosity reward signals hebben we dit uiteindelijk toch werkende gekregen. We hebben met deze kamer de meeste moeite gehad en hebben doorheen de tijd de config meerdere keren aangepast, omdat we geen verandering kregen, maar uiteindelijk kregen we na curiosity toe te voegen en na 1.3mil stappen eindelijk wat resultaten te zien.

### Opvallende waarneming
Ik weet niet of dat effectief waar is maar ik heb het vermoeden dat de Agent de demonstraties niet gebruikt als je de default demonstratie path/folder gebruikt. Alsook denk ik dat CTRL+C drukken in anaconda bij het trainen van je agent mogelijks bugs kan veroorzaken in de onderliggende code.

Het verschil tussen Set en Add-reward is me opgevallen, AddReward wordt gebruikt voor een beloning/straf zonder een EndEpisode te triggeren en SetReward in combinatie met een EndEpisode omdat SetReward veel definitiever is als AddReward.  

Enige aanpassing aan de RayPerceptionSensors zal zorgen dat agent hertrained moeten worden want vorige NNs (brains) zullen niet meer werken op deze agent. Alsook is de keuze van het aantal sensors en de richting belangrijk want dit beslist wat de agent wel en niet kan zien.

Als je in heuristic controls gebruik maakt van bv 3 continious actions en 1 discrete action maar in OnActionReceived (agent movement) gebruik je 4 continious actions, dan zal je na een tijdje een error krijgen tijdens het trainen door je anaconda. Zorg er dus steeds voor dat je hetzelfde gebruikt in beiden methoden.

## Conclusie
Tijdens ons VR-project hebben we veel nieuwe kennis opgedaan, vooral op het gebied van AI. We hebben ons origneel voorstel wel wat moeten verkleinen door tijdsbeperkingen en problemen waar we langer op vast zaten. Desondanks zijn we wel trots op dit resultaat dat we zijn bekomen. 

# bronvermelding
Kirkwood, A. (2014, 2 juni). Fantasy Scene. Pinterest. https://www.pinterest.com/pin/107382772340229319/
List of block textures – Minecraft Wiki. (z.d.). Minecraft Wiki. https://minecraft.fandom.com/wiki/List_of_block_textures
Downdate. (2012, 5 december). Basket Ball Texture. OpenGameArt.org. https://opengameart.org/content/basket-ball-texture
Lava Beetle Caves | Wulverheim Wiki | Fandom. (z.d.). Wulverheim Wiki. https://wulverheim.fandom.com/wiki/Lava_Beetle_Caves
A Wizard’s Wish | Madness of the hells Wiki | Fandom. (z.d.). Madness of the hells Wiki. https://madness-of-the-hells.fandom.com/wiki/A_Wizard's_Wish
Whitehoune. (z.d.). Rough stone wall high resolution close-up texture. Dreamstime.com. https://www.dreamstime.com/dramatic-surface-flat-stones-forming-high-detailed-natural-backdrop-rough-stone-wall-resolution-close-up-texture-image137810828
YouTube. (2022, March 11). Make a VR game in 2022! - puzzles. puzzle. https://www.youtube.com/watch?v=iSYfs6NXZck 
Nowling, S. S. (2018, 20 februari). darkness. Pinterest. https://www.pinterest.com/pin/arts-and-crafts--38491771799397417/
Depositphotos, Inc. (z.d.). Helictites Stock Photos, Royalty Free Helictites Images | Depositphotos. Depositphotos. https://depositphotos.com/stock-photos/helictites.html 
Texelsaur. (z.d.). Big Doors. legacy.curseforge.com. https://legacy.curseforge.com/minecraft/mc-mods/big-doors
Digital Illustration Fantasy Underground Stalactites Cave Stock Illustration 612143759 | Shutterstock. (z.d.). Shutterstock. https://www.shutterstock.com/image-illustration/digital-illustration-fantasy-underground-stalactites-cave-612143759
Incompetech: Royalty-free music. Incompetech.com. (n.d.). https://incompetech.com/music/ 
Technologies, U. (n.d.). Training configuration file information. Training Configuration File - Unity ML-Agents Toolkit. https://unity-technologies.github.io/ml-agents/Training-Configuration-File/#reward-signals
Unity-Technologies. (n.d.). Unity-Technologies/ML-Agents: The Unity Machine Learning Agents Toolkit (ML-agents) is an open-source project that enables games and simulations to serve as environments for training intelligent agents using deep reinforcement learning and imitation learning. GitHub. https://github.com/Unity-Technologies/ml-agents 
outvector. (n.d.-b). Jewel pack: 3D props. Unity Asset Store. https://assetstore.unity.com/packages/3d/props/jewel-pack-19902 


## schrijvers:
- Kelvin Bogaerts     s117203
- Fleur Van Buijten   s127848
- Michiel Hoerée      s126389
