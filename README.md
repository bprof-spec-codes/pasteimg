# PasteIMG

## Csapatbeosztás

|Név|Feladatkör|
|--|--|
|Béres Benjámin|Backend|
|Kiss Levente|Frontend, team leader|
|Léránt Sámuel|Frontend|
|Nagy Máté|Frontend|


## User Manual

### Telepítés, beüzemelés

A szoftvertermék beüzemeléséhez működő Visual Studio fejlesztőkörnyezet szükséges, amely támogatja a .NET6 keretrendszerben készült projekteket. Fontos, hogy telepítve legyenek az ASP.NET and web development, Data storage and processing, SQL Server Data Tools, IIS Express, NuGet package manager, NuGet targets and build tasks és Entity Framework 6 tools komponensek.
A projekt fejlesztőkörnyezetben történő megnyitását követően a szerver réteg a Ctrl + F5 billentyűkombinációval indítható.
A felugró böngészőablakkal (swagger) nem szükséges foglalkozni, bezárása azonban nem javasolt, mivel az a szerver leállásához vezet.
Későbbiekben, igény esetén csapatunk nyitott arra, hogy a fenti projektet publikusan elérhető weboldalra migráljuk (pl. Azure), hogy a szerver eléréséhez ne legyenek szükségesek a fent leírtak.
A kliens alkalmazást indításához, (amit Ön ténylegesen használni fog), a Frontend mappát érdemes egy parancssorral megnyitni, és ott egy 'npm install' (idézőjelek nélkül) parancs telepíteni fogja az alkalmazáshoz szükséges függőségeket. Ha a parancs végigfutott, az 'ng serve' paranccsal futtatni tudja a programot a böngészőben, a következő weboldalon: [számítógép ip címe]:4200 (szögletes zárójelek nélkül). Ha az Ön helyi számítógépéről futtatja, akkor elegendő a számítógép ip címét azzal helyettesíteni, hogy 'localhost' (idézőjelek nélkül).

### Használat
#### Admin felület elérése

Ha a böngészőben az alábbi ablakot látja (és Visual Studio alkalmazásból elindította a szerver alkalmazást), akkor az Ön gépén már fut a programunk, amelyet rendeltetésszerűen tud használni.
Az adminisztrációs felület eléréséhez az alapértelmezett email-cím + jelszó páros:
- admin@admin.com
- 123Admin456

#### API funkciólista
##### Admin API funkciók

- /api/Admin/DeleteImage/{id}
	- Egy adott egyedi azonosítójú képet töröl (ami egy upload alatt van)
	- Ha ez a kép egy feltöltés (upload) utolsó képe, akkor a hozzá tartozó feltöltés objektum is törlésre kerül
- /api/Admin/DeleteUpload/{id}
	- Egy adott egyedi azonosítójú feltöltés objektumot töröl, beleértve az összes képet az adott feltöltés alatt
- /api/Admin/EditImage
	- Egy adott egyedi azonosítójú kép objektumban a leírást lehet módosítani, illetve az NSFW tulajdonságot lehet indokolt esetben igazzá tenni (vagy éppen hamissá)
- /api/Admin/GetAllImage
	- Az összes feltöltött kép elérhető
- /api/Admin/GetAllUpload
	- Az összes feltöltött feltöltés objektum elérhető
- /api/Admin/Login
	- Az admin felületbe lehet vele bejelentkezni, melynek működési elve:
		- a kérés header-ében a korábban backend által generált tokent (API-SESSION-KEY) elküldjük, a kérés törzsében pedig json formátumban az email-jelszó párost
		- ha helyesek az adatok, akkor a backend a mi munkamenetünkhöz hozzárendeli az admin jogokat
		- ez azért fontos, mert a fentebb részletezett végpontok is elküldik a headerben a munkamenetünk azonosítóját, és ha nem vagyunk admin jogúak, akkor a kérések (pl. feltöltés törlése, összes feltöltés megtekintése) megtagadásra kerülnek
- /api/Admin/Logout
	- Az admin felületből lehet kijelentkezni (a kérés törzsében elküldjük a munkamenet azonosítónkat) és a backend eltávolítja az admin jogokat ezen azonosítóról
- /api/Admin/IsAdmin
	- egy adott munkamenet azonosítóról eldöntni, hogy admin-e vagy sem, és ennek megfelelően egy true vagy false választ ad
		- ez technikailag volt szükséges bizonyos funkciók megvalósításakor a kliens alkalmazásban
- /api/Admin/GetRegisterKey
	- ha új admin felhasználót szeretnénk hozzáadni, akkor egy már meglévő admin felhasználónak kulcsot kell generálnia, hogy lehessen admin felhasználóként regisztrálni
- vannak további végpontok, amik admin joggal is indíthatók, de kimenetük ugyanaz lesz, mintha felhasználóként indítanánk el, ezek a felhasználó API funkcióknál kerülnek majd részletezésre

##### Felhasználó API funkciók

- /api/Public/EnterPassword/{uploadId}
	- ha egy jelszóval védett feltöltéshez (vagy képhez) kapunk linket, akkor ez a végpont fogja a szervernek elküldeni az adott munkamenet által megadni próbált jelszót, mely ha megfelel, hozzáférést fog nyerni az adott feltöltéshez/képhez
		- ha nem felel meg, 10 percen belül 3 helytelen próbálkozás után az adott munkamenetet kitiltja
