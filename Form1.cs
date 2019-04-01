using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


/*
 * Программа, предназначеная для генерации чисел, распределенных по заданному закону распределения (одному из 11)
* Важно! Программе требуется хорошая процедура (или формула(ы)) определения границ Плотностей распределения (по осям ординат и абсисс), из-за
* этого и имеются недоработки,т.к. все границы определялись из "геометрических" соображений.
 * В программе генерируются 2 независимые СВ, это слчайные координаты X, Y. Затем они отображаются на декартовой
 * плоскости в виде набора черных точек.
 * (Имеются проблемы с генераторами дискретных случайных величин (практически не работают))
 * Для генерации точек необходимо выбрать ЗР, ввести его параметры, ввести колличество генерируемых точек.
 * Плотности распределений представленные в программе:
 * ----- Непрерывные СВ ---------
 * Треугольный ЗР (Распределение Симпсона) - вводимые данные: концы отрезка.  Такое    распределение   наблюдается тогда, 
 *                 когда суммируются  две  или вычитаются две случайные величины, которые  имеют равномерный закон распределения.
 *                 Ограничения на ввод - левая граница отрезка должна быть меньше правой.
 * Экспоненциальный ЗР - вводимые данные : Лямбда - интенсивность появления какого-либо события.ЗР моделирует время 
 *                       между двумя последовательными свершениями одного и того же события.
 *                       Ограничения на ввод - лямбда не больше 13.
 * Нормальный ЗР - вводимые значения : Сигма - СКО, М - мат. ожидание, медиана и мода распр. Фундаментальное распределение, на нем
 *                 базируются многие законы стат. физики и мат. статистики, например, в математической статистике и статистической 
 *                 физике вытекает из центральной предельной теоремы теории вероятностей.
 *                 Ограничения на ввод - без ограничений.
 * ЛогНормальный ЗР (проблемы с заданием границ) - логарифмированное норм. распределение. ПАраметры ввода те же.
 *                   Ограничения на ввод - мат. ожидание не больше 6 (При этом дисперсия не должна быть большой).
 * Гамма - распред. - это двухпараметрическое семейство абсолютно непрерывных распределений. Базируется на понятии Гамма-Функции
 *                    Эйлера. Вводимые параметры - k, Teta. Ограничения на ввод - никаких отрицательных значений.
 * Бетта-распр. - двухпараметрическое семейство абсолютно непрерывных распределений. Используется для описания случайных величин, 
 *                значения которых ограничены конечным интервалом. Вводимые параметры - Альфа и Бета. Ограничения на ввод - аналогично
 *                Гамма распр.
 * Хи-квадрат - это распределение суммы квадратов k независимых стандартных нормальных случайных величин. Вводимые параметры - 
 *               число степ. свободы k. Число k желательно больше 2.
 * Распр. Стьюдента -  это однопараметрическое семейство абсолютно непрерывных распределений. Вводимые параметры - число степ. свободы.
 *                     Ограничения на ввод - число степ. свободы больше 0.
 * Распр. Фишера-Снедекора - это двухпараметрическое семейство абсолютно непрерывных распределений. Вводимые параметры - числа степеней
 *                           свободы. Числа степ. свободы больше 0, причем d1 > 1.
 * ------- Дискретные СВ ----------
 * Ограничения на ввод - Необходима доработка, пераметры по умолчанию пока лучше не трогать.
 * Пуассоновский ЗР - вероятностное распределение дискретного типа, моделирует случайную величину, представляющую собой число событий, 
 *                    произошедших за фиксированное время, при условии, что данные события происходят с некоторой фиксированной средней 
 *                    интенсивностью и независимо друг от друга.
 * Биномиальное распр. -  распределение количества «успехов» в последовательности из n независимых случайных экспериментов, таких, 
 *                        что вероятность «успеха» в каждом из них постоянна и равна p.
 */
