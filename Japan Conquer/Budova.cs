using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Japan_Conquer
{
    class Budova
    {
        public string Jmeno { get; protected set; }
        public byte MaxLevel { get; protected set; }
        public byte Level { get; protected set; }

        protected int cenaDrevo;
        protected int cenaZlato;

        public int CenaDrevo { get { return Convert.ToInt32(Math.Floor(cenaDrevo * ((double)(Level + 1) * 1.2 ))); } protected set { } }
        public int CenaZlato { get { return Convert.ToInt32(Math.Floor(cenaZlato * ((double)(Level + 1) * 1.2))); } protected set { } }

        public List<Jednotka> Jednotky { get; protected set; }
        public Jednotka[] JednotkyProReferenci { get; protected set; }
        public Provincie VlastnenaProvincie { get { return ZjistiVlastnenouProvincii(); } protected set { } }

        public List<string> Informace { get; private set; }
        protected int volba;

        public static Budova Hrad = new Hrad();
        public static Budova Pole = new Pole();
        public static Budova Pila = new Pila();
        public static Budova Trziste = new Trziste();
        public static Budova Dojo = new Dojo();
        public static Budova Staje = new Staje();
        public static Budova Hradby = new Hradby();

        public static Budova[] SeznamBudov = { Hrad, Pole, Pila, Trziste, Dojo, Staje, Hradby };
                                             

        public Budova()
        {
            Level = 0;
            Informace = new List<string>();
        }


        public virtual void Nahled()
        {
            ZobrazInformace();
            if (Informace.Count != 0) ZobrazInformaceOlevelech();
        }

        public virtual void ZobrazInformace()
        {
            Console.Clear();
            Hra.ZobrazPrehled(Hra.Player, true);

            Console.Write(Hra.AktualniProvincie.JmenoProvincie);
            Console.WriteLine("            Zlato: {0}  Dřevo: {1}", Hra.AktualniProvincie.Zlato, Hra.AktualniProvincie.Drevo);
            Console.Write("\n{0}   Úroveň: {1}  ", Jmeno, Level);
        }

        protected void RozsirBudovu()
        {
            Hra.Player.RozsirBudovu(this);
        }

        protected virtual void RekrutujJednotky()
        {
            Console.Clear();
            Hra.ZobrazPrehled(Hra.Player, true);

            ZobrazInformace();

            VypisDostupnostPopulace();

            int max;
            if (Level == 0)
            {
                Console.WriteLine("Budova dosud nebyla postavena");
                max = 1;
            }
            else
            {
                if (Jednotky.Count == 0)
                {
                    Console.WriteLine("V budově zatím nejsou dostupné žádné jednotky");
                    max = 1;
                }
                else
                {
                    for (int i = 0; i < Jednotky.Count; i++)
                    {
                        Console.WriteLine("{0} -> {1}", i + 1, Jednotky[i].Jmeno);
                    }

                    max = Jednotky.Count + 1;
                }
            }

            Console.WriteLine("\n{0} -> Zpět", max);
            volba = Hra.VyberZnabidky(max);

            if (volba != max)
            {
                ZobrazInformace();
                VypisDostupnostPopulace();

                if (Hra.AktualniProvincie.Populace >= Hra.AktualniProvincie.MaxPopulace)
                {
                    Console.WriteLine("Nelze rekrutovat nové jednotky - nedostatek volných obyvatel");
                    Console.WriteLine("\n{0} -> Zpět", 1);
                    volba = Hra.VyberZnabidky(1);
                    RekrutujJednotky();
                }
                else
                {
                    if (!(Jednotky[volba - 1] is Ninja)) //výběr obyčejných jednotek
                    {
                        int index = 0; //index jednotky v provincii je jinačí než index jednotky v budově
                        for (int i = 0; i < Hra.AktualniProvincie.Jednotky.Length; i++)
                        {
                            if (Hra.AktualniProvincie.Jednotky[i].Jmeno == Jednotky[volba - 1].Jmeno) index = i;
                        }

                        Hra.AktualniProvincie.Jednotky[index].NaverbujJednotky();
                    }
                    else Hra.AktualniProvincie.Ninjove.NaverbujJednotky(); // výběr ninjy (není v provincie.Jednotky)

                    RekrutujJednotky();
                }
            }
            else
            {
                Nahled();
            }
        }

        public virtual void ZvysLevelBudovy()
        {
            Hra.AktualniProvincie.PrepoctiSuroviny(-CenaZlato, -CenaDrevo, 0);
            Level++;
        }

        public bool JsouSurovinyNaRozsireni()
        {
            if (CenaZlato > Hra.AktualniProvincie.Zlato || CenaDrevo > Hra.AktualniProvincie.Drevo || Level == MaxLevel) return false;
            else return true;
        }

        public void VypisDostupnostPopulace()
        {
            if (Hra.AktualniProvincie.Populace < Hra.AktualniProvincie.MaxPopulace)
            {
                Hra.ObarviAVypisText("green", string.Format("Populace: [{0}/{1}]\n\n", Hra.AktualniProvincie.Populace, Hra.AktualniProvincie.MaxPopulace));
            }
            else
            {
                Hra.ObarviAVypisText("red", string.Format("Populace: [{0}/{1}]\n\n", Hra.AktualniProvincie.Populace, Hra.AktualniProvincie.MaxPopulace));
            }
        }

        protected void VypisDostupnostSurovin()
        {
            if (Level != MaxLevel)
            {
                if (CenaZlato > Hra.AktualniProvincie.Zlato) Console.ForegroundColor = ConsoleColor.Red;
                else Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[Zlato: {0}  ", CenaZlato);
                Console.ResetColor();

                if (CenaDrevo > Hra.AktualniProvincie.Drevo) Console.ForegroundColor = ConsoleColor.Red;
                else Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Dřevo: {0}]", CenaDrevo);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Max level]");
                Console.ResetColor();
            }
        }

        protected virtual void OdemkniBudovu(Budova budovaProOdemknuti)
        {
            Hra.AktualniProvincie.Budovy.Add(budovaProOdemknuti);
            
            if (Hra.AktualniHrac == Hra.Player) Console.WriteLine("Byla odemknuta nová budova: {0}", budovaProOdemknuti.Jmeno);
        }

        protected virtual void OdemkniJednotku(Jednotka jednotkaProOdemknuti)
        {
            if (!(jednotkaProOdemknuti is Ninja)) Jednotky.Add(new Jednotka(jednotkaProOdemknuti));
            else Jednotky.Add(new Ninja(jednotkaProOdemknuti));
            
            if (Hra.AktualniHrac == Hra.Player) Console.WriteLine("Byla odemknuta nová jednotka: {0}", jednotkaProOdemknuti.Jmeno);
        }
        protected Provincie ZjistiVlastnenouProvincii()
        {
            Provincie vlastnenaProvincie = null;

            foreach (Provincie p in Hra.SeznamProvincii)
            {
                foreach (Budova b in p.Budovy)
                {
                    if (b == this)
                    {
                        vlastnenaProvincie = p;
                        break;
                    }
                }
            }

            return vlastnenaProvincie;
        }

        public bool JeOdemknutaVProvincii()
        {
            bool jeOdemknuta = false;

            Provincie vlastnenaProvincie = VlastnenaProvincie;

            if (vlastnenaProvincie != null)
            {
                foreach (Budova b in vlastnenaProvincie.Budovy)
                {
                    if (b == this)
                    {
                        jeOdemknuta = true;
                        break;
                    }
                }
            }

            return jeOdemknuta;
        }

        protected void ZobrazInformaceOlevelech()
        {
            List<string> vypis = new List<string>();

            foreach (string s in Informace) if (s != "") vypis.Add(s);

            if (vypis.Count != 0)
            {
                for (int i = 1; i <= vypis.Count; i++)
                {
                    if (i % 3 == 0) Console.WriteLine();
                    Console.Write(vypis[i - 1] + "".PadLeft(28 - vypis[i-1].Length));
                }

                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }




    /*////////////////////////////////////////////////////////////////
     * //                   Hrad                       //////////////
     * /////////////////////////////////////////////////////////////*/

    class Hrad : Budova
    {
        public Hrad()
        {
            Jmeno = "Hrad";
            Level = 1;
            MaxLevel = 10;
            Jednotky = new List<Jednotka>();
            JednotkyProReferenci = new Jednotka[2] { Jednotka.strelec, Jednotka.samuraj };

            cenaDrevo = 280;
            cenaZlato = 380;

            try
            {
                Informace.Add(string.Format("Level 3: {0}", Hradby.Jmeno));
                Informace.Add(string.Format("Level 5: {0}", Staje.Jmeno));
                Informace.Add(string.Format("Level 7: {0}", Jednotka.strelec.Jmeno));
                Informace.Add(string.Format("Level 8: {0}", Jednotka.samuraj.Jmeno));
            }
            catch { }
        }

        public int produkce { get { return 500 * Level; } private set { } }

        private Hrac souper;
        private Provincie vybranaProvincie;
        public int Obyvatele
        {
            get
            {
                if (Level != 0) return 100;
                else return 0;
            }
            private set { }
        }

        public override void Nahled()
        {
            base.Nahled();

            Console.WriteLine("Produkce: {0} Zlata\n", produkce);

            Console.Write("1 -> Rozšířit "); VypisDostupnostSurovin();
            Console.WriteLine("2 -> Rekrutovat jednotky");
            Console.WriteLine("3 -> Jednotky v provincii\n");
            Console.WriteLine("4 -> Naplánovat akci");
            Console.WriteLine("5 -> Poslat podporu\n");
            Console.WriteLine("6 -> Zpět");

            volba = Hra.VyberZnabidky(6);

            switch (volba)
            {
                case 1: RozsirBudovu(); break;
                case 2: RekrutujJednotky(); break;
                case 3: ZobrazJednotky(); break;
                case 4: NaplanujUtok(); break;
                case 5: PoslatPodporu(); break;
                case 6: break;
            }
        }

        private void ZobrazJednotky()
        {
            ZobrazInformace();

            Hra.AktualniProvincie.ZobrazJednotky();

            Console.WriteLine("\n1 -> Zpět");

            volba = Hra.VyberZnabidky(1);

            Nahled();
        }

        private void NaplanujUtok()
        {
            //Výběr soupeře
            ZobrazInformace();

            Console.WriteLine("Soupeři:\n");
            for (int i = 0; i < Hra.Player.Souperi.Count; i++)
            {
                Console.WriteLine("{0} {1}", i + 1, Hra.Player.Souperi[i].Jmeno);
                Console.WriteLine("   {0}\n", Hra.Player.Souperi[i].VypisVlastniciProvincie());
            }

            Console.WriteLine("\n{0} -> Zpět", Hra.Player.Souperi.Count + 1);

            volba = Hra.VyberZnabidky(Hra.Player.Souperi.Count + 1);

            if (volba != Hra.Player.Souperi.Count + 1)
            {
                souper = Hra.Player.Souperi[volba - 1];
                VyberNepratelskouProvincii();
            }
            else Nahled();
        }

        private void VyberNepratelskouProvincii()
        {
            //Výběr nepřátelské provincie
            ZobrazInformace();

            Console.WriteLine("{0} \n", souper.Jmeno);

            souper.VypisProvincieProNabidku();
            Console.WriteLine("\n{0} -> Zpět", souper.SeznamProvincii.Count + 1);

            volba = Hra.VyberZnabidky(souper.SeznamProvincii.Count + 1);

            if (volba == souper.SeznamProvincii.Count + 1) NaplanujUtok();
            else
            {
                vybranaProvincie = souper.SeznamProvincii[volba - 1];
                VyberAkci();
            }
        }

        public void VyberAkci()
        {
            //Výběr akce
            ZobrazInformace();

            Console.WriteLine("{0}  {1}\n", souper.Jmeno, vybranaProvincie.JmenoProvincie);

            Console.WriteLine("1 -> Útok");
            Console.WriteLine("2 -> Špehovat");
            Console.WriteLine("3 -> Informace");
            Console.WriteLine("\n4 -> Zpět");

            volba = Hra.VyberZnabidky(4);

            switch (volba)
            {
                case 1: Hra.Player.VyberJednoktyDoBoje(vybranaProvincie); break;
                case 2: Hra.Player.VyslatNinju(vybranaProvincie); break;
                case 3: Hra.Player.VyberZpravu(vybranaProvincie, false); break;
                case 4: VyberNepratelskouProvincii(); break;
            }
        }

        public void PoslatPodporu()
        {
            ZobrazInformace();

            if (Hra.AktualniHrac.SeznamProvincii.Count > 1)
            {
                Console.WriteLine("Poslat podporu do:\n");

                List<Provincie> vlastneneProvincie = Hra.VratListVlastnenychProvinciiKromAktualni();

                volba = Hra.VyberZnabidky(vlastneneProvincie.Count + 1);

                if (volba != vlastneneProvincie.Count + 1) Hra.Player.VyberJednotkyProPodporu(vlastneneProvincie[volba - 1]);
                else Nahled();
            }
            else
            {
                Console.WriteLine("Nejsou dostupné žadné jiné provincie");
                Console.WriteLine("\n1 -> Zpět");
                volba = Hra.VyberZnabidky(1);
                Nahled();
            }
        }

        public override void ZvysLevelBudovy()
        {
            base.ZvysLevelBudovy();

            switch (Level)
            {
                case 3: OdemkniBudovu(new Hradby()); Informace[0] = ""; break;
                case 5: OdemkniBudovu(new Staje()); Informace[1] = ""; break;
                case 7: OdemkniJednotku(Jednotka.strelec); Informace[2] = ""; break;
                case 8: OdemkniJednotku(Jednotka.samuraj); Informace[3] = ""; break;
            }
        }

        public int VyprodukujSuroviny()
        {
            return produkce;
        }

        public override void ZobrazInformace()
        {
            base.ZobrazInformace();

            foreach (Jednotka j in Jednotky)
            {
                foreach (Jednotka k in Hra.AktualniProvincie.Jednotky) if (j.Jmeno == k.Jmeno) j.NastavPocet(k.Pocet);
                Console.Write("  {0}: {1}", j.Jmeno, j.Pocet);
             
            }

            Console.WriteLine("\n");
        }
    }
    
    /*////////////////////////////////////////////////////////////////
     * //                   Dojo                        //////////////
     * /////////////////////////////////////////////////////////////*/

    class Dojo : Budova
    {
        public Dojo()
        {
            Jmeno = "Dódžo";
            MaxLevel = 5;

            Jednotky = new List<Jednotka>();
            Jednotky.Add(new Jednotka(Jednotka.kopijnik));

            JednotkyProReferenci = new Jednotka[3] { Jednotka.kopijnik, Jednotka.lucistnik, Jednotka.ninja };

            cenaDrevo = 150;
            cenaZlato = 90;

            Informace.Add(string.Format("Level 2: {0}", Jednotka.lucistnik.Jmeno));
            Informace.Add(string.Format("Level 3: {0}", Jednotka.ninja.Jmeno));
        }

        public int MaxNinju { get { return Level; } private set { } }

        public override void Nahled()
        {
            base.Nahled();

            Console.Write("1 -> Rozšířit "); VypisDostupnostSurovin();
            Console.WriteLine("2 -> Rekrutovat jednotky");
            Console.WriteLine("3 -> Zpět");

            volba = Hra.VyberZnabidky(3);

            switch (volba)
            {
                case 1: RozsirBudovu(); break;
                case 2: RekrutujJednotky(); break;
                case 3: break;
            }
        }

        public override void ZobrazInformace()
        {
            base.ZobrazInformace();

            foreach (Jednotka j in Jednotky)
            {
                if (j is Ninja) j.NastavPocet(Hra.AktualniProvincie.Ninjove.Pocet);
                else
                {
                    foreach (Jednotka k in Hra.AktualniProvincie.Jednotky) if (j.Jmeno == k.Jmeno) j.NastavPocet(k.Pocet);
                }
                Console.Write("  {0}: {1}", j.Jmeno, j.Pocet);
            }

            Console.WriteLine("\n");
        }

        public override void ZvysLevelBudovy()
        {
            base.ZvysLevelBudovy();

            switch (Level)
            {
                case 2: OdemkniJednotku(Jednotka.lucistnik); Informace[0] = ""; break;
                case 3: OdemkniJednotku(Jednotka.ninja); Informace[1] = ""; break;
            }
        }
    }



    /*////////////////////////////////////////////////////////////////
     * //                   Stáje                        //////////////
     * /////////////////////////////////////////////////////////////*/

    class Staje : Budova
    {
        public Staje()
        {
            Jmeno = "Stáje";
            MaxLevel = 2;

            Jednotky = new List<Jednotka>();
            Jednotky.Add(new Jednotka(Jednotka.jezdec));

            JednotkyProReferenci = new Jednotka[2] { Jednotka.jezdec, Jednotka.lucistnikNaKoni };

            cenaDrevo = 200;
            cenaZlato = 120;

            Informace.Add(string.Format("Level 2: {0}", Jednotka.lucistnikNaKoni.Jmeno));
        }

        public override void Nahled()
        {
            base.Nahled();

            Console.Write("1 -> Rozšířit "); VypisDostupnostSurovin();
            Console.WriteLine("2 -> Rekrutovat jednotky");
            Console.WriteLine("3 -> Zpět");

            volba = Hra.VyberZnabidky(3);

            switch (volba)
            {
                case 1: RozsirBudovu(); break;
                case 2: RekrutujJednotky(); break;
                case 3: break;
            }
        }

        public override void ZobrazInformace()
        {
            base.ZobrazInformace();

            foreach (Jednotka j in Jednotky)
            {
                foreach (Jednotka k in Hra.AktualniProvincie.Jednotky) if (j.Jmeno == k.Jmeno) j.NastavPocet(k.Pocet);
                Console.Write("  {0}: {1}", j.Jmeno, j.Pocet);
            }

            Console.WriteLine("\n");
        }

        public override void ZvysLevelBudovy()
        {
            base.ZvysLevelBudovy();

            switch (Level)
            {
                case 2: OdemkniJednotku(Jednotka.lucistnikNaKoni); Informace.RemoveAt(0); break;
            }
        }
    }


    /*////////////////////////////////////////////////////////////////
     * //                   Domy                       //////////////
     * /////////////////////////////////////////////////////////////*/

    class Domy : Budova
    {
        public Domy()
        {
            Jmeno = "Domy";
            MaxLevel = 30;

            cenaDrevo = 60;
            cenaZlato = 20;
        }

        public int Obyvatele
        {
            get
            {
                return 10 * Level;
            }
            private set
            {
            }
        }

        public override void Nahled()
        {
            base.Nahled();

            Console.Write("1 -> Rozšířit "); VypisDostupnostSurovin();
            Console.WriteLine("2 -> Zpět");

            volba = Hra.VyberZnabidky(2);

            switch (volba)
            {
                case 1: RozsirBudovu(); break;
                case 2: break;
            }
        }

        public override void ZvysLevelBudovy()
        {
            base.ZvysLevelBudovy();

            if (Hra.AktualniHrac == Hra.Player) Console.WriteLine("Populace byla navýšena o {0} obyvatel", Obyvatele);
        }

        public override void ZobrazInformace()
        {
            base.ZobrazInformace();

            Console.WriteLine("  Populace: {0}/{1}\n",Hra.AktualniProvincie.Populace, Hra.AktualniProvincie.MaxPopulace);
        }
    }


    /*////////////////////////////////////////////////////////////////
     * //                   Tržiště                       ////////////
     * /////////////////////////////////////////////////////////////*/

    class Trziste : Budova
    {
        public Trziste()
        {
            Jmeno = "Tržiště";
            MaxLevel = 1;

            cenaDrevo = 100;
            cenaZlato = 50;

            AutomatickeProdaniRyze = false;
        }

        public int PomerRyze { get { return 2; } private set { } }
        public int PomerDreva { get { return 1; } private set { } }
        public bool AutomatickeProdaniRyze { get; private set; }

        public override void Nahled()
        {
            base.Nahled();

            string prodaniRyze = "Vypnuto";
            if (AutomatickeProdaniRyze) prodaniRyze = "Zapnuto";

            Console.Write("1 -> Rozšířit "); VypisDostupnostSurovin();
            Console.WriteLine("2 -> Obchod");
            Console.WriteLine("3 -> Poslat suroviny");
            Console.Write("4 -> Automatické prodání rýže [", prodaniRyze);

            if (AutomatickeProdaniRyze) Hra.ObarviAVypisText("green", prodaniRyze);
            else Hra.ObarviAVypisText("red", prodaniRyze);
            Console.WriteLine("]\n");

            Console.WriteLine("5 -> Zpět");

            volba = Hra.VyberZnabidky(5);

            switch (volba)
            {
                case 1: RozsirBudovu(); break;
                case 2: Obchod(); break;
                case 3: PoslatSuroviny(); break;
                case 4: NastavProdaniRyze(); break;
                case 5: break;
            }
        }

        public void Obchod()
        {
            ZobrazInformace();

            if (Level != 0)
            {
                Console.WriteLine("1 -> Prodat rýži");
                Console.WriteLine("2 -> Prodat dřevo\n");
                Console.WriteLine("3 -> Koupit rýži");
                Console.WriteLine("4 -> Koupit dřevo\n");
                Console.WriteLine("5 -> Zpět");

                volba = Hra.VyberZnabidky(5);

                switch (volba)
                {
                    case 1: Hra.Player.VyberSurovinyKProdeji("ryze"); break;
                    case 2: Hra.Player.VyberSurovinyKProdeji("drevo"); break;
                    case 3: Hra.Player.VbyerSurovinyKeKoupi("ryze"); break;
                    case 4: Hra.Player.VbyerSurovinyKeKoupi("drevo"); break;
                    case 5: Nahled(); break;
                }
            }
            else
            {
                Console.WriteLine("Budova dosud nebyla postavena");
                Console.WriteLine("\n1 -> Zpět");
                volba = Hra.VyberZnabidky(1);
                Nahled();
            }
        }

        public void Prodej(string surovina, int pocet)
        {
            switch (surovina)
            {
                case "ryze":
                    {
                        Hra.AktualniProvincie.PrepoctiSuroviny(pocet / PomerRyze, 0, -pocet);
                        break;
                    }
                case "drevo":
                    {
                        Hra.AktualniProvincie.PrepoctiSuroviny(pocet / PomerDreva, -pocet, 0);
                        break;
                    }
            }
        }

        public void Kup(string surovina, int pocet)
        {
            switch (surovina)
            {
                case "ryze":
                    {
                        Hra.AktualniProvincie.PrepoctiSuroviny(-(pocet / PomerRyze), 0, pocet);
                        break;
                    }
                case "drevo":
                    {
                        Hra.AktualniProvincie.PrepoctiSuroviny(-(pocet / PomerDreva), pocet, 0);
                        break;
                    }
            }
        }

        public void PoslatSuroviny()
        {
            ZobrazInformace();

            if (Level != 0)
            {
                if (Hra.AktualniHrac.SeznamProvincii.Count > 1)
                {
                    List<Provincie> listProvincii = Hra.VratListVlastnenychProvinciiKromAktualni();

                    volba = Hra.VyberZnabidky(listProvincii.Count + 1);

                    if (volba != listProvincii.Count + 1) VyberSurovinyProProvincii(listProvincii[volba - 1]);
                    else Nahled();
                }
                else
                {
                    Console.WriteLine("Nejsou dostupné žadné jiné provincie");
                    Console.WriteLine("\n1 -> Zpět");
                    volba = Hra.VyberZnabidky(1);
                    Nahled();
                }
            }
            else
            {
                Console.WriteLine("Budova dosud nebyla postavena");
                Console.WriteLine("\n1 -> Zpět");
                volba = Hra.VyberZnabidky(1);
                Nahled();
            }
        }

        private void VyberSurovinyProProvincii(Provincie vybranaProvincie)
        {
            int[] suroviny = new int[3];
            string[] jmenaSurovn = { "Zlato: ", "Dřevo: ", "Rýže: " };
            int[] surovinyMax = { Hra.AktualniProvincie.Zlato, Hra.AktualniProvincie.Drevo, Hra.AktualniProvincie.Ryze };

            for (int i = 0; i <= suroviny.Length; i++)
            {
                ZobrazInformace();

                Console.WriteLine("Poslat suroviny do {0}", vybranaProvincie.JmenoProvincie);
                Console.WriteLine("Vybrané suroviny: Zlato: {0}  Dřevo: {1}  Rýže: {2}", suroviny[0], suroviny[1], suroviny[2]);

                if (i < suroviny.Length) suroviny[i] = Hra.CtiCislo(jmenaSurovn[i], surovinyMax[i]);
            }

            Console.WriteLine("\n1 -> Polsat suroviny");
            Console.WriteLine("2 -> Zpět");

            volba = Hra.VyberZnabidky(2);

            switch (volba)
            {
                case 1:
                    Hra.AktualniProvincie.PridejProvinciiProDovozSurovin(vybranaProvincie);
                    Hra.AktualniProvincie.PridejSurovinyProDovoz(suroviny);
                    Hra.AktualniProvincie.PrepoctiSuroviny(-suroviny[0], -suroviny[1], -suroviny[2]);

                    ZobrazInformace();
                    Console.WriteLine("Suroviny jsou na cestě do provincie {0}\n", vybranaProvincie.JmenoProvincie);
                    Console.WriteLine("1 -> Zpět");

                    volba = Hra.VyberZnabidky(1);

                    PoslatSuroviny();
                    break;
                case 2: PoslatSuroviny(); break;
            }
        }

        private void NastavProdaniRyze()
        {
            if (Level != 0)
            {
                if (AutomatickeProdaniRyze) AutomatickeProdaniRyze = false;
                else AutomatickeProdaniRyze = true;
            }
            else
            {
                ZobrazInformace();

                Console.WriteLine("Budova ještě nebyla postavena\n");
                Console.WriteLine("1 -> Zpět");

                volba = Hra.VyberZnabidky(1);
            }

            Nahled();
        }

        public override void ZobrazInformace()
        {
            Console.Clear();
            Hra.ZobrazPrehled(Hra.Player, true);

            Console.WriteLine(Hra.AktualniProvincie.JmenoProvincie);
            Console.Write("\n{0}   Úroveň: {1}  ", Jmeno, Level);

            Console.WriteLine("  Zlato: {0}  Dřevo: {1}  Rýže: {2}\n", Hra.AktualniProvincie.Zlato, Hra.AktualniProvincie.Drevo, Hra.AktualniProvincie.Ryze);
        }
    }


    /*////////////////////////////////////////////////////////////////
     * //                   Pole                        //////////////
     * /////////////////////////////////////////////////////////////*/

    class Pole : Budova
    {
        public int produkce
        {
            get
            {
                if (Level == 0) return 0;
                else return 500 * Level; //dříve 100 * level + 500
            }

            private set { }
        }

        public Pole()
        {
            Jmeno = "Rýžová pole";
            MaxLevel = 15;

            Level = 1;

            cenaDrevo = 50;
            cenaZlato = 20;
        }

        public override void Nahled()
        {
            base.Nahled();

            Console.Write("1 -> Rozšířit "); VypisDostupnostSurovin();
            Console.WriteLine("2 -> Zpět");

            volba = Hra.VyberZnabidky(2);

            switch (volba)
            {
                case 1: RozsirBudovu(); break;
                case 2: break;
            }
        }

        public int VyprodukujSuroviny()
        {
            return produkce;
        }

        public override void ZobrazInformace()
        {
            base.ZobrazInformace();

            Console.WriteLine("  Produkce: {0} Rýže\n", produkce);
        }
    }


    /*////////////////////////////////////////////////////////////////
     * //                   Pila                       ////////////
     * /////////////////////////////////////////////////////////////*/

    class Pila : Budova
    {
        public int produkce
        {
            get
            {
                if (Level == 0) return 0;
                else return 500 * Level; //dříve 100 * level + 500
            }

            private set { }
        }

        public Pila()
        {
            Jmeno = "Pila";
            MaxLevel = 15;

            Level = 1;

            cenaDrevo = 40;
            cenaZlato = 30;
        }
        
        public override void Nahled()
        {
            base.Nahled();

            Console.Write("1 -> Rozšířit "); VypisDostupnostSurovin();
            Console.WriteLine("2 -> Zpět");

            volba = Hra.VyberZnabidky(2);

            switch (volba)
            {
                case 1: RozsirBudovu(); break;
                case 2: break;
            }
        }

        public int VyprodukujSuroviny()
        {
            return produkce;
        }

        public override void ZobrazInformace()
        {
            base.ZobrazInformace();

            Console.WriteLine("  Produkce: {0} Dřeva\n", produkce);
        }
    }

    /*////////////////////////////////////////////////////////////////
     * //                   Hradby                       ////////////
     * /////////////////////////////////////////////////////////////*/

    class Hradby : Budova
    {
        public int ObranaBonus { get { return Level * 10; } private set { } }

        public Hradby()
        {
            Jmeno = "Hradby";
            MaxLevel = 10;
            Level = 0;

            cenaDrevo = 180;
            cenaZlato = 130;
        }

        public override void Nahled()
        {
            base.Nahled();

            Console.Write("1 -> Rozšířit "); VypisDostupnostSurovin();
            Console.WriteLine("2 -> Zpět");

            volba = Hra.VyberZnabidky(2);

            switch (volba)
            {
                case 1: RozsirBudovu(); break;
                case 2: break;
            }
        }

        public override void ZobrazInformace()
        {
            base.ZobrazInformace();

            Console.WriteLine("  Obranný bonus: {0}%\n", ObranaBonus);
        }
    }
}