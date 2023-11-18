using System;
using System.Collections.Generic;

namespace B0L3FV_starcraft
{

    public class tamado
    {
        public int elet;
        public int tamadasiero;
        public int tamadasitav;
        public int foglaltpopul;
        public int gyartasiido;
        public int x;
        public int y;
    }
    public class cor
    {
        public int x;
        public int y;
    }

    public class fovar
    {
        public int x;
        public int y;
        public int elet;
    }

    class Program
    {


        //Az első érték  a sort (y), a második az oszlopot adja meg(x)
        static int[,] map = new int[10, 10];            //0 = üres mező, 1 a kurzor, 2 a terrán fővár, 3 a zerg fővűr, 4 a terrán támadó, 5 a zerg támadó, 6 terran gyartodó egység, 7 zerg gyártodó egység, 8 ide léphet a kiválaszott
        static List<tamado> Zergegyseg = new List<tamado>();
        static List<tamado> Terranegyseg = new List<tamado>();
        static List<cor> IdeigHely = new List<cor>();
        static List<cor> ahovalephet = new List<cor>();
        static List<cor> merreuthet = new List<cor>();
        static cor voltCor = new cor() { x = 0, y = 0 };
        static bool game = false;       //Megye a játék
        static bool Terrankov = true;          //Ha a terran jön
        static int[] cursor = new int[2];        //cursor helyzete
        static int Kor = 1;                     //Hányadik körben vagyunk
        static int lepesek = 0;                 //Hány lépést tettünk eddig meg a körben
        static char minvanCursor = 'x';         //Kurzor elmentse milyen mezőn áll
        static int mivolt = 0;                  //Kurzor elmentse min volt amikor rálép
        static int mivolt2 = 0;                 //Kurzor elmentse min volt egység mozgatásnál
        static bool felemelve = false;          //Felemeltünke egységet
        static int melyikid = 100;              //A kiválaszott egység idje
        static int Pop = 0;                     //Foglalt populáció
        static fovar TerranFovar = new fovar() { y = 9, x = 4, elet = 1 };  //A terran fővár helye és élete 
        static fovar ZergFovar = new fovar() { y = 0, x = 4, elet = 1 };    //A zerg fővár helye és élete 
        static bool terranynert = false;        //Végén megmondja hogy kinyert a kiíráshoz
        static int terranegyseg = 0;            //Terrannak hány egysége van hogy tudjunk maxot megadni
        static int zergegyseg = 0;              //zergnek hány egysége van hogy tudjunk maxot megadni

        static int maxegyseg = 8;               //minden oldalnak max 8 egysége lehet
        static int maxpop = 20;                 //összesen populáció max 20 lehet