namespace DifferentDistribution
{
    public partial class Form1 : Form
    {
        //Distribution - виды распределений - Треугольное, Экспоненциальное, Биномиальное, Нормальное, Логнормальное, Бетта, Хи-квадрат, 
        //распр. Стьюдента, распр. Фишера-Снедекора
        public enum Distribution { Treangle, Exp, Puason, Binomial, Normal, LogNormal, Gamma, Betta, HeaSqr, Student, Fisher };
        public Distribution Distrib; //заданное пользователем распределение
        public const int A = 50;//колличество отрезков, на который делится интервал
        public delegate double DistributionFunct(double x, double Param1, double Param2);//вид функции распределения для задания плотности вероятности непрерывной СВ
        //Таблица значений Гамма Функции на отрезке [1..2]
        public static double[,] GammaFunct_1_2 = {{1, 1.01,   1.02,  1.03,  1.04,  1.05,  1.06,  1.07,  1.08,  1.09,  1.1,   1.11,  1.12,  1.13,  1.14,  1.15,  1.16,  1.17,  1.18,1.19,1.2,1.21,1.22,1.23,1.24,1.25,1.26,1.27,1.28,1.29,1.3,1.31,1.32,1.33,1.34,1.35,1.36,1.37,1.38,1.39,1.4,1.41,1.42,1.43,1.44,1.45,1.46,1.47,1.48,1.49,1.5,1.51,1.52,1.53,1.54,1.55,1.56,1.57,1.58,1.59,1.6,1.61,1.62,1.63,1.64,1.65,1.66,1.67,1.68,1.69,1.7,1.71,1.72,1.73,1.74,1.75,1.76,1.77,1.78,1.79,1.8,1.81,1.82,1.83,1.84,1.85,1.86,1.87,1.88,1.89,1.9,1.91,1.92,1.93,1.94,1.95,1.96,1.97,1.98,1.99,2},
                                                  {1, 0.9943, 0.9888,0.9835,0.9784,0.9735,0.9687,0.9642,0.9597,0.9555,0.9514,0.9474,0.9436,0.9399,0.9364,0.9330,0.9298,0.9267,0.9237,0.9209,0.9182,0.9156,0.9131,0.9108,0.9030,0.9064,0.9044,0.9025,0.9007,0.8990,0.8975,0.8960,0.8946,0.8934,0.8922,0.8912,0.8902,0.8893,0.8885,0.8879,0.8873,0.8868,0.8864,0.8860,0.8858,0.8857,0.8856,0.8856,0.8857,0.8859,0.8862,0.8866,0.8870,0.8876,0.8882,0.8889,0.8896,0.8905,0.8914,0.8924,0.8935,0.8947,0.8959,0.8972,0.8986,0.9001,0.9017,0.9033,0.9050,0.9068,0.9086,0.9106,0.9126,0.9147,0.9168,0.9191,0.9214,0.9238,0.9262,0.9288,0.9314,0.9341,0.9368,0.9397,0.9426,0.9456,0.9187,0.9518,0.9551,0.9584,0.9618,0.9652,0.9688,0.9724,0.9761,0.9799,0.9837,0.9877,0.9917,0.9959,1}};

        //факториал
        public uint Fact(uint n)
        {
            if (n == 0)
                return 1;
            else
            {
                uint p = 1;
                for (uint i = n; i >= 1; i--)
                    p *= i;
                return p;
            }

        }

        //-------------------------------ГАММА-ФУНКЦИЯ--------------------------------
        //(работает для любых вещестенных чисел (кроме своих асимптот, само собой)
        public double GammaFunction(double X)
        {
            if ((X < 1) || (X > 2))
            {
                //для чисел меньших 1 (отрицательных и на отрезке [0..1] (ваш К.О.)
                //согласно свойству Гамма функции - s*Г(s) = Г(s+1) рекурсивно из 
                //отрицательной области выходим на отрезок [1,2], на котором функция известна
                if (X < 1)
                    return (1.0 / X) * GammaFunction(X + 1);
                else
                    if (X == 0)
                        return 1;
                    else//для чисел больших 2 совершаем обратный ход к отрезку 
                        return (X - 1) * GammaFunction(X - 1);
            }
            else
            {
                //для чисел лежащих на данном отрезке применяем простую линейную интерполяцию, и получаем нужное значение функции
                //Г(х). В итоге, после рекурсивного возврата получаем нужное нам значение функции от вещественного аргумента.
                for (int i = 0; i < 100; i++)
                    if ((X >= GammaFunct_1_2[0, i]) && (X <= GammaFunct_1_2[0, i + 1])) return GammaFunct_1_2[1, i] + (X - GammaFunct_1_2[0, i]) * ((GammaFunct_1_2[1, i + 1] - GammaFunct_1_2[1, i]) / (GammaFunct_1_2[0, i + 1] - GammaFunct_1_2[0, i]));
            }
            return 0;
        }

