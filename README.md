AJEsolverTester
===============

Standalone program to study &amp; test the jet engines of Kerbal Space Prgram

Content:
===============

Basically a re-write of NASA EngineSim's code. Using SI units and clear nomination. 

Still using stead-state model, a detailed explanation can be found http://en.wikipedia.org/wiki/Jet_engine_performance

See the Excel table included for engine specification.

Improvements over the original program include: 

1.Simple off-design calculation of compressors

2.Real throttle

3.All engine is one type

4.An option to mix fan duct and core flow before the nozzle

Possible improvements that I think worth doing and not too hard include:

1.More sophisticated off-design using compressor/turbine performance maps

2.Or use variable eta_c with Mach number

3.An option to have leaky compressor


Explanation of Parameters:
=================

Area: size of engine, does not include fan area

TPR: total pressure recovery of inlet. =1 at low Mach and decrease as Mach number increase

FHV: fuel combustion heat in J/kg. Kerosene=46.8E6

FPR: Fan pressure ratio

BPR: Bypass ratio (if BPR=0 then it's a turbojet)

CPR: Compressor pressure ratio AT THE DESIGN POINT

eta_c: compressor efficiency

TIT: turbine inlet temperature in K

eta_t: turbine efficiency

TAB: afterburner temperature in K (if =0 then no afterburner)

eta_n: nozzle efficiency

Design Mach: the mach number of design point

Design Temperature: the air temperature of the design point

Throttle: if afterburner is used, 66% Throttle=mil thrust, 100% Throttle=full afterburner

Exhaust Mixer: gas from engine core and fan duct are mixed before the nozzle