        static void Main(string[] args)
        {
            for (int i = 0; i < 3; i++)
            {
                Random rnd = new Random();  //y 1-8 alapból
                Zergegyseg.Add(new tamado() { elet = 10, tamadasiero = rnd.Next(1, 4), tamadasitav = rnd.Next(1, 3), foglaltpopul = 1, gyartasiido = 0, y = 1, x = i * 3 + 2 });       //Hozzáadja a zerg egységet a listájához
                Terranegyseg.Add(new tamado() { elet = 10, tamadasiero = rnd.Next(1, 4), tamadasitav = rnd.Next(1, 3), foglaltpopul = 1, gyartasiido = 0, y = 8, x = i * 3 + 1 });     //Hozzáadja a terran egységet a listájához
                Pop += 2;
                terranegyseg++;
                zergegyseg++;
            }

            Console.CursorVisible = false;      //Ne látszódjon a kurzor

            cursor[0] = 4; cursor[1] = 4;       //Kurzor koordinátáinak középre helyezése
            map[cursor[0], cursor[1]] = 1;      //Kurzor a pályán megjelenítése


            game = true;
            MapRefresh();
            while (game)
            {
                ConsoleKeyInfo input = Console.ReadKey();      //Elmenti a lenyomott billentyűt
                switch (input.Key)
                {
                    case ConsoleKey.W:
                        if (cursor[0] > 0)        //Leelenörzi nicnse vége a pályának
                        {
                            MireLep(map[cursor[0] - 1, cursor[1]]);         //MireLep fügyvény hívása ami csak lementi charként hogy mi az a mező amin vagyunk a kiíráshoz
                            cursor[0]--;
                        }
                        break;
                    case ConsoleKey.S:
                        if (cursor[0] < 9)        //Leelenörzi nicnse vége a pályának
                        {
                            MireLep(map[cursor[0] + 1, cursor[1]]);         //MireLep fügyvény hívása ami csak lementi charként hogy mi az a mező amin vagyunk a kiíráshoz
                            cursor[0]++;
                        }
                        break;
                    case ConsoleKey.A:
                        if (cursor[1] > 0)        //Leelenörzi nicnse vége a pályának
                        {
                            MireLep(map[cursor[0], cursor[1] - 1]);         //MireLep fügyvény hívása ami csak lementi charként hogy mi az a mező amin vagyunk a kiíráshoz
                            cursor[1]--;
                        }
                        break;
                    case ConsoleKey.D:
                        if (cursor[1] < 9)        //Leelenörzi nicnse vége a pályának
                        {
                            MireLep(map[cursor[0], cursor[1] + 1]);         //MireLep fügyvény hívása ami csak lementi charként hogy mi az a mező amin vagyunk a kiíráshoz
                            cursor[1]++;
                        }
                        break;
                    case ConsoleKey.G:
                        if (mivolt == 0 && Pop <= maxpop)                //Ha üres területen vagyunk
                        {
                            Random rnd = new Random();                  //random a tamadási erő és távolság választásához
                            if (Terrankov && terranegyseg <= maxegyseg)     //ha a terrán jön éa van helye plusz egyslége
                            {
                                Terranegyseg.Add(new tamado() { elet = 10, tamadasiero = rnd.Next(1, 4), tamadasitav = rnd.Next(1, 3), foglaltpopul = 1, gyartasiido = 1, y = cursor[0], x = cursor[1] });     //Hozzáadja a terran egységet a listájához
                                Pop++;      //növeli a populációt
                                MireLep(6);     //megadja a mappon neki a 6-ost
                                lepesek++;      //1 lépéssel több lett
                                Terrankov = false;  //zerg következik
                                terranegyseg++;     //növeli a meglévő egységek számát
                            }
                            else if (!Terrankov && zergegyseg <= maxegyseg)
                            {
                                Zergegyseg.Add(new tamado() { elet = 10, tamadasiero = rnd.Next(1, 4), tamadasitav = rnd.Next(1, 3), foglaltpopul = 1, gyartasiido = 1, y = cursor[0], x = cursor[1] });       //Hozzáadja a zerg egységet a listájához
                                Pop++;          //növeli a populációt
                                MireLep(7);     //megadja a mappon neki a 6-ost
                                lepesek++;      //1 lépéssel több lett
                                Terrankov = true;   //zerg következik
                                zergegyseg++;       //növeli a meglévő egységek számát
                            }
                        }
                        break;
                    case ConsoleKey.Enter:
                        bool sikereslepes = false;
                        if (Terrankov)                                          //Hogyha a Terra jön
                        {
                            if (mivolt == 0 || !felemelve)   //Ha terra egységen van a kurzor sé még nem emeltünk fel semmit
                            {
                                int i = 0;
                                for (i = 0; i < Terranegyseg.Count; i++)
                                {
                                    if (Terranegyseg[i].y == cursor[0] && Terranegyseg[i].x == cursor[1])
                                        melyikid = i;
                                }
                                if (melyikid != 100)
                                {
                                    voltCor.y = Terranegyseg[melyikid].y; voltCor.x = Terranegyseg[melyikid].x;
                                    felemelve = true;                               //Elmenti hogy felemeltónk valamit
                                    MireLep(0);                                     //Amin áll átállítja üres mezőre
                                    mivolt2 = map[cursor[0], cursor[1]];            //Elmenti hogy min álltunk alapból, amit késöbb le rakunk

                                    i = -Terranegyseg[melyikid].tamadasitav;                        //Támadási táv kiválasztásához hogy kijelezze a térkép és lehessen vele ellenőtizni
                                    int x = Terranegyseg[melyikid].x;                               //ideiglenes változó hogy átláthatóbb legyen a számolás
                                    int y = Terranegyseg[melyikid].y;                               //ideiglenes változó hogy átláthatóbb legyen a számolás
                                    while (i <= Terranegyseg[melyikid].tamadasitav)                 //Amíg az i kisebb vagy egyenlő mint amennyit támadhat
                                    {
                                        if (x + i < 10 && x + i >= 0 && map[y, x + i] == 0)          //ha van még hely lerakni, valamint ha üres terület van abba az irányba
                                            IdeigHely.Add(new cor() { x = x + i, y = y });          //hozzáadja ideiglenes hely liostához ami csak a lehetőségek koordinátáit tárolja
                                        if (y + i < 10 && y + i >= 0 && map[y + i, x] == 0)          //ha van még hely lerakni, valamint ha üres terület van abba az irányba
                                            IdeigHely.Add(new cor() { x = x, y = y + i });          //hozzáadja ideiglenes hely liostához ami csak a lehetőségek koordinátáit tárolja

                                        if (x + i < 10 && x + i >= 0)          //ha van még hely támadni
                                            merreuthet.Add(new cor() { x = x + i, y = y });          //hozzáadja ideiglenes hely liostához ami csak a lehetőségek koordinátáit tárolja
                                        if (y + i < 10 && y + i >= 0)          //ha van még hely támadni
                                            merreuthet.Add(new cor() { x = x, y = y + i });          //hozzáadja ideiglenes hely liostához ami csak a lehetőségek koordinátáit tárolja

                                        i++;
                                    }                                                               //idáig

                                    i = -1;                                                     //Lépéshez segít egyszerű ellenőrzés miatt ugyanúgy megy mint az elöző for
                                    x = Terranegyseg[melyikid].x;
                                    y = Terranegyseg[melyikid].y;
                                    while (i <= 1)
                                    {
                                        if (x + i < 10 && x + i >= 0 && map[y, x + i] == 0)
                                            ahovalephet.Add(new cor() { x = x + i, y = y });
                                        if (y + i < 10 && y + i >= 0 && map[y + i, x] == 0)
                                            ahovalephet.Add(new cor() { x = x, y = y + i });

                                        i++;
                                    }                                                           //idáig tart
                                }
                            }
                            else if (felemelve)                                                  //ha már kiválasztottuk mivel szeretnénk lépni
                            {
                                if (idelephete() || tamade(true) != 0)                          //hogyha a koordináta ahol vagyunk benne van a lépési lehetőségek listájában (ami csak üres mezőt tárol), vagy ha valamit tud támadni
                                {

                                    int temp = tamade(true);                            //ideiglenes változó ne kelljen folyamatosan meghívni a függvényt
                                    if (temp == 1 && vaneBenne())                       //hogyha sima egységet támadunk és benne van a támadási körzetben
                                    {
                                        int melyikellen = 100;
                                        for (int i = 0; i < Zergegyseg.Count; i++)                              //Ez a ciklus megkeresi kit támadtunk, id szerint
                                        {
                                            if (cursor[0] == Zergegyseg[i].y && cursor[1] == Zergegyseg[i].x)
                                                melyikellen = i;
                                        }
                                        if (melyikellen != 100)
                                        {
                                            Zergegyseg[melyikellen].elet -= Terranegyseg[melyikid].tamadasiero;       //Levonja az életét annyival amennyi a támadásunk
                                            Terranegyseg[melyikid].y = voltCor.y; Terranegyseg[melyikid].x = voltCor.x;                             //Visszarakja az egységet a helyére mielőtt felemeltük
                                            if (Zergegyseg[melyikellen].elet <= 0)          //Ha a zerg egységnek elfogyott az élete kitörli és leveszi a populációt valamint az egységek számát
                                            {
                                                Zergegyseg.RemoveAt(melyikellen);
                                                Pop--;
                                                zergegyseg--;
                                            }
                                        }
                                        sikereslepes = true;                                //sikeresen léptünk
                                    }
                                    else if (temp == 2 && vaneBenne())                   //ha fővárat támadunk és a körzetben vna
                                    {
                                        ZergFovar.elet -= Terranegyseg[melyikid].tamadasiero;       // levonja a fővárból az életet
                                        if (ZergFovar.elet <= 0)                                        //ha elfogyott az élete
                                        {
                                            game = false;                                           //játék vége
                                            terranynert = true;                                     //terra nyert
                                        }
                                        sikereslepes = true;    //sikeresen léptünk
                                        Terranegyseg[melyikid].y = voltCor.y; Terranegyseg[melyikid].x = voltCor.x;                             //Visszarakja az egységet a helyére mielőtt felemeltük

                                    }
                                    else if (idelephete())
                                    {
                                        Terranegyseg[melyikid].y = cursor[0]; Terranegyseg[melyikid].x = cursor[1]; //a megemelt terrán egység helyét atállítjuk a cursor helyére
                                        MireLep(mivolt2);                               //Kurzor helyét átírja a megfelelő karakterre
                                        sikereslepes = true;        //sikeresen léptünk
                                    }
                                    ahovalephet.Clear();                                //Kiürítjük a listát ami a lépésp lehetőségeket tárolta
                                    IdeigHely.Clear();                                  //Kiürítjük a listát ami a támadási lehetőségeket tárolta
                                }
                            }
                        }
                        else                                                    //Ha a zerg jön
                        {
                            if (mivolt == 5 && !felemelve)   //Ha zerg egység fölött vagyunk és nem emeltünk még fel semmit
                            {
                                int i = 0;

                                for (i = 0; i < Zergegyseg.Count; i++)                  //végigmegy a zergeken hogy megtudja melyikkel támadtunk
                                {
                                    if (Zergegyseg[i].y == cursor[0] && Zergegyseg[i].x == cursor[1])
                                        melyikid = i;
                                }
                                if (melyikid != 100)            //ha talált egyet azonsoat
                                {
                                    MireLep(0);                                     //Amin áll átállítja üres mezőre
                                    mivolt2 = map[cursor[0], cursor[1]];            //Elmenti hogy min álltunk alapból, amit késöbb le rakunk
                                    felemelve = true;                               //Elmenti hogy felemeltónk valamit
                                    voltCor.y = Zergegyseg[melyikid].y; voltCor.x = Zergegyseg[melyikid].x;     //beállítja melyik koordinátán volt eredetlieg

                                    i = -Zergegyseg[melyikid].tamadasitav;          //Támadási táv kiválasztásához hogy kijelezze a térkép és lehessen vele ellenőtizni
                                    int x = Zergegyseg[melyikid].x;                 //ideiglenes változó hogy átláthatóbb legyen a számolás
                                    int y = Zergegyseg[melyikid].y;                 //ideiglenes változó hogy átláthatóbb legyen a számolás
                                    while (i <= Zergegyseg[melyikid].tamadasitav)   //Amíg az i kisebb vagy egyenlő mint amennyit támadhat
                                    {
                                        if (x + i < 10 && x + i >= 0 && map[y, x + i] == 0)  //ha van még hely lerakni, valamint ha üres terület van abba az irányba
                                            IdeigHely.Add(new cor() { x = x + i, y = y });  //hozzáadja ideiglenes hely liostához ami csak a lehetőségek koordinátáit tárolja
                                        if (y + i < 10 && y + i >= 0 && map[y + i, x] == 0)  //ha van még hely lerakni, valamint ha üres terület van abba az irányba
                                            IdeigHely.Add(new cor() { x = x, y = y + i });  //hozzáadja ideiglenes hely liostához ami csak a lehetőségek koordinátáit tárolja

                                        i++;
                                    }                                                       //idáig

                                    i = -1;                                                 //Lépéshez segít egyszerű ellenőrzés miatt ugyanúgy megy mint az elöző for
                                    x = Zergegyseg[melyikid].x;
                                    y = Zergegyseg[melyikid].y;
                                    while (i <= 1)
                                    {
                                        if (x + i < 10 && x + i >= 0 && map[y, x + i] == 0)
                                            ahovalephet.Add(new cor() { x = x + i, y = y });
                                        if (y + i < 10 && y + i >= 0 && map[y + i, x] == 0)
                                            ahovalephet.Add(new cor() { x = x, y = y + i });

                                        i++;
                                    }                                                       //idáig tart
                                }
                            }
                            else if (felemelve)                                              //ha már kiválasztottuk mivel szeretnénk lépni
                            {
                                if (idelephete() || tamade(false) != 0)                     //hogyha a koordináta ahol vagyunk benne van a lépési lehetőségek listájában (ami csak üres mezőt tárol), vagy ha valamit tud támadni
                                {

                                    int temp = tamade(false);                           //ideiglenes változó ne kelljen folyamatosan meghívni a függvényt

                                    if (temp == 1 && vaneBenne())                       //hogyha sima egységet támadunk és benne van a támadási körzetben
                                    {
                                        int melyikellen = 100;
                                        for (int i = 0; i < Terranegyseg.Count; i++)                              //Ez a ciklus megkeresi kit támadtunk, id szerint
                                        {
                                            if (cursor[0] == Terranegyseg[i].y && cursor[1] == Terranegyseg[i].x)
                                                melyikellen = i;
                                        }
                                        if (melyikellen != 100)
                                        {
                                            Terranegyseg[melyikellen].elet = Terranegyseg[melyikellen].elet - Zergegyseg[melyikid].tamadasiero;       //Levonja az életét annyival amennyi a támadásunk
                                            Zergegyseg[melyikid].y = voltCor.y; Zergegyseg[melyikid].x = voltCor.x;                             //Visszarakja az egységet a helyére mielőtt felemeltük
                                            if (Terranegyseg[melyikellen].elet <= 0)                                                            //Ha a zerg egységnek elfogyott az élete kitörli és leveszi a populációt valamint az egységek számát
                                            {
                                                Terranegyseg.RemoveAt(melyikellen);
                                                Pop--;
                                            }
                                            sikereslepes = true;                    //sikeresen léptünk
                                        }
                                    }
                                    else if (temp == 2 && vaneBenne())                              //ha fővárat támadunk és a körzetben vna
                                    {
                                        TerranFovar.elet -= Zergegyseg[melyikid].tamadasiero;
                                        if (TerranFovar.elet <= 0)
                                        {
                                            game = false;
                                            terranynert = false;
                                        }
                                        sikereslepes = true;            //sikeres lépés
                                        Terranegyseg[melyikid].y = voltCor.y; Terranegyseg[melyikid].x = voltCor.x;                             //Visszarakja az egységet a helyére mielőtt felemeltük

                                    }
                                    else if (idelephete())
                                    {
                                        Zergegyseg[melyikid].y = cursor[0]; Zergegyseg[melyikid].x = cursor[1]; //a megemelt zerg egység helyét atállítjuk a cursor helyére
                                        MireLep(mivolt2);                               //Kurzor helyét átírja a megfelelő karakterre
                                        sikereslepes = true;            //sikeres lépés
                                    }
                                    ahovalephet.Clear();                                //Kiürítjük a listát ami a lépésp lehetőségeket tárolta
                                    IdeigHely.Clear();                                  //Kiürítjük a listát ami a támadási lehetőségeket tárolta
                                }

                            }
                        }
                        if (sikereslepes)
                        {
                            felemelve = false;                              //Visszaállítja leengedetre
                            lepesek++;                                      //Hozzáad egy lépést
                            Terrankov = !Terrankov;                         //Beállítja hogy a Zergek következnek
                            cursor[0] = 4; cursor[1] = 4;                   //Lerakás után visszadobja középre a kurzort
                            melyikid = 100;
                            MireLep(0);                                     //Amikor visszakerült középre akkor oda is lerakta az egységet egyel többnek, ez megoldja
                            sikereslepes = !sikereslepes;

                        }
                        break;

                }

                if (lepesek == 6)                                               //Ha elértük a 6 lépést (3+3)
                {
                    Kor++;                                                      //Új kört kezd
                    lepesek = 0;                                                //Visszaállítja a lépés számát

                    for (int i = 0; i < Terranegyseg.Count; i++)
                    {
                        if (Terranegyseg[i].gyartasiido > 0)
                            Terranegyseg[i].gyartasiido--;

                        if (Terranegyseg[i].gyartasiido == 0)
                            map[Terranegyseg[i].y, Terranegyseg[i].x] = 4;
                    }
                    for (int i = 0; i < Zergegyseg.Count; i++)
                    {
                        if (Zergegyseg[i].gyartasiido > 0)
                            Zergegyseg[i].gyartasiido--;

                        if (Zergegyseg[i].gyartasiido == 0)
                            map[Zergegyseg[i].y, Zergegyseg[i].x] = 5;
                    }

                }
                map[cursor[0], cursor[1]] = 1;                                  //Ahol a kurzor van beállítja a térképen is hogy megjelenjen
                MapRefresh();                                                   //Frissíti a pályát/képernyőt
            }

        }