        //-----------------------------Бетта функция---------------------------------
        public double BettaFunction(double X, double Y)
        {//выражается через гамма функцию
            return (GammaFunction(X)*GammaFunction(Y))/GammaFunction(X + Y);
        }


        //-----------------------ГЕнерация случайной точки для непрерывной СВ-----------------------------------
        /*
         * Входные данные:
         *  Distrib - переменная делегат, для передачи в метод функции с необходимым распределением
         *  Param1, Param2 - параметры распределения
         *  MinX, MaxX - границы изменения функции распр., в этих границах с вероятностью почти 1 лежат все случ. точки
         *  MaxY - максимум функции распределения (данные параметры задают прямоугольник, для генерации случайных точек)
         */
        public double GeneratePoint(DistributionFunct Distrib, double Param1, double Param2, 
                                    double MinX, double MaxX, double MaxY, Random Rnd)
        {
            double R, x, y;

            R = Rnd.NextDouble();
            x = R * MaxX + (1 - R) * MinX;
            y = Rnd.NextDouble() * MaxY;
            //проверяется, попадает ли генерируемая точка под кривую ПР СВ, если попадает, то данная точка распределена по
            //данному закону
            while (Distrib(x, Param1, Param2) < y)
            {
                R = Rnd.NextDouble();
                x = R * MaxX + (1 - R) * MinX;
                y = Rnd.NextDouble() * MaxY;
            }

            return x;
        }

        //=========================== Плотности Распределений Непрерывных СВ ======================================
        //Треугольная
        public double TreangleDistr(double x, double a, double b)
        {
            if ((x >= a) && (x <= b))
                return (2.0 / (b - a)) - (2.0 / Math.Pow(b - a, 2)) * Math.Abs(a + b - 2 * x);
            else
                return 0;
        }

        //Экспоненциальная
        public double ExpDistr(double x, double lambda, double fict)
        {

            if (x >= 0)
                return lambda * Math.Exp(-lambda*x);
            else
                return 0;
        }

        //Нормальная
        public double Normal(double x, double m, double sigma)
        {
            return (1 / (sigma * Math.Sqrt(2 * Math.PI))) * Math.Exp(-Math.Pow(x - m, 2) / (2 * sigma * sigma));
        }

        //Логарифмическая нормальная
        public double LogNormal(double x, double m, double sigma)
        {
            return (1 / (x * sigma * Math.Sqrt(2 * Math.PI))) * Math.Exp(-Math.Pow(Math.Log(x) - m, 2) / (2 * sigma * sigma));     
        }

        //Гамма Распределение
        public double Gamma(double x, double k, double teta)
        {
            return Math.Pow(x, k - 1) * (Math.Exp(-x / teta) / (Math.Pow(teta, k) * GammaFunction(k)));
        }

        //Бетта распределение
        public double Betta(double x, double alpha, double betta)
        {
            return (1 / BettaFunction(alpha, betta)) * Math.Pow(x, alpha - 1) * Math.Pow(1 - x, betta - 1);
        }

        //Хи-квадрат
        public double HeaSqr(double x, double k, double fict)
        {
            double s = (Math.Pow(0.5, k / 2) / GammaFunction(k / 2)) * Math.Pow(x, k / 2 - 1) * Math.Exp(-x / 2);
            return (Math.Pow(0.5, k / 2) / GammaFunction(k / 2)) * Math.Pow(x, k / 2 - 1) * Math.Exp(-x / 2);
        }

        //распр.  Стьюдента
        public double Student(double x, double n, double fict)
        {
            double s = ((GammaFunction((n + 1) / 2)) / (Math.Sqrt(Math.PI * n) * GammaFunction(n / 2))) * Math.Pow(1 + (x * x) / n, -(n + 1) / 2);
            return s;
        }

        //распр. Фишера-Снедекора
        public double Fisher(double x, double d1, double d2)
        {
            return Math.Sqrt((Math.Pow(d1*x,d1)*Math.Pow(d2,d2))/(Math.Pow(d1*x+d2,d1+d2)))/(x*BettaFunction(d1/2,d2/2));
        }
        //***************************************************************************

        //================= Распределения дискретных СВ ==========================
        //Пуассона
        public double Puasson(uint k, double lambda)
        {
            double f = Fact(k);
            double z = Math.Pow(lambda, k);
            double s1 = (z / f);
            double s2 =  Math.Exp(-lambda);
            return s1*s2;
        }

