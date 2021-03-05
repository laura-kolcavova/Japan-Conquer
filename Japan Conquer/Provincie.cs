using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Japan_Conquer
{
    class Provincie
    {
        
        public string JmenoProvincie { get; private set; }

        public int Drevo { get; private set; }
        public int Zlato { get; private set; }
        public int Ryze { get; private set; }

        public int MaxDreva { get; private set; }
        public int MaxZlata { get; private set; }
        public int MaxRyze { get; private set; }

        public Jednotka[] Jednotky { get; private set; }

        public List<Jednotka[]> JednotkyDoUtoku { get; private set; }
        public List<Provincie> ProvincieProUtok { get; private set; }

        public List<Jednotka[]> JednotkyProPodporu { get; private set; }
        public List<Provincie> ProvincieProPodporu { get; private set; }

        public Jednotka Ninjove { get; private set; }
        public Jednotka NinjoveMimoProvincii { get; private set; }
        public List<Provincie> NepratelskeProvincieProSpehovani { get; private set; }

        public List<Budova> Budovy { get; private set; }

        public List<Provincie> ProvincieProDovozSurovin { get; private set; }
        public List<int[]> SurovinyProDovoz { get; private set; }

        public Hrac Vlastnik
        {
            get { return ZjistiVlastnika(); }
            private set { }
        }
        public int MaxPopulace
        {
            get { return PrepocitejAVratMaxPopulaci(); }
            private set { }
        }
        public int Populace
        {
            get { return PrepocitejAVratPopulaci(); }
            private set { }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /*//////////////  KONSTRUKTOR  /////////////*/

        public Provincie(string jmenoProvincie)
        {
            JmenoProvincie = jmenoProvincie;

            Drevo = 4000;
            Zlato = 5000;
            Ryze = 2000;

            MaxDreva = 50000;
            MaxZlata = 50000;
            MaxRyze = 50000;

            Jednotky = new Jednotka[6];
            Jednotky[0] = new Jednotka(Jednotka.kopijnik);
            Jednotky[1] = new Jednotka(Jednotka.lucistnik);
            Jednotky[2] = new Jednotka(Jednotka.jezdec);
            Jednotky[3] = new Jednotka(Jednotka.lucistnikNaKoni);
            Jednotky[4] = new Jednotka(Jednotka.strelec);
            Jednotky[5] = new Jednotka(Jednotka.samuraj);
                
            Budovy = new List<Budova>();
            Budovy.Add(new Hrad());
            Budovy.Add(new Pole());
            Budovy.Add(new Pila());
            Budovy.Add(new Trziste());
            Budovy.Add(new Domy());
            Budovy.Add(new Dojo());

            JednotkyDoUtoku = new List<Jednotka[]>();
            ProvincieProUtok = new List<Provincie>();

            JednotkyProPodporu = new List<Jednotka[]>();
            ProvincieProPodporu = new List<Provincie>();

            Ninjove = new Ninja(Jednotka.ninja);
            NinjoveMimoProvincii = new Ninja(Jednotka.ninja);
            NepratelskeProvincieProSpehovani = new List<Provincie>();

            ProvincieProDovozSurovin = new List<Provincie>();
            SurovinyProDovoz = new List<int[]>();
        }
        /*////////////////////////////////////////////////////////////////////*/
        /*////////////////////////////////////////////////////////////////////*/

        public void ZobrazPrehledProvincie()
        {
            Console.WriteLine("{0}\n", JmenoProvincie);
            Console.WriteLine("Populace: {0}/{1}", Populace, MaxPopulace);

            Console.WriteLine("\nSuroviny: {0}", VratVypisSuroviny());
            
            Console.WriteLine("\nBudovy:\n");

            ZobrazBudovy();
        }

        public string VratVypisSuroviny()
        {
            return string.Format("Zlato: {0}   Dřevo: {1}   Rýže: {2}\n", Zlato, Drevo, Ryze);
        }

        public void ZobrazBudovy()
        {
            for (int i = 0; i < Budovy.Count; i++)
            {
                if (i == 5) Console.WriteLine();
                Console.WriteLine("{0} -> {1}({2})", i+1, Budovy[i].Jmeno, Budovy[i].Level);
                
            }
        }

        public void ZobrazJednotky()
        {
            foreach (Jednotka j in Jednotky)
            {
                Console.WriteLine("{0}: {1}", j.Jmeno, j.Pocet);
            }
        }

        public void PrepoctiSuroviny(int zlato, int drevo, int ryze)
        {
            if (Drevo + drevo > MaxDreva) Drevo += MaxDreva - Drevo;
            else Drevo += drevo;

            if (Zlato + zlato > MaxZlata) Zlato += MaxZlata - Zlato;
            else Zlato += zlato;

            if (Ryze + ryze > MaxRyze) Ryze += MaxRyze - Ryze;
            else Ryze += ryze;
        }

        private int PrepocitejAVratPopulaci()
        {
            int populace = 0;
            foreach (Jednotka j in Jednotky)
            {
                populace += j.Pocet;
            }

            populace += Ninjove.Pocet + NinjoveMimoProvincii.Pocet;

            return populace;
        }

        private int PrepocitejAVratMaxPopulaci()
        {
            int maxPopulace = 0;
            for (int i = 1; i <= Convert.ToInt32(((Domy)Budovy[4]).Level); i++)
            {
                maxPopulace += 10 * i; //Level
            }

            maxPopulace += ((Hrad)Budovy[0]).Obyvatele;

            return maxPopulace;
        }

        public void PridejJednotkyDoUtoku(Jednotka[] pluk) { JednotkyDoUtoku.Add(pluk); }
        public void PridejProvincieProUtok(Provincie nepratelskaProvincie) { ProvincieProUtok.Add(nepratelskaProvincie); }
        public void VynulujProvincieAJednotkyProUtok() { ProvincieProUtok = new List<Provincie>(); JednotkyDoUtoku = new List<Jednotka[]>(); }

        public void PridejJednotkyProPodporu(Jednotka[] pluk) { JednotkyProPodporu.Add(pluk); }
        public void PridejProvincieProPodporu(Provincie provincie) { ProvincieProPodporu.Add(provincie); }
        public void VynulujJednoktyAProvincieProPodporu() { JednotkyProPodporu = new List<Jednotka[]>(); ProvincieProPodporu = new List<Provincie>(); }

        public void PridejNinjyMimoProvicnii(int odchoziNinjove) { NinjoveMimoProvincii.PridejPocet(odchoziNinjove); }
        public void PridejProvinciiProSpehovani(Provincie nepratelskaProvincie) { NepratelskeProvincieProSpehovani.Add(nepratelskaProvincie); }
        public void VynulujProvincieProSpehovani() { NepratelskeProvincieProSpehovani = new List<Provincie>(); }
       
        public void PridejProvinciiProDovozSurovin(Provincie vybranaProvincie) { ProvincieProDovozSurovin.Add(vybranaProvincie); }
        public void PridejSurovinyProDovoz(int[] suroviny) { SurovinyProDovoz.Add(suroviny); }
        public void VynulujProvincieASurovinyProDovoz() { ProvincieProDovozSurovin = new List<Provincie>(); SurovinyProDovoz = new List<int[]>(); }

        private Hrac ZjistiVlastnika()
        {
            Hrac vlastnik = null;
            foreach (Hrac vladce in Hra.Vladci)
            {
                foreach (Provincie p in vladce.SeznamProvincii) if (p == this) vlastnik = vladce;
            }

            return vlastnik;
        }

        public void PrepravSuroviny(int[] suroviny, Provincie vybranaProvincie)
        {
            if (Hra.AktualniHrac == Hra.Player)
            {
                Hra.ZobrazPrehled(Hra.Player, true);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Suroviny z provincie {0} byly doručeny do provincie {1}\n", JmenoProvincie, vybranaProvincie.JmenoProvincie);
                Console.WriteLine("Zlato: {0}  Dřevo: {1}  Rýže: {2}\n", suroviny[0], suroviny[1], suroviny[2]);
                Console.ResetColor();

                Console.WriteLine("ENTER");
                Console.ReadLine();
            }
            vybranaProvincie.PrepoctiSuroviny(suroviny[0], suroviny[1], suroviny[2]);
        }

        public void PrijmiVyprodukovaneSuroviny()
        {
            int zlato = ((Hrad)Budovy[0]).VyprodukujSuroviny();
            int ryze = ((Pole)Budovy[1]).VyprodukujSuroviny();
            int drevo = ((Pila)Budovy[2]).VyprodukujSuroviny();

            PrepoctiSuroviny(zlato, drevo, ryze);
        }

        private void ZautocNaProvincii(Provincie nepratelskaProvincie, List<Jednotka> utociciArmada)
        {
            Hrac utocnik = Vlastnik;
            Hrac obrance = nepratelskaProvincie.Vlastnik;

            List<Jednotka> braniciArmada = new List<Jednotka>();
            foreach (Jednotka j in nepratelskaProvincie.Jednotky) if (j.Pocet > 0) braniciArmada.Add(j);

            Jednotka[] puvodniArmadaUtocnika = new Jednotka[utociciArmada.Count]; //Pro zprávu
            for (int i = 0; i < puvodniArmadaUtocnika.Length; i++) puvodniArmadaUtocnika[i] = new Jednotka(utociciArmada[i]);

            Jednotka[] puvodniArmadaObrance = new Jednotka[braniciArmada.Count]; //Pro zprávu
            for (int i = 0; i < puvodniArmadaObrance.Length; i++) puvodniArmadaObrance[i] = new Jednotka(braniciArmada[i]);

                //Útočník = A
                //Obránce = B

            nepratelskaProvincie.ZvysJednotkamObranuZhradeb(braniciArmada.ToArray());

                while (Hra.VratPocetArmady(utociciArmada) != 0 && Hra.VratPocetArmady(braniciArmada) != 0)
                {
                    /*
                    List<Jednotka> braniciArmadaReference = VyhodnotUtok(utociciArmada, braniciArmada);
                    List<Jednotka> utociciArmadaReference = VyhodnotUtok(braniciArmada, utociciArmada);*/

                    List<Jednotka> braniciArmadaReference = VvyhodnotUtok(utociciArmada, braniciArmada);
                    List<Jednotka> utociciArmadaReference = VvyhodnotUtok(braniciArmada, utociciArmada);

                    /*Console.WriteLine(Hra.VratPocetArmady(utociciArmada));
                    Console.WriteLine(Hra.VratPocetArmady(braniciArmada));
                    Console.WriteLine();*/

                    for (int i = 0; i < braniciArmadaReference.Count; i++) braniciArmada[i].NastavPocet(braniciArmadaReference[i].Pocet);
                    for (int i = 0; i < utociciArmadaReference.Count; i++) utociciArmada[i].NastavPocet(utociciArmadaReference[i].Pocet);


                    /*Console.WriteLine(Hra.VratPocetArmady(utociciArmada));
                    Console.WriteLine(Hra.VratPocetArmady(braniciArmada));
                    Console.WriteLine();
                    Console.WriteLine();*/
                    
                }

                nepratelskaProvincie.NastavJednoktamObranuNaPuvodni(braniciArmada.ToArray());
                
            //Vyhodnocení bitvy
            if (Hra.VratPocetArmady(utociciArmada) > 0) //Vyhrála útočíci armáda
            {
                if (utocnik == Hra.Player) utocnik.ZobrazZpravuPoBitve(true, true, this, nepratelskaProvincie, utociciArmada.ToArray(), puvodniArmadaUtocnika, braniciArmada.ToArray(), puvodniArmadaObrance);
                else
                    if (obrance == Hra.Player) obrance.ZobrazZpravuPoBitve(false, false, this, nepratelskaProvincie,braniciArmada.ToArray(), puvodniArmadaObrance, utociciArmada.ToArray(), puvodniArmadaUtocnika);

                Vlastnik.ObsadProvincii(nepratelskaProvincie);

                //Návrat armády
                nepratelskaProvincie.PrijmiJednotky(utociciArmada.ToArray());
            }
            else
            {
                if (utocnik == Hra.Player) utocnik.ZobrazZpravuPoBitve(true, false, this, nepratelskaProvincie, utociciArmada.ToArray(), puvodniArmadaUtocnika, braniciArmada.ToArray(), puvodniArmadaObrance);
                else
                    if (obrance == Hra.Player) obrance.ZobrazZpravuPoBitve(false, true, this, nepratelskaProvincie, braniciArmada.ToArray(), puvodniArmadaObrance, utociciArmada.ToArray(), puvodniArmadaUtocnika);

                if (obrance is Pocitac)
                {
                    ((Pocitac)obrance).ZvysPriorituProvincie(this, 1);
                    ((Pocitac)obrance).ZvzsPriorituVladce(Vlastnik, 2);
                }
            }


        }

        private List<Jednotka> VvyhodnotUtok(List<Jednotka> utocnici, List<Jednotka> cilovaArmada)
        {
            //Náhodne zamíchá pluky v armmáde soupeře

            List<Jednotka> nezamichaniSouperi = new List<Jednotka>(); //Reference, refrence a zase ty reference
            foreach (Jednotka j in cilovaArmada) nezamichaniSouperi.Add(new Jednotka(j));

            List<Jednotka> listProZamichani = new List<Jednotka>();
            foreach (Jednotka j in nezamichaniSouperi) if (j.Pocet != 0) listProZamichani.Add(j);

            List<Jednotka> zamichaniSouperi = new List<Jednotka>();

                //Zamíchání
            for (int i = 0; i < listProZamichani.Count; i++) //změnit podmínku - listProZamichani.Count != 0
            {
                int nahodnyIndex = Hra.NahodnyGenerator.Next(0, listProZamichani.Count);

                zamichaniSouperi.Add(listProZamichani[nahodnyIndex]);
                listProZamichani.RemoveAt(nahodnyIndex);
                
            }


            //Souboj:
            List<Jednotka> braniciArmada = new List<Jednotka>();
            foreach (Jednotka j in zamichaniSouperi) braniciArmada.Add(j);
            
            for (int i = 0; i < utocnici.Count; i++)
            {
                if (i < braniciArmada.Count - 1)
                {
                    utocnici[i].ZautocNaNepritele(braniciArmada[i]);
                }
                else
                {

                    braniciArmada = new List<Jednotka>();
                    foreach (Jednotka j in zamichaniSouperi) braniciArmada.Add(j);

                    if (braniciArmada.Count != 0)
                    {
                        int index = Hra.NahodnyGenerator.Next(0, braniciArmada.Count);

                        Jednotka braniciPluk = braniciArmada[index];

                        utocnici[i].ZautocNaNepritele(braniciPluk);
                    }
                    else break;
                }
            }

            return nezamichaniSouperi;

        }

        /*
        private List<Jednotka> VyhodnotUtok(List<Jednotka> utociciArmada, List<Jednotka> cilovaArmada)
        {
            List<Jednotka> braniciArmada = new List<Jednotka>();
            foreach (Jednotka j in cilovaArmada) braniciArmada.Add(new Jednotka(j));

            Random r = new Random();

            int sila = Hra.VratSiluArmady(utociciArmada);

            if (sila >= 10000) sila = sila / 100;
            else sila = sila / 10;

            int kzabiti = r.Next(Convert.ToInt32(Math.Ceiling((double)sila / 2.5)), Convert.ToInt32(Math.Ceiling((double)sila / 2)) + 1);

            while (kzabiti > 0)
            {
                foreach (Jednotka j in braniciArmada)
                {
                    int mrtvi = r.Next(Convert.ToInt32(Math.Ceiling((double)kzabiti / 3)), kzabiti + 1);
                    kzabiti -= mrtvi;

                    mrtvi -= j.Obrana;

                    if (mrtvi > j.Pocet)
                    {
                        kzabiti += mrtvi - j.Pocet;
                        mrtvi = j.Pocet;
                    }

                    if (mrtvi > 0) j.NastavPocet(j.Pocet - mrtvi);

                    if (kzabiti <= 0) break;
                }
            }

            return braniciArmada;
            //ZAPMATUJ SI ŽE POLE OBJEKTŮ SE CHOVÁ STEJNĚ REFERENČNĚ JAKO OBJEKT SAMOTNÝ
        }

         */

        private void ZvysJednotkamObranuZhradeb(Jednotka[] braniciArmada)
        {
            int bonusHradby = 0;
            foreach (Budova b in Budovy) if (b is Hradby) bonusHradby = ((Hradby)b).ObranaBonus;

            foreach (Jednotka j in braniciArmada) j.ZvysObranuZleveluHradeb(bonusHradby);
        }

        private void NastavJednoktamObranuNaPuvodni(Jednotka[] braniciArmada)
        {
            foreach (Jednotka j in braniciArmada) j.NastavObranuNaPuvodniHodnotu();
        }

        public void PosliJednotky(Provincie cilovaProvincie, Jednotka[] armada)
        {
            //Pokud je ćilová provincie naše - příjme jednokty - !zaútočí
            if (Hra.AktualniHrac.SeznamProvincii.IndexOf(cilovaProvincie) != -1)
            {
                if (Hra.AktualniHrac == Hra.Player)
                {
                    Hra.ZobrazPrehled(Hra.Player, true);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Jednotky z provincie {0} dorazily do provincie {1}", JmenoProvincie, cilovaProvincie.JmenoProvincie);
                    Hra.VypisArmadu(armada.ToList());
                    Console.ResetColor();

                    Console.WriteLine("\n\nENTER");
                    Console.ReadLine();
                }

                cilovaProvincie.PrijmiJednotky(armada);
            }
            else ZautocNaProvincii(cilovaProvincie, armada.ToList());
        }

        public void PrijmiJednotky(Jednotka[] armada)
        {
            foreach (Jednotka j in armada)
            {
                foreach (Jednotka k in Jednotky) if (j.Jmeno == k.Jmeno) k.PridejPocet(j.Pocet);
            }
        }
    }
}