- /api/Public/PostUpload
	- egy feltöltés objektumot tudunk vele feltölteni a képekkel együtt
- /api/Public/PostImage
	- egy kép feltöltéséhez szükséges, végül ez nem került felhasználásra
- /api/Public/PostRegister
	- ha adminként szeretnénk regisztrálni, ezt ennek a végpontnak a segítségével tehetjük meg, természetesen szükséges ehhez a korábban GetRegisterKey végpont által generált kulcs
	- /api/Public/GetImage/{id}
		- egy adott egyedi azonosítójú kép tulajdonságait tudjuk lekérdezni (leírás, felnőtt tartalom-e, melyik feltöltéshez tartozik)
- /api/Public/GetImagewithSourceFile/{id}
	- egy adott egyedi azonosítójú kép bináris adatát adja vissza, mely alapján maga a kép kirajzolható
- /api/Public/GetImageWithThumbnailFile/{id}
	- egy adott egyedi azonosítójú kép bélyegképének (csökkentett felbontás és így csökkentett fájlméret) bináris adatát adja vissza, mely alapján maga a kép (kisebb változata) kirajzolható
- /api/Public/GetUpload/{id}
	- egy adott egyedi azonosítójú feltöltést tudunk lekérdezni vele, melyben az összes, az adott feltöltésbe tartozó képek tulajdonságai is elérhetők
- /api/Public/GetUploadWithSourceFiles/{id}
	- egy adott egyedi azonosítójú feltöltésnek az összes képének bináris adatát elérhetjük
- /api/Public/GetUploadWithThumbnailFiles/{id}
	- egy adott egyedi azonosítójú feltöltésnek az összes képének a bélyegképének a bináris adatát elérhetjük

#### UI felületek
- új feltöltés, kezdőoldal:
 ![image](https://github.com/bprof-spec-codes/pasteimg/assets/92106195/3782153e-7508-42bb-ae4c-e940b62b9187)
 ![image](https://github.com/bprof-spec-codes/pasteimg/assets/92106195/414ca054-b193-48a7-a290-5869a4e5964f)
- meglévő feltöltés megtekintése:
 ![image](https://github.com/bprof-spec-codes/pasteimg/assets/92106195/fd7e623a-3d95-4b3c-9b46-4f039810e9fb)
 ![image](https://github.com/bprof-spec-codes/pasteimg/assets/92106195/9168cbfe-862c-4466-a9b2-1c1d5231e8f5)
- adott képre kattintás után:
 ![image](https://github.com/bprof-spec-codes/pasteimg/assets/92106195/61e8bb6b-1e01-49bb-a1d1-7ff23f8bd2ea)
- login felület adminra:
 ![image](https://github.com/bprof-spec-codes/pasteimg/assets/92106195/f5e188ba-6003-4d39-a404-ccd41ab47931)
- admin felhasználói felület:
 ![image](https://github.com/bprof-spec-codes/pasteimg/assets/92106195/c4253e92-6816-444a-b96e-d8b865c7764e)
 - szerkesztés után: ![image](https://github.com/bprof-spec-codes/pasteimg/assets/92106195/c79878ba-eef1-42fa-aa36-48aacf42ff92)
 ![image](https://github.com/bprof-spec-codes/pasteimg/assets/92106195/8576cf66-5d81-4148-aa4e-b5f7b171e063)
- admin regisztráció:
 ![image](https://github.com/bprof-spec-codes/pasteimg/assets/92106195/e0b57bce-1cb4-4f4a-9047-1fb020f61f51)

## Probléma jegyzőkönyv
- A legelején elfelejtettünk .gitignore fájlt betenni, így rengeteg felesleges dolog került fel a repoba
 - megoldás: töröltük az egész projektet, újrakezdtük, de előtte betettük a gitignore fájlt
- Eredetileg api projektet kellett volna létrehozni, helyette ASP.NET MVC appot hoztunk létre
 - megoldás: a swaggert kézzel üzemeltettük be, macerásnak tűnt
- Adminlogin készítésekor félrecsúszott a kommunikáció a csapatban, és két csapattag készítette egyidőben ugyanazt, commitkor pedig konfliktus keletkezett, mivel bár más módon, de ugyanazon funkciót implementálták, a sessionid között is volt összeakadás
 - megoldás: az egyikük egyenként, kézzel megoldotta a conflictot, ellentmondásokat
- frontend: modelek nem jól lettek megírva / nem konzisztensen (kis/nagybetű pl.)
 - megoldás: mivel későn jöttünk rá (valószínűleg A és B ember más konvenciókkal dolgozott), ezekkel együtt dolgoztunk tovább, lassabb lett a kliens fejlesztése
- már majdnem készen volt a teljes kliens, amikor egyikünk észrevette, hogy új munkamenetben az egész alkalmazás egy fehér ablak, nem működik
 - megoldás: 'API-SESSION-KEY' -t nem szerzünk localStorage -ból, helyette a sessionIdService service-en belül található getSessionId-t meghívja jutunk id-hez, és azt az id-t állítjuk be a kérések header-ébe
