using System;
using QueueSimulatorApp.Models;

namespace QueueSimulatorApp.Services
{
    
    public static class QueueCalculator
    {
       
        public static QueueMetrics CalculateMM1(double meanArrival, double meanService)
        {
            double lambda = 1.0 / meanArrival;
            double mu = 1.0 / meanService;

            var m = new QueueMetrics
            {
                Model = QueueingModelType.MM1,
                Lambda = lambda,
                Mu = mu,
                MeanArrival = meanArrival,
                MeanService = meanService,
                VarArrival = meanArrival * meanArrival,
                VarService = meanService * meanService,
                Ca2 = 1.0,
                Cs2 = 1.0
            };

            m.Rho = lambda / mu;
            if (m.Rho >= 1.0) return MarkUnstable(m);

            m.IsStable = true;
            m.P0 = 1.0 - m.Rho;
            m.L = lambda / (mu - lambda);
            m.Lq = (lambda * lambda) / (mu * (mu - lambda));
            m.W = 1.0 / (mu - lambda);
            m.Wq = lambda / (mu * (mu - lambda));
            return m;
        }

      
        public static QueueMetrics CalculateMG1(double meanArrival, double meanService, double varService)
        {
            double lambda = 1.0 / meanArrival;
            double es = meanService;

            var m = new QueueMetrics
            {
                Model = QueueingModelType.MG1,
                Lambda = lambda,
                Mu = 1.0 / es,
                MeanArrival = meanArrival,
                MeanService = es,
                VarArrival = meanArrival * meanArrival,
                VarService = varService,
                Ca2 = 1.0,
                Cs2 = varService / (es * es)
            };

            m.Rho = lambda * es;
            if (m.Rho >= 1.0) return MarkUnstable(m);

            double es2 = varService + es * es;          

            m.IsStable = true;
            m.P0 = 1.0 - m.Rho;
            m.Wq = (lambda * es2) / (2.0 * (1.0 - m.Rho));
            m.W = m.Wq + es;
            m.Lq = lambda * m.Wq;
            m.L = lambda * m.W;
            return m;
        }

        
        public static QueueMetrics CalculateGG1(
            double meanArrival, double varArrival,
            double meanService, double varService)
        {
            double lambda = 1.0 / meanArrival;
            double es = meanService;

            var m = new QueueMetrics
            {
                Model = QueueingModelType.GG1,
                Lambda = lambda,
                Mu = 1.0 / es,
                MeanArrival = meanArrival,
                MeanService = es,
                VarArrival = varArrival,
                VarService = varService,
                Ca2 = varArrival / (meanArrival * meanArrival),
                Cs2 = varService / (es * es)
            };

            m.Rho = lambda * es;
            if (m.Rho >= 1.0) return MarkUnstable(m);

            double rho2 = m.Rho * m.Rho;

            m.IsStable = true;
            m.P0 = 1.0 - m.Rho;
            m.Lq = (rho2 * (1.0 + m.Cs2) * (m.Ca2 + rho2 * m.Cs2))
                   / (2.0 * (1.0 - m.Rho) * (1.0 + rho2 * m.Cs2));
            m.Wq = m.Lq / lambda;
            m.W = m.Wq + es;
            m.L = lambda * m.W;
            return m;
        }

        private static QueueMetrics MarkUnstable(QueueMetrics m)
        {
            m.IsStable = false;
            m.P0 = double.NaN;
            m.L = double.PositiveInfinity;
            m.Lq = double.PositiveInfinity;
            m.W = double.PositiveInfinity;
            m.Wq = double.PositiveInfinity;
            return m;
        }
    }
}