        //Биномиальное
        public double Binomial(uint k, uint n, double p)
        {
            return (Fact(n) / (Fact(k) * Fact(n - k))) * Math.Pow(p, k) * Math.Pow(1 - p, n - k);
        }
        //***************************************************************************

        //для правки полей ввода на форме, в зависимости от выбранного распределения
        public void EditInput(bool S)
        {
            textBox4.Visible = S;
            textBox5.Visible = S;
            label5.Visible = S;
            label6.Visible = S;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int N = Convert.ToInt32(textBox1.Text); //число генерируемых точек
            double x, y, Xdis, Ydis, delX = 0, delY = 0, MinX = 0, MinY = 0, MaxY = 0, MaxX = 0,
                   Param1X = 0, Param1Y = 0, Param2X = 0, Param2Y = 0, DistrMaxX = 0, DistrMaxY = 0, delta;
            double alphax, alphay, bettax, bettay, ax, ay, bx, by, lambda_x, lambda_y,
                   sigmax, sigmay, mx, my, kx, ky, tetax, tetay, d1x, d2x, d1y, d2y, px, py;
            int[] Yk = new int[A];//массивы для рассчета колличества точек, попавших
            int[] Xk = new int[A];//в заданный интервал, для координат X и Y.
            int i, j, nx, ny;
            Random Rnd = new Random();
            DistributionFunct DistrFunction = null;
            chart1.Series[0].Points.Clear();
            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();
            chart3.Series[0].Points.Clear();
            chart3.Series[1].Points.Clear();

            //пользователь выбрал нужное распределение
            switch (Distrib)
            {
                case Distribution.Treangle:
                    {
                        ax = Convert.ToDouble(textBox2.Text);
                        bx = Convert.ToDouble(textBox3.Text);
                        ay = Convert.ToDouble(textBox4.Text);
                        by = Convert.ToDouble(textBox5.Text);
                        delX = Math.Abs(bx - ax) / (double)A;
                        delY = Math.Abs(by - ay) / (double)A;
                        //границы ПР есть заданные пользователем границы отрезков
                        MinX = ax; MaxX = bx;
                        MinY = ay; MaxY = by;
                        DistrFunction = TreangleDistr;
                        Param1X = ax; Param2X = bx;
                        Param1Y = ay; Param2Y = by;
                        //определяем максимум функции, из геометрических соображений
                        DistrMaxX = 2.0 / Math.Abs(bx - ax);
                        DistrMaxY = 2.0 / Math.Abs(by - ay);
                    }
                    break;
                case Distribution.Exp:
                    {
                        lambda_x = Convert.ToDouble(textBox2.Text);
                        lambda_y = Convert.ToDouble(textBox3.Text);
                        //здесь, для нахождения максимума аргумента в цикле сравним разницу между кривой и осью Ох, если она 
                        //ничтожно мала, значит кривая распределения очень близко подошла к оси, и участком кривой правее можно
                        //пренебречь
                        delta = lambda_x / A;
                        x = 0;
                        while (ExpDistr(x, lambda_x, 0) > delta)
                        {
                            MaxX = x;
                            x += delta;
                        }
                        delta = lambda_y / A;
                        y = 0;
                        while (ExpDistr(y, lambda_y, 0) > delta)
                        {
                            MaxY = y;
                            y += delta;
                        }
                        delX = Math.Abs(MaxX) / (double)A;
                        delY = Math.Abs(MaxY) / (double)A;
                        MinX = 0;
                        MinY = 0;
                        DistrFunction = ExpDistr;
                        Param1X = lambda_x; Param2X = 0;
                        Param1Y = lambda_y; Param2Y = 0;
                        //максимумы функций
                        DistrMaxX = lambda_x;
                        DistrMaxY = lambda_y;
                    }
                    break;
                case Distribution.Normal:
                    sigmax = Convert.ToDouble(textBox2.Text);
                    mx = Convert.ToDouble(textBox3.Text);
                    sigmay = Convert.ToDouble(textBox4.Text);
                    my = Convert.ToDouble(textBox5.Text);
                    //все величины задаются из правила трех сигм
                    MinX = mx - 3 * sigmax;
                    MaxX = mx + 3 * sigmax;
                    MinY = my - 3 * sigmay;
                    MaxY = my + 3 * sigmay;
                    delX = Math.Abs(MaxX - MinX) / (double)A;
                    delY = Math.Abs(MaxY - MinY) / (double)A;
                    DistrFunction = Normal;
                    Param1X = mx; Param2X = sigmax;
                    Param1Y = my; Param2Y = sigmay;
                    DistrMaxX = Normal(mx, mx, sigmax);
                    DistrMaxY = Normal(my, my, sigmay);
                    break;
                case Distribution.LogNormal:
                    sigmax = Convert.ToDouble(textBox2.Text);
                    mx = Convert.ToDouble(textBox3.Text);
                    sigmay = Convert.ToDouble(textBox4.Text);
                    my = Convert.ToDouble(textBox5.Text);
                    //действует аналогичное правило трех сигм, за исключением того, что величины надо брать в экспоненте
                    MinX = 0;
                    MaxX = Math.Exp(mx) + 3 * Math.Exp(mx) * Math.Exp(sigmax);
                    MinY = 0;
                    MaxY = Math.Exp(my) + 3 * Math.Exp(my) * Math.Exp(sigmay);
                    delX = Math.Abs(MaxX) / (double)A;
                    delY = Math.Abs(MaxY) / (double)A;
                    DistrFunction = LogNormal;
                    Param1X = mx; Param2X = sigmax;
                    Param1Y = my; Param2Y = sigmay;
                    DistrMaxX = LogNormal(Math.Exp(mx - (sigmax * sigmax)), mx, sigmax);
                    DistrMaxY = LogNormal(Math.Exp(my - (sigmay * sigmay)), my, sigmay);
                    break;
                case Distribution.Gamma:
                    //задаем границы как [0..m+D]
                    //максимум функции находится в моде распределения
                    kx = Convert.ToDouble(textBox2.Text);
                    tetax = Convert.ToDouble(textBox3.Text);
                    ky = Convert.ToDouble(textBox4.Text);
                    tetay = Convert.ToDouble(textBox5.Text);
                    MinX = 0;
                    MaxX = kx * tetax * (2 + tetax);
                    MinY = 0;
                    MaxY = ky * tetay * (2 + tetay);
                    delX = Math.Abs(MaxX) / (double)A;
                    delY = Math.Abs(MaxY) / (double)A;
                    DistrFunction = Gamma;
                    Param1X = kx; Param2X = tetax;
                    Param1Y = ky; Param2Y = tetay;
                    DistrMaxX = Gamma((kx - 1) * tetax, kx, tetax);
                    DistrMaxY = Gamma((ky - 1) * tetay, ky, tetay);
                    break;
                case Distribution.Betta:
                    //из построения видно что аргумент лежит в пределах 0 и 1
                    //максимум лежит на моде
                    alphax = Convert.ToDouble(textBox2.Text);
                    bettax = Convert.ToDouble(textBox3.Text);
                    alphay = Convert.ToDouble(textBox4.Text);
                    bettay = Convert.ToDouble(textBox5.Text);
                    MinX = 0.01;
                    MaxX = 0.99;
                    MinY = 0.01;
                    MaxY = 0.99;
                    delX = Math.Abs(MaxX) / (double)A;
                    delY = Math.Abs(MaxY) / (double)A;
                    DistrFunction = Betta;
                    Param1X = alphax; Param2X = bettax;
                    Param1Y = alphay; Param2Y = bettay;
                    double max;
                    if ((alphax > 1) && (bettax > 1))
                        DistrMaxX = Betta((alphax - 1) / (alphax + bettax - 2), alphax, bettax);
                    else
                    {
                        if (alphax < bettax) max = 0.01; else max = 0.99;
                        DistrMaxX = Betta(max, alphax, bettax);
                    }

                    if ((alphay > 1) && (bettay > 1))
                        DistrMaxY = Betta((alphay - 1) / (alphay + bettay - 2), alphay, bettay);
                    else
                    {
                        if (alphay < bettay) max = 0.01; else max = 0.99;
                        DistrMaxY = Betta(max, alphay, bettay);
                    }
                    break;
                case Distribution.HeaSqr:
                    //аналогично предыдущему, так и далее
                    kx = Convert.ToDouble(textBox2.Text);
                    ky = Convert.ToDouble(textBox3.Text);
                    MinX = 0;
                    MinY = 0;
                    MaxX = 3 * kx;
                    MaxY = 3 * ky; ;
                    delX = Math.Abs(MaxX) / (double)A;
                    delY = Math.Abs(MaxY) / (double)A;
                    DistrFunction = HeaSqr;
                    Param1X = kx; Param2X = 0;
                    Param1Y = ky; Param2Y = 0;
                    if (kx >= 2)
                        DistrMaxX = HeaSqr(kx - 2, kx, 0);
                    else
                        DistrMaxX = HeaSqr(0.01, kx, 0);

                    if (ky >= 2)
                        DistrMaxY = HeaSqr(ky - 2, ky, 0);
                    else
                        DistrMaxY = HeaSqr(0.01, ky, 0);
                    break;
                case Distribution.Student:
                    kx = Convert.ToDouble(textBox2.Text);
                    ky = Convert.ToDouble(textBox3.Text);
                    MinX = -10;
                    MinY = -10;
                    MaxX = 10;
                    MaxY = 10;
                    delX = Math.Abs(MaxX - MinX) / (double)A;
                    delY = Math.Abs(MaxY - MinY) / (double)A;
                    DistrFunction = Student;
                    Param1X = kx; Param2X = 0;
                    Param1Y = ky; Param2Y = 0;
                    DistrMaxX = Student(0, kx, 0);
                    DistrMaxY = Student(0, ky, 0);
                    break;
                case Distribution.Fisher:
                    d1x = Convert.ToDouble(textBox2.Text);
                    d2x = Convert.ToDouble(textBox3.Text);
                    d1y = Convert.ToDouble(textBox4.Text);
                    d2y = Convert.ToDouble(textBox5.Text);
                    MinX = 0;
                    MinY = 0;
                    MaxX = 10;
                    MaxY = 10;
                    delX = Math.Abs(MaxX - MinX) / (double)A;
                    delY = Math.Abs(MaxY - MinY) / (double)A;
                    DistrFunction = Fisher;
                    Param1X = d1x; Param2X = d2x;
                    Param1Y = d1y; Param2Y = d2y;

                    if (d1x > 2)
                        DistrMaxX = Fisher(((d1x - 2) / d1x) * (d2x / (d2x + 2)), d1x, d2x);
                    else
                        DistrMaxX = Fisher(0.01, d1x, d2x);

                    if (d1y > 2)
                        DistrMaxY = Fisher(((d1y - 2) / d1y) * (d2y / (d2y + 2)), d1y, d2y);
                    else
                        DistrMaxY = Fisher(0.01, d1y, d2y);
                    break;
            }

            //для непрерывных распределений имеем единый алгоритм для генерации
            if ((Distrib != Distribution.Puason) && (Distrib != Distribution.Binomial))
            {
                for (j = 1; j <= N; j++)
                {
                    //генерация точек X и Y
                    Xdis = GeneratePoint(DistrFunction, Param1X, Param2X, MinX, MaxX, DistrMaxX, Rnd);
                    Ydis = GeneratePoint(DistrFunction, Param1Y, Param2Y, MinY, MaxY, DistrMaxY, Rnd);

                    //расчет попадания на какой-либо отрезок
                    for (i = 0; i < A; i++)
                    {
                        if ((Xdis >= MinX + i * delX) && (Xdis <= MinX + (i + 1) * delX))
                            Xk[i]++;
                        if ((Ydis >= MinY + i * delY) && (Ydis <= MinY + (i + 1) * delY))
                            Yk[i]++;
                    }

                    chart1.Series[0].Points.AddXY(Xdis, Ydis);
                }

                //вывод столбчатой диаграммы
                for (i = 0, x = MinX, y = MinY; i < A; i++, x += delX, y += delY)
                {
                        chart2.Series[0].Points.AddXY(x, (double)Xk[i] / (delX * N));
                        chart3.Series[0].Points.AddXY(y, (double)Yk[i] / (delY * N));
                }

                //вывод графика распределения
                for (x = MinX; x <= MaxX; x += delX)
                    chart2.Series[1].Points.AddXY(x, DistrFunction(x, Param1X, Param2X));
                for (y = MinY; y <= MaxY; y += delY)
                    chart3.Series[1].Points.AddXY(y, DistrFunction(y, Param1Y, Param2Y));
            }
            else
            {//для дискретных СВ необходим несколько иной подход (через ж..., т.е. не работает)
                //(хотя можно и сделать так же, как и с непрерывными расп.

                switch (Distrib)
                {
                    case Distribution.Puason:
                        lambda_x = Convert.ToDouble(textBox2.Text);
                        lambda_y = Convert.ToDouble(textBox3.Text);
                        uint k;
                        int Min_X = 0;
                        int Min_Y = 0;
                        int Max_X = Convert.ToInt32(2*lambda_x);
                        int Max_Y = Convert.ToInt32(2*lambda_y);
                        delX = Max_X / (double)20;
                        delY = Max_Y / (double)20;
                        double DistrMax_X = Puasson(Convert.ToUInt16(lambda_x), lambda_x);
                        double DistrMax_Y = Puasson(Convert.ToUInt16(lambda_y), lambda_y);
                        for (j = 1; j <= N; j++)
                        {
                            k = (uint)Rnd.Next(0, Max_X);
                            y = Rnd.NextDouble() * DistrMax_X;
                            while (y > Puasson(k, lambda_x))
                            {
                                k = (uint)Rnd.Next(0, Max_X);
                                y = Rnd.NextDouble() * DistrMax_X;
                            }
                            Xdis = k;
                            k = (uint)Rnd.Next(0, Max_Y);
                            y = Rnd.NextDouble() * DistrMax_Y;
                            while (y > Puasson(k, lambda_y))
                            {
                                k = (uint)Rnd.Next(0, Max_Y);
                                y = Rnd.NextDouble() * DistrMax_Y;
                            }
                            Ydis = k;
                            for (i = 0; i < 20; i++)
                            {
                                if ((Xdis >= Min_X + i * delX) && (Xdis <= Min_X + (i + 1) * delX))
                                    Xk[i]++;
                                if ((Ydis >= Min_Y + i * delY) && (Ydis <= Min_Y + (i + 1) * delY))
                                    Yk[i]++;
                            }
                            chart1.Series[0].Points.AddXY(Xdis, Ydis);
                        }

                            for (i = 0, x = Min_X, y = Min_Y; i < 20; i++, x += delX, y += delY)
                            {
                                chart2.Series[0].Points.AddXY(x, (double)Xk[i] / N);
                                chart3.Series[0].Points.AddXY(y, (double)Yk[i] / N);
                            }

                            for (k = 0; k <= Max_X; k++)
                                chart2.Series[1].Points.AddXY(k, Puasson(k, lambda_x));
                            for (k = 0; k <= Max_Y; k++)
                                chart3.Series[1].Points.AddXY(k, Puasson(k, lambda_y));
                        
                        break;
                    case Distribution.Binomial:
                        nx = Convert.ToInt32(textBox2.Text);
                        px = Convert.ToDouble(textBox3.Text);
                        ny = Convert.ToInt32(textBox4.Text);
                        py = Convert.ToDouble(textBox5.Text);
                        Min_X = 0;
                        Min_Y = 0;
                        Max_X = nx;
                        Max_Y = ny;
                        delX = Max_X / (double)20;
                        delY = Max_Y / (double)20;
                        DistrMax_X = Binomial(Convert.ToUInt16((nx+1)*px), (uint)nx, px);
                        DistrMax_Y = Binomial(Convert.ToUInt16((ny + 1) * py), (uint)ny, py);
                        for (j = 1; j <= N; j++)
                        {
                            k = (uint)Rnd.Next(0, Max_X);
                            y = Rnd.NextDouble() * DistrMax_X;
                            while (y > Binomial(k, (uint)nx, px))
                            {
                                k = (uint)Rnd.Next(0, Max_X);
                                y = Rnd.NextDouble() * DistrMax_X;
                            }
                            Xdis = k;
                            k = (uint)Rnd.Next(0, Max_Y);
                            y = Rnd.NextDouble() * DistrMax_Y;
                            while (y > Binomial(k, (uint)ny, py))
                            {
                                k = (uint)Rnd.Next(0, Max_Y);
                                y = Rnd.NextDouble() * DistrMax_Y;
                            }
                            Ydis = k;
                            for (i = 0; i < 20; i++)
                            {
                                if ((Xdis >= Min_X + i * delX) && (Xdis <= Min_X + (i + 1) * delX))
                                    Xk[i]++;
                                if ((Ydis >= Min_Y + i * delY) && (Ydis <= Min_Y + (i + 1) * delY))
                                    Yk[i]++;
                            }
                            chart1.Series[0].Points.AddXY(Xdis, Ydis);
                        }

                            for (i = 0, x = Min_X, y = Min_Y; i < 20; i++, x += delX, y += delY)
                            {
                                chart2.Series[0].Points.AddXY(x, (double)Xk[i] / N);
                                chart3.Series[0].Points.AddXY(y, (double)Yk[i] / N);
                            }

                            for (k = 0; k <= Max_X; k++)
                                chart2.Series[1].Points.AddXY(k, Binomial(k, (uint)nx, px));
                            for (k = 0; k <= Max_Y; k++)
                                chart3.Series[1].Points.AddXY(k, Binomial(k, (uint)ny, py));
                        break;
                }
            }
        }

