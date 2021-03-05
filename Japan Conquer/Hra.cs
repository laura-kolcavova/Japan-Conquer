using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Japan_Conquer
{
    static class Hra
    {
        public static List<Hrac> Vladci { get; private set; }
        public static Hrac Player { get; private set; }
        public static Provincie[] SeznamProvincii { get; private set; }

        public static Hrac AktualniHrac { get; private set; }
        public static Provincie AktualniProvincie { get; private set; }
        public static Budova AktualniBudova { get; private set; }

        public static string Path { get; private set; }
        public static int Den { get; private set; }
        public static Random NahodnyGenerator { get; private set; }


        /*/////////////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////////////*/

        public static void DeklarujHru(Hrac[] vladci, Provincie[] seznamProvincii, int indexHrace)
        {
            Vladci = vladci.ToList();
            
            Player = Vladci[indexHrace];

            SeznamProvincii = seznamProvincii;

            PrepocitejSoupereVladcu();
            foreach (Hrac h in Vladci) if (h is Pocitac) ((Pocitac)h).NastavPocatecniPriority();

            Den = 1;

            NahodnyGenerator = new Random();
        }

        /*/////////////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////////////*/

        public static int VyberVladce(Hrac[] seznamVladcu)
        {
            Console.WriteLine("\n           Vládci a jejich provincie: ");
            for (int i = 0; i < 49; i++) Console.Write("-");
            Console.WriteLine("\n");

            for (int i = 0; i < seznamVladcu.Length; i++)
            {
                string mezera = " ";
                if (i >= 9) mezera = "";

                Console.Write("#{0}  " + mezera, i + 1 );
                Hra.ObarviAVypisText("yellow", seznamVladcu[i].Jmeno);
                Console.WriteLine(" - " + seznamVladcu[i].VypisVlastniciProvincie());

            }

            Console.WriteLine("\n{0} -> Zpět", seznamVladcu.Length + 1);

            int index = VyberZnabidky(seznamVladcu.Length + 1);

            if (index == seznamVladcu.Length + 1) Program.VypisMenu("menu");
            return index - 1;
        }

        public static void PrepocitejSoupereVladcu()
        {
            foreach (Hrac vladce in Vladci)
            {
                vladce.PrepocitejSoupere();
            }
        }

        public static void OdeberVladce(Hrac vladceKodebrani) { Vladci.Remove(vladceKodebrani); }

        private static int volba;
        public static void ZobraNabidku(string umisteni)
        {
            ZobrazPrehled(Player, true);

            switch (umisteni)
            {
                case "koren":
                    {
                        AktualniHrac = Player;

                        Console.WriteLine("1 -> Mapa");
                        Console.WriteLine("2 -> Jednotky");
                        Console.WriteLine("3 -> Ukončit kolo");
                        Console.WriteLine("4 -> Konec");

                        volba = VyberZnabidky(4);

                        switch (volba)
                        {
                            case 1: ZobraNabidku("mapa"); break;
                            case 2: ZobraNabidku("jednotky"); break;
                            case 3: break;
                            case 4:
                                {
                                    string odpoved = Prompt();

                                    if (odpoved.ToUpper() == "A") Program.VypisMenu("menu");
                                    else ZobraNabidku("koren");

                                    break;
                                }
                        }

                        break;
                    }
                case "mapa":
                    {

                        Console.WriteLine("1 -> Moje provincie");
                        Console.WriteLine("2 -> Celá mapa");
                        Console.WriteLine("3 -> Zpět");

                        volba = VyberZnabidky(3);

                        switch (volba)
                        {
                            case 1: ZobraNabidku("mojeProvincie"); break;
                            case 2: ZobraNabidku("celaMapa"); break;
                            case 3: ZobraNabidku("koren"); break; //Kořen přepisuje path na default
                        }
                        break;
                    }
                case "mojeProvincie":
                    {
                        Player.VypisProvincieProNabidku();

                        Console.WriteLine("\n{0} -> {1}", Player.SeznamProvincii.Count + 1, "Zpět");

                        volba = VyberZnabidky(Player.SeznamProvincii.Count + 1);

                        if (volba == Player.SeznamProvincii.Count + 1)
                        {
                            ZobraNabidku("mapa");
                        }
                        else
                        {
                            AktualniProvincie = Player.SeznamProvincii[volba - 1];
                            PrejitNaProvincii(AktualniProvincie);
                        }

                        break;
                    }

                case "celaMapa":
                    {
                        ZobrazMapu();

                        Console.WriteLine("\n{0} -> Zpět", Vladci.Count + 1);

                        volba = VyberZnabidky(Vladci.Count + 1);

                        if (volba == Vladci.Count + 1)
                        {
                            ZobraNabidku("mapa");
                        }
                        else
                        {
                            AktualniHrac = Vladci[volba - 1];
                            ZobraNabidku("prehled");
                        }

                        break;
                    }
                case "prehled":
                    {
                        ZobrazPrehled(AktualniHrac, false);

                        Console.WriteLine();

                        AktualniHrac.VypisProvincieProNabidku();

                        Console.WriteLine("\n{0} -> {1}", AktualniHrac.SeznamProvincii.Count + 1, "Zpět");

                        volba = VyberZnabidky(AktualniHrac.SeznamProvincii.Count + 1);

                        if (volba == AktualniHrac.SeznamProvincii.Count + 1)
                        {
                            ZobraNabidku("celaMapa");
                        }
                        else
                        {
                            AktualniProvincie = AktualniHrac.SeznamProvincii[volba - 1];
                            PrejitNaProvincii(AktualniProvincie);
                        }

                        break;
                    }
                case "vProvincii": //Player
                    {
                        AktualniHrac = Player;
                        AktualniProvincie.ZobrazPrehledProvincie();

                        Console.WriteLine("\n{0} -> Zpět",AktualniProvincie.Budovy.Count + 1);

                        volba = VyberZnabidky(AktualniProvincie.Budovy.Count + 1);

                        if (volba == AktualniProvincie.Budovy.Count + 1)
                        {
                            ZobraNabidku("mojeProvincie");
                        }
                        else
                        {
                            AktualniBudova = AktualniProvincie.Budovy[volba - 1];

                            AktualniBudova.Nahled();
                            ZobraNabidku("vProvincii");
                        }

                        break;
                    }
                case "vNepratelskeProvincii":
                    {
                        ZobrazPrehled(AktualniHrac, false);
                        Console.WriteLine();
                        Console.WriteLine(AktualniProvincie.JmenoProvincie); //Soupeř
                        Console.WriteLine();

                        Console.WriteLine("1 -> Informace");
                        Console.WriteLine("2 -> Zpět");

                        volba = VyberZnabidky(2);

                        switch (volba)
                        {
                            case 1: Player.VyberZpravu(AktualniProvincie, true); ZobraNabidku("vNepratelskeProvincii"); break;
                            case 2: ZobraNabidku("prehled"); break;
                        }

                        break;
                    }

                case "jednotky":
                    {
                        Player.VypisProvincieProNabidku();
                        Console.WriteLine("\n{0} -> Zpět", Player.SeznamProvincii.Count + 1);

                        volba = VyberZnabidky(Player.SeznamProvincii.Count + 1);

                        if (volba != Player.SeznamProvincii.Count + 1)
                        {
                            Console.Clear();
                            ZobrazPrehled(Player, true);

                            Console.WriteLine("{0}\n", Player.SeznamProvincii[volba - 1].JmenoProvincie);
                            Player.SeznamProvincii[volba - 1].ZobrazJednotky();
                            Console.WriteLine("\n1 -> Zpět");

                            volba = VyberZnabidky(1);
                            ZobraNabidku("jednotky");
                        }
                        else ZobraNabidku("koren");

                        break;
                    }
            }
        }

        public static int VyberZnabidky(int max)
        {
            int moznost;
            Console.Write("\n>> ");
            while ((!int.TryParse(Console.ReadLine(), out moznost)) || (moznost > max || moznost < 1)) Console.Write(">> ");

            return moznost;
        }

        public static int CtiCislo(string text, int max)
        {
            int cislo;
            Console.Write("\n{0}", text);
            while ((!int.TryParse(Console.ReadLine(), out cislo)) ||(cislo > max || cislo < 0)) Console.Write(text);

            return cislo;
        }

        private static void ZobrazMapu()
        {
            Console.WriteLine("_______________Japonsko_______________");

            Console.WriteLine("\nVládci:\n");
            for (int i = 0; i < Vladci.Count; i++)
            {
                Console.WriteLine("{0} {1}", i + 1, Vladci[i].Jmeno);
                Console.WriteLine("   {0}\n", Vladci[i].VypisVlastniciProvincie());
            }

        }

        

        public static void ZobrazPrehled(Hrac vladce, bool zobrazitLajnu)
        {
            if (zobrazitLajnu) Console.Clear();

            Console.Write("Vládce: ");
            if (zobrazitLajnu) Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(vladce.Jmeno);
            Console.ResetColor();

            if (zobrazitLajnu) Console.WriteLine("                        Den: " + Den);
            else Console.WriteLine();

            Console.WriteLine("Japonsko: {0}", vladce.ZobrazPostup(SeznamProvincii.Length));
            if (zobrazitLajnu) Console.WriteLine("----------------------------------------------------------\n");
        }

        private static void PrejitNaProvincii(Provincie provincie)
        {
            if (Player.SeznamProvincii.IndexOf(provincie) != -1) ZobraNabidku("vProvincii");
            else
            {
                ZobraNabidku("vNepratelskeProvincii");
            }
        }

        public static void PridejDoPath(string text)
        {
            Path.Replace(text, "");
            Path += "/" + text;
        }

        public static void OdeberZPath(string text)
        {
            Path.Replace(text, "");
        }

        public static void VypisPath()
        {
            Console.WriteLine(Path + "\n");
        }

        public static void NastavAktualnihoHrace(Hrac hrac) { AktualniHrac = hrac; }
        public static void NastavAktualniProvincii(Provincie provincie) { AktualniProvincie = provincie; }
        public static void NastavAktualniBudovu(Budova budova) { AktualniBudova = budova; }
        public static void InkrementujDen() { Den++; }

        public static List<Provincie> VratListVlastnenychProvinciiKromAktualni()
        {
            List<Provincie> listProvincii = new List<Provincie>();
            //Přiřazení provincii k výběru krom aktualni provincie
            foreach (Provincie p in Hra.Player.SeznamProvincii)
            {
                if (p.JmenoProvincie != AktualniProvincie.JmenoProvincie) listProvincii.Add(p);
            }

            //výpis
            for (int i = 0; i < listProvincii.Count; i++) Console.WriteLine("{0} -> {1}", i + 1, listProvincii[i].JmenoProvincie);

            Console.WriteLine("\n{0} -> Zpět", listProvincii.Count + 1);

            return listProvincii;
        }

        public static int VratSiluArmady(List<Jednotka> armada)
        {
            int hodnota = 0;
            foreach (Jednotka j in armada) hodnota += j.Utok * j.Pocet;
            return hodnota;
        }

        public static int VratPocetArmady(List<Jednotka> armada)
        {
            int hodnota = 0;
            foreach (Jednotka j in armada) hodnota += j.Pocet;
            return hodnota;
        }

        public static void VypisArmadu(List<Jednotka> armada)
        {
            Console.WriteLine("\n");

            int index = 1;
            foreach (Jednotka j in armada)
            {
                if (index % 4 == 0) Console.WriteLine();

                Console.Write("{0}: {1}  ", j.Jmeno, j.Pocet);

                index++;
            }

            Console.WriteLine();
        }

        public static void ZobrazViteznouZpravu(Hrac vitez)
        {
            Console.Clear();

            if (vitez == Player)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("               ###################");
                Console.WriteLine("               #    VÍTĚZSTVÍ!   #");
                Console.WriteLine("               ###################");
                Console.WriteLine("__________________________________________________________");
                Console.WriteLine("\n       {0} ovdládl celé Japonsko!", vitez.Jmeno);
                Console.WriteLine("__________________________________________________________\n");

                Console.WriteLine("Stáváte se vítězem");

                string vstup = "";
                while (vstup.ToLower() != "a" && vstup.ToLower() != "n")
                {
                    Console.Write("Přejete si pokračovat? [a/n]: ");
                    vstup = Console.ReadLine();
                }

                Console.ResetColor();

                if (vstup == "n") Program.VypisMenu("menu");
                //else hra pokračuje dál
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("               ###################");
                Console.WriteLine("               #     PORÁŽKA!    #");
                Console.WriteLine("               ###################");
                Console.WriteLine("__________________________________________________________");
                Console.WriteLine("\n       {0} obsadil všechny naše provincie", vitez.Jmeno);
                Console.WriteLine("__________________________________________________________\n");

                Console.WriteLine("Byl jste poražen\n");
                Console.WriteLine("Stiskem ENTER se dostanete do hlavní nabídky");
                Console.ReadLine();

                Console.ResetColor();
                Program.VypisMenu("menu");
            }
        }

        public static void VypisZpravuOobsazeni(string jmenoUtocnika, string jmenoProvincie, string jmenoPorazeneho, bool vladceVyhlazen)
        {
            ZobrazPrehled(Player, true);

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("{0} obsadil provincii {1} - {2}", jmenoUtocnika, jmenoProvincie, jmenoPorazeneho);

            if (vladceVyhlazen) Console.WriteLine("{0} byl vyhlazen!", jmenoPorazeneho);

            Console.ResetColor();

            Console.WriteLine("\nENTER");
            Console.ReadLine();
        }

        public static void VypisTextZeSouboru(string cesta)
        {
            try
            {
                using (StreamReader sr = new StreamReader(@cesta, Encoding.Default))
                {
                    string vypis = sr.ReadToEnd();
                    Console.WriteLine(vypis);
                    sr.Close();
                    
                    /*string s;

                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }*/
                }
            }
            catch
            {
                Console.WriteLine("Soubor {0} nenalezen", cesta);
            }
        }

        public static string Prompt()
        {
            string odpoved = "";

            while (odpoved.ToUpper() != "A" && odpoved.ToUpper() != "N")
            {
                ZobrazPrehled(Hra.Player, true);

                Console.WriteLine("Chystáte se ukončit hru a vrátit se do hlavní nabídky");
                Console.WriteLine("Veškerý postup bude ztracen!");

                Console.Write("\nOpravdu chcete ukončit hru? [a/n]: ");
                odpoved = Console.ReadLine();
            }

            return odpoved;
        }

        public static void ObarviAVypisText(string color, string text)
        {
            switch (color)
            {
                case "red": Console.ForegroundColor = ConsoleColor.Red; break;
                case "green": Console.ForegroundColor = ConsoleColor.Green; break;
                case "yellow": Console.ForegroundColor = ConsoleColor.Yellow; break;
            }

            Console.Write(text);
            Console.ResetColor();
        }

        public static void SpustExecko(string cesta)
        {
            try
            {
                Process.Start(@cesta);
            }
            catch
            {
                Console.Clear();
                Console.WriteLine("\nSoubor {0} nenalezen", cesta);
                Console.WriteLine("\nENTER");
                Console.ReadLine();
            }
            finally
            {
                Program.VypisMenu("menu");
            }
        }
    }

    class Zprava
    {
        public string JmenoVladce { get; private set; }
        public string JmenoProvincie { get; private set; }
        public int PocetObyvatel { get; private set; }
        public int LevelHradeb { get; private set; }

        public int DenSpehovani;
        public int Stari { get { return Hra.Den - DenSpehovani; } private set { } }

        public Zprava(string jmenoVladce, string jmenoProvincie, int pocetObyvatel, int levelHradeb)
        {
            JmenoVladce = jmenoVladce;
            JmenoProvincie = jmenoProvincie;
            PocetObyvatel = pocetObyvatel;
            LevelHradeb = levelHradeb;
            DenSpehovani = Hra.Den;
        }
    }
}
