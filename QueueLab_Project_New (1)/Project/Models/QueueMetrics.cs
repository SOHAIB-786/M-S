using System;

namespace QueueSimulatorApp.Models
{

    public class QueueMetrics
    {
        public QueueingModelType Model { get; set; }
        public bool IsStable { get; set; }

        public double Lambda { get; set; }          
        public double Mu { get; set; }             
        public double MeanArrival { get; set; }  
        public double MeanService { get; set; }  
        public double VarArrival { get; set; } 
        public double VarService { get; set; }  
        public double Ca2 { get; set; }            
        public double Cs2 { get; set; }            

        public double Rho { get; set; }  
        public double P0 { get; set; }   
        public double L { get; set; }    
        public double Lq { get; set; }    
        public double W { get; set; }  
        public double Wq { get; set; } 
    }
}
