﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AJE2Tester
{
    public class AJESolver
    {
        //freestream flight conditions; static pressure, static temperature, and mach number
        private double p0, t0, M0;

        //overall engine design parameters; inlet total pressure recovery, bypass ratio, fan pressure ratio, 
        //compressor pressure ratio, turbine temperature ratio, 
        private double TPR, BPR, FPR, CPR, TTR;

        //engine design point; mach number, temperature 
        private double M_d, T_d;

        //conditions at inlet; pressure, temperature;
        private double P1, T1;

        //conditions at fan; pressure, temperature;
        private double P2, T2;

        //Conditions at burner inlet / compressor exit
        private double P3, T3, eta_c;

        //conditions at burner exit / turbine entrance; pressure, temperature, mass flow rate
        private double P4, T4, eta_t;

        //conditions at ab inlet / turbine exit
        private double P5, T5;

        //conditions at ab rear / nozzle entrance;
        private double P6, T6, eta_n;


        //gas properties, pre-burner, post-burner, post afterburner
        private double gamma_c, gamma_t, gamma_ab;
        private double R_c, R_t, R_ab;
        private double Cp_c, Cp_t, Cp_ab;
        private double Cv_c, Cv_t, Cv_ab;

        //Throttles for burner and afterburner
        private double mainThrottle, abThrottle;

        //Air flow, fuel mass fraction for burner and afterburner
        private double mdot, ff, ff_ab;



        //Fuel heat of burning and peak temperatures
        private double h_f, Tt4, Tt6;

        //Reference area of the engine, combustor, and nozzle
        private double Aref, Acomb, Anozzle;

        //thrust and Isp of the engine
        private double thrust, Isp;


        public string debugstring;
        //---------------------------------------------------------
        //Initialization Functions

        public void InitializeOverallEngineData(
            double nozzleArea,
            double totalPressureRecovery,
            double bypassRatio,
            double compressorRatio,
            double fanRatio,
            double designMach,
            double designTemperature,
            double compressorEta,
            double turbineEta,
            double nozzleEta,
            double heatOfFuel,
            double max_TIT,
            double max_TAB
            )
        {

            Aref = nozzleArea;
            TPR = totalPressureRecovery;
            BPR = bypassRatio;
            CPR = compressorRatio;
            FPR = fanRatio;
            M_d = designMach;
            T_d = designTemperature;
            eta_c = compressorEta;
            eta_t = turbineEta;
            eta_n = nozzleEta;
            h_f = heatOfFuel;
            Tt4 = max_TIT;
            Tt6 = max_TAB;

            gamma_c = CalculateGamma(T_d, 0);
            Cp_c = CalculateCp(T_d, 0);
            T1 = T_d * (1 + 0.5 * (gamma_c - 1) * M_d * M_d); //calculate TTR at design point first
            T2 = T1 * Math.Pow(FPR, (gamma_c - 1) / gamma_c / eta_c);
            T3 = T1  * Math.Pow(CPR, (gamma_c - 1) / gamma_c/ eta_c);
            
            ff = Cp_c * (Tt4 - T3) / (Cp_c * (Tt4 - T3) + h_f);//fuel fraction
            gamma_t = CalculateGamma(Tt4, ff);
            Cp_t = CalculateCp(Tt4, ff);


            TTR = 1 - (BPR * Cp_c * (T2 - T1) + Cp_c * (T3 - T1)) / ((1 + ff) * Cp_t * Tt4);

        }



        public void CalculatePerformance(double pressure, double temperature, double velocity, double commandedThrottle)
        {
            if (Tt6 == 0)
            {
                mainThrottle = commandedThrottle;
            }
            else
            {
                mainThrottle = Math.Min(commandedThrottle * 1.5, 1.0);
                abThrottle = Math.Max(commandedThrottle - 0.667, 0);
            }
            gamma_c = CalculateGamma(t0, 0);
            Cp_c = CalculateCp(t0, 0);
            Cv_c = Cp_c / gamma_c;
            R_c = Cv_c * (gamma_c - 1);

            p0 = pressure * 1000;          //freestream
            t0 = temperature;
            M0 = velocity / Math.Sqrt(gamma_c * R_c * t0);
            

            T1 = t0 * (1 + 0.5 * (gamma_c - 1) * M0 * M0);      //inlet
            P1 = p0 * Math.Pow(T1 / t0, gamma_c / (gamma_c - 1)) * TPR;

            double prat3 = CPR;
            double prat2 = FPR;
            double k = FPR / CPR;
            double p = Math.Pow(k, (gamma_c - 1) / eta_c / gamma_c);
            for (int i = 0; i < 20; i++)    //use iteration to calculate CPR
            {
                P2 = prat2 * P1;
                P3 = prat3 * P1;                
                T2 = T1 * Math.Pow(prat2, (gamma_c - 1) / gamma_c / eta_c); //fan
                T3 = T1 * Math.Pow(prat3, (gamma_c - 1) / gamma_c / eta_c); //compressor

                T4 = (Tt4 - T3 * 1.2) * mainThrottle + T3 * 1.2;    //burner
                P4 = P3;
                ff = Cp_c * (T4 - T3) / (Cp_c * (T4 - T3) + h_f);//fuel fraction

                gamma_t = CalculateGamma(T4, ff);//gas parameters
                Cp_t = CalculateCp(T4, ff);
                Cv_t = Cp_t / gamma_t;
                R_t = Cv_t * (gamma_t - 1);

                T5 = T4 * TTR;      //turbine
                double x = prat3;

                prat3 = (1 + ff) * Cp_t * (T4 - T5) / T1 / Cp_c + 1 + BPR;
                prat3 /= 1 + BPR * p;
                prat3 = Math.Pow(prat3, eta_c * gamma_c / (gamma_c - 1));
                prat2 = k * prat3; 

                if (Math.Abs(x - prat3) < 0.01)
                    break;               
            }
            
            P5 = P4 * Math.Pow((1 - 1 / eta_t * (1 - TTR)), gamma_c / (gamma_c - 1));

            mdot = Aref * (P1 / R_c / t0) * Math.Sqrt(gamma_c * R_c * t0) * 0.4;

            if (Tt6 > 0)
            {
                T6 = (Tt6 - T5) * abThrottle * 3 + T5;//afterburner  
            }
            else
            {
                T6 = T5;
            }
            
            P6 = P5;//rayleigh loss?

            ff_ab = ff + Cp_t * (T6 - T5) / (Cp_t * (T6 - T5) + h_f);//fuel fraction
            gamma_ab = CalculateGamma(T6, ff_ab);//gas parameters
            Cp_ab = CalculateCp(T6, ff_ab);
            Cv_ab = Cp_ab / gamma_ab;
            R_ab = Cv_ab * (gamma_ab - 1);

            //nozzle
            double P8=P6;
            double T8=T6;
            double t8, p8, V8, rho8, A8=0;

            double P_crit = Math.Pow((0.5 * gamma_ab + 0.5), gamma_ab / (gamma_ab - 1));
            if(P8/p0>=P_crit)//chocked
            {
                t8 = 2 * T8 / (gamma_ab + 1);//nozzle static temperature
                p8 = P8 / Math.Pow(T8 / t8, gamma_ab / (gamma_ab - 1));//nozzle static pressure
                V8 = Math.Sqrt(2 * Cp_ab * (T8 - t8));//nozzle throat velocity
                rho8 = p8 / R_ab / t8;//gas density at throat
                A8 = mdot / rho8 / V8;//effective nozzle throat area
                thrust = eta_n * (mdot * V8  + A8 * (p8 - p0));//gross thrust
            }
            else//unchoked
            {
                p8 = p0;
                t8 = T8 / Math.Pow(P8 / p8, (gamma_ab - 1) / gamma_ab);//nozzle static temperature;
                V8 = Math.Sqrt(2 * Cp_ab * (T8 - t8));//nozzle throat velocity
                thrust = eta_n * (mdot * V8);//gross thrust
            }

            if (BPR > 0 && FPR > 1)
            {
                double fac1 = (gamma_c - 1) / gamma_c; //fan thrust from NASA
                double snpr = P2 / p0;
                double ues = Math.Sqrt(2.0 * R_c / fac1 * T2 * eta_n *
                    (1.0 - Math.Pow(1.0 / snpr, fac1)));
                // m2 = getMach(0, eair * (1.0 + byprat) * Math.Sqrt(tt[0] / 518f) /
                //     (prat[2] * pt[0] / 14.7 * areaFan * 144f), gamma);
                double pfexit;
                if (snpr <= 1.893) pfexit = p0;
                else pfexit = .52828 * P2;
                thrust += (BPR * ues + (pfexit - p0) * BPR * Aref);
             }


            double netthrust = thrust - mdot * (1 + BPR) * (velocity-0.4*Math.Sqrt(gamma_c * R_c * t0));//ram drag
           
            Isp = netthrust / (mdot * ff_ab * 9.81);

            debugstring = "";
            debugstring += "TTR:\t" + TTR.ToString("F3");
            debugstring += "\tCPR:\t" + prat3.ToString("F3");            
            debugstring += "\t\tp0: " + p0.ToString("F2") + "\tt0: " + t0.ToString("F2");
            debugstring += "\t\tP1: " + P1.ToString("F2") + "\tT1: " + T1.ToString("F2");
            debugstring += "\t\tP3: " + P3.ToString("F2") + "\tT3: " + T3.ToString("F2");
            debugstring += "\t\tP4: " + P4.ToString("F2") + "\tT4: " + T4.ToString("F2");
            debugstring += "\t\tP5: " + P5.ToString("F2") + "\tT5: " + T5.ToString("F2");
            debugstring += "\t\tP6: " + P6.ToString("F2") + "\tT6: " + T6.ToString("F2");
            debugstring += "\t\tFF: " + ff.ToString("P");
            debugstring += "\tFF_AB: " + ff_ab.ToString("P");
            debugstring += "\tV8: " + V8.ToString("F2") + "\tA8: " + A8.ToString("F2");
            debugstring += "\t\tThrust: " + (thrust/1000).ToString("F1") +"\tmdot: " + mdot.ToString("F2");
            debugstring += "\t\tNetThrust: " + (netthrust/1000).ToString("F1") + "\tSFC: " + (3600/Isp).ToString("F3"); 
        }

        public double GetThrust() { return thrust; }
        public double GetIsp() { return Isp; }


        private double CalculateGamma(double temperature, double fuel_fraction)
        {
            double gamma = 1.4 - 0.1 * Math.Max((temperature - 300) * 0.0005, 0) * (1 + fuel_fraction);
            gamma = Math.Min(1.4, gamma);
            gamma = Math.Max(1.1, gamma);
            return gamma;
        }

        private double CalculateCp(double temperature, double fuel_fraction)
        {
            double Cp = 1004.5 + 250 * Math.Max((temperature - 300) * 0.0005, 0) * (1 + 10 * fuel_fraction);
            Cp = Math.Min(1404.5, Cp);
            Cp = Math.Max(1004.5, Cp);
            return Cp;
        }

    }

}