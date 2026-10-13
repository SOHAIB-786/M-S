# QueueLab – Project Report (M/M/1, M/G/1, G/G/1)

Yeh report batati hai ki project me **kya badla**, **kaunsa code kahan hai**, **formulas kahan hain**, aur **program ka flow kya hai** (kaunsa code kaise run hota hai).

---

## 1. Project ka overview

- Technology: **C# + WPF (.NET 8)**, project file: `QueueSimulatorApp.csproj`
- Look: **website jaisa** – upar dark **navbar**, uske neeche **"Select Queueing Model" dropdown**, phir chuney hue model ka page: **hero banner**, **Inputs card**, **Results card**, **Formulas card**, aur footer.
- Pehle dropdown se model chuna jata hai (M/M/1, M/G/1 ya G/G/1); phir **sirf usi model ke input fields** aur result aate hain. Shuru me koi model selected nahi hota.
- Har model ka **alag page** hai (alag View), taaki code ek jagah mile.

### Pehle kya tha vs ab kya hai

| Pehle | Ab |
|---|---|
| Ek hi form, model popup se select hota tha | Model dropdown se select karo → us model ke alag page ke inputs/results |
| Mean/Rate aur Minutes/Hours **poore form ke liye ek hi** radio | **Har input ka apna** Mean/Rate dropdown (arrival mean me, service rate me – alag alag ho sakta hai). **Inputs me koi time unit nahi** – unit sirf Results me chuni jati hai |
| Teeno models ek hi Kingman formula se | **Har model ka apna formula** (aapke diye formulas ke mutabiq) |
| Variance sirf std-dev se | Variance / Std Dev / Squared coefficient (C²) / **Uniform(Min, Max)** / Exponential / Deterministic |
| Output Minutes + Hours dono dikhta tha | Results me unit dropdowns (neeche dekhein) – badalte hi result turant update |
| L, Lq, W, Wq, P0 | L, Lq, W, Wq, ρ, μ, P0, λ, arrival time, service time, variance, Ca², Cs² |

---

## 2. Har model me Input kya hai

### M/M/1
1. **Arrival** – dropdown: *Mean (time)* ya *Rate (per time)* + value
2. **Service** – dropdown: *Mean (time)* ya *Rate (per time)* + value (alag dropdown)

### M/G/1
1. **Arrival** – Mean/Rate (Poisson arrivals, isliye λ chahiye)
2. **Service time** – dropdown me sirf 3 options: **Uniform distribution / Mean / Rate**
   - *Uniform distribution* → **Min (a)** aur **Max (b)** boxes khulte hain. App khud `1/μ = (a+b)/2` aur `Var(S) = (b−a)²/12` nikalti hai (live preview ke saath).
   - *Mean* → mean service time ka box + **Variance of service time** ka box.
   - *Rate* → service rate μ ka box (`mean = 1/μ`) + **Variance of service time** ka box.

### G/G/1
1. **Arrival (inter-arrival time)** – dropdown: *Mean (time)* ya *Rate (per time)* + value, aur uske neeche alag box **Variance of arrival time**.
2. **Service time** – M/G/1 jaisa hi: **Uniform distribution / Mean / Rate**. Uniform me Min a / Max b dene par mean aur variance (`(b−a)²/12`) khud calculate hote hain; Mean/Rate me alag Variance box.

### Start screen
App khulte hi beech me project ka naam **QueueLab** aur uske neeche ek dropdown (M/M/1, M/G/1, G/G/1) aata hai. Model chunte hi uska page khulta hai. Page par upar navbar me model badalne ka dropdown aur **Back** button hai.

> Note: Inputs me koi unit nahi aati. Saari values ek hi unit me maani jati hain (jo Results me "Input values are in" me chuni jati hai), aur Variance us unit ka square hota hai.

---

## 3. Output kya aata hai

| Model | Output tiles |
|---|---|
| M/M/1 | L, Lq, W, Wq, ρ, μ, P0, λ, arrival time 1/λ, service time 1/μ |
| M/G/1 | upar wale sab + Variance of service time |
| G/G/1 | upar wale sab + Variance of service time, Variance of arrival time, Ca², Cs² |

