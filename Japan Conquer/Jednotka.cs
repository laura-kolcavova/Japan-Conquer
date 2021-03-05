using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Japan_Conquer
{
    class Jednotka
    {
        public string Jmeno { get; protected set; }
        public int Pocet { get; protected set; }
        public byte Utok { get; private set; }
        public byte Obrana { get; private set; }
        private byte PuvodniObrana { get; set; }
        public int CenaDrevo { get; protected set; }
        public int CenaZlato { get; protected set; }

        public Provincie VlastnenaProvincie { get { return ZjistiVlastnenouProvincii(); } protected set { } }
        public Budova VlastnenaBudova { get { return ZjistiVlastnenouBudovu(); } protected set { } }
        private Random r = new Random();

        public static Jednotka lucistnik =   new Jednotka("Lučištník", 15, 10, 10, 20);
        public static Jednotka kopijnik = new Jednotka("Kopiník", 10, 10, 20, 25);
        public static Jednotka jezdec = new Jednotka("Jezdec", 25, 20, 30, 45);
        public static Jednotka lucistnikNaKoni = new Jednotka("Lučištník na koni", 30, 20, 35, 55);
        public static Jednotka samuraj = new Jednotka("Samuraj", 50, 30, 70, 90);
        public static Jednotka strelec = new Jednotka("Voják s puškou", 40, 20, 50, 70);

        public static Jednotka ninja = new Ninja("Ninja", 70, 100);

        public static Jednotka[] SeznamJednotek = { kopijnik, lucistnik, jezdec, lucistnikNaKoni, strelec, samuraj };

        /****************************************  KONŠTRUKTOR   ***************************************/
          /******************************************************************************************/

        public Jednotka(string jmeno, byte utok, byte obrana, int cenaDrevo, int cenaZlato)
        {
            Jmeno = jmeno;
            Utok = utok;
            Obrana = obrana;
            PuvodniObrana = obrana;
            CenaDrevo = cenaDrevo;
            CenaZlato = cenaZlato;
            Pocet = 0;
        }

        public Jednotka(Jednotka jednotka)
        {
            Jmeno = jednotka.Jmeno;
            Utok = jednotka.Utok;
            Obrana = jednotka.Obrana;
            PuvodniObrana = jednotka.Obrana;
            CenaDrevo = jednotka.CenaDrevo;
            CenaZlato = jednotka.CenaZlato;
            Pocet = jednotka.Pocet;
        }

        public Jednotka(string jmeno, int cenaDrevo, int cenaZlato) { Jmeno = jmeno; CenaDrevo = cenaDrevo; CenaZlato = cenaZlato; Pocet = 0; } //Pro ninju

        /*******************************************************************************************/
        /******************************************************************************************/

        public virtual void NaverbujJednotky()
        {
                int mozno = ZjistiKolikJeMoznoRekrutovat();
                int pocet = 0;

                if (mozno > 0)
                {
                    if (Hra.AktualniHrac == Hra.Player)
                    {

                        Console.WriteLine("{0}: Zlato: {1}  Dřevo: {2}\n", Jmeno, CenaZlato, CenaDrevo);
                        Console.WriteLine("\nMožno rekrutovat: " + mozno);

                        pocet = Hra.CtiCislo("Počet: ", mozno);
                    }
                    else // Hráč je počítač
                    {
                        pocet = Convert.ToInt32(Math.Ceiling(((double)mozno / 100) * 60));
                    }

                    Pocet += pocet;

                    Hra.AktualniProvincie.PrepoctiSuroviny(-(pocet * CenaZlato), -(pocet * CenaDrevo), 0);
                }
        }

        protected virtual int ZjistiKolikJeMoznoRekrutovat()
        {
            if (JeOdemknutaVbudove() == true && VlastnenaBudova.Level != 0 && Hra.AktualniProvincie.Populace < Hra.AktualniProvincie.MaxPopulace)
            {
                int moznoPopulace = Hra.AktualniProvincie.MaxPopulace - Hra.AktualniProvincie.Populace;
                double[] moznoZaCenu = { Math.Floor((double)Hra.AktualniProvincie.Drevo / CenaDrevo), Math.Floor((double)Hra.AktualniProvincie.Zlato / CenaZlato), moznoPopulace };

                return  Convert.ToInt32(moznoZaCenu.Min());
            }
            else return 0;
        }



        public virtual void NaverbujJednotky(int podminkaZlato, int podminkaDrevo)
        {
            int mozno = ZjistiKolikJeMoznoRekrutovat(podminkaZlato, podminkaDrevo);

            if (mozno != 0)
            {
                int pocet = Convert.ToInt32(Math.Ceiling(((double)mozno / 100) * 60));

                Pocet += pocet;

                Hra.AktualniProvincie.PrepoctiSuroviny(-(pocet * CenaZlato), -(pocet * CenaDrevo), 0);
            }
        }

        protected virtual int ZjistiKolikJeMoznoRekrutovat(int podminkaZlato, int podminkaDrevo)
        {
            if (JeOdemknutaVbudove() == true && VlastnenaBudova.Level != 0 && Hra.AktualniProvincie.Populace < Hra.AktualniProvincie.MaxPopulace && Hra.AktualniProvincie.Zlato > podminkaZlato && Hra.AktualniProvincie.Drevo > podminkaDrevo)
            {
                int moznoPopulace = Hra.AktualniProvincie.MaxPopulace - Hra.AktualniProvincie.Populace;
                double[] moznoZaCenu = { Math.Floor((double)(Hra.AktualniProvincie.Drevo - podminkaDrevo) / CenaDrevo), Math.Floor((double)(Hra.AktualniProvincie.Zlato - podminkaZlato) / CenaZlato), moznoPopulace };

                return Convert.ToInt32(moznoZaCenu.Min());
            }
            else return 0;
        }

        public void ZautocNaNepritele(Jednotka nepritel)
        {
            //int bonusZPoctu = Convert.ToInt32(Math.Round((double)Pocet / 7));
     
            /*int nahoda = r.Next(1, 4);

            int poskozeni = (Utok + nahoda); //+ bonus
            poskozeni = Convert.ToInt32(Math.Round(((double)poskozeni / 100) * ((double)nepritel.Obrana / 2)));*/

            int bonusZUtoku = Convert.ToInt32(Math.Round((double)Pocet / 100));
            int bonusZobrany = Convert.ToInt32(Math.Round((double)nepritel.Pocet / 100));

            int nahoda;

            int utok = Utok / 5;
            int obranaNepritele = Convert.ToInt32(Math.Round((double)nepritel.Obrana / 2));

            double poskozeni = 0;
            if (obranaNepritele + bonusZobrany >= utok + bonusZUtoku) poskozeni = r.Next(0, 2);
            else
            {
                nahoda = utok - obranaNepritele;
                if (nahoda < 0) nahoda = 1;

                poskozeni = r.Next(1, nahoda + 1);
            }


            if (poskozeni > nepritel.Pocet)
            {
                poskozeni = nepritel.Pocet;
            }

            if (poskozeni > 0) nepritel.PridejPocet(-Convert.ToInt32(poskozeni));
        }

        public void NastavPocet(int pocet)
        {
            Pocet = pocet;
        }

        public void PridejPocet(int pocet)
        {
            Pocet += pocet;
        }

        public void ZvysObranuZleveluHradeb(int bonusHradeb)
        {
            Obrana += Convert.ToByte(Math.Round((double)(Obrana / 100) * (bonusHradeb)));
        }

        public void NastavObranuNaPuvodniHodnotu()
        {
            Obrana = PuvodniObrana;
        }

        protected virtual Provincie ZjistiVlastnenouProvincii()
        {
            Provincie vlastnenaProvincie = null;

            foreach (Provincie p in Hra.SeznamProvincii)
            {
                foreach (Jednotka j in p.Jednotky)
                {
                    if (j == this)
                    {
                        vlastnenaProvincie = p;
                        break;
                    }
                }
            }

            return vlastnenaProvincie;
        }

        protected Budova ZjistiVlastnenouBudovu()
        {
            Provincie vlastnenaProvincie = VlastnenaProvincie;
            Budova vlastnenaBudova = null;

            foreach (Budova b in vlastnenaProvincie.Budovy)
            {
                if (b.JednotkyProReferenci != null)
                {
                    foreach (Jednotka j in b.JednotkyProReferenci)
                    {
                        if (j.Jmeno == Jmeno) 
                        {
                            vlastnenaBudova = b;
                            break;
                        }
                    }
                }
            }

            return vlastnenaBudova;
        }

        public bool JeOdemknutaVbudove()
        {
            bool jeOdemknuta = false;

            Budova vlastnenaBudova = VlastnenaBudova;

            if (vlastnenaBudova != null)
            {
                foreach (Jednotka j in vlastnenaBudova.Jednotky)
                {
                    if (j.Jmeno == Jmeno)
                    {
                        jeOdemknuta = true;
                        break;
                    }
                }
            }

            return jeOdemknuta;
        }
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////
    /// ///////////////////////////////////////////////////////////////////
    /// </summary>
    class Ninja : Jednotka
    {
        public Ninja(string jmeno, int cenaDrevo, int cenaZlato) :base (jmeno, cenaDrevo, cenaZlato)
        {
            Pocet = 0;
        }

        public Ninja(Jednotka ninja) :base (ninja)
        {
            Pocet = 0;
        }

        public override void NaverbujJednotky()
        {
            Budova vlastnenaBudova = VlastnenaBudova;
            int maxNinju = ((Dojo)vlastnenaBudova).MaxNinju;

            int mozno = ZjistiKolikJeMoznoRekrutovat();
            int pocet = 0;

            if (Hra.AktualniHrac == Hra.Player)
            {

                Console.WriteLine("{0}: {1} z {2}\n", Jmeno, Pocet + Hra.AktualniProvincie.NinjoveMimoProvincii.Pocet, maxNinju);
                Console.WriteLine("V provincii: {0}", Pocet);
                Console.WriteLine("Mimo provincii: {0}\n", Hra.AktualniProvincie.NinjoveMimoProvincii.Pocet);
                Console.WriteLine("Možno rekrutovat: " + mozno);

                pocet = Hra.CtiCislo("Počet: ", mozno);
            }
            else //Hráč je počítač
            {

                if (mozno != 0) pocet = Hra.NahodnyGenerator.Next(1, mozno + 1);
            }

            Pocet += pocet;

            Hra.AktualniProvincie.PrepoctiSuroviny(-(pocet * CenaZlato), -(pocet * CenaDrevo), 0);
        }

        protected override int ZjistiKolikJeMoznoRekrutovat()
        {
            Budova vlastnenaBudova = VlastnenaBudova;

            if (JeOdemknutaVbudove() && vlastnenaBudova.Level != 0 && Hra.AktualniProvincie.Populace < Hra.AktualniProvincie.MaxPopulace)
            {
                int maxNinju = ((Dojo)vlastnenaBudova).MaxNinju;
                int moznoPopulace = Hra.AktualniProvincie.MaxPopulace - Hra.AktualniProvincie.Populace;
                double[] moznoZaCenu = { Math.Floor((double)Hra.AktualniProvincie.Drevo / CenaDrevo), Math.Floor((double)Hra.AktualniProvincie.Zlato / CenaZlato), moznoPopulace, maxNinju - Hra.AktualniProvincie.Ninjove.Pocet - Hra.AktualniProvincie.NinjoveMimoProvincii.Pocet };

                return Convert.ToInt32(moznoZaCenu.Min());
            }
            else return 0;
        }


        public override void NaverbujJednotky(int podminkaZlato, int podminkaDrevo)
        {
            int mozno = ZjistiKolikJeMoznoRekrutovat(podminkaZlato, podminkaDrevo);

            if (mozno != 0)
            {
                int pocet = Hra.NahodnyGenerator.Next(1, mozno);
                Pocet += pocet;
                Hra.AktualniProvincie.PrepoctiSuroviny(-(pocet * CenaZlato), -(pocet * CenaDrevo), 0);
            }
        }

        protected override int ZjistiKolikJeMoznoRekrutovat(int podminkaZlato, int podminkaDrevo)
        {
            Budova vlastnenaBudova = VlastnenaBudova;

            if (JeOdemknutaVbudove() && vlastnenaBudova.Level != 0 && Hra.AktualniProvincie.Populace < Hra.AktualniProvincie.MaxPopulace && Hra.AktualniProvincie.Zlato > podminkaZlato && Hra.AktualniProvincie.Drevo > podminkaDrevo)
            {
                int maxNinju = ((Dojo)vlastnenaBudova).MaxNinju;
                int moznoPopulace = Hra.AktualniProvincie.MaxPopulace - Hra.AktualniProvincie.Populace;
                double[] moznoZaCenu = { Math.Floor((double)(Hra.AktualniProvincie.Drevo - podminkaDrevo) / CenaDrevo), Math.Floor((double)(Hra.AktualniProvincie.Zlato - podminkaZlato) / CenaZlato), moznoPopulace, maxNinju - Hra.AktualniProvincie.Ninjove.Pocet - Hra.AktualniProvincie.NinjoveMimoProvincii.Pocet };

                return Convert.ToInt32(moznoZaCenu.Min());
            }
            else return 0;
        }

        public void JdiDoNepratelskeProvincie(Provincie nepratelksProvincie) //NinjaMimoProvincii
        {
            bool prezilCestu = true;

            Random r = new Random();

            Hrac vlastnikSpehovaneVesncie = nepratelksProvincie.Vlastnik;

            int procentualniNahoda = r.Next(1, 101);

            if (procentualniNahoda <= nepratelksProvincie.Ninjove.Pocet * 8) prezilCestu = false; //40% šance na odhlaení ninjy (když má týpek 5 ninjů)

            switch (prezilCestu)
            {
                case true:
                    {
                        Spehuj(nepratelksProvincie);

                        if (Hra.AktualniHrac == Hra.Player)
                        {
                            Hra.ZobrazPrehled(Hra.Player, true);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("             ŠPEHOVÁNÍ PROVINCIE {0}\n", nepratelksProvincie.JmenoProvincie.ToUpper());
                            Console.WriteLine("Špehování vesnice {0} - {1} - bylo úspěšné!\n", nepratelksProvincie.JmenoProvincie, vlastnikSpehovaneVesncie);
                            Hra.Player.ZobrazVybranouZpravu(Hra.Player.ZpravyNinju.Last());
                            Console.ResetColor();
                            Console.WriteLine("\nENTER");
                            Console.ReadLine();
                        }
                        
                        break;
                    }
                case false: //Zemřel
                    {
                        if (Hra.AktualniHrac == Hra.Player)
                        {
                            Hra.ZobrazPrehled(Hra.Player, true);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("             ŠPEHOVÁNÍ PROVINCIE {0}\n", nepratelksProvincie.JmenoProvincie.ToUpper());
                            Console.WriteLine("Špehování vesnice {0} - {1} - selhalo - náš ninja byl odhalen!", nepratelksProvincie.JmenoProvincie, vlastnikSpehovaneVesncie);
                            Console.ResetColor();
                            Console.WriteLine("\nENTER");
                            Console.ReadLine();
                        }
                        if (nepratelksProvincie.Vlastnik == Hra.Player)
                        {
                            Hra.ZobrazPrehled(Hra.Player, true);

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("             DOPADENÍ NINJY V PROVINCII {0}\n", nepratelksProvincie.JmenoProvincie.ToUpper());
                            Console.WriteLine("Dopadli jsme nepřátelského ninju z vesnice: {0} - {1}", VlastnenaProvincie.JmenoProvincie, VlastnenaProvincie.Vlastnik);
                            Console.ResetColor();
                            Console.WriteLine("\nENTER");
                            Console.ReadLine();
                        }
                        //Zemřel
                        Pocet--;

                        if (vlastnikSpehovaneVesncie is Pocitac)
                        {
                            ((Pocitac)vlastnikSpehovaneVesncie).ZvysPriorituProvincie(VlastnenaProvincie, 1);
                            ((Pocitac)vlastnikSpehovaneVesncie).ZvzsPriorituVladce(VlastnenaProvincie.Vlastnik, 2);
                        }

                        if (Hra.AktualniHrac is Pocitac)
                        {
                            ((Pocitac)Hra.AktualniHrac).ZvysPriorituProvincie(nepratelksProvincie, 1);
                        }
                        break;
                    }
            }
        }

        public void Spehuj(Provincie nepratelskaProvincie) //NinjaMimoProvincii
        {
            string jmenoVladce = nepratelskaProvincie.Vlastnik.Jmeno;
            string jmenoProvincie = nepratelskaProvincie.JmenoProvincie;
            int pocetObyvatel = nepratelskaProvincie.Populace;
            int levelHradeb = 0;

            //Level Hradeb - zjištění zda soupeř postavil hradby
            Budova hradby = new Budova();
            foreach(Budova budova in nepratelskaProvincie.Budovy)
            {
                if (budova is Hradby) levelHradeb = hradby.Level;
            }

            //Kontrola pokud již provincie špehována byla :)
            if (Hra.AktualniHrac.ZpravyNinju.Count > 0)
            {
                int index = -1;
                for (int i = 0; i < Hra.AktualniHrac.ZpravyNinju.Count; i++ )
                {
                    if (Hra.AktualniHrac.ZpravyNinju[i].JmenoProvincie == nepratelskaProvincie.JmenoProvincie) index = i;
                }

                //V cyklu nelze odebrat element z kolekce --> chyba - změna délky kolekce
                if (index != -1) Hra.AktualniHrac.ZpravyNinju.RemoveAt(index);
            }

            //Návrat
            Hra.AktualniProvincie.Ninjove.PridejPocet(1);
            Pocet--; //ninja mimo provincii

            Hra.AktualniHrac.ZpravyNinju.Add(new Zprava(jmenoVladce, jmenoProvincie, pocetObyvatel, levelHradeb));
        }

        protected override Provincie ZjistiVlastnenouProvincii()
        {
            Provincie vlastnenaProvincie = null;

            foreach (Provincie p in Hra.SeznamProvincii)
            {
                if (this == p.Ninjove || this == p.NinjoveMimoProvincii) vlastnenaProvincie = p;
            }

            return vlastnenaProvincie;
        }
    }
}