        static int tamade(bool terran) //a mező ami ki van választva tartalmaze ellenfelet, a bool arra hogy terranegységet vagy zerget nézzen
        {
            if (terran)
            {
                for (int i = 0; i < Zergegyseg.Count; i++)
                {
                    if (Zergegyseg[i].y == cursor[0] && Zergegyseg[i].x == cursor[1])
                        return 1;
                    if (ZergFovar.y == cursor[0] && ZergFovar.x == cursor[1])
                        return 2;
                }
            }
            else
            {
                for (int i = 0; i < Terranegyseg.Count; i++)
                {
                    if (Terranegyseg[i].y == cursor[0] && Terranegyseg[i].x == cursor[1])
                        return 1;
                    if (TerranFovar.y == cursor[0] && TerranFovar.x == cursor[1])
                        return 2;
                }
            }

            return 0;
        }

        static bool vaneBenne()                     //Tude ide támadni, csak véginézi a listát valamelyik egyezike a cursor pozicióval
        {
            for (int i = 0; i < merreuthet.Count; i++)
            {
                if (merreuthet[i].y == cursor[0] && merreuthet[i].x == cursor[1])
                    return true;
            }

            return false;
        }

        static bool idelephete()                    //végignézi al listát hogy tartalmazzae a kurzor helyére való lépést
        {
            for (int i = 0; i < ahovalephet.Count; i++)
            {
                if (ahovalephet[i].y == cursor[0] && ahovalephet[i].x == cursor[1])
                    return true;
            }

            return false;
        }

