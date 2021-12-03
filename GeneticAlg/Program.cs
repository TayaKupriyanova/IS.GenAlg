using System;

namespace GeneticAlg
{
    public class Individ
    {
        public int[] genes; // массив генов
        public int n_gen;// число генов
        public int bites = 6; // кол-во бит в одном гене

        public Individ(int n)
        {
            n_gen = n;
            genes = new int[n_gen*bites];
        }

        public void CreateIndivid()
        {
            Random r = new Random();
            int bukva;
            int[] code = new int[bites];
            for (int i = 0; i< n_gen; i++)
            {
                bukva = r.Next(33);

                // представляем в двоичной системе
                // и записываем в ген
                for(int j = i * bites + 5; j>= i*bites; j--)
                {
                    //genes[j] =code[j%bites];
                    if (bukva % 2 == 0) genes[i] = 0;
                    else genes[i] = 1;
                    bukva /= 2;
                }
            }
        }

        public void bit_mutation(int k)
        {
            // меняем k-тое значение на обратное
            if (genes[k] == 1) genes[k] = 0;
            if (genes[k] == 0) genes[k] = 1;
        }

        public int cf()
        {
            // возвращает кол-во совпавших битов в сравнении генов текущей особи и особи МИР
            // чем больше совпадений, тем приспособленнее особь
            Individ mir = new Individ(3);
            mir.genes = new int[18] { 0,0,1,1,0,1,0,0,1,0,0,1,0,1,0,0,0,1};
            int counter = 0;
            for(int i=0; i< n_gen * bites; i++)
            {
                if (genes[i] == mir.genes[i]) counter++;
            }
            return counter;
        }
    }

    public class Population
    {
        public Individ[] popul; // массив особей
        public int sizep; // размерность популяции

        public Population(int n)
        {
            // выделяем память под массив особей
            sizep = n;
            popul = new Individ[sizep];
            for (int i = 0; i < n; i++) popul[i] = new Individ(3);
        }

        public void GeneratePopulation()
        {
            // для каждой особи вызывем popul[i].CreateOsob();
            for(int i = 0; i< sizep; i++)
            {
                popul[i].CreateIndivid();
            }
        }

        public Population crossingover()
        {
            // задаем вероятность скрещивания
            double p = 0.8; int i, j;
            Population potomki = new Population(sizep);
            Random r = new Random();
            for( int k =0; k< sizep-2; k += 2)
            {
                i = (int)(r.NextDouble() * sizep);
                j = (int)(r.NextDouble() * sizep);
                if(r.NextDouble() < p)
                {
                    skrestch(i, j, potomki, k);
                }
                else
                {
                    potomki.popul[k] = popul[i];
                    potomki.popul[k+1] = popul[j];
                }
            }
            return potomki;  
        }
        public void skrestch(int i, int j, Population potomki, int k)
        {
            // реализация 2точечного кроссинговера
            Random r = new Random();
            int t1, t2; int c, c2; int buf;
            t1 = (int)(r.NextDouble() * popul[i].bites * popul[i].n_gen); //
            t2 = (int)(r.NextDouble() * popul[i].bites * popul[i].n_gen); //  случайно выбрали точки разрыва
            if (t1 > t2){ c = t2; c2 = t1; }
            else { c = t1; c2 = t2; }
            for(int e = c; e< c2;e++)
            {
                buf = popul[i].genes[e];
                popul[i].genes[e] = popul[j].genes[e];
                popul[j].genes[e] = buf;
            }
            potomki.popul[k] = popul[i];
            potomki.popul[k+1] = popul[j];
        }

        public void mutation()
        {
            double p = 1/(double)18;// задаем вероятность мутации
            Random r = new Random();
            for(int i =0; i<sizep; i++){
                for(int j=0; j<popul[i].n_gen * popul[i].bites; j++){
                    if(r.NextDouble() < p)
                    {
                        popul[i].bit_mutation(j);
                    }
                }
            }
        }

        public Population selection()
        {
            Population selected = new Population(sizep / 2); // к скрещиванию допускаются половина текущей популяции
            Random r = new Random(); int i, j;
            // проводим селекцию бинарным турниром
            // для каждой особи вызываем функцию cf и сравниваем их
            for (int k = 0; k< sizep/2; k++)
            {
                i = (int)(r.NextDouble() * sizep);
                j = (int)(r.NextDouble() * sizep);

                if (popul[i].cf() > popul[j].cf()) selected.popul[k] = popul[i];
                else selected.popul[k] = popul[j];
            }
            return selected;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
         //for(int m=0; m < 20; m++) 
         //{ 
            int sizethefirst = 60000;
            // создаем исходную популяцию
            int yearcounter = 1;
            Population[] years = new Population[yearcounter];
            years[yearcounter-1] = new Population(sizethefirst);
            years[yearcounter - 1].GeneratePopulation();
            int[] cfs = new int[years[yearcounter - 1].sizep];
            for (int i = 0; i < years[yearcounter - 1].sizep; i++) { 
                cfs[i] = years[yearcounter - 1].popul[i].cf(); 
            }

            while (!cfs.Contains(18) & years[yearcounter - 1].sizep>=1) // начинаем цикл пока не найдем подходящую особь МИР и пока популяция не вырождена
            {
                // проводим селекцию, отбирая половину самых приспособленных особей
                yearcounter++;
                Array.Resize<Population>(ref years, yearcounter);
                years[yearcounter - 1] = new Population(years[yearcounter - 2].sizep / 2);
                years[yearcounter - 1] = years[yearcounter - 2].selection();

                // проводим кроссинговер
                years[yearcounter - 1].crossingover();

                // проводим мутацию
                years[yearcounter - 1].mutation();

                // получаем новое поколение популяции 
                // ищем в нем требуемую особь
                cfs = new int[years[yearcounter - 1].sizep];
                for (int i = 0; i < years[yearcounter - 1].sizep; i++) cfs[i] = years[yearcounter - 1].popul[i].cf();
            }

            // если нашли, то выводим в консоль информацию об успешном поколении (номер поколения)
            if (cfs.Contains(18)) 
            Console.WriteLine("Особь найдена в поколении {0}", yearcounter);

            if (years[yearcounter - 1].sizep <1)
            Console.WriteLine("Популция выродилась, особь не найдена");
         //}
        }
    }
}