- Results card me do dropdowns hain (dono Seconds / Minutes / Hours):
  - **"Input values are in"** – aapne inputs jis unit me daale the (default Minutes).
  - **"Show results in"** – result jis unit me dekhna hai (default Minutes).
  Dono same hon to koi conversion nahi hota. Alag hon to W, Wq, arrival time, service time, variance (unit²) aur rates (λ, μ per unit) automatically convert hote hain, aur dropdown badalte hi turant update hota hai.
- Agar ρ ≥ 1 ho to system unstable hai: L, Lq, W, Wq = ∞, P0 = N/A, aur note dikhta hai.

---

## 4. Files aur unka kaam (kaunsa code kahan hai)

```
Project/
├─ App.xaml                    ← Poore app ke colours/styles (card, button, navbar button, input box)
├─ App.xaml.cs                 ← App class (startup)
├─ MainWindow.xaml / .cs       ← Navbar + "Select Queueing Model" dropdown + page area + footer
│
├─ Views/                      ← HAR MODEL KA APNA PAGE
│   ├─ MM1View.xaml / .cs      ← M/M/1 page (UI + Calculate/Reset code)
│   ├─ MG1View.xaml / .cs      ← M/G/1 page
│   └─ GG1View.xaml / .cs      ← G/G/1 page
│
├─ Controls/                   ← Reusable UI tukde
│   ├─ RateTimeInput.xaml/.cs  ← "Mean ya Rate" dropdown + value (har input ka apna, koi unit nahi)
│   ├─ DistributionInput.xaml/.cs ← Service time block (Uniform / Mean / Rate)
│   └─ ResultsPanel.xaml/.cs   ← Results tiles + "Input values are in" / "Show results in" dropdowns + conversion
│
├─ Services/
│   ├─ QueueCalculator.cs      ← ★ SAARE FORMULAS yahin hain (3 methods)
│   └─ InputHelper.cs          ← Sirf number type karne dena, text → double parse
│
└─ Models/
    ├─ QueueingModelType.cs    ← enum MM1, MG1, GG1
    ├─ QueueMetrics.cs         ← Calculation ka result (L, Lq, W, Wq, ρ, P0, ...)
    └─ TimeUnit.cs             ← Seconds/Minutes/Hours aur unka conversion factor (sirf Results ke liye)
```

### Formulas kahan hain – `Services/QueueCalculator.cs`

| Model | Method | Formulas |
|---|---|---|
| M/M/1 | `CalculateMM1(meanArrival, meanService)` | ρ = λ/μ, P0 = 1−ρ, L = λ/(μ−λ), Lq = λ²/(μ(μ−λ)), W = 1/(μ−λ), Wq = λ/(μ(μ−λ)) |
| M/G/1 | `CalculateMG1(meanArrival, meanService, varService)` | ρ = λ/μ, Wq ke liye second moment = σS² + (1/μ)², Wq = λ(σS² + (1/μ)²)/(2(1−ρ)), W = Wq + 1/μ, Lq = λWq, L = λW |
| G/G/1 | `CalculateGG1(meanArrival, varArrival, meanService, varService)` | ρ = λ/μ, Ca² = Var(A)/(1/λ)², Cs² = Var(S)/(1/μ)², Lq = ρ²(1+Cs²)(Ca²+ρ²Cs²) / [2(1−ρ)(1+ρ²Cs²)], Wq = Lq/λ, W = Wq + 1/μ, L = λW |

Uniform ke formulas (`E = (a+b)/2`, `Var = (b−a)²/12`) `Controls/DistributionInput.xaml.cs` ke `TryGet()` me hain (`MethodUniform` wali branch), aur wahi preview ke liye `UpdatePreview()` me.

Formulas ab screen par nahi dikhte; wo sirf `QueueCalculator.cs` me hain. Har page ab beech me ek hi column me hai: Hero, Inputs, phir Results.

---

## 5. Flow – program kaise chalta hai

### 5.1 Startup
1. `App.xaml` → `StartupUri="MainWindow.xaml"` → `MainWindow` khulta hai. Koi model selected nahi hota; "No model selected yet" likha aata hai.
2. `MainWindow()` teeno pages (`MM1View`, `MG1View`, `GG1View`) **ek baar** banata hai (abhi dikhte nahi).
3. User **"Select Queueing Model"** dropdown se model chunta hai → `Model_Changed` (index 0/1/2) → `pageHost.Content =` us model ka page. Tabhi us model ke inputs, Calculate/Reset, Results aur Formulas card dikhte hain.
   Pages dobara nahi bante, isliye model badal kar wapas aane par typed values bachi rehti hain.