        static void MireLep(int kar) //Lementi a pályából kapott számból hogy milyen jelet írjon ki
        {
            switch (kar)
            {
                case 0:                     //üres mező esetén
                    minvanCursor = 'x';     //x jelenjen majd meg
                    mivolt = 0;
                    break;
                case 2:                     //Terran fővár mező esetén
                    minvanCursor = 'T';     //T jelenjen majd meg
                    mivolt = 2;
                    break;
                case 3:                     //Zerg fővár mező esetén
                    minvanCursor = 'Z';     //Z jelenjen majd meg
                    mivolt = 3;
                    break;
                case 4:                     //Terrán támadó mező esetén
                    minvanCursor = 'T';     //T jelenjen majd meg
                    mivolt = 4;
                    break;
                case 5:                     //Zerg támadó mező esetén
                    minvanCursor = 'Z';     //T jelenjen majd meg
                    mivolt = 5;
                    break;
                case 6:                     //Terrán készülő egység mezője esetén
                    minvanCursor = 'T';     //T jelenjen meg
                    mivolt = 6;
                    break;
                case 7:                     //Terrán készülő egység mezője esetén
                    minvanCursor = 'Z';     //Z jelenjen meg
                    mivolt = 7;
                    break;
                case 8:
                    minvanCursor = 'x';     //Ide tud támadni terület
                    mivolt = 8;
                    break;

            }
        }

