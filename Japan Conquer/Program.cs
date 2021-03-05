using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Japan_Conquer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Japan Conquer";
  
            VypisMenu("menu");
        }

        private static void ZahajHru()
        {
            Console.Clear();

            //Deklarace provincií 

            Provincie owari = new Provincie("Owari");
            Provincie suruga = new Provincie("Suruga");
            Provincie totomi = new Provincie("Tótomi");
            Provincie mino = new Provincie("Mino");
            Provincie mikawa = new Provincie("Mikawa");
            Provincie kai = new Provincie("Kai");
            Provincie omi = new Provincie("Ōmi");
            Provincie ecizen = new Provincie("Echizen");
            Provincie ecigo = new Provincie("Echigo");
            Provincie aki = new Provincie("Aki");
            Provincie izu = new Provincie("Izu");
            Provincie izumo = new Provincie("Izumo");

            Provincie[] seznamProvincii = { owari, suruga, totomi, mino, mikawa, kai, omi, ecizen, ecigo, aki, izu, izumo };


            //Deklarace vládců - typ Pocitac

            Hrac[] vladci = new Hrac[11];
            vladci[0] = new Pocitac("Nobunaga Oda", new Provincie[] { owari });
            vladci[1] = new Pocitac("Imagawa Jošimoto", new Provincie[] { suruga, totomi });
            vladci[2] = new Pocitac("Dosan Saito", new Provincie[] { mino });
            vladci[3] = new Pocitac("Iejasu Tokugawa", new Provincie[] { mikawa });
            vladci[4] = new Pocitac("Azai Nagamasa", new Provincie[] { omi });
            vladci[5] = new Pocitac("Asakura Yoshikage", new Provincie[] { ecizen });
            vladci[6] = new Pocitac("Shingen Takeda", new Provincie[] { kai });
            vladci[7] = new Pocitac("Kenshin Uesugi", new Provincie[] { ecigo });
            vladci[8] = new Pocitac("Motonari Mori", new Provincie[] { aki });
            vladci[9] = new Pocitac("Soun Hojo", new Provincie[] { izu });
            vladci[10] = new Pocitac("Amago Yoshihisa", new Provincie[] { izumo });


            //Vybraní si vládce za kterého bude hrát hráč - ostatní zůstanou typu Pocitac
            int indexVybranehoVladce = Hra.VyberVladce(vladci);
            vladci[indexVybranehoVladce] = new Hrac(vladci[indexVybranehoVladce].Jmeno, vladci[indexVybranehoVladce].SeznamProvincii.ToArray());

            //Deklarace hry 
            Hra.DeklarujHru(vladci, seznamProvincii, indexVybranehoVladce);


            ///////////////////////////////////////////////////////////////////////////////////////
            //Hra
            while (true)
            {
                foreach (Hrac h in Hra.Vladci)
                {
                    h.Hraj();
                }


                Console.Clear();
                Console.WriteLine("Kolo ukončeno");
                Console.WriteLine("ENTER");
                Console.ReadLine();


                /////////////////////////////
                //Skončení kol - vyhodnocení:

                //Vyhodnocení produkce, špehování a přesunů suroviny atd atd atd atd
                foreach (Hrac h in Hra.Vladci)
                {
                    Hra.NastavAktualnihoHrace(h);

                    foreach (Provincie provinceVladce in h.SeznamProvincii)
                    {
                        Hra.NastavAktualniProvincii(provinceVladce);

                        //Vyhodnocení produkce
                        provinceVladce.PrijmiVyprodukovaneSuroviny();

                        //Vyhodnocení špehovacích požadavků
                        if (provinceVladce.NepratelskeProvincieProSpehovani.Count != 0)
                        {
                            foreach (Provincie provincieProSpehovani in provinceVladce.NepratelskeProvincieProSpehovani)
                            {
                                ((Ninja)provinceVladce.NinjoveMimoProvincii).JdiDoNepratelskeProvincie(provincieProSpehovani);
                            }
                            provinceVladce.VynulujProvincieProSpehovani();
                        }

                        //Vyhodnocení dovozních požadavků
                        if (provinceVladce.ProvincieProDovozSurovin.Count != 0)
                        {
                            int index = 0;
                            foreach (Provincie provincieProDovoz in provinceVladce.ProvincieProDovozSurovin)
                            {
                                provinceVladce.PrepravSuroviny(provinceVladce.SurovinyProDovoz[index], provincieProDovoz);
                                index++;
                            }
                            provinceVladce.VynulujProvincieASurovinyProDovoz();
                        }

                        //Vyhodnocení požadávků podpory
                        if (provinceVladce.ProvincieProPodporu.Count != 0)
                        {
                            for (int i = 0; i < provinceVladce.ProvincieProPodporu.Count; i++)
                            {
                                provinceVladce.PosliJednotky(provinceVladce.ProvincieProPodporu[i], provinceVladce.JednotkyProPodporu[i]);
                            }
                            provinceVladce.VynulujJednoktyAProvincieProPodporu();
                        }
                    }

                }


                //Vyhodnocení útočných požadavků  2. foreach kvůli změnám v kolekci hra.vládci
                //Kvůli změně kolekci

                bool pruchod = true;
                while (pruchod)
                {
                    pruchod = false;

                    foreach (Hrac utociciVladce in Hra.Vladci)
                    {
                        Hra.NastavAktualnihoHrace(utociciVladce);

                        foreach (Provincie utociciProvincie in utociciVladce.SeznamProvincii)
                        {
                            Hra.NastavAktualniProvincii(utociciProvincie);

                            if (utociciProvincie.ProvincieProUtok.Count != 0)
                            {
                                for (int i = 0; i < utociciProvincie.ProvincieProUtok.Count; i++)
                                {
                                    utociciProvincie.PosliJednotky(utociciProvincie.ProvincieProUtok[i], utociciProvincie.JednotkyDoUtoku[i]);
                                }

                                if (Hra.Vladci.Count == 1 || Hra.Player.SeznamProvincii.Count == 0)
                                {
                                    //utočící vládce porazil posledního vládce a stává se vítězem - hra končí
                                    Hra.ZobrazViteznouZpravu(utociciVladce);
                                }

                                utociciProvincie.VynulujProvincieAJednotkyProUtok();

                                pruchod = true;
                                break;
                            }
                        }

                        if (pruchod == true) break;
                    }
                }

                //Zvyýší den o 1
                Hra.InkrementujDen();
            }
        }

        private static int volba;
        public static void VypisMenu(string lokace)
        {
            Console.Clear();
            Console.WriteLine("\n                Japan Conquer");
            Console.WriteLine("-------------------------------------------------\n");

            switch (lokace)
            {
                case "menu":
                    {
                        string[] polozkyMenu = {
                                                 "Nová hra",
                                                 "O hře",
                                                 "Historie",
                                                 "Simulátor bitvy",
                                                 "Ukončit"
                                             };
        
                        volba = VypisPolozkyAVratIndex(polozkyMenu);

                        switch (volba)
                        {
                            case 0: ZahajHru(); break;
                            case 1: VypisMenu("oHre"); break;
                            case 2: VypisMenu("historie"); break;
                            case 3: Hra.SpustExecko("Simulator bitvy.exe"); break;
                            case 4: Environment.Exit(0); break;
                        }

                        break;
                    }
                case "oHre":
                    {
                        Hra.VypisTextZeSouboru("JakHrat.txt");

                        Console.WriteLine("\n1 -> Zpět");

                        volba = Hra.VyberZnabidky(1);
                        VypisMenu("menu");

                        break;
                    }

                case "historie":
                        {
                            Hra.VypisTextZeSouboru("Historie.txt");

                            Console.WriteLine("\n1 -> Zpět");

                            volba = Hra.VyberZnabidky(1);
                            VypisMenu("menu");

                            break;
                        }
            }
        }

        public static int VypisPolozkyAVratIndex(string[] polozky)
        {
            int aktivniPolozka = 0;
            bool vyberDokoncen = false;

            while(!vyberDokoncen)
            {
                //Výpis položek
                for (int i = 0; i < polozky.Length; i++)
                {
                    if (aktivniPolozka == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }

                    Console.SetCursorPosition(2, 4 + i);
                    Console.WriteLine(polozky[i]);
                    Console.ResetColor();
                }

                //Výběr položky
                Console.CursorVisible = false;
                ConsoleKeyInfo stisknutaKlavesa = Console.ReadKey(true);

                switch (stisknutaKlavesa.Key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (aktivniPolozka == 0) aktivniPolozka = polozky.Length - 1;
                            else aktivniPolozka--;
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (aktivniPolozka == polozky.Length - 1) aktivniPolozka = 0;
                            else aktivniPolozka++;
                            break;
                        }
                    case ConsoleKey.Enter:
                        {
                            vyberDokoncen = true;
                            break;
                        }
                }

                Console.Clear();
                Console.WriteLine("\n                Japan Conquer");
                Console.WriteLine("-------------------------------------------------\n");
            }

            return aktivniPolozka;

        }


    }
}
