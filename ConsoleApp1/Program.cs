using System;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;


//Меню
//переработать
//подсчет уровня
//частота выпадения цифр на кубиках
//частота сумм бросков
//самая частая цифра и сумма
//сделать полную статистику выпавших сумм
//  6;9,10;8,5;7;11;6;
namespace R20
{
    class Program
    {
        public int[] Just_Throw(int[] input)
        {
            int dice_counter = input[0]; // количество брошенных кубов
            int dice_face = input[1]; // количество граней кубов
            var rand = new Random(); 
            int[] array_output = new int[input[0]];
            for (int i = 0; i < dice_counter; i++) // бросок
            {
                array_output[i] = rand.Next(1, dice_face + 1);
            }
            //for (int i = 0; i < array_output.Length; i++) // 
            //{
            //    Console.WriteLine($">{array_output[i]}");
            //}
            return array_output;
        } // Произвольный бросок
        public int[] GURPS_Throw(int[] array_input)
        {
            var rand = new Random();
            int a = rand.Next(1, 7);
            int b = rand.Next(1, 7);
            int c = rand.Next(1, 7);
            int[] array_output = { a, b, c };

            return array_output;
        } // Бросок трёх кубов
        public int[][] GURPS_throw_processing(int[][] stat, int counter, int skill_value)
        {
            Program Dice_throw = new Program();
            for (int i = 0; i < counter; i++)
            {
                int[] dice_roll = Dice_throw.GURPS_Throw(new int[3]); // получение бросков на кубах
                stat[0][i] = dice_roll[0] + dice_roll[1] + dice_roll[2]; // Сумма бросков
                stat[1][i] = skill_value - stat[0][i]; //результат броска
                for (int c = 0; c < 3; c++) // перебор кубиков
                {
                    for (int n = 1; n <= 6; n++) // перебор значения на кубе
                    {
                        if (dice_roll[c] == n) // проверка совпадения значение кубика и индекса массива
                        {
                            stat[2][n]++; // счётчик значений
                        }
                    }
                }
            }
            return stat;
        }// Сумма,успехи,счётчик значений кубов
        public int[][] just_throw_check_result(int[][] stat)
        {
            double[] numbers = new double [2];

            numbers[1] = stat[4][1];
            Array.Clear(stat[3], 0,stat.Length-1);
            for (int i=0; i< stat[4][0]; i++)
            {
                numbers[0] = stat[0][i];
                double result = 1 + (numbers[0]/numbers[1]);
                if (result < 1.05)
                {
                    stat[3][3]++;//крит
                }
                else if (result < 1.50)
                {
                    stat[3][2]++;// успех
                }
                else if (result < 1.95)
                {
                    stat[3][1]++;// провал
                }
                else
                {
                    stat[3][0]++;// крит провал
                }
            }
            return stat;
        }// счёт успехов/провалов и крит для произвольных бросков 
        public int[][] GURPS_stat_processing(int[][] stat, int counter, int sk_val)
        {
            for (int i = 0; i < counter; i++) //цикл = кол-ву бросков
            {
                if ((stat[1][i] >= 0) && (stat[0][i] < 17)) // условие успеха и крит успеха
                {
                    if ((stat[0][i] <= 4 && sk_val >= 3) | (stat[0][i] <= 5 && sk_val >= 15) | (stat[0][i] <= 6 && sk_val >= 16))
                    {
                        //Критический успех
                        stat[3][3]++;
                    }
                    else
                    {
                        // Успех
                        stat[3][2]++;
                    }
                }
                else
                {
                    if ((stat[0][i] == 18) || (stat[0][i] == 17 && sk_val <= 15) | (stat[0][i] <= -10 & sk_val < 10)) // условие крит провала
                    {
                        //Критический провал
                        stat[3][0]++;
                    }
                    else
                    {
                        //Провал
                        stat[3][1]++;
                    }
                }

            }
            return stat;
        } // результат броска(усп,крит усп, првл, крит првл); проверка когда эфф навык <3 
        public int[][] Console_stat_output(int[][] stat)
        {
            Console.WriteLine($"Количество критических провалов: {stat[3][0]}");
            Console.WriteLine($"Количество провалов : {stat[3][1]}");
            Console.WriteLine($"Количество успехов : {stat[3][2]}");
            Console.WriteLine($"Количество критических успехов: {stat[3][3]}");
            return stat;
        } // вывод реузльтата проверок
        public int[][] GURPS_console_output_most_dice_value(int[][] stat)
        {
            int most_numb = stat[2].Max(); // самое частое значение
            int max_numb = Array.IndexOf(stat[2], (int)most_numb); // что это за значение
            Console.WriteLine($"Самое часто встречающееся число на кубах {max_numb}. Встречается {most_numb} раз.");
            return stat;
        }// вывод самого частого знач на кубах
        public int[][] GURPS_console_output_all_dice_value(int[][] stat)
        {   
            for (int i = 1; i < stat[4][1]; i++) //счётчк выпавших знач
            {   if (stat[2][i] != 0)
                {
                    Console.WriteLine($"Количество выпавших {i} = {stat[2][i]}"); //вывод
                }
            }
            return stat;
        }// вывод кол-ва выпавших знач на кубах
        public int[][] GURPS_console_output_most_sum_value(int[][] stat)
        {
            var array_most_sum = stat[0].GroupBy(x => x).OrderByDescending(x => x.Count()).First(); //сорт-ка массива

            Console.WriteLine($"Самая часто встречающаяся сумма значений кубов {array_most_sum.Key}. Встречается {array_most_sum.Count()} раз");
            return stat;
        }// самая частая сумма знач кубов
        public int[][] GURPS_console_output_sum_stat(int[][] stat)
        {
            int counter = stat[4][0];
            int[] sum_stat = new int [(stat[4][1] +1)*3 ]; // массив для сумм (от 3 до 18) 
            for (int c = 0; c < counter; c++) // перебор массива сумм
            {
                for (int n = 1; n < sum_stat.Length; n++) // перебор значения суммы
                {
                    if (stat[0][c] == n) // проверка совпадения значение суммы и индекса массива
                    {
                        sum_stat[n]++; // счётчик значений
                    }
                }
            }
            for (int n = 0 ; n < sum_stat.Length; n++) // перебор значения на кубе
            {
                if (sum_stat[n] != 0)
                {
                    Console.WriteLine($"Количество выпавших {n} = {sum_stat[n]}");
                }
            }
            return stat;
        }
        public int Goblin_city_console_output_levelup(int tkn_exp, int curr_exp, int lvl_ctr, int exp_step) // подсчёт уровней для гг
        {
            int curr_lvl = new int(); // текущий уровень
            int exp = new int(); // опыт после повышения
            for (exp = curr_exp + tkn_exp; exp > lvl_ctr * exp_step; lvl_ctr++) //счётчик опыта
            {
                exp = exp - lvl_ctr * exp_step;// сколько опыта после повышения
                curr_lvl++; // повышение уровня
            }
            Console.WriteLine($"Вы получите {curr_lvl} уровней! Ваш текущий уровень будет {lvl_ctr}. Оставшееся количество опыта: {exp}"); // 
            return lvl_ctr; // возвращаем счетчик уровня
        }
        public int Cin_check(int input)
        {
            int counter = new int();
            int i = 0;
            while (i == 0)
            {
                if ((Int32.TryParse(Console.ReadLine(), out counter)) && (counter > 0)) // введеное значение должно быть int 
                { i++; }
                else
                {
                    Console.WriteLine("Введите целочисленное положительное число"); // просьба ввести число
                }
            }

            return counter;
        }// проверка ввода переменных; добавить проверку отрицательных чисел
        static void Main(string[] args)
        {
            int[][] Stat_Array = new int[5][]; // массив для статистики
            int input = 0; //ввод с консоли
            int counter = 1; //кол-во бросков
            Stat_Array[4]  = new int[2]; // [0]- количетсво кубов [1]- количество граней.
            Program Method = new Program();
            Console.WriteLine("Введите 1 если вам нужно сделать произвольный бросок");
            Console.WriteLine("Введите 2 если вам нужно сделать много бросков по системе gurps");
            Console.WriteLine("Введите 3 что-бы разбить полученный опыт на уровни (актуально только для гг!)");
            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Введите ваш бросок");
                    string str = Console.ReadLine();
                    string[] splited = str.Split('d');
                    Stat_Array[4][0] = Convert.ToInt32(splited[0]);
                    Stat_Array[4][1] = Convert.ToInt32(splited[1]);
                    Stat_Array[0] = new int[Stat_Array[4][0]];// сумма знач каждого броска кубов
                    Stat_Array[1] = new int[Stat_Array[4][0]];// результат проверки
                    Stat_Array[2] = new int[Stat_Array[4][1]];//количество выпавших значений на кубах
                    Stat_Array[3] = new int[4];//[0] - крит провалов; [1] - провалов; [2] - успехов;[3] - крит успехов;



                    Console.Clear();
                    Console.Write("Напишите stop, если хотите выйти из приложения");
                    while (Console.ReadLine().ToUpper() != "STOP") // возможность выйти из программы
                    {
                        Console.Clear();
                        Console.WriteLine("Введите 0 что-бы сгенерировать новый бросок");
                        Console.WriteLine("Введите 1 что-бы узнать статистику успехов");
                        Console.WriteLine("Введите 2 что-бы узнать самую частую сумму бросков кубов");
                        Console.WriteLine("Введите 3 что-бы узнать количество выпавших значений на кубиках");
                        Console.WriteLine("Введите 9 что-бы выйти из программы");
                        switch (Console.ReadLine())
                        {

                            case "0":
                                Stat_Array[3] = [0, 0, 0, 0]; // Зануление массива с результатами.
                                Stat_Array[0] = Method.Just_Throw(Stat_Array[4]);
                                Stat_Array = (Method.just_throw_check_result(Stat_Array));
                                goto case "1";
                            case "1":
                                Stat_Array = (Method.Console_stat_output(Stat_Array));
                                break;
                            case "2":
                                Stat_Array = (Method.GURPS_console_output_most_sum_value(Stat_Array));
                                break;
                            case "3":
                                Stat_Array = (Method.GURPS_console_output_sum_stat(Stat_Array));
                                break;

                            case "9":
                                Environment.Exit(0);
                                break;
                        }
                    }
                    break;
            

                case "2":
                    {
                        Console.WriteLine("Введите количество бросков");
                        counter = Method.Cin_check(counter);
                        Console.WriteLine("Значение эффективного навыка: ");
                        input = Method.Cin_check(input);
                        while (input < 3) // проверка что эффективный навык >3
                        {
                            Console.WriteLine("Значение эффективного навык не может быть меньше 3!");
                            Console.WriteLine("Введите новое значение эффективного навыка: ");
                            input = Method.Cin_check(input);
                        }
                        Stat_Array[0] = new int[counter];// сумма знач каждого броска кубов
                        Stat_Array[1] = new int[counter];// результат проверки
                        Stat_Array[2] = new int[7];//количество выпавших значений на кубах
                        Stat_Array[3] = new int[4];//[0] - крит провалов; [1] - провалов; [2] - успехов;[3] - крит успехов;
                        Stat_Array[4][0] = counter; // Количество кубов
                        Stat_Array[4][1] = 6; // Количество граней
                        Console.Write("Напишите stop, если хотите выйти из приложения");
                        while (Console.ReadLine().ToUpper() != "STOP") // возможность выйти из программы
                        {
                            Console.Clear();
                            Console.WriteLine("Введите 0 что-бы сгенерировать новый результат бросков");
                            Console.WriteLine("Введите 1 что-бы узнать статистику успехов");
                            Console.WriteLine("Введите 2 что-бы узнать самое частовыпадающее значение на кубах");
                            Console.WriteLine("Введите 3 что-бы узнать самую частую сумму бросков кубов");
                            Console.WriteLine("Введите 4 что-бы узнать количество выпавших значений на кубиках");
                            Console.WriteLine("Введите 5 что-бы узнать количество различных сумм на кубиках");
                            Console.WriteLine("Введите 9 что-бы выйти из программы");
                            switch (Console.ReadLine())
                            {
                                case "0":
                                    Stat_Array[3] = [0, 0, 0, 0]; // Зануление массива с результатами.
                                    Stat_Array = (Method.GURPS_throw_processing(Stat_Array, counter, input));
                                    Stat_Array = (Method.GURPS_stat_processing(Stat_Array, counter, input));
                                    goto case "1";
                                case "1":
                                    Stat_Array = (Method.Console_stat_output(Stat_Array));
                                    break;
                                case "2":
                                    Stat_Array = (Method.GURPS_console_output_most_dice_value(Stat_Array));
                                    break;
                                case "3":
                                    Stat_Array = (Method.GURPS_console_output_most_sum_value(Stat_Array));
                                    break;
                                case "4":
                                    Stat_Array = (Method.GURPS_console_output_all_dice_value(Stat_Array));
                                    break;
                                case "5":
                                    Stat_Array = (Method.GURPS_console_output_sum_stat(Stat_Array));
                                    break;

                                case "9":
                                    Environment.Exit(0);
                                    break;
                            }
                        }

                    }
                    break;
                case "3":
                    int exp_taken = 0; // Полученный опыт
                    int exp = 0; // Текущий опыт
                    int current_level = 0; // Текущий уровень 
                    int race_exp_step = 0; // Сколько прибавляется опыта для достижения следующего уровня
                    Console.WriteLine("Введите полученное количество опыта");
                    exp_taken = Method.Cin_check(exp_taken);
                    Console.WriteLine("Введите количество текущего опыта");
                    exp = Method.Cin_check(exp);
                    Console.WriteLine("Введите нынешний уровень");
                    current_level = Method.Cin_check(current_level);
                    Console.WriteLine("Введите количество опыта на первом уровне");
                    race_exp_step = Method.Cin_check(race_exp_step);

                    int GG_stat_result = Method.Goblin_city_console_output_levelup(exp_taken, exp, current_level, race_exp_step);
                    break;
            }



        }
    }

}