        static void MapRefresh()
        {
            Console.Clear();                    //Console ürítése

            if (game)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        map[i, j] = 0;
                    }
                }//Űríti a pályát
                for (int i = 0; i < Terranegyseg.Count; i++)
                {
                    if (Terranegyseg[i].gyartasiido == 0)
                        map[Terranegyseg[i].y, Terranegyseg[i].x] = 4;
                    else
                        map[Terranegyseg[i].y, Terranegyseg[i].x] = 6;
                }//Terrán gyártodó és sima egységeket elhelyezi
                for (int i = 0; i < Zergegyseg.Count; i++)
                {
                    if (Zergegyseg[i].gyartasiido == 0)
                        map[Zergegyseg[i].y, Zergegyseg[i].x] = 5;
                    else
                        map[Zergegyseg[i].y, Zergegyseg[i].x] = 7;
                }//Zerg gyártodó és sima katonákat elhelyezi
                for (int i = 0; i < IdeigHely.Count; i++)
                {
                    map[IdeigHely[i].y, IdeigHely[i].x] = 8;
                }//Az ideiglenes mezőket kiírja a pályán
                map[TerranFovar.y, TerranFovar.x] = 2;  //Ideiglenes terrán fővár kiíratás
                map[ZergFovar.y, ZergFovar.x] = 3;  //ideiglenes zerg fővár kiíratás
                map[cursor[0], cursor[1]] = 1;

