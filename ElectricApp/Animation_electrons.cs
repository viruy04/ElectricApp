using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using ElectricApp;

public class Animation_electrons
{
    private readonly Canvas canvas;
    private readonly DispatcherTimer timer;
    private readonly List<Point> pathPoints;
    private readonly List<double> segmentLengths;
    private readonly double totalLength;
    private readonly List<Ellipse> particles;
    private readonly double[] particleDistances;

    private int particleCount = 20;
    private double particleSpeed;

    public Animation_electrons(Canvas targetCanvas, double current)
    {
        canvas = targetCanvas;

        // Задание пути движения электронов (без масштаба)
        pathPoints = new List<Point> {
        new Point(30, 40),
        new Point(300, 40),
        new Point(300, 150),
        new Point(240, 150),
        new Point(240, 200),
        new Point(30, 200),
        new Point(30, 80),
        new Point(30, 40)
    };

        double scale = 2.5;
        double offsetX = -25;

        // Масштабируем точки пути
        for (int i = 0; i < pathPoints.Count; i++)
        {
            pathPoints[i] = new Point(
                offsetX + pathPoints[i].X * scale,
                pathPoints[i].Y * scale
            );
        }

        // Вычисляем длины сегментов пути и общую длину
        segmentLengths = CalculateSegmentLengths(pathPoints, out totalLength);
        particles = new List<Ellipse>();
        particleDistances = new double[particleCount];

        // Скорость электронов зависит от тока
        particleSpeed = Clamp(Math.Pow(current, 0.6) * 1.5, 0.2, 10.0);

        // Удаляем электроны, создаём новые
        InitializeParticles();

        // Таймер анимации
        timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(30)
        };
        timer.Tick += MoveParticles;
        timer.Start();
    }


    // Ограничиваем скорость в заданных пределах
    private double Clamp(double value, double min, double max)
    {
        return Math.Max(min, Math.Min(max, value));
    }

    // Вычисление длин сегментов пути и общей длины пути
    private List<double> CalculateSegmentLengths(List<Point> points, out double total)
    {
        var lengths = new List<double>();
        total = 0;

        for (int i = 0; i < points.Count - 1; i++)
        {
            var dx = points[i + 1].X - points[i].X;
            var dy = points[i + 1].Y - points[i].Y;
            var len = Math.Sqrt(dx * dx + dy * dy);
            lengths.Add(len);
            total += len;
        }

        return lengths;
    }

    // Создаём электроны и размещаем их на пути равномерно
    private void InitializeParticles()
    {
        // Удаляем предыдущие электроны (Ellipse) с Canvas
        for (int i = canvas.Children.Count - 1; i >= 0; i--)
        {
            if (canvas.Children[i] is Ellipse)
                canvas.Children.RemoveAt(i);
        }

        particles.Clear();

        // Создаём и добавляем электроны на Canvas
        for (int i = 0; i < particleCount; i++)
        {
            var ellipse = new Ellipse
            {
                Width = 14,  // размер электрона
                Height = 14,
                Fill = Brushes.Orange,
                Stroke = Brushes.DarkOrange,
                StrokeThickness = 1.5,
                Tag = "electron"
            };
            canvas.Children.Add(ellipse);
            particles.Add(ellipse);

            // Распределяем электроны равномерно по длине пути
            particleDistances[i] = (totalLength / particleCount) * i;
        }
    }

    // Обновляем позицию каждого электрона для анимации движения
    private void MoveParticles(object sender, EventArgs e)
    {
        for (int i = 0; i < particles.Count; i++)
        {
            particleDistances[i] += particleSpeed;
            if (particleDistances[i] > totalLength)
                particleDistances[i] -= totalLength;

            double distance = particleDistances[i];
            int segmentIndex = 0;

            // Определяем сегмент пути, на котором находится частица
            while (distance > segmentLengths[segmentIndex])
            {
                distance -= segmentLengths[segmentIndex];
                segmentIndex++;
                if (segmentIndex >= segmentLengths.Count)
                {
                    segmentIndex = 0;
                    distance = 0;
                    break;
                }
            }

            double t = distance / segmentLengths[segmentIndex];
            var p1 = pathPoints[segmentIndex];
            var p2 = pathPoints[segmentIndex + 1];

            // Интерполируем положение частицы между двумя точками сегмента
            double x = p1.X + (p2.X - p1.X) * t;
            double y = p1.Y + (p2.Y - p1.Y) * t;

            // Устанавливаем позицию частицы на Canvas
            Canvas.SetLeft(particles[i], x - particles[i].Width / 2);
            Canvas.SetTop(particles[i], y - particles[i].Height / 2);
        }
    }

    // Запуск анимации электронов
    public void Start() => timer.Start();

    // Остановка анимации электронов
    public void Stop() => timer.Stop();
}
