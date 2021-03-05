using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Japan_Conquer
{
    class Pocitac : Hrac
    {
       

        private List<int> PrioritySouperu;
        private List<List<int>> PriorityProvinciiSouperu;
        private bool zvoleneUtocneKolo;

        public Pocitac(string jmeno, Provincie[] provincie): base(jmeno, provincie)
        {
        }

        public override void Hraj()
        {
            Hra.NastavAktualnihoHrace(this);

            foreach (Provincie p in SeznamProvincii)
            {
                Hra.NastavAktualniProvincii(p);

                //každý 10. den vynuluje priority (zabránění patové situace)
                if (Hra.Den % 10 == 0) NastavPocatecniPriority();

                int maxLevely = 0;
                foreach (Budova b in p.Budovy) if (b.Level == b.MaxLevel && !(b is Domy)) maxLevely++;

                int vyberTypuHry;

                if (maxLevely == p.Budovy.Count - 1) vyberTypuHry = 0;
                else
                {
                    vyberTypuHry = Hra.NahodnyGenerator.Next(0, 2);
                }

                //0 = Útočné kolo
                //1 = Ekonomické kolo

                Obchoduj();

                if (vyberTypuHry == 0)
                {
                    zvoleneUtocneKolo = true;
                    ZagrajUtocneKolco();
                    ZagrajEkonomickeKolco();
                }
                else
                {
                    zvoleneUtocneKolo = false;
                    ZagrajEkonomickeKolco();
                    ZagrajUtocneKolco();
                }
            }
        }

        public void NastavPocatecniPriority()
        {
            PrioritySouperu = new List<int>();
            PriorityProvinciiSouperu = new List<List<int>>();

            foreach (Hrac h in Souperi)
            {
                PrioritySouperu.Add(0);

                List<int> priorityProvincii = new List<int>();

                foreach (Provincie p in h.SeznamProvincii) priorityProvincii.Add(0);

                PriorityProvinciiSouperu.Add(priorityProvincii);
            }
        }

        private void ZagrajUtocneKolco()
        {

            Budova dojo = Hra.AktualniProvincie.Budovy[5];
            if (dojo.Level == 0)
            {
                int postavit = Hra.NahodnyGenerator.Next(0, 2);

                if (postavit == 1)
                {
                    while (dojo.Level != 3 && dojo.JsouSurovinyNaRozsireni()) RozsirBudovu(dojo);
                }
            }


            PostavJednotky();

            List<Hrac> souperiSnejvyssiPrioritou = new List<Hrac>();
            
            int nejvyssiPrioritaSouperu = PrioritySouperu.Max();

            //Do pole souepriSnejvysiiBlabla přida soupeře se stejnou prioritou ktera je nejvyssi
            for (int i = 0; i < Souperi.Count; i++)
            {
                if (PrioritySouperu[i] == nejvyssiPrioritaSouperu) souperiSnejvyssiPrioritou.Add(Souperi[i]);
            }

            int indexVybranehoSoupere = Hra.NahodnyGenerator.Next(0, souperiSnejvyssiPrioritou.Count);
            Hrac vybranySouper = souperiSnejvyssiPrioritou[indexVybranehoSoupere];

            int indexVybranehoSoupereVkolekci = Souperi.IndexOf(vybranySouper);


            //Do poleProvincieSnejvyssiPrioritou přidá provincie s prioritou té nejvyšší
            int nejvyssiPrioritaProvincie = PriorityProvinciiSouperu[indexVybranehoSoupereVkolekci].Max();

            List<Provincie> provincieSnejvyssiPrioritou = new List<Provincie>();

            for (int i = 0; i < vybranySouper.SeznamProvincii.Count; i++ )
            {
                if (PriorityProvinciiSouperu[indexVybranehoSoupereVkolekci][i] == nejvyssiPrioritaProvincie) provincieSnejvyssiPrioritou.Add(vybranySouper.SeznamProvincii[i]);
            }


            int indexVybraneProvincie = Hra.NahodnyGenerator.Next(0, provincieSnejvyssiPrioritou.Count);
            Provincie vybranaProvincie = provincieSnejvyssiPrioritou[indexVybraneProvincie];
            int indexVybraneProvincieVkolekci = vybranySouper.SeznamProvincii.IndexOf(vybranaProvincie);

            

            //Kontrola zda byla provincie špehována
            Zprava zpravaOSpehovani = null;

            bool bylaSpehovana = false;
            foreach (Zprava z in ZpravyNinju)
            {
                if (vybranaProvincie.JmenoProvincie == z.JmenoProvincie)
                {
                    bylaSpehovana = true;
                    zpravaOSpehovani = z;
                }
            }

            if (bylaSpehovana && zpravaOSpehovani.Stari < 4)
            {
                if (Hra.AktualniProvincie.Populace <= zpravaOSpehovani.PocetObyvatel) PriorityProvinciiSouperu[indexVybranehoSoupereVkolekci][indexVybraneProvincieVkolekci]++;
                int priorita = PriorityProvinciiSouperu[indexVybranehoSoupereVkolekci][indexVybraneProvincieVkolekci];

                if (Hra.AktualniProvincie.Populace >= zpravaOSpehovani.PocetObyvatel || priorita > 30) { VyberJednoktyDoBoje(vybranaProvincie); }
            }
            else
            {
                Jednotka ninjove = Hra.AktualniProvincie.Ninjove;
                if (ninjove.Pocet != 0) { VyslatNinju(vybranaProvincie); }
                else   
                {
                    if (ninjove.JeOdemknutaVbudove() == false) //dojo
                    {
                        RozsirBudovu(ninjove.VlastnenaBudova);
                    }

                    if (ninjove.JeOdemknutaVbudove() == true)
                    {
                        ninjove.NaverbujJednotky();
                        if (ninjove.Pocet != 0) { VyslatNinju(vybranaProvincie); }
                    }
                }
            }

            //Vyšpehuje ostatní vesnice
            if (Hra.AktualniProvincie.Ninjove.Pocet != 0)
            {

                List<Provincie> dalsiProvincieKeSpehovani = new List<Provincie>();
                foreach (Provincie p in Hra.SeznamProvincii)
                {
                    if (p.Vlastnik != this && Hra.AktualniProvincie.NepratelskeProvincieProSpehovani.IndexOf(p) == -1)
                    {
                        dalsiProvincieKeSpehovani.Add(p);
                    }
                }

                for (int i = 0; i < Hra.AktualniProvincie.Ninjove.Pocet; i++)
                {
                    if (dalsiProvincieKeSpehovani.Count == 0) break;

                    int nahodnyIndexProvincie = Hra.NahodnyGenerator.Next(0, dalsiProvincieKeSpehovani.Count);

                    Provincie provincieKeSpehovani = dalsiProvincieKeSpehovani[nahodnyIndexProvincie];

                    VyslatNinju(vybranaProvincie);

                    dalsiProvincieKeSpehovani.RemoveAt(nahodnyIndexProvincie);
                }
            }
        }

        private void ZagrajEkonomickeKolco()
        {
            //Změna budovy pro rozšíření podle dostatků suroviny

            Provincie p = Hra.AktualniProvincie;

            if (p.Budovy[5].Level == 0) RozsirBudovu(p.Budovy[5]); //Dojo

            List<int> indexyProVybraniBudov = new List<int>();
            for (int i = 0; i < p.Budovy.Count; i++) if (i != 1 && i != 2) indexyProVybraniBudov.Add(i);


            List<Budova> budovyKrozsireni = new List<Budova>();
            budovyKrozsireni.Add(p.Budovy[1]); //pole
            budovyKrozsireni.Add(p.Budovy[2]); //pila

            for (int i = 0; i < indexyProVybraniBudov.Count; i++)
            {
                int index = Hra.NahodnyGenerator.Next(0, indexyProVybraniBudov.Count);
                budovyKrozsireni.Add(p.Budovy[indexyProVybraniBudov[index]]);
                indexyProVybraniBudov.RemoveAt(index);
            }

            int podminkaZustatekDreva = 0;
            int podminkaZustatekZlata = 0;

            if (zvoleneUtocneKolo == false)
            {
                if (Hra.Den <= 5)
                {
                    podminkaZustatekDreva = 1200;
                    podminkaZustatekZlata = 1200;
                }
                else
                {
                    podminkaZustatekDreva = 600;
                    podminkaZustatekZlata = 600;
                }
            }

            while (true)
            {
                bool lzePostaviAsponJednuBudovu = false;

                for (int i = 0; i < p.Budovy.Count; i++)
                {
                    //Určení budov ke stavbě
                    ///////////////////////////////////////////////////////////
                    //P.Budovy se může zvětit po vyzkoumání nových budov
                    Budova b = null;

                    if (i >= budovyKrozsireni.Count)
                    {
                        if (p.Budovy.Count > i) b = p.Budovy[i];
                        else
                        {
                            int nahodnyIndex = Hra.NahodnyGenerator.Next(0, p.Budovy.Count);
                            b = p.Budovy[nahodnyIndex];
                        }
                    }
                    else
                    {
                        b = budovyKrozsireni[i];
                    }

                    //////////////////////////////////////////////////////
                    int puvodniLevel = b.Level;

                    Obchoduj();

                    //Výstavba
                    if (p.Zlato - b.CenaZlato >= podminkaZustatekZlata && p.Drevo - b.CenaDrevo >= podminkaZustatekDreva)
                    {
                        if (b is Pole || b is Pila)
                        {
                            int nahoda = Hra.NahodnyGenerator.Next(2, 5);

                            for (int j = 1; j < nahoda; j++)
                            {
                                if (p.Zlato - b.CenaZlato >= podminkaZustatekZlata && p.Drevo - b.CenaDrevo >= podminkaZustatekDreva) RozsirBudovu(b);
                            }
                        }

                        if (b is Domy)
                        {
                            if ((p.MaxPopulace - Convert.ToInt32(Math.Round((double)p.Populace / 100) * 20)) <= p.Populace || p.Populace == p.MaxPopulace) RozsirBudovu(b);
                        }
                        else RozsirBudovu(b);

                        if (puvodniLevel != b.Level)
                        {
                            lzePostaviAsponJednuBudovu = true;

                        }


                    }
                }

                if (!lzePostaviAsponJednuBudovu) break;
            }

        }

        private void PostavJednotky()
        {
            Provincie p = Hra.AktualniProvincie;

            Jednotka[] JednotkyProRekrutovani = p.Jednotky.ToArray();

            int nahoda = Hra.NahodnyGenerator.Next(0, 2);

            if (nahoda == 1) Array.Reverse(JednotkyProRekrutovani);

            int podminkaZlata = 0;
            int podminkaDreva = 0;

            if (zvoleneUtocneKolo)
            {
                bool jeBudovaKrozsireni = false;
                foreach (Budova b in Hra.AktualniProvincie.Budovy)
                {
                    if (!(b is Domy) && b.JsouSurovinyNaRozsireni()) jeBudovaKrozsireni = true;
                }

                if (jeBudovaKrozsireni)
                {
                    podminkaZlata = 500;
                    podminkaDreva = 500;
                }
            }

            while (true)
            {
                bool lzeRekrutovatAsponJednuJednotku = false;

                foreach (Jednotka j in JednotkyProRekrutovani)
                {
                    Obchoduj();

                    Budova vlastnenaBudova = j.VlastnenaBudova;

                    if (j.JeOdemknutaVbudove() == false || vlastnenaBudova.Level == 0)
                    {
                        if (vlastnenaBudova is Hrad)
                        {
                            if (Hra.Den > 5) RozsirBudovu(vlastnenaBudova);
                        }
                        else RozsirBudovu(vlastnenaBudova);
                    }

                    if (j.JeOdemknutaVbudove() == true && vlastnenaBudova.Level != 0)
                    {
                        if (Hra.AktualniProvincie.Zlato <= podminkaZlata || Hra.AktualniProvincie.Drevo <= podminkaDreva) break;

                        if (((p.MaxPopulace - Convert.ToInt32(Math.Round(((double)p.Populace / 100) * 20))) <= p.Populace) || (Hra.AktualniProvincie.Populace == Hra.AktualniProvincie.MaxPopulace))
                        {
                            Budova domy = p.Budovy[4];
                            if (domy.JsouSurovinyNaRozsireni()) RozsirBudovu(domy); //Domy
                            else break;
                        }

                        int puvodniPocet = j.Pocet;

                        j.NaverbujJednotky(podminkaZlata, podminkaDreva);

                        if (puvodniPocet != j.Pocet) lzeRekrutovatAsponJednuJednotku = true;

                    }
                }

                if (!lzeRekrutovatAsponJednuJednotku) break;

                if (((p.MaxPopulace - Convert.ToInt32(Math.Round(((double)p.Populace / 100) * 20))) <= p.Populace) || (Hra.AktualniProvincie.Populace == Hra.AktualniProvincie.MaxPopulace))
                {
                    Budova domy = p.Budovy[4];
                    if ((!domy.JsouSurovinyNaRozsireni()) || domy.Level == domy.MaxLevel) break;
                }

                //if (zvoleneUtocneKolo == false && Hra.Den > 5) break;

                Array.Reverse(JednotkyProRekrutovani);
            }

            Hra.AktualniProvincie.Ninjove.NaverbujJednotky(podminkaZlata, podminkaDreva);
        }

        public override void  Obchoduj()
        {
            Budova trziste = Hra.AktualniProvincie.Budovy[3];
            if (trziste.Level != 0)
            {
                //Prodá rýži
                ((Trziste)trziste).Prodej("ryze", Hra.AktualniProvincie.Ryze);

                if (Hra.AktualniProvincie.Zlato / 2 > Hra.AktualniProvincie.Drevo)
                {
                    ((Trziste)trziste).Kup("drevo", Hra.AktualniProvincie.Zlato / 2);
                }

                if (Hra.AktualniProvincie.Drevo / 2 > Hra.AktualniProvincie.Zlato)
                {
                    ((Trziste)trziste).Prodej("drevo", Hra.AktualniProvincie.Drevo / 2);
                }
            }
            else
            {
                RozsirBudovu(trziste);
                if (trziste.Level != 0) Obchoduj();
            }
        }

        public override void VyberJednoktyDoBoje(Provincie nepratelskaProvincie)
        {
            List<Jednotka> armada = VyberJednotky(null);

            if (armada != null)
            {
                Hra.AktualniProvincie.PridejJednotkyDoUtoku(armada.ToArray());
                Hra.AktualniProvincie.PridejProvincieProUtok(nepratelskaProvincie);
            }
            else
            {
            }
        }

        public override void VyberJednotkyProPodporu(Provincie cilovaProvincie)
        {
            List<Jednotka> armada = VyberJednotky(null);

            if (armada != null)
            {
                Hra.AktualniProvincie.PridejJednotkyProPodporu(armada.ToArray());
                Hra.AktualniProvincie.PridejProvincieProPodporu(cilovaProvincie);
            }
            else
            {
            }
        }

        protected override List<Jednotka> VyberJednotky(string informace)
        {
            List<Jednotka> pluky = new List<Jednotka>();
            List<Jednotka> puvodniPluky = new List<Jednotka>();

            /*Ověření zda se v provincii nacházejí jednotky*/
            foreach (Jednotka j in Hra.AktualniProvincie.Jednotky)
            {
                if (j.Pocet != 0)
                {
                    pluky.Add(new Jednotka(j)); // viz Reference
                    puvodniPluky.Add(j);
                }
            }

            if (pluky.Count != 0)
            {
                List<Jednotka> armada = new List<Jednotka>();

                for (int i = 0; i < pluky.Count; i++)
                {
                    Jednotka pluk = pluky[i];

                    int jednotkyDoPryc = Convert.ToInt32(Math.Round(((double)pluk.Pocet / 100) * 90));

                    pluk.NastavPocet(jednotkyDoPryc);
                    puvodniPluky[i].NastavPocet(puvodniPluky[i].Pocet - jednotkyDoPryc);
                    armada.Add(pluk);
                }

                return armada;
            }
            else
            {
                return null;
            }
        }

        public override void VyslatNinju(Provincie nepratelskaProvincie)
        {
            Hra.AktualniProvincie.PridejProvinciiProSpehovani(nepratelskaProvincie);
            Hra.AktualniProvincie.PridejNinjyMimoProvicnii(1);
            Hra.AktualniProvincie.Ninjove.PridejPocet(-1);
        }

        public void OdeberSoupereZPrioritSouperuAprovincii(Hrac souperProOdebrani)
        {
            int index = Souperi.IndexOf(souperProOdebrani);

            PrioritySouperu.RemoveAt(index);
            PriorityProvinciiSouperu.RemoveAt(index);
        }

        public void PrepocitejPriorityProvinciiProNovehoVlastnika(Hrac novyVlastnik, Hrac byvalyVlastnik, Provincie obsazenaProvincie)
        {
            if (byvalyVlastnik != this)
            {
                int indexByvalehoVlastnika = Souperi.IndexOf(byvalyVlastnik);
                int indexProvincieByvalehoVlastnika = byvalyVlastnik.SeznamProvincii.IndexOf(obsazenaProvincie);
                PriorityProvinciiSouperu[indexByvalehoVlastnika].RemoveAt(indexProvincieByvalehoVlastnika);
            }

            if (novyVlastnik != this)
            {
                int indexNovehoVlastnika = Souperi.IndexOf(novyVlastnik);
                PriorityProvinciiSouperu[indexNovehoVlastnika].Add(0);
            }
        }

        public void ZvysPriorituProvincie(Provincie provincieProInkrementaci, int hodnota)
        {
            Hrac vlastnik = provincieProInkrementaci.Vlastnik;
            int indexVlastnika = Souperi.IndexOf(vlastnik);
            int indexProvincie = vlastnik.SeznamProvincii.IndexOf(provincieProInkrementaci);

            PriorityProvinciiSouperu[indexVlastnika][indexProvincie] += hodnota;
        }

        public void ZvzsPriorituVladce(Hrac vladceProInkrementaci, int hodnota)
        {
            int indexVladce = Souperi.IndexOf(vladceProInkrementaci);
            PrioritySouperu[indexVladce] += hodnota;
        }

        public override void  RozsirBudovu(Budova budovaKRozsireni)
        {
            if (budovaKRozsireni != null)
            {
                if ((budovaKRozsireni.Level != budovaKRozsireni.MaxLevel) && (budovaKRozsireni.JeOdemknutaVProvincii()))
                {
                    if (budovaKRozsireni.JsouSurovinyNaRozsireni())
                    {
                        budovaKRozsireni.ZvysLevelBudovy();
                    }
                    else
                    {
                        if (Hra.AktualniProvincie.Budovy[3].Level == 0) //tržiště
                        {
                            if (Hra.AktualniProvincie.Budovy[3].JsouSurovinyNaRozsireni()) RozsirBudovu(Hra.AktualniProvincie.Budovy[3]); //Tržiště
                        }
                        else
                        {
                            if (budovaKRozsireni.CenaZlato > Hra.AktualniProvincie.Zlato) //Koupě zlata
                            {
                                int kolikChybiZlata = budovaKRozsireni.CenaZlato - Hra.AktualniProvincie.Zlato;

                                int kolikMoznoProdat = 0;

                                if (Hra.AktualniProvincie.Drevo / ((Trziste)Hra.AktualniProvincie.Budovy[3]).PomerDreva >= kolikChybiZlata) kolikMoznoProdat = kolikChybiZlata * ((Trziste)Hra.AktualniProvincie.Budovy[3]).PomerDreva;
                                else kolikMoznoProdat = Hra.AktualniProvincie.Drevo;

                                ((Trziste)Hra.AktualniProvincie.Budovy[3]).Prodej("drevo", kolikMoznoProdat);
                            }

                            if (budovaKRozsireni.CenaDrevo > Hra.AktualniProvincie.Drevo) //Koupě dřeva
                            {
                                int kolikChybiDreva = budovaKRozsireni.CenaDrevo - Hra.AktualniProvincie.Drevo;
                                int kolikMoznoKoupit = 0;

                                //potřebaZlata = cenaDrevo / pomerDřeva

                                if (Hra.AktualniProvincie.Zlato > budovaKRozsireni.CenaDrevo / ((Trziste)Hra.AktualniProvincie.Budovy[3]).PomerDreva) kolikMoznoKoupit = kolikChybiDreva;
                                else kolikMoznoKoupit = Hra.AktualniProvincie.Zlato * ((Trziste)Hra.AktualniProvincie.Budovy[3]).PomerDreva;

                                ((Trziste)Hra.AktualniProvincie.Budovy[3]).Kup("drevo", kolikMoznoKoupit);
                            }

                            if (budovaKRozsireni.JsouSurovinyNaRozsireni()) RozsirBudovu(budovaKRozsireni);
                        }
                    }
                }
            }
        }



    }
}