### 5.2 Calculate dabane par (teeno pages me same pattern)

```
[Calculate button]
      │  Click="Calculate_Click"                         (Views/XXXView.xaml.cs)
      ▼
1. Inputs padho (koi unit nahi – sirf numbers)
   • M/M/1 :  arrivalInput.TryGetMean()  +  serviceInput.TryGetMean()
   • M/G/1 :  arrivalInput.TryGetMean()  +  serviceInput.TryGet()  → (mean, variance)
   • G/G/1 :  arrivalInput.TryGetMean() + txtArrivalVar (variance box)  +  serviceInput.TryGet()
      │
      │  (andar kya hota hai)
      │  RateTimeInput.TryGetMean():   mean v → v      rate v → 1 / v
      │  DistributionInput.TryGet():
      │      Uniform(a,b) → mean=(a+b)/2, var=(b−a)²/12
      │      Mean → mean=v, var=Variance box      Rate → mean=1/v, var=Variance box
      │  Galat/khali input par error message → txtError, aur function yahin return.
      ▼
2. Services/QueueCalculator.cs
   • CalculateMM1(...)  /  CalculateMG1(...)  /  CalculateGG1(...)
   • ρ ≥ 1 ho to unstable (∞ / N/A), warna formulas se L, Lq, W, Wq, P0
   • Result: Models/QueueMetrics (usi unit me jisme inputs the)
      ▼
3. results.Show(metrics)                                  (Controls/ResultsPanel.xaml.cs)
   • "Input values are in" aur "Show results in" dropdowns ki units uthata hai
   • r = (minutes per input unit) / (minutes per output unit)
   • time × r,  variance × r²,  rate ÷ r
   • Tiles (L, Lq, W, Wq, ρ, μ, P0, λ, 1/λ, 1/μ, ...) banakar ItemsControl me dikhata hai
      ▼
4. Dono unit dropdowns me se koi badalne par: ResultsPanel.Unit_Changed → Render() dobara (Calculate dubara dabana nahi padta)
```

### 5.3 Reset
`Reset_Click` → saare inputs khali + Mean/Rate dropdowns default par (Mean) → error chhupta hai → `results.Hide()`.

---

## 6. Ek example (check ke liye)

M/M/1: Arrival **Mean = 10**, Service **Rate = 0.1333** (alag alag mode), Results me "Input values are in" = **Minutes**:
- 1/μ = 1/0.1333 = 7.5 → λ = 0.1, μ = 0.1333
- ρ = 0.75, P0 = 0.25, L = 3, Lq = 2.25, W = 30 min, Wq = 22.5 min
- "Show results in" ko **Hours** karte hi W = 0.5000 hr, Wq = 0.3750 hr dikhega.

M/G/1: Arrival mean = 10, Service Uniform(4, 8) (inputs minutes me) → 1/μ = 6, Var = 16/12 = 1.3333
- ρ = 0.6, Wq = 4.6667, W = 10.6667, Lq = 0.4667, L = 1.0667

(Consistency checks: M/G/1 with exponential service = M/M/1, aur G/G/1 with Ca² = Cs² = 1 = M/M/1 – dono match karte hain.)

---

G/G/1: Arrival Mean = 12, Variance of arrival time = 30, Service Uniform(a = 7, b = 15):
- λ = 1/12, 1/μ = 11, ρ = 0.9167, Ca² = 0.2083, Var(S) = 64/12 = 5.3333, Cs² = 0.0441
- Lq = ρ²(1+Cs²)(Ca²+ρ²Cs²) / [2(1−ρ)(1+ρ²Cs²)] = 1.2455
- Wq = Lq/λ = 14.9457, W = Wq + 1/μ = 25.9457, L = λW = 2.1621

---

## 7. Run kaise karein

Visual Studio me `QueueSimulatorApp.csproj` kholein (ya folder me `dotnet run`). Pehli baar build karne par `obj/` aur `bin/` apne aap ban jayenge (zip me purane `bin/obj/.vs` hata diye gaye hain).