        //отображаемые поля ввода и значения по умолчанию (самые показательные для всех распределений)
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0: Distrib = Distribution.Treangle;
                        EditInput(true);
                        label3.Text = "ax";
                        label4.Text = "bx";
                        label5.Text = "ay";
                        label6.Text = "by";
                        textBox2.Text = "3";
                        textBox3.Text = "5";
                        textBox4.Text = "10";
                        textBox5.Text = "20";
                        break;
                case 1: Distrib = Distribution.Exp;
                        EditInput(false);
                        label3.Text = "λx";
                        label4.Text = "λy";
                        textBox2.Text = "4";
                        textBox3.Text = "3";
                        break;
                case 2: Distrib = Distribution.Puason;
                        EditInput(false);
                        label3.Text = "λx";
                        label4.Text = "λy";
                        textBox2.Text = "6";
                        textBox3.Text = "4";
                        break;
                case 3: Distrib = Distribution.Binomial;
                        EditInput(true);
                        label3.Text = "nx";
                        label4.Text = "px";
                        label5.Text = "ny";
                        label6.Text = "py";
                        textBox2.Text = "10";
                        textBox3.Text = "0,5";
                        textBox4.Text = "10";
                        textBox5.Text = "0,5";
                        break;
                case 4: Distrib = Distribution.Normal;
                        EditInput(true);
                        label3.Text = "σx";
                        label4.Text = "mx";
                        label5.Text = "σy";
                        label6.Text = "my";
                        textBox2.Text = "1";
                        textBox3.Text = "0";
                        textBox4.Text = "3";
                        textBox5.Text = "-4";
                        break;
                case 5: Distrib = Distribution.LogNormal;
                        EditInput(true);
                        label3.Text = "σx";
                        label4.Text = "mx";
                        label5.Text = "σy";
                        label6.Text = "my"; 
                        textBox2.Text = "0,4";
                        textBox3.Text = "2";
                        textBox4.Text = "1,05";
                        textBox5.Text = "0";                     
                        break;
                case 6: Distrib = Distribution.Gamma;
                        EditInput(true);
                        label3.Text = "kx";
                        label4.Text = "θx";
                        label5.Text = "ky";
                        label6.Text = "θy";
                        textBox2.Text = "3";
                        textBox3.Text = "5";
                        textBox4.Text = "2";
                        textBox5.Text = "1";
                        break;
                case 7: Distrib = Distribution.Betta;
                        EditInput(true);
                        label3.Text = "αx";
                        label4.Text = "βx";
                        label5.Text = "αy";
                        label6.Text = "βy";
                        textBox2.Text = "0,6";
                        textBox3.Text = "0,5";
                        textBox4.Text = "3";
                        textBox5.Text = "4";
                        break;
                case 8: Distrib = Distribution.HeaSqr;
                        EditInput(false);
                        label3.Text = "kx";
                        label4.Text = "ky";
                        textBox2.Text = "2";
                        textBox3.Text = "10";
                        break;
                case 9: Distrib = Distribution.Student;
                        EditInput(false);
                        label3.Text = "nx";
                        label4.Text = "ny";
                        textBox2.Text = "1";
                        textBox3.Text = "4";
                        break;
                case 10: Distrib = Distribution.Fisher;
                        EditInput(true);
                        label3.Text = "d1x";
                        label4.Text = "d2x";
                        label5.Text = "d1y";
                        label6.Text = "d2y"; 
                        textBox2.Text = "80";
                        textBox3.Text = "20";
                        textBox4.Text = "50";
                        textBox5.Text = "5";                        
                        break;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            Distrib = Distribution.Treangle;
        }
    }
}