                if (Terrankov)
                    Console.WriteLine("Terran következik    {0}    Populáció: {1}/{2}", Kor, Pop, maxpop);
                else
                    Console.WriteLine("Zerg következik      {0}    Populáció: {1}/{2}", Kor, Pop, maxpop);

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        Console.ForegroundColor = ConsoleColor.White;       //Háttér alapból fekete
                        switch (map[i, j])
                        {
                            case 0:                     //üres mező esetén
                                Console.Write("x");     //mező kiírása
                                break;
                            case 1:                     //Ha  játékos
                                Console.ForegroundColor = ConsoleColor.Blue;       //Játékos esetén kék legyen
                                Console.Write(minvanCursor);     //Mező kiírása
                                break;
                            case 2:                     //Terran fővár mező esetén
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.Write("T");     //mező kiírása
                                break;
                            case 3:                     //Zerg fővár mező esetén
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.Write("Z");     //mező kiírása
                                break;
                            case 4:                     //Terrán támadó mező esetén
                                Console.Write("T");     //mező kiírása
                                break;
                            case 5:                     //Zerg támadó mező esetén
                                Console.Write("Z");     //mező kiírása
                                break;
                            case 6:                     //Terrán támadó mező esetén
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("T");     //mező kiírása
                                break;
                            case 7:                     //Zerg támadó mező esetén
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Z");     //mező kiírása
                                break;
                            case 8:                     //támadható területek szine
                                Console.ForegroundColor = ConsoleColor.Yellow;  //sárga legyen a szine
                                Console.Write("x");
                                break;
                        }
                    }
                    Console.Write("\n");                //Új sor kezdése
                }
                Console.WriteLine("G - Egységek gyártására");
                Console.WriteLine("Enter - Egységek mozgatásához");
            }
            else
            {
                if (terranynert)                        //Ha terra nyert kiírás
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("Terrán");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" nyerte a játékot!");
                }
                else                                    //Ha zerg nyert kiírás
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("Zerg");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" nyerte a játékot!");
                }
                Console.ReadKey();
            }
        }

    }
}

