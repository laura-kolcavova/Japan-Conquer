using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Japan_Conquer
{
    class Hrac
    {
        public string Jmeno { get; protected set; }
        public List<Provincie> SeznamProvincii { get; protected set; }

        public List<Hrac> Souperi { get; protected set; }

        public List<Zprava> ZpravyNinju { get; protected set; }

        /***********************************************************************/
        /***********************************************************************/   //Jaffa kree!
        public Hrac(string jmeno, Provincie[] provincie)
        {
            Jmeno = jmeno;
            SeznamProvincii = provincie.ToList();

            Souperi = new List<Hrac>();

            ZpravyNinju = new List<Zprava>();
        }
        /************************************************************************/
        /***********************************************************************/

        public virtual void Hraj()
        {
            Hra.NastavAktualnihoHrace(this);

            Obchoduj();

            Hra.ZobraNabidku("koren");
        }

        public override string ToString()
        {
            return Jmeno;
        }

        public string VypisVlastniciProvincie()
        {
            string s = "";
            for (int i = 0; i < SeznamProvincii.Count; i++)
            {
                s += SeznamProvincii[i].JmenoProvincie;
                if (SeznamProvincii.Last() != SeznamProvincii[i]) s += ", ";
            }

            return s;
        }

        public void VypisProvincieProNabidku()
        {
            for (int i = 0; i < SeznamProvincii.Count; i++)
            {
                Console.WriteLine("{0} -> {1}", i+1, SeznamProvincii[i].JmenoProvincie);
            }
        }

        public string ZobrazPostup(int CelkemProvincii)
        {
            // CelkemProvincii = 100%
            // SeznamProvincii.Count = x%

            double procento = ((double)SeznamProvincii.Count / CelkemProvincii) * 100;
            procento = Math.Round(procento);

            return procento + "%";
        }

        public  void PrepocitejSoupere()
        {
            Souperi = new List<Hrac>();

            foreach (Hrac vladce in Hra.Vladci)
            {
                if (vladce.Jmeno != Jmeno) Souperi.Add(vladce);
            }
        }

        public virtual void VyberJednoktyDoBoje(Provincie nepratelskaProvincie)
        {
            Hra.AktualniBudova.ZobrazInformace();
            string informace = string.Format("Útok na: {1}, {0}", nepratelskaProvincie.Vlastnik.Jmeno, nepratelskaProvincie.JmenoProvincie);

            List<Jednotka> armada = VyberJednotky(informace);

            if (armada != null)
            {
                if (armada.Count != 0)
                {
                    Console.WriteLine("\n\n1 -> Zaútočit!");
                    Console.WriteLine("2 -> Zpět");

                    int volba = Hra.VyberZnabidky(2);

                    switch (volba)
                    {
                        case 1:
                            {
                                Hra.AktualniProvincie.PridejJednotkyDoUtoku(armada.ToArray());
                                Hra.AktualniProvincie.PridejProvincieProUtok(nepratelskaProvincie);
                                Hra.AktualniBudova.ZobrazInformace();

                                Console.WriteLine("Armáda byla vyslána do provincie {0}", nepratelskaProvincie.JmenoProvincie);
                                Hra.VypisArmadu(armada);

                                Console.WriteLine("\n\n1 -> Zpět");
                                volba = Hra.VyberZnabidky(1);

                                ((Hrad)Hra.AktualniBudova).VyberAkci();
                                break;
                            }
                        case 2:
                            {
                                //Vrátit vybrané vojáky zpět
                                Hra.AktualniProvincie.PrijmiJednotky(armada.ToArray());

                                ((Hrad)Hra.AktualniBudova).VyberAkci(); break;
                            }
                    }
                }
                else
                {
                    Hra.AktualniBudova.ZobrazInformace();
                    Console.WriteLine("Nebyly vybrány žádné jednotky");
                    Console.WriteLine("\n1 -> Zpět");

                    int volba = Hra.VyberZnabidky(1);
                    ((Hrad)Hra.AktualniBudova).VyberAkci();
                }
            }
            else
            {
                Console.WriteLine("\nV provincii se nenacházejí žádné jednotky pro útok\n");

                Console.WriteLine("1 -> Zpět");

                int volba = Hra.VyberZnabidky(1);

                ((Hrad)Hra.AktualniBudova).VyberAkci();
            }
        }

        public virtual void VyberJednotkyProPodporu(Provincie cilovaProvincie)
        {
            Hra.AktualniBudova.ZobrazInformace();
            string inforamce = string.Format("Poslat jednotky do {0}", cilovaProvincie.JmenoProvincie);

            List<Jednotka> armada = VyberJednotky(inforamce);

            if (armada != null)
            {
                if (armada.Count != 0)
                {
                    Console.WriteLine("\n\n1 -> Poslat podporu");
                    Console.WriteLine("2 -> Zpět");

                    int volba = Hra.VyberZnabidky(2);

                    switch (volba)
                    {
                        case 1:
                            {
                                Hra.AktualniProvincie.PridejJednotkyProPodporu(armada.ToArray());
                                Hra.AktualniProvincie.PridejProvincieProPodporu(cilovaProvincie);
                                Hra.AktualniBudova.ZobrazInformace();

                                Console.WriteLine("Armáda byla vyslána do provincie {0}", cilovaProvincie.JmenoProvincie);
                                Hra.VypisArmadu(armada);

                                Console.WriteLine("\n\n1 -> Zpět");
                                volba = Hra.VyberZnabidky(1);

                                ((Hrad)Hra.AktualniBudova).PoslatPodporu();

                                break;
                            }
                        case 2:
                            {
                                //Vrácení jednotek
                                Hra.AktualniProvincie.PrijmiJednotky(armada.ToArray());
                                ((Hrad)Hra.AktualniBudova).PoslatPodporu();
                                break;
                            }
                    }
                }
                else
                {
                    Hra.AktualniBudova.ZobrazInformace();
                    Console.WriteLine("Nebyly vybrány žádné jednotky");
                    Console.WriteLine("\n1 -> Zpět");

                    int volba = Hra.VyberZnabidky(1);
                    ((Hrad)Hra.AktualniBudova).Nahled();
                }
            }
            else
            {
                Console.WriteLine("\nV provincii se nenacházejí žádné jednotky\n");
                Console.WriteLine("1 - > Zpět");
                int volba = Hra.VyberZnabidky(1);
                ((Hrad)Hra.AktualniBudova).Nahled();
            }
        }

        protected virtual List<Jednotka> VyberJednotky(string inforamce)
        {
            Console.WriteLine(inforamce);

            List<Jednotka> pluky = new List<Jednotka>();
            List<Jednotka> puvodniPluky = new List<Jednotka>();

            /*Ověření zda se v provincii nacházejí jednotky*/
            foreach (Jednotka j in Hra.AktualniProvincie.Jednotky)
            {
                if (j.Pocet != 0)
                {
                    pluky.Add(new Jednotka(j)); // viz Reference, v konstrukotru se předává i počet - viz souboje
                    puvodniPluky.Add(j);
                }
            }

            if (pluky.Count != 0)
            {
                List<Jednotka> armada = new List<Jednotka>();

                for (int i = 0; i < pluky.Count; i++)
                {
                    Console.WriteLine("\n\nVybrat jednotky:");

                    Jednotka pluk = pluky[i];

                    string s = string.Format("{0} [{1}]: ", pluk.Jmeno, pluk.Pocet);
                    int jednoktyDoPryc = Hra.CtiCislo(s, pluk.Pocet);

                    if (jednoktyDoPryc != 0)
                    {
                        pluk.NastavPocet(jednoktyDoPryc);
                        puvodniPluky[i].NastavPocet(puvodniPluky[i].Pocet - jednoktyDoPryc);
                        armada.Add(pluk);
                    }

                    ///Informace
                    Hra.AktualniBudova.ZobrazInformace();
                    Console.WriteLine(inforamce);
                    if (armada.Count != 0) Hra.VypisArmadu(armada);
                }

                return armada;
            }
            else
            {
                return null;
            }
        }

        public void ObsadProvincii(Provincie porazenaProvincie)
        {
            Hrac porazenySouper = porazenaProvincie.Vlastnik;

            //Přepočítání priorit !!!!!!!!!!!!!!!!!!!!!!ZměnA

            foreach (Hrac h in Hra.Vladci)
            {
                if (h is Pocitac)
                {
                    ((Pocitac)h).PrepocitejPriorityProvinciiProNovehoVlastnika(this, porazenySouper, porazenaProvincie);
                }
            }

            porazenaProvincie.VynulujProvincieAJednotkyProUtok();
            porazenaProvincie.VynulujProvincieASurovinyProDovoz();
            porazenaProvincie.VynulujProvincieProSpehovani();
            porazenaProvincie.VynulujJednoktyAProvincieProPodporu();

            porazenaProvincie.Ninjove.NastavPocet(0);
            porazenaProvincie.NinjoveMimoProvincii.NastavPocet(0);

            foreach (Jednotka j in porazenaProvincie.Jednotky) j.NastavPocet(0);

            SeznamProvincii.Add(porazenaProvincie);
            porazenySouper.SeznamProvincii.Remove(porazenaProvincie);

            if (this is Pocitac) ((Pocitac)this).ZvzsPriorituVladce(porazenySouper, 1);
            if (porazenySouper is Pocitac)
            {
                ((Pocitac)porazenySouper).ZvysPriorituProvincie(porazenaProvincie, 2);
                ((Pocitac)porazenySouper).ZvzsPriorituVladce(this, 2);
            }

            bool vladceVyhlazen = false;
            if (porazenySouper.SeznamProvincii.Count == 0)
            {
                //Priority - dříve - referenční pole Soupeři majngot
                foreach (Hrac h in Hra.Vladci)
                {
                    if (h is Pocitac && h != porazenySouper) ((Pocitac)h).OdeberSoupereZPrioritSouperuAprovincii(porazenySouper);
                }

                Hra.OdeberVladce(porazenySouper);
                vladceVyhlazen = true;

                //Přepočítá všem hráčům ve hře jejich soupeře
                Hra.PrepocitejSoupereVladcu();
            }

            if (this != Hra.Player && porazenySouper != Hra.Player) Hra.VypisZpravuOobsazeni(Jmeno, porazenaProvincie.JmenoProvincie, porazenySouper.Jmeno, vladceVyhlazen);
        }

        public virtual void VyslatNinju(Provincie nepratelskaProvincie)
        {
            Hra.AktualniBudova.ZobrazInformace();
            Console.WriteLine("Vyslat ninju do: {1}, {0}\n", nepratelskaProvincie.Vlastnik.Jmeno, nepratelskaProvincie.JmenoProvincie);

            if (Hra.AktualniProvincie.Ninjove.Pocet == 0)
            {
                Console.WriteLine("Nejsou dostupní žádní ninjové\n");

                Console.WriteLine("1 -> Zpět");

                int volba = Hra.VyberZnabidky(1);

                ((Hrad)Hra.AktualniBudova).VyberAkci();
            }
            else
            {
                Console.WriteLine("Ninjové: {0}\n", Hra.AktualniProvincie.Ninjove.Pocet);

                Console.WriteLine("1 -> Vyslat ninju!");
                Console.WriteLine("2 -> Zpět");

                int volba = Hra.VyberZnabidky(2);

                switch (volba)
                {
                    case 1:
                        Hra.AktualniBudova.ZobrazInformace();
                        Hra.AktualniProvincie.PridejProvinciiProSpehovani(nepratelskaProvincie);

                        Hra.AktualniProvincie.PridejNinjyMimoProvicnii(1);
                        Hra.AktualniProvincie.Ninjove.PridejPocet(-1);

                        Console.WriteLine("Ninja byl poslán do provincie {1} - {0}", nepratelskaProvincie.Vlastnik.Jmeno, nepratelskaProvincie.JmenoProvincie);

                        Console.WriteLine("\n1 -> Zpět");
                        volba = Hra.VyberZnabidky(1);

                        ((Hrad)Hra.AktualniProvincie.Budovy[0]).VyberAkci();
                        break;
                    case 2: ((Hrad)Hra.AktualniBudova).VyberAkci(); break;
                }
            }
        }

        public void VyberZpravu(Provincie vybranaProvincie, bool lokaceCelaMapa)
        {
            if (lokaceCelaMapa)
            {
                Console.Clear();
                Hra.ZobrazPrehled(Hra.Player, true);
            }
            else ((Hrad)Hra.AktualniProvincie.Budovy[0]).ZobrazInformace();
            ////////////////////////////////////////////////////////////////////

            bool je = false;
            foreach (Zprava zprava in ZpravyNinju)
            {
                if (zprava.JmenoProvincie == vybranaProvincie.JmenoProvincie)
                {
                    ZobrazVybranouZpravu(zprava);
                    je = true;
                    break;
                }
            }

            ///////////////////////////////////////////////////////////////////////

            if (je == false) Console.WriteLine("Provincie nebyla špehována - vyšli do ni Ninju aby o ní poskytl informace");
            
            Console.WriteLine("\n1 -> Zpět");

            int volba = Hra.VyberZnabidky(1);
            

            if (!lokaceCelaMapa) ((Hrad)Hra.AktualniProvincie.Budovy[0]).VyberAkci();
            // else = Hra -> ZobrazNabidku(vNepratelskeProvincii)
        }

        public void ZobrazVybranouZpravu(Zprava zprava)
        {
            Console.WriteLine("Provincie: {0}  Vladce: {1}      Den: {2}\n", zprava.JmenoProvincie, zprava.JmenoVladce, zprava.DenSpehovani);
            Console.WriteLine("Počet obyvatel: {0}", zprava.PocetObyvatel);
            Console.WriteLine("Hradby: {0}", zprava.LevelHradeb);
        }

        public void ZobrazZpravuPoBitve(bool jeHracUtocnik, bool vyhral,Provincie utociciProvincie, Provincie oblehanaProvincie, Jednotka[] utociciArmada, Jednotka[] puvodniUtociciArmada, Jednotka[] braniciArmada, Jednotka[] puvodniBraniciArmada)
        {
            Hra.ZobrazPrehled(this, true);

            if (jeHracUtocnik)
            {
                if (vyhral)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("             ÚTOK NA PROVINCII {0}\n", oblehanaProvincie.JmenoProvincie.ToUpper());
                    Console.WriteLine("Útok na provincii {0} - {1} byl úspěšný!", oblehanaProvincie.JmenoProvincie, oblehanaProvincie.Vlastnik.Jmeno);
                    Console.WriteLine("Provincie {0} byla obsazena", oblehanaProvincie.JmenoProvincie);
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("             ÚTOK NA PROVINCII {0}\n", oblehanaProvincie.JmenoProvincie.ToUpper());
                    Console.WriteLine("Útok na provincii {0} - {1} selhal!", oblehanaProvincie.JmenoProvincie, oblehanaProvincie.Vlastnik.Jmeno);
                    Console.WriteLine("Naše jednotky byly poraženy");
                    Console.ResetColor();
                }
            }
            else
            {
                if (vyhral)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("             OBRANA PROVINCIE {0}\n", oblehanaProvincie.JmenoProvincie.ToUpper());
                    Console.WriteLine("Na naší provincii {0} zaútočil {1} z provincie {2}", oblehanaProvincie.JmenoProvincie, utociciProvincie.Vlastnik.Jmeno, utociciProvincie.JmenoProvincie);
                    Console.WriteLine("Naše provincie byla ubráněna!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("             OBRANA PROVINCIE {0}\n", oblehanaProvincie.JmenoProvincie.ToUpper());
                    Console.WriteLine("Na naší provincii {0} zaútočil {1} z provincie {2}", oblehanaProvincie.JmenoProvincie, utociciProvincie.Vlastnik.Jmeno, utociciProvincie.JmenoProvincie);
                    Console.WriteLine("Naše provincie byla obsazena!");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("\nNaše jednotky:");
            for (int i = 0; i < puvodniUtociciArmada.Length; i++)
            {
                Jednotka j = puvodniUtociciArmada[i];
                Console.WriteLine("{0}: {1}  -{2}", j.Jmeno, j.Pocet, j.Pocet - utociciArmada[i].Pocet);
            }

            Console.WriteLine("\n\nNepřátelské jednotky:");
            for (int i = 0; i < puvodniBraniciArmada.Length; i++)
            {
                Jednotka j = puvodniBraniciArmada[i];
                Console.WriteLine("{0}: {1}  -{2}", j.Jmeno, j.Pocet, j.Pocet - braniciArmada[i].Pocet);
            }

            Console.WriteLine("\nENTER");
            Console.ReadLine();
        }

        public virtual void RozsirBudovu(Budova budovaKRozsireni)
        {
            budovaKRozsireni.ZobrazInformace();

            if (budovaKRozsireni.Level != budovaKRozsireni.MaxLevel)
            {
                if (budovaKRozsireni.JsouSurovinyNaRozsireni())
                {
                    Console.WriteLine("Budova byla rozšířena na level " + (budovaKRozsireni.Level + 1));
                    budovaKRozsireni.ZvysLevelBudovy();
                }
                else
                {
                    Console.WriteLine("Nejsou dostupné suroviny na rozšíření budovy");
                }
            }
            else
            {
                Console.WriteLine("Budova dosáhla maximální úrovně");
            }

            Console.WriteLine("\n1 -> Zpět");

            int volba = Hra.VyberZnabidky(1);

            budovaKRozsireni.Nahled();
        }

        public virtual void Obchoduj()
        {
            foreach (Provincie p in SeznamProvincii)
            {
                Hra.NastavAktualniProvincii(p);
                Budova trziste = p.Budovy[3];

                if (trziste.Level > 0 && ((Trziste)trziste).AutomatickeProdaniRyze == true)
                {
                    ((Trziste)trziste).Prodej("ryze", p.Ryze);
                }
            }
        }

        public void VbyerSurovinyKeKoupi(string surovina)
        {
            Budova trziste = Hra.AktualniProvincie.Budovy[3];
            trziste.ZobrazInformace();

            int mozno;
            switch (surovina)
            {
                case "ryze":
                    {
                        Console.WriteLine("Poměr zlata k rýži: 1:{0}\n", ((Trziste)trziste).PomerRyze);

                        mozno = Hra.AktualniProvincie.Zlato * ((Trziste)trziste).PomerRyze;
                        Console.WriteLine("Možno koupit: {0} Rýže", mozno);

                        int pocet = Hra.CtiCislo("Rýže: ", mozno);

                        ((Trziste)trziste).Kup("ryze", pocet);
                        break;
                    }
                case "drevo":
                    {
                        Console.WriteLine("Poměr zlata ke dřevu: 1:{0}\n", ((Trziste)trziste).PomerDreva);

                        mozno = Hra.AktualniProvincie.Zlato * ((Trziste)trziste).PomerDreva;
                        Console.WriteLine("Možno koupit: {0} Dřeva", mozno);

                        int pocet = Hra.CtiCislo("Dřevo: ", mozno);

                        ((Trziste)trziste).Kup("drevo", pocet);
                        break;
                    }
            }

            ((Trziste)trziste).Obchod();
        }

        public void VyberSurovinyKProdeji(string surovina)
        {
            Budova trziste = Hra.AktualniProvincie.Budovy[3];

            trziste.ZobrazInformace();

            int mozno;
            switch (surovina)
            {
                case "ryze":
                    {
                        Console.WriteLine("Poměr zlata k rýži: 1:{0}\n", ((Trziste)trziste).PomerRyze);

                        mozno = Hra.AktualniProvincie.Ryze;
                        Console.WriteLine("Možno prodat: {0} rýže", mozno);
                        int pocet = Hra.CtiCislo("Rýže: ", mozno);

                        ((Trziste)trziste).Prodej("ryze", pocet);
                        break;
                    }
                case "drevo":
                    {
                        Console.WriteLine("Poměr zlata ke dřevu: 1:{0}\n", ((Trziste)trziste).PomerDreva);

                        mozno = Hra.AktualniProvincie.Drevo;
                        Console.WriteLine("Možno prodat: {0} dřeva", mozno);
                        int pocet = Hra.CtiCislo("Dřevo: ", mozno);

                        ((Trziste)trziste).Prodej("drevo", pocet);
                        break;
                    }
            }

            ((Trziste)trziste).Obchod();
        }
    }
}